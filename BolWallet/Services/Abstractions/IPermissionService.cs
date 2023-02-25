namespace BolWallet.Services.Abstractions;

public interface IPermissionService
{
    Task<PermissionStatus> CheckPermissionAsync<T>() where T : Permissions.BasePermission, new();
    Task DisplayWarningAsync<T>() where T : Permissions.BasePermission, new();
}