#if IOS || MACCATALYST
using Foundation;
using UIKit;
#endif

namespace BolWallet.Services.PermissionServices
{
    public class MacCatalystPermissionService : BasePermissionService, IPermissionService
    {
        protected override async Task OpenSystemSettingsAsync<T>()
        {
#if IOS || MACCATALYST
            var url = new NSUrl("x-apple.systempreferences:com.apple.preference.security?Privacy");
            await UIApplication.SharedApplication.OpenUrlAsync(url, new UIApplicationOpenUrlOptions());
#endif
        }
    }
}
