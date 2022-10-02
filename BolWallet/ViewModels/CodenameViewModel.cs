namespace BolWallet.ViewModels;

public class CodenameViewModel : BaseViewModel
{
	public CodenameViewModel(INavigationService navigationService, CodenameFormDataProvider codenameFormDataProvider)
		: base(navigationService)
	{
		CodenameFormDataProvider = codenameFormDataProvider;
		Form = new CodenameForm { DateOfBirth = DateTime.Today };
	}

	public CodenameForm Form { get; }
	public CodenameFormDataProvider CodenameFormDataProvider { get; }
}