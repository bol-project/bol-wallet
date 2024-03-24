namespace BolWallet.Services.Abstractions;
using static Permissions;

public interface IPermissionService
{
    Task<PermissionStatus> CheckPermissionAsync<T>() where T : BasePermission, new();
    Task<PermissionStatus> RequestPermissionAsync<T>() where T : BasePermission, new();
    Task PromptToOpenSettingsAsync<T>() where T : BasePermission, new();
    Task<bool> TryGetPermissionAsync<T>() where T : BasePermission, new();
}
