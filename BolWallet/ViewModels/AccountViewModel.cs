using Bol.Core.Model;
using Newtonsoft.Json;

namespace BolWallet.ViewModels;

public partial class AccountViewModel : BaseViewModel
{
    private BolAccount _bolAccount;


    public AccountViewModel(INavigationService navigationService) : base(navigationService)
    {
        InitAsync();
    }

    [ObservableProperty]
    private CodenameForm _codenameForm;

    public string CodenameLabel => "Greetings " + _bolAccount?.CodeName + "!";

    public string EdiLabel => "Edi: " + _bolAccount?.Edi;
    public string Address { get; set; }
    // TODO section

    //Replace 2nd string with var
    public string BolLabel => "Bol: " + _bolAccount?.TotalBalance;

    //Replace button text and var name for both below

    public string TransactionsButtonLabel => "Transactions";

    //Implement the navigation commands after creation of their respective views

    [RelayCommand]
    private void NavigateToTransactionsPage()
    {
        NavigationService.NavigateTo<TransactionsViewModel>(true);
    }

    [RelayCommand]
    private void NavigateToTSettingsPage()
    {
        //NavigationService.NavigateTo<>(true);
    }

    private async Task InitAsync()
    {
        string filePath = "TestAccount.json";
        var stream = await FileSystem.OpenAppPackageFileAsync(filePath);

        if (stream != null)
        {
            string str = (new System.IO.StreamReader(stream)).ReadToEnd();
            _bolAccount = JsonConvert.DeserializeObject<BolAccount>(str);
        }
    }

}