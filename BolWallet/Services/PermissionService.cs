using static Microsoft.Maui.ApplicationModel.Permissions;

namespace Bol.App.Core.Services
{
    public class PermissionService : IPermissionService
    {
        public async Task<PermissionStatus> CheckPermissionAndDisplayMessageAsync<T>() where T : BasePermission, new()
        {
            PermissionStatus status = await CheckStatusAsync<T>();

            if (status == PermissionStatus.Denied)
                status = await RequestAsync<T>();

            if (ShouldShowRationale<T>())
                await DisplayPermissionWarning();
            else if (status == PermissionStatus.Denied)
                await DisplayPermissionWarningAndInstructions(typeof(T).Name);

            return status;
        }
        private static async Task DisplayPermissionWarningAndInstructions(string permissionName)
        {
            await DisplayPermissionWarning($"We're sorry, but you cannot register and use the app without granting us access.\n\nTo enable access, please follow these steps:\n1. " +
                    "Open the device settings.\n2. Navigate to the app permissions settings.\n3. Find the permission and make sure it is enabled for our app.\n\nOnce you've granted us access," +
                    $" you can use all the features of our app that require {permissionName} functionality.\n\n");
        }

        private static async Task DisplayPermissionWarning(string message = "Please grant permission to use the functionality")
        {
            await Application.Current.MainPage.DisplayAlert("Permission Required", message, "OK");
        }
    }
}
