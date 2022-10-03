using DevExpress.Maui.DataForm;

namespace BolWallet.Services
{
	public class CodenameFormDataProvider : IPickerSourceProvider
	{
		private readonly RegisterContent _content;

		public CodenameFormDataProvider(RegisterContent content)
		{
			_content = content;
		}

		public IEnumerable GetSource(string propertyName)
		{
			return propertyName switch
			{
				nameof(CodenameForm.Country) => _content.Countries.ToArray(),
				_ => null
			};
		}
	}
}