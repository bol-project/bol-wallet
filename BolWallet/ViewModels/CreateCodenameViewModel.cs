using System.Globalization;
using Bol.Core.Abstractions;
using Microsoft.Extensions.Logging;
using SimpleResults;

namespace BolWallet.ViewModels;

public partial class CreateCodenameViewModel : BaseViewModel
{
    protected readonly ICodeNameService _codeNameService;
    protected readonly ISecureRepository _secureRepository;
    protected readonly IBolRpcService BolRpcService;
    protected readonly IBolService BolService;
    private readonly ILogger _logger;
    
    public CreateCodenameViewModel(
        INavigationService navigationService,
        ICodeNameService codeNameService,
        ISecureRepository secureRepository,
        IBolRpcService bolRpcService,
        IBolService bolService,
        ILogger logger)
        : base(navigationService)
    {
        _codeNameService = codeNameService;
        _secureRepository = secureRepository;
        BolRpcService = bolRpcService;
        BolService = bolService;
        _logger = logger;
    }

    [ObservableProperty]
    protected string _codename = " ";
    
    [RelayCommand]
    public async Task Submit()
    {
        if (userData.IsIndividualRegistration)
            await NavigationService.NavigateTo<CreateEdiViewModel>(true);
        else
            await NavigationService.NavigateTo<CreateCompanyEdiViewModel>(true);
    }

    protected async Task<Result<bool>> CheckCodenameExists(string codename, CancellationToken token = default)
    {
        var result = await BolRpcService.GetBolAccount(codename, token);
        if (result.IsSuccess)
        {
            return Result.Success(true);
        }

        if (result.Status == ResultStatus.NotFound)
        {
            return Result.Success(false);
        }

        return Result.CriticalError(result.Message, result.Errors);
    }

    protected async Task<Result<CodenameExistsCheck>> CodenameExists(string codename, CancellationToken token = default)
    {
        try
        {
            var exists = await BolService.CodeNameExists(codename, token);
            if (!exists)
            {
                return Result.Success(CodenameExistsCheck.CodenameDoesNotExist());
            }

            var alternatives = await BolService.FindAlternativeCodeNames(codename, token);

            return Result.Success(CodenameExistsCheck.CodenameExists(alternatives));
        }
        catch (Exception e)
        {
            _logger.LogError(e, "Error checking if codename {Codename} exists", codename);
            return Result.CriticalError(e.Message);
        }
    }

    protected static DateTime GetBirthDate(string value) =>
        DateOnly.ParseExact(value, Constants.BirthDateFormat, CultureInfo.InvariantCulture)
            .ToDateTime(TimeOnly.MinValue, DateTimeKind.Utc);
}
