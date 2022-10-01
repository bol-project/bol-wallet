namespace BolWallet.ViewModels;

public class CodenameViewModel : BaseViewModel
{
    public CodenameViewModel(INavigationService navigationService) : base(navigationService)
    {
		Form = new CreateCodenameForm();
	}

	public CreateCodenameForm Form { get; }
}