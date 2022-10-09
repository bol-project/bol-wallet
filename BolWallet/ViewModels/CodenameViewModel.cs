using Bol.Core.Abstractions;
using Bol.Core.Model;

namespace BolWallet.ViewModels;

public partial class CodenameViewModel : BaseViewModel
{
    private readonly ICodeNameService _codeNameService;
	private readonly ISecureRepository _secureRepository;

	public CodenameViewModel(
        INavigationService navigationService,
        CodenameFormDataProvider codenameFormDataProvider,
        ICodeNameService codeNameService,
		ISecureRepository secureRepository)
        : base(navigationService)
    {
        CodenameFormDataProvider = codenameFormDataProvider;
        _codeNameService = codeNameService;
		_secureRepository = secureRepository;
		Form = new CodenameForm();
    }

    [ObservableProperty]
    private CodenameForm _form;

    [ObservableProperty]
    private string _codename = " ";

    public CodenameFormDataProvider CodenameFormDataProvider { get; }

    [RelayCommand]
    private async Task Submit()
    {
		
		var person = new NaturalPerson
        {
            FirstName = Form.Firstname,
            MiddleName = Form.MiddleName,
            Surname = Form.Surname,
            ThirdName = Form.ThirdName,
            CountryCode = Form.Country,
            Gender = Form.Gender,
            Combination = Form.Combination,
            Nin = "1234567",
            Birthdate = Form.Birthday.Value
        };

        var result = _codeNameService.Generate(person);
		
		await _secureRepository.SetAsync("codename", result);
		
		var codename = await _secureRepository.GetAsync("codename");
		
		Codename = codename;
    }
}