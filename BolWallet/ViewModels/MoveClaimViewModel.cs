namespace BolWallet.ViewModels;
public partial class MoveClaimViewModel : BaseViewModel
{
    public string MoveClaimLabel => "Move Claim";
    public string ToComAddressText => "To my Commercial Address";
    public string AmountText => "Amount";
    public string MoveText => "Move";

    [ObservableProperty]
    private MoveClaimForm _moveClaimForm;

    [ObservableProperty]
    public string _testLabel;

    public MoveClaimViewModel(INavigationService navigationService) : base(navigationService)
    {
        MoveClaimForm = new MoveClaimForm();
    }

    [RelayCommand]
    private void MoveClaim()
    {
        TestLabel = "To Address: " + MoveClaimForm.ComAddress +
                    "\nAmount: " + MoveClaimForm.Amount;
    }
}
