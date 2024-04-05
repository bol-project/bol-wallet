using CommunityToolkit.Maui.Alerts;
using CommunityToolkit.Maui.Core;
using Microsoft.Extensions.Logging;

namespace BolWallet.ViewModels;

public partial class CitizenshipViewModel : BaseViewModel
{
    private readonly RegisterContent _content;
    private readonly ISecureRepository _secureRepository;
    private readonly ILogger<CitizenshipViewModel> _logger;
    private readonly List<string> _allCountries;
    private UserData UserData { get; set; }
    
    [ObservableProperty]
    public CitizenshipsForm citizenshipsForm;

    public CitizenshipViewModel(
        RegisterContent content,
        ISecureRepository secureRepository,
        INavigationService navigationService,
        ILogger<CitizenshipViewModel> logger) : base(navigationService)
    {
        _content = content;
        _secureRepository = secureRepository;
        _logger = logger;
        _allCountries = _content.Countries.Select(country => country.Name).ToList();
        
        citizenshipsForm = new CitizenshipsForm();
    }

    public override async Task OnInitializedAsync()
    {
        UserData = await GetUserData();
        
        var citizenships = UserData.Citizenships.Select(citizenship => citizenship.Name).ToArray();
        
        CitizenshipsForm.FirstCountry = citizenships.FirstOrDefault() ?? string.Empty;
        CitizenshipsForm.SecondCountry = citizenships.Skip(1).FirstOrDefault() ?? string.Empty;
        CitizenshipsForm.ThirdCountry = citizenships.Skip(2).FirstOrDefault() ?? string.Empty;
        
        await base.OnInitializedAsync();
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

            var savedCitizenships = UserData
                .EncryptedCitizenshipForms
                .Where(c => SelectedCountries.Contains(c.CountryName));
            
            var newCitizenships = newCountries
                .Select(c => new EncryptedCitizenshipForm
                {
                    CountryName = c.Name,
                    CountryCode = c.Alpha3,
                    CitizenshipHashes = new Bol.Core.Model.CitizenshipHashTable(),
                    CitizenshipActualBytes = new Bol.Core.Model.CitizenshipHashTable(),
                    CitizenshipHashTableFileNames = new CitizenshipHashTableFileNames()
                });

            UserData.Citizenships = savedCountriesToKeep.Concat(newCountries).ToList();
            UserData.EncryptedCitizenshipForms = savedCitizenships.Concat(newCitizenships).ToList();
            UserData.EncryptedCitizenshipForms.ForEach(e => e.IsSubmitted = false);

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
