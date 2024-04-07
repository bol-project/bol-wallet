#nullable enable
namespace BolWallet.Services;

public class MediaService : IMediaService
{
    private readonly IMediaPicker _mediaPicker;
    private readonly IPermissionService _permissionService;

    public MediaService(IMediaPicker mediaPicker, IPermissionService permissionService)
    {
        _mediaPicker = mediaPicker;
        _permissionService = permissionService;
    }

    public async Task<string> TakePhotoAsync(string saveLocation)
    {
        var hasGivenPermission = await _permissionService.TryGetPermissionAsync<Permissions.Camera>();

        if (!hasGivenPermission) return String.Empty;

        var photo = await _mediaPicker.CapturePhotoAsync();

        if (photo is null) return String.Empty;

        // save the file into the provided save location
        var localFilePath = Path.Combine(saveLocation, photo.FileName);

        await using var sourceStream = await photo.OpenReadAsync();

        await using var fileStream = new FileStream(localFilePath, FileMode.Create, FileAccess.Write);
    
        await sourceStream.CopyToAsync(fileStream);

        return localFilePath;
    }

    public async Task<FileResult?> PickPhotoAsync()
    {
        var photo = await _mediaPicker.PickPhotoAsync();

        return photo;
    }
}
