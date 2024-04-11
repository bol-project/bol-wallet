#nullable enable
using Microsoft.Extensions.Logging;
using Plugin.Maui.Audio;

namespace BolWallet.Services;

public class MediaService : IMediaService
{
    private readonly IMediaPicker _mediaPicker;
    private readonly IFilePicker _filePicker;
    private readonly IAudioRecorder _audioRecorder;
    private readonly IPermissionService _permissionService;
    private readonly ILogger<MediaService> _logger;

    private readonly FilePickerFileType _supportedFileTypes = new FilePickerFileType(new Dictionary<DevicePlatform, IEnumerable<string>>
    {
        { DevicePlatform.iOS, new[] { "com.adobe.pdf", "public.image", "public.audio" } },
        { DevicePlatform.Android, new[] { "application/pdf", "image/*", "audio/*" } },
        { DevicePlatform.MacCatalyst, new[] { "pdf", "png", "jpeg", "jpg", "public.image", "public.audio" } },
        { DevicePlatform.WinUI, new[] { ".pdf", ".gif", ".mp3", ".png", ".jpg", ".jpeg" } },
    });
    
    private readonly FilePickerFileType _supportedAudioFileTypes = new FilePickerFileType(new Dictionary<DevicePlatform, IEnumerable<string>>
    {
        { DevicePlatform.iOS, new[] {"public.audio" , ".mp3"} },
        { DevicePlatform.Android, new[] {  "audio/*" } },
        { DevicePlatform.MacCatalyst, new[] {"public.audio", ".mp3" } },
        { DevicePlatform.WinUI, new[] { ".mp3" } }
    });

    public MediaService(
        IMediaPicker mediaPicker,
        IFilePicker filePicker,
        IAudioManager audioManager,
        IPermissionService permissionService,
        ILogger<MediaService> logger)
    {
        _mediaPicker = mediaPicker;
        _filePicker = filePicker;
        _audioRecorder = audioManager.CreateRecorder();
        _permissionService = permissionService;
        _logger = logger;
    }

    public async Task<FileResult?> PickFileAsync()
    {
        return await PickFileAsync(_supportedFileTypes, "Pick a file");
    }

    public async Task<FileResult?> PickAudioFileAsync()
    {
        return await PickFileAsync(_supportedAudioFileTypes, "Pick an audio file");
    }

    public async Task<FileResult?> TakePhotoAsync(string saveLocation)
    {
        var hasGivenPermission = await _permissionService.TryGetPermissionAsync<Permissions.Camera>();

        if (!hasGivenPermission) throw new PermissionException("The user has not given permission to access the camera.");

        var photo = await _mediaPicker.CapturePhotoAsync();

        if (photo is null) return null;

        // save the file into the provided save location
        var photoFilePath = Path.Combine(saveLocation, photo.FileName);

        await using var sourceStream = await photo.OpenReadAsync();

        await using var fileStream = new FileStream(photoFilePath, FileMode.Create, FileAccess.Write);
    
        await sourceStream.CopyToAsync(fileStream);

        return new FileResult(photoFilePath);
    }

    public async Task<FileResult?> PickPhotoAsync()
    {
        var photo = await _mediaPicker.PickPhotoAsync(new MediaPickerOptions
        {
            Title = "Pick a photo"
        });

        return photo;
    }

    public async Task<bool> StartRecordingAudioAsync()
    {
        var hasGivenPermission = await _permissionService.TryGetPermissionAsync<Permissions.Microphone>();

        if (!hasGivenPermission)
        {
            var exception = new PermissionException("The user has not given permission to access the microphone.");
            _logger.LogError(exception, "Cannot record audio without microphone permission.");
            throw exception;
        }
        
        if (_audioRecorder.IsRecording)
        {
            return false;
        }

        await _audioRecorder.StartAsync();
        return true;
    }

    public async Task<FileResult> StopRecordingAudioAsync()
    {
        if (!_audioRecorder.IsRecording)
        {
            var exception = new InvalidOperationException("No audio is being recorded.");
            _logger.LogError(exception, "Cannot stop recording audio without starting it first.");
            throw exception;
        }

        var recording = await _audioRecorder.StopAsync();

        var audioStream = recording.GetAudioStream();

        string directoryPath = FileSystem.AppDataDirectory; // or FileSystem.CacheDirectory for temporary files
        string filePath = Path.Combine(directoryPath, "personalVoice.mp3");

        await using (var fileStream = new FileStream(filePath, FileMode.Create, FileAccess.Write))
        {
            await audioStream.CopyToAsync(fileStream);
        }

        return new FileResult(filePath);
    }

    private async Task<FileResult?> PickFileAsync(FilePickerFileType supportedFileTypes, string pickerTitle)
    {
        var fileResult = await _filePicker.PickAsync(new PickOptions
        {
            FileTypes = supportedFileTypes, PickerTitle = pickerTitle
        });

        return fileResult;
    }
}
