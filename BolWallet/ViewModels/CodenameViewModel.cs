namespace BolWallet.ViewModels;

public partial class CodenameViewModel : BaseViewModel
{
	public CodenameViewModel(INavigationService navigationService, CodenameFormDataProvider codenameFormDataProvider)
		: base(navigationService)
	{
		CodenameFormDataProvider = codenameFormDataProvider;
		Form = new CodenameForm();
	}

	public CodenameForm Form { get; }
	public CodenameFormDataProvider CodenameFormDataProvider { get; }
}