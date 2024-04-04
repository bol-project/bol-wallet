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
            { FirstCountry, SecondCountry, ThirdCountry }
        .Where(country => !string.IsNullOrEmpty(country))
        .ToList();
    
    public Task<IEnumerable<string>> FilterCountries(string filter)
    {
        if (string.IsNullOrEmpty(filter)) 
        {
            return Task.FromResult(_allCountries.AsEnumerable());
        }

        var filteredCountries = _allCountries
            .Where(country => country.Contains(filter, StringComparison.OrdinalIgnoreCase))
            .Where(country => !SelectedCountries.Contains(country)); // exclude already selected countries
        
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
            var totalCitizenships = SelectedCountries.Count + UserData.EncryptedCitizenshipForms.Count;

            if (totalCitizenships > 3)
            {
                await Toast
                    .Make($"" +
                          $"You have exceeded the maximum allowed citizenships. Please, choose up to three.",
                        ToastDuration.Long)
                    .Show();
                throw new ArgumentException(
                    "You have exceeded the maximum allowed citizenships. Please, choose up to three.");
            }

            var savedCountryNames = UserData
                .EncryptedCitizenshipForms
                .Select(form => form.CountryName)
                .ToArray();

            var newCountriesSelected = SelectedCountries.Except(savedCountryNames).ToArray();

            var newCountries = _content
                .Countries
                .Where(c => newCountriesSelected.Contains(c.Name))
                .ToArray();

            var newCitizenshipForms = newCountries
                .Select(c => new EncryptedCitizenshipForm
                {
                    CountryName = c.Name,
                    CountryCode = c.Alpha3,
                    CitizenshipHashes = new Bol.Core.Model.CitizenshipHashTable(),
                    CitizenshipActualBytes = new Bol.Core.Model.CitizenshipHashTable(),
                    CitizenshipHashTableFileNames = new CitizenshipHashTableFileNames(),
                    FirstName = string.Empty,
                    SecondName = string.Empty,
                    ThirdName = string.Empty,
                    SurName = string.Empty,
                    Nin = string.Empty
                }).ToArray();

            UserData.Citizenships.AddRange(newCountries);
            UserData.EncryptedCitizenshipForms.AddRange(newCitizenshipForms);

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
