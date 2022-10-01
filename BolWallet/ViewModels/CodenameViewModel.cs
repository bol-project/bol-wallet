using BolWallet.Models;

namespace BolWallet.ViewModels;

public class CodenameViewModel
{
	private readonly INavigationService _navigationService;

	public CodenameViewModel(INavigationService navigationService)
	{
		_navigationService = navigationService;
		Form = new CreateCodenameForm();
	}

	public CreateCodenameForm Form { get; }
}