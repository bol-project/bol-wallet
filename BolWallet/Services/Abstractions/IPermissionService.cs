namespace BolWallet.Services.Abstractions;

public interface IPermissionService
{
    Task<PermissionStatus> CheckPermissionAndDisplayMessageAsync<T>() where T : Permissions.BasePermission, new();
}