#if IOS || MACCATALYST
using Foundation;
using UIKit;
#endif

namespace BolWallet.Services.PermissionServices;

public class IosPermissionService : BasePermissionService, IPermissionService
{
    protected override async Task OpenSystemSettingsAsync<T>()
    {
#if IOS || MACCATALYST
        var url = new NSUrl("app-settings:");
        await UIApplication.SharedApplication.OpenUrlAsync(url, new UIApplicationOpenUrlOptions());
#endif
    }
}
