using static Microsoft.Maui.ApplicationModel.Permissions;

namespace BolWallet.Services.PermissionServices;

public abstract class BasePermissionService : IPermissionService
{
    /// <summary>
    /// Each platform-specific implementation should override this method to open the system settings.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <returns></returns>
    protected abstract Task OpenSystemSettingsAsync<T>() where T : BasePermission, new();

    public virtual async Task<PermissionStatus> CheckPermissionAsync<T>() where T : BasePermission, new()
    {
        try
        {
            PermissionStatus status = await CheckStatusAsync<T>();

            return status;
        }
        // From Maui Docs: A PermissionException is thrown if the required permission isn't declared.
        catch (Exception e)
        {
            Console.WriteLine(e);
            throw;
        }
    }

    public virtual async Task<PermissionStatus> RequestPermissionAsync<T>() where T : BasePermission, new()
    {
        var result = await RequestAsync<T>();

        return result;
    }

    public virtual async Task PromptToOpenSettingsAsync<T>() where T : BasePermission, new()
    {
        var permissionName = typeof(T).Name;

        var accepted = await Application.Current.MainPage.DisplayAlert(
            "Permission Required",
            $"To continue, please give {permissionName} permissions for this app in your device settings.",
            "Open Settings",
            "Cancel");

        if (accepted)
        {
           await OpenSystemSettingsAsync<T>();
        }
    }

    public async Task<bool> TryGetPermissionAsync<T>() where T : BasePermission, new()
    {
        var initialPermissionStatus = await CheckPermissionAsync<T>();

        if (initialPermissionStatus is PermissionStatus.Granted) return true;

        var result = await RequestPermissionAsync<T>();

        if (result is PermissionStatus.Granted) return true;

        await PromptToOpenSettingsAsync<T>();
        return false;
    }
}
