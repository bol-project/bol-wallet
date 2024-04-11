#nullable enable
namespace BolWallet.Services.Abstractions;

public interface IMediaService
{
    /// <summary>
    /// Pick a file from the device.
    /// </summary>
    /// <returns></returns>
    Task<FileResult?> PickFileAsync();
    
    /// <summary>
    /// Pick an audio file from the device.
    /// </summary>
    /// <returns></returns>
    Task<FileResult?> PickAudioFileAsync();
    
    /// <summary>
    /// Takes a photo and saves it to the provided location.
    /// Calling this method will request the necessary permissions.
    /// </summary>
    /// <param name="saveLocation"></param>
    /// <returns></returns>
    Task<FileResult?> TakePhotoAsync(string saveLocation);
    
    /// <summary>
    /// Pick a photo from the device.
    /// No permissions are requested when calling this method.
    /// </summary>
    /// <returns></returns>
    Task<FileResult?> PickPhotoAsync();
    
    /// <summary>
    /// Starts recording audio and returns a boolean indicating whether the recording was started successfully.
    /// </summary>
    /// <returns></returns>
    Task<bool> StartRecordingAudioAsync();
    
    /// <summary>
    /// Stops recording audio and returns the recorded audio file.
    /// </summary>
    /// <returns></returns>
    Task<FileResult> StopRecordingAudioAsync();
}
