using DevExpress.Maui.DataForm;

namespace BolWallet.Services
{
	public class CodenameFormDataProvider : IPickerSourceProvider
	{
		private readonly ICountriesService _countriesService;

		public CodenameFormDataProvider(ICountriesService countriesService)
		{
			_countriesService = countriesService;
		}

		public IEnumerable GetSource(string propertyName)
		{
			return propertyName switch
			{
				nameof(CodenameForm.Country) =>  _countriesService.Get()
					.Select(c => c.Name)
					.ToArray(),
				_ => null
			};
		}
	}
}