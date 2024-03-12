using Bol.Address;
using Bol.Core.Abstractions;
using Bol.Core.Model;
using CommunityToolkit.Maui.Alerts;
using ICSharpCode.SharpZipLib.Zip;

namespace BolWallet.ViewModels;
public partial class CertifierViewModel : BaseViewModel
{
	private readonly IBolService _bolService;
	private readonly IAddressTransformer _addressTransformer;

	public CertifierViewModel(
		INavigationService navigationService,
		IBolService bolService,
		IAddressTransformer addressTransformer) : base(navigationService)
	{
		_bolService = bolService;
		_addressTransformer = addressTransformer;
    }

	[ObservableProperty]
	private string _codeNameToCertify = "";

	[ObservableProperty]
	private string _mainAddressToWhitelist = "";

	[ObservableProperty]
	private BolAccount _bolAccount = new();

	[ObservableProperty]
	private bool _isLoading = false;

    [ObservableProperty]
    private string _certificationZipPath = "Select a certification documents zip file";

    [RelayCommand]
	private async Task Certify()
	{
		try
		{
			if (string.IsNullOrEmpty(CodeNameToCertify))
				throw new Exception("Please Select a CodeName");

			IsLoading = true;

			await Task.Delay(100);

			BolAccount = await _bolService.Certify(CodeNameToCertify);

			await Toast.Make($"{CodeNameToCertify} has received the certification.").Show();

		}
		catch (Exception ex)
		{
			await Toast.Make(ex.Message).Show();
		}
		finally
		{
			IsLoading = false;
			CodeNameToCertify = string.Empty;
		}
	}

	[RelayCommand]
	private async Task Whitelist()
	{
		try
		{
			if (string.IsNullOrEmpty(MainAddressToWhitelist))
				throw new Exception("Please Select a Main Address");

			IsLoading = true;

			await Task.Delay(100);

			var isWhitelisted = await _bolService.Whitelist(_addressTransformer.ToScriptHash(MainAddressToWhitelist));

			await Toast.Make($"Main Address {MainAddressToWhitelist} is whitelisted now.").Show();
		}
		catch (Exception ex)
		{
			await Toast.Make(ex.Message).Show();
		}
		finally
		{
			IsLoading = false;
			MainAddressToWhitelist = string.Empty;
		}
	}

    [RelayCommand]
    private async Task SelectZipFile(CancellationToken token)
    {
        try
        {
            var file = await FilePicker.PickAsync(new PickOptions
            {
                PickerTitle = "Select the zip file",
                FileTypes = new FilePickerFileType(new Dictionary<DevicePlatform, IEnumerable<string>>
                {
                    { DevicePlatform.iOS, new[] { "public.zip-archive" } }, // UTType for zip files in iOS
                    { DevicePlatform.Android, new[] { "application/zip" } }, // MIME type for zip files in Android
                    { DevicePlatform.WinUI, new[] { ".zip" } }, // file extension for zip files in Windows
                    { DevicePlatform.MacCatalyst, new[] { "public.zip-archive" } }, // UTType for zip files in macOS
                })
            });

            if (file is null)
                return;

            CertificationZipPath = file.FullPath;

            await Toast.Make($"Selected {CertificationZipPath}").Show(token);
        }
        catch (Exception ex)
        {
            await Toast.Make(ex.Message).Show(token);
        }
    }
    
    [RelayCommand]
    private async Task ValidateCertificationDocuments(CancellationToken token)
    {
        try
        {
            IsLoading = true;

            await using var stream = File.OpenRead(CertificationZipPath);
            ExtractZipFile(stream, CodeNameToCertify);

            await Toast.Make($"{CertificationZipPath} validated successfully").Show(token);
        }
        catch (Exception ex)
        {
            await Toast.Make(ex.Message).Show(token);
        }
        finally
        {
            IsLoading = false;
        }
    }
    
    private void ExtractZipFile(Stream fileStream, string password)
    {
        using (var zipInputStream = new ZipInputStream(fileStream))
        {
            if (!string.IsNullOrEmpty(password))
            {
                zipInputStream.Password = password;
            }

            ZipEntry entry;
            while ((entry = zipInputStream.GetNextEntry()) != null)
            {
                // Try to validate hash of entry with EDI hashes
                // For now, just throw away
                zipInputStream.CopyTo(Stream.Null);
            }
        }
    }
}
