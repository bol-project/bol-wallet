#nullable enable
namespace BolWallet.Services.Abstractions;

public interface IMediaService
{
    /// <summary>
    /// Takes a photo and saves it to the provided location.
    /// Calling this method will request the necessary permissions.
    /// </summary>
    /// <param name="saveLocation"></param>
    /// <returns></returns>
    Task<string> TakePhotoAsync(string saveLocation);
    
    /// <summary>
    /// Pick a photo from the device.
    /// No permissions are requested when calling this method.
    /// </summary>
    /// <returns></returns>
    Task<FileResult?> PickPhotoAsync();
}
