using BolWallet.Bolnformation;
using BolWallet.Components;
using CommunityToolkit.Maui.Alerts;
using CommunityToolkit.Maui.Core;
using Microsoft.Extensions.Logging;
using MudBlazor;

namespace BolWallet.ViewModels;

public partial class CitizenshipViewModel : BaseViewModel
{
    private readonly RegisterContent _content;
    private readonly ISecureRepository _secureRepository;
    private readonly IDialogService _dialogService;
    private readonly ILogger<CitizenshipViewModel> _logger;
    private readonly List<string> _allCountries;
    private UserData UserData { get; set; }
    
    [ObservableProperty]
    public CitizenshipsForm citizenshipsForm;

    [ObservableProperty] bool _isLoading;

    public CitizenshipViewModel(
        RegisterContent content,
        ISecureRepository secureRepository,
        INavigationService navigationService,
        IDialogService dialogService,
        ILogger<CitizenshipViewModel> logger) : base(navigationService)
    {
        _content = content;
        _secureRepository = secureRepository;
        _dialogService = dialogService;
        _logger = logger;
        _allCountries = _content.Countries.Select(country => country.Name).ToList();
        
        citizenshipsForm = new CitizenshipsForm();
    }

    public override async Task OnInitializedAsync()
    {
        IsLoading = true;
        UserData = await GetUserData();
        
        var citizenships = UserData.Citizenships.Select(citizenship => citizenship.Name).ToArray();
        
        CitizenshipsForm.FirstCountry = citizenships.FirstOrDefault() ?? string.Empty;
        CitizenshipsForm.SecondCountry = citizenships.Skip(1).FirstOrDefault() ?? string.Empty;
        CitizenshipsForm.ThirdCountry = citizenships.Skip(2).FirstOrDefault() ?? string.Empty;
        
        await base.OnInitializedAsync();
        IsLoading = false;
    }
    
    public void OpenMoreInfo()
    {
        var parameters = new DialogParameters<MoreInfoDialog>
        {
            { x => x.Title, SelectCountryInformation.Title },
            { x => x.Paragraph1, SelectCountryInformation.Description },
            {χ => χ.Paragraph2, SelectCountryInformation.Content}
        };

        var options = new DialogOptions { CloseOnEscapeKey = true };
        
        _dialogService.Show<MoreInfoDialog>("", parameters, options);
    }

    [RelayCommand]
    private async Task<UserData> GetUserData()
    {
        try
        {
            UserData = await _secureRepository.GetAsync<UserData>("userdata");

            return UserData ?? new UserData { Citizenships = [], EncryptedCitizenshipForms = [] };
        }
        catch (Exception e)
        {
            _logger.LogError(e, "Error getting user data");
            throw;
        }
    }
    
    public List<string> SelectedCountries => new List<string> 
            { CitizenshipsForm.FirstCountry, CitizenshipsForm.SecondCountry, CitizenshipsForm.ThirdCountry }
        .Where(country => !string.IsNullOrEmpty(country))
        .ToList();
    
    public Task<IEnumerable<string>> FilterCountries(string filter)
    {
        var filteredCountries = _allCountries
            .Where(country => !SelectedCountries.Contains(country));
        
        if (!string.IsNullOrEmpty(filter)) 
        {
            filteredCountries = filteredCountries
                .Where(country => country.Contains(filter, StringComparison.OrdinalIgnoreCase));
        }
        
        return Task.FromResult(filteredCountries);
    }

    public void RemoveCountry(string country)
    {
        switch (country)
        {
            case var _ when country.Equals(CitizenshipsForm.FirstCountry):
                CitizenshipsForm.FirstCountry = "";
                break;
            case var _ when country.Equals(CitizenshipsForm.SecondCountry):
                CitizenshipsForm.SecondCountry = "";
                break;
            case var _ when country.Equals(CitizenshipsForm.ThirdCountry):
                CitizenshipsForm.ThirdCountry = "";
                break;
        }
    }
    
    [RelayCommand]
    private async Task TrySubmitForm(CitizenshipsForm context)
    {
        try
        {
            var savedCountriesToKeep = UserData
                .Citizenships
                .Where(c => SelectedCountries.Contains(c.Name))
                .ToArray();

            var newCountries = _content
                .Countries
                .Where(c => SelectedCountries.Contains(c.Name))
                .Where(c => !savedCountriesToKeep.Select(sc => sc.Name).Contains(c.Name)) // exclude already saved countries
                .ToArray();

            var selectionOrder = SelectedCountries
                .Select((country, index) => (c: country, i: index))
                .ToDictionary(c => c.c, c => c.i);

            // We want the countries to be in the same order as the user selected them
            // Otherwise, if the order randomly changes, EDI creation will not be deterministic.
            UserData.Citizenships = savedCountriesToKeep
                .Concat(newCountries)
                .OrderBy(c => selectionOrder[c.Name])
                .ToList();

            await _secureRepository.SetAsync("userdata", UserData);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error submitting citizenship form.");
            await Toast.Make(ex.Message, ToastDuration.Long).Show();
            throw;
        }
    }
}
