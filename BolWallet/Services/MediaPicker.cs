namespace BolWallet.Services;
public class MediaPicker : IMediaPicker
{
    private readonly IMediaPicker _mediaPicker;

    public MediaPicker()
    {
        _mediaPicker = Microsoft.Maui.Media.MediaPicker.Default;
    }

    public Task<FileResult> PickPhotoAsync(MediaPickerOptions options = null)
    {
        return _mediaPicker.PickPhotoAsync(options);
    }

    public Task<FileResult> CapturePhotoAsync(MediaPickerOptions options = null)
    {
        return _mediaPicker.CapturePhotoAsync(options);
    }

    public Task<FileResult> PickVideoAsync(MediaPickerOptions options = null)
    {
        return _mediaPicker.PickVideoAsync(options);
    }

    public Task<FileResult> CaptureVideoAsync(MediaPickerOptions options = null)
    {
        return _mediaPicker.CaptureVideoAsync(options);
    }

    public bool IsCaptureSupported => _mediaPicker.IsCaptureSupported;
}
