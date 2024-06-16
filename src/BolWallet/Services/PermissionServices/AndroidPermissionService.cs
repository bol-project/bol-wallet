namespace BolWallet.Services.PermissionServices;
public class AndroidPermissionService : BasePermissionService, IPermissionService
{
    protected override Task OpenSystemSettingsAsync<T>()
    {
        AppInfo.Current.ShowSettingsUI();
        return Task.CompletedTask;
    }
}
