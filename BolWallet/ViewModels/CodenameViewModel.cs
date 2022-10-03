using Bol.Core.Abstractions;
using Bol.Core.Model;

namespace BolWallet.ViewModels;

public partial class CodenameViewModel : BaseViewModel
{
    private readonly ICodeNameService _codeNameService;

    public CodenameViewModel(
        INavigationService navigationService,
        CodenameFormDataProvider codenameFormDataProvider,
        ICodeNameService codeNameService)
        : base(navigationService)
    {
        CodenameFormDataProvider = codenameFormDataProvider;
        _codeNameService = codeNameService;
        Form = new CodenameForm();
    }

    [ObservableProperty]
    private CodenameForm _form;

    [ObservableProperty]
    private string _codename = " ";

    public CodenameFormDataProvider CodenameFormDataProvider { get; }

    [RelayCommand]
    private void Submit()
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

        Codename = _codeNameService.Generate(person);
    }
}