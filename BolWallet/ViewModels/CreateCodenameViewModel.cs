using Bol.Core.Abstractions;
using Bol.Core.Model;
using Microsoft.Extensions.Options;
using SimpleResults;

namespace BolWallet.ViewModels;

public partial class CreateCodenameViewModel : BaseViewModel
{
    protected readonly ICodeNameService _codeNameService;
    protected readonly ISecureRepository _secureRepository;
    protected readonly IBolRpcService BolRpcService;
    private readonly IOptions<BolConfig> _bolConfig;

    public CreateCodenameViewModel(
        INavigationService navigationService,
        ICodeNameService codeNameService,
        RegisterContent content,
        ISecureRepository secureRepository,
        IBolRpcService bolRpcService,
        IOptions<BolConfig> bolConfig)
        : base(navigationService)
    {
        _codeNameService = codeNameService;
        _secureRepository = secureRepository;
        BolRpcService = bolRpcService;
        _bolConfig = bolConfig;
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
}
