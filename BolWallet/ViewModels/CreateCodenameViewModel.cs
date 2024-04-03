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

    [ObservableProperty]
    protected bool _isLoading = false;
    
    [RelayCommand]
    public async Task Submit()
    {
        if (userData.IsIndividualRegistration)
            await NavigationService.NavigateTo<CreateEdiViewModel>(true);
        else
            await NavigationService.NavigateTo<CreateCompanyEdiViewModel>(true);
    }

    [Obsolete($"Use {nameof(CodenameExists)} instead.")]
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
            IsLoading = true;
            var alternatives = (await BolService.FindAlternativeCodeNames(codename, token)).ToArray();

            return alternatives.Length == 0
                ? Result.Success(CodenameExistsCheck.CodenameDoesNotExist())
                : Result.Success(CodenameExistsCheck.CodenameExists(alternatives));
        }
        catch (Exception e)
        {
            _logger.LogError(e, "Error checking if codename {Codename} exists", codename);
            return Result.CriticalError(e.Message);
        }
        finally
        {
            IsLoading = false;
        }
    }

    protected static DateTime GetBirthDate(string value) =>
        DateOnly.ParseExact(value, Constants.BirthDateFormat, CultureInfo.InvariantCulture)
            .ToDateTime(TimeOnly.MinValue, DateTimeKind.Utc);
}
