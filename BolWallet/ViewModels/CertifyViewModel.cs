using Bol.Core.Abstractions;
using CommunityToolkit.Maui.Alerts;

namespace BolWallet.ViewModels;

public partial class CertifyViewModel : BaseViewModel
{
    private readonly IBolService _bolService;

    public CertifyViewModel(INavigationService navigationService, IBolService bolService) : base(navigationService)
    {
        _bolService = bolService;
    }

    [ObservableProperty]
    private bool _isLoading = false;

    [ObservableProperty]
    private string _codeName;

    public async Task Certify()
    {
        try
        {
            if (string.IsNullOrEmpty(CodeName))
                throw new Exception("Please Select a CodeName");

            IsLoading = true;

            await Task.Delay(100);

            await _bolService.Certify(CodeName);

            await Toast.Make($"{CodeName} has received the certification.").Show();
        }
        catch (Exception ex)
        {
            await Toast.Make(ex.Message).Show();
        }
        finally
        {
            IsLoading = false;
            CodeName = string.Empty;
        }
    }
}
