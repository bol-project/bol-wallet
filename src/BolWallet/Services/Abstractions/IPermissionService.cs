namespace BolWallet.Services.Abstractions;
using static Permissions;

public interface IPermissionService
{
    Task<PermissionStatus> CheckPermissionAsync<T>() where T : BasePermission, new();
    Task<PermissionStatus> RequestPermissionAsync<T>() where T : BasePermission, new();
    
    /// <summary>
    /// Use this method to prompt the user to open the system settings to grant the permission.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <returns></returns>
    Task PromptToOpenSettingsAsync<T>() where T : BasePermission, new();
    
    /// <summary>
    /// Orchestrates the permission request process.
    /// It first checks the permission status, then requests the permission if it's not granted.
    /// If the permission is denied, it prompts the user to open the system settings to grant the permission.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <returns></returns>
    Task<bool> TryGetPermissionAsync<T>() where T : BasePermission, new();
}
