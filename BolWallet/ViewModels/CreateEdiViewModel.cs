using Bol.Core.Abstractions;
using Bol.Core.Model;
using Bol.Cryptography;
using CommunityToolkit.Maui.Alerts;
using Microsoft.Maui.Storage;
using Plugin.AudioRecorder;
using System.Reflection;

namespace BolWallet.ViewModels;

public partial class CreateEdiViewModel : BaseViewModel
{
    private readonly IPermissionService _permissionService;
    private readonly IBase16Encoder _base16Encoder;
    private readonly ISecureRepository _secureRepository;
    private readonly IEncryptedDigitalIdentityService _encryptedDigitalIdentityService;
    private readonly IMediaPicker _mediaPicker;
    private EncryptedDigitalMatrix encryptedDigitalMatrix;
    public EdiFiles ediFiles;

    AudioRecorderService recorder;

    public CreateEdiViewModel(
        INavigationService navigationService,
        IPermissionService permissionService,
        IBase16Encoder base16Encoder,
        ISecureRepository secureRepository,
        IEncryptedDigitalIdentityService encryptedDigitalIdentityService,
        IMediaPicker mediaPicker)
        : base(navigationService)
    {
        _permissionService = permissionService ?? throw new ArgumentNullException(nameof(permissionService));
        _base16Encoder = base16Encoder ?? throw new ArgumentNullException(nameof(base16Encoder));
        _secureRepository = secureRepository ?? throw new ArgumentNullException(nameof(secureRepository));
        _encryptedDigitalIdentityService = encryptedDigitalIdentityService ?? throw new ArgumentNullException(nameof(encryptedDigitalIdentityService));
        _mediaPicker = mediaPicker ?? throw new ArgumentNullException(nameof(mediaPicker));
        EdiForm = new EdiForm();
        recorder = new AudioRecorderService
        {
            AudioSilenceTimeout = TimeSpan.FromMilliseconds(5000),
            TotalAudioTimeout = TimeSpan.FromMilliseconds(5000),
        };
        encryptedDigitalMatrix = new EncryptedDigitalMatrix() { Hashes = new HashTable() };
        ediFiles = new EdiFiles() { };
    }

    [ObservableProperty] private EdiForm _ediForm;

    [ObservableProperty] private bool _isLoading = false;

    [RelayCommand]
    private async Task PickPhotoAsync(string propertyName)
    {
        var customFileType = new FilePickerFileType(new Dictionary<DevicePlatform, IEnumerable<string>>
        {
            { DevicePlatform.iOS, new[] { "com.adobe.pdf", "public.image", "public.audio" } },
            { DevicePlatform.Android, new[] { "application/pdf", "image/*", "audio/*" } },
            { DevicePlatform.MacCatalyst, new[] { "pdf", "public.image", "public.audio" } },
            { DevicePlatform.WinUI, new[] { ".pdf", ".gif", ".mp3", ".png" } },
        });

        var pickResult = await FilePicker.PickAsync(new PickOptions
        {
            FileTypes = customFileType, PickerTitle = "Pick a file"
        });

        PropertyInfo propertyNameInfo = GetPropertyInfo(propertyName);

        await PathPerImport(propertyNameInfo, pickResult);
    }

    [RelayCommand]
    private async Task TakePhotoAsync(string propertyName)
    {
        if (await _permissionService.CheckPermissionAsync<Permissions.Camera>() != PermissionStatus.Granted)
        {
            await _permissionService.DisplayWarningAsync<Permissions.Camera>();
            return;
        }

        FileResult takePictureResult = await _mediaPicker.CapturePhotoAsync();

        PropertyInfo propertyNameInfo = GetPropertyInfo(propertyName);

        await PathPerImport(propertyNameInfo, takePictureResult);
    }

    [RelayCommand]
    private async Task RecordAudio()
    {
        if (await _permissionService.CheckPermissionAsync<Permissions.Speech>() != PermissionStatus.Granted)
        {
            await _permissionService.DisplayWarningAsync<Permissions.Speech>();
            return;
        }

        if (recorder.IsRecording) await recorder.StopRecording();

        Task<string> audioRecordTask = await recorder.StartRecording();

        string audiofilePath = await audioRecordTask;

        PropertyInfo propertyNameInfo = GetPropertyInfo(nameof(EdiForm.Voice));

        await PathPerImport(propertyNameInfo, new FileResult(audiofilePath));
    }

    [RelayCommand]
    private async Task Submit()
    {
        try
        {
            if ((string.IsNullOrEmpty(encryptedDigitalMatrix.Hashes.IdentityCard) &&
                 string.IsNullOrEmpty(encryptedDigitalMatrix.Hashes.Passport)))
            {
                return;
            }

            IsLoading = true;

            UserData userData = await this._secureRepository.GetAsync<UserData>("userdata");

            encryptedDigitalMatrix.BirthDate = userData.Person.Birthdate;
            encryptedDigitalMatrix.FirstName = userData.Person.FirstName;
            encryptedDigitalMatrix.Nin = userData.Person.Nin;
            encryptedDigitalMatrix.BirthCountryCode = userData.Person.CountryCode;
            encryptedDigitalMatrix.CodeName = userData.Codename;

            var result = await Task.Run(() => _encryptedDigitalIdentityService.Generate(encryptedDigitalMatrix));

            userData.Edi = result;

            userData.EncryptedDigitalMatrix = encryptedDigitalMatrix;

            await _secureRepository.SetAsync("userdata", userData);

            await NavigationService.NavigateTo<GenerateWalletWithPasswordViewModel>(true);
        }
        catch (Exception ex)
        {
            await Toast.Make(ex.Message).Show();
        }
        finally
        {
            IsLoading = false;
        }
    }

    private async Task PathPerImport(PropertyInfo propertyNameInfo, FileResult fileResult)
    {
        if (fileResult == null) return;

        var fileBytes = File.ReadAllBytes(fileResult.FullPath);

        var ediFileItem = new EdiFileItem { Content = fileBytes, FileName = Path.GetFileName(fileResult.FullPath) };

        SetFileHash(propertyNameInfo, fileResult, fileBytes);

        ediFiles
            .GetType()
            .GetProperty(propertyNameInfo.Name)
            .SetValue(ediFiles, ediFileItem);

        OnPropertyChanged(nameof(EdiForm));

        await _secureRepository.SetAsync<EdiFiles>("ediFiles", ediFiles);
    }

    private void SetFileHash(PropertyInfo propertyNameInfo, FileResult fileResult, byte[] fileBytes)
    {
        var encodedFileBytes = _base16Encoder.Encode(fileBytes);

        propertyNameInfo.SetValue(EdiForm, fileResult.FileName);

        encryptedDigitalMatrix.Hashes
            .GetType()
            .GetProperty(propertyNameInfo.Name)
            .SetValue(encryptedDigitalMatrix.Hashes, encodedFileBytes);
    }

    public async Task Initialize()
    {
        var savedEdiFiles = await _secureRepository.GetAsync<EdiFiles>("ediFiles");

        if (savedEdiFiles is null) return;

        ediFiles = savedEdiFiles;

        foreach (var propertyInfo in ediFiles.GetType().GetProperties())
        {
            PropertyInfo propertyNameInfo = GetPropertyInfo(propertyInfo.Name);

            var ediFileItem = (EdiFileItem)propertyInfo.GetValue(ediFiles, null);

            if (ediFileItem?.Content != null)
            {
                propertyNameInfo.SetValue(EdiForm, ediFileItem.FileName);

                encryptedDigitalMatrix.Hashes
                    .GetType()
                    .GetProperty(propertyNameInfo.Name)
                    .SetValue(encryptedDigitalMatrix.Hashes, _base16Encoder.Encode(ediFileItem.Content));

                ediFiles
                    .GetType()
                    .GetProperty(propertyNameInfo.Name)
                    .SetValue(ediFiles, ediFileItem);
            }
        }

        OnPropertyChanged(nameof(EdiForm));
    }

    private PropertyInfo GetPropertyInfo(string propertyName)
    {
        return this.EdiForm.GetType().GetProperty(propertyName);
    }
}
