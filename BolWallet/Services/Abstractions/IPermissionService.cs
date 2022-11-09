namespace BolWallet.Services.Abstractions;

public interface IPermissionService
{
    Task<bool> CheckCameraPermission();
    Task<bool> CheckSpeechPermission();
    Task<bool> CheckStoragePermission();
}