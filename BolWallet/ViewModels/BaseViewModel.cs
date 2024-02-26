using Blazing.Mvvm.ComponentModel;
using Plugin.Fingerprint.Abstractions;
using Plugin.Fingerprint;

namespace BolWallet.ViewModels;

public class BaseViewModel : ViewModelBase
{
    protected readonly INavigationService NavigationService;

    private readonly IFingerprint _fingerprint;

    public UserData userData;

	protected BaseViewModel(INavigationService navigationService)
    {
        NavigationService = navigationService;
    }

    protected BaseViewModel(INavigationService navigationService, IFingerprint fingerprint)
    {
        NavigationService = navigationService;
        _fingerprint = fingerprint;
    }

    protected async Task<bool> FingerprintAuthAsync()
    {
        if (_fingerprint != null)
        {

            var isBiometricAvailable = await _fingerprint.IsAvailableAsync();

            if (isBiometricAvailable)
            {
                var request = new AuthenticationRequestConfiguration
                ("Login using biometrics", "Confirm login with your biometrics");

                var result = await _fingerprint.AuthenticateAsync(request);

                return result.Authenticated;
            }
        }

        return true;
    }
}
