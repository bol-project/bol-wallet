using static Microsoft.Maui.ApplicationModel.Permissions;

namespace Bol.App.Core.Services
{
    public class PermissionService : IPermissionService
    {
        public async Task<PermissionStatus> CheckPermissionAsync<T>() where T : BasePermission, new()
        {
            PermissionStatus status = await CheckStatusAsync<T>();

            if (status == PermissionStatus.Denied)
                status = await RequestAsync<T>();

            return status;
        }

        public async Task DisplayWarningAsync<T>() where T : BasePermission, new()
        {
            if (ShouldShowRationale<T>())
                await Application.Current.MainPage.DisplayAlert("Permission Required", PermissionWarningMesssage(typeof(T).Name), "OK");
            else
                await Application.Current.MainPage.DisplayAlert("Permission Required", DisplayPermissionWarningAndInstructions(typeof(T).Name), "OK");
        }

        private string DisplayPermissionWarningAndInstructions(string permissionName)
        {
            return $"We're sorry, but you cannot register and use the app without granting us access.\n\nTo enable access, please follow these steps:\n1. " +
                    "Open the device settings.\n2. Navigate to the app permissions settings.\n3. Find the permission and make sure it is enabled for our app.\n\nOnce you've granted us access," +
                    $" you can use all the features of our app that require {permissionName} functionality.\n\n";
        }

        private string PermissionWarningMesssage(string permissionName)
        {
            return $"Please grant permission to use all the features of our app that require {permissionName} functionality";
        }
    }
}
