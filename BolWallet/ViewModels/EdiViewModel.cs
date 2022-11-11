using Bol.Core.Abstractions;
using Bol.Core.Model;
using Bol.Cryptography;
using FluentValidation;
using Microsoft.Maui.Storage;
using Plugin.AudioRecorder;
using System.Runtime.Intrinsics.X86;
using MediaPicker = Microsoft.Maui.Media.MediaPicker;

namespace BolWallet.ViewModels;
public partial class EdiViewModel : BaseViewModel
{
	private readonly IPermissionService _permissionService;
	private readonly IBase16Encoder _base16Encoder;
	private readonly ISecureRepository _secureRepository;
	private readonly IEncryptedDigitalIdentityService _encryptedDigitalIdentityService;

	private EncryptedDigitalMatrix encryptedDigitalMatrix;

	AudioRecorderService recorder;

	public EdiViewModel(INavigationService navigationService,
		IPermissionService permissionService,
		IBase16Encoder base16Encoder,
		ISecureRepository secureRepository,
		IEncryptedDigitalIdentityService encryptedDigitalIdentityService)
		: base(navigationService)
	{
		_permissionService = permissionService ?? throw new ArgumentNullException(nameof(permissionService));
		_base16Encoder = base16Encoder ?? throw new ArgumentNullException(nameof(base16Encoder));
		_secureRepository = secureRepository ?? throw new ArgumentNullException(nameof(secureRepository));
		_encryptedDigitalIdentityService = encryptedDigitalIdentityService ?? throw new ArgumentNullException(nameof(encryptedDigitalIdentityService));

		EdiForm = new EdiForm();
		recorder = new AudioRecorderService
		{
			AudioSilenceTimeout = TimeSpan.FromMilliseconds(5000),
			TotalAudioTimeout = TimeSpan.FromMilliseconds(5000),
		};
		encryptedDigitalMatrix = new EncryptedDigitalMatrix();
	}

	[ObservableProperty]
	private EdiForm _ediForm;

	[ObservableProperty]
	private EdiForm _ediFormPaths;

	[RelayCommand]
	private async Task PickPhotoAsync(string propertyName)
	{
		if (!(await _permissionService.CheckStoragePermission())) return;
		var customFileType =
			new FilePickerFileType(new Dictionary<DevicePlatform, IEnumerable<string>>
		{
					 { DevicePlatform.iOS, new[] { "com.adobe.pdf" } },
					 { DevicePlatform.Android, new[] { "application/pdf", "image/*","audio/*" } },
					 { DevicePlatform.Tizen, new[] { "*/*" } },
					 { DevicePlatform.macOS, new[] { "pdf"} },
			});
		var pickResult = await FilePicker.PickAsync(new PickOptions
		{
			FileTypes = customFileType,
			PickerTitle = "Pick a file"
		});

		PathPerImport(propertyName, pickResult);

	}
	[RelayCommand]
	private async Task TakePhotoAsync(string propertyName)
	{
		if (!(await _permissionService.CheckCameraPermission())) return;

		var takePictureResult = await MediaPicker.CapturePhotoAsync();

		PathPerImport(propertyName, takePictureResult);
	}
	[RelayCommand]
	private async Task RecordAudio()
	{
		if (!await _permissionService.CheckSpeechPermission()) return;
		try
		{
			if (!recorder.IsRecording)
			{
				var audioRecordTask = await recorder.StartRecording();
				var audiofile = await audioRecordTask;

				if (audiofile != null)
				{
					var fileBytes = File.ReadAllBytes(audiofile);
					var encodedFileBytes = _base16Encoder.Encode(fileBytes);
					encryptedDigitalMatrix.Hashes.Voice = encodedFileBytes;
					EdiForm.VoicePath = audiofile;
					OnPropertyChanged(nameof(EdiFormPaths));
				}
				else throw new();
			}
			else
			{
				await recorder.StopRecording();
			}
		}
		catch (Exception ex)
		{
			throw new InvalidOperationException(ex.Message);
		}
	}

	[RelayCommand]
	private async Task StopRecordAudio()
	{
		if (!recorder.IsRecording) return;
		try
		{
			await recorder.StopRecording();
		}
		catch (Exception ex)
		{
			throw new InvalidOperationException(ex.Message);
		}
	}

	private void PathPerImport(string propertyName, FileResult fileResult)
	{
		string encodedFileBytes = null;
		encryptedDigitalMatrix.Hashes = encryptedDigitalMatrix.Hashes ?? new HashTable();
		if (fileResult != null)
		{
			var fileBytes = File.ReadAllBytes(fileResult.FullPath);
			encodedFileBytes = _base16Encoder.Encode(fileBytes);

		}
		else return;

		switch (propertyName)
		{
			case nameof(EdiFormPaths.IdentityCardPath):
				{
					encryptedDigitalMatrix.Hashes.IdentityCard = encodedFileBytes;
					EdiForm.IdentityCardPath = fileResult.FullPath;
					OnPropertyChanged(nameof(EdiForm));
					break;
				}
			case nameof(EdiFormPaths.PassportPath):
				{
					encryptedDigitalMatrix.Hashes.Passport = encodedFileBytes;
					EdiForm.PassportPath = fileResult.FullPath;
					OnPropertyChanged(nameof(EdiForm));
					break;
				}
			case nameof(EdiFormPaths.DrivingLicencePath):
				{
					encryptedDigitalMatrix.Hashes.DrivingLicense = encodedFileBytes;
					EdiForm.DrivingLicencePath = fileResult.FullPath;
					OnPropertyChanged(nameof(EdiForm));
					break;
				}
			case nameof(EdiFormPaths.PhotoPath):
				{
					encryptedDigitalMatrix.Hashes.Photo = encodedFileBytes;
					EdiForm.PhotoPath = fileResult.FullPath;
					OnPropertyChanged(nameof(EdiForm));
					break;
				}
			case nameof(EdiFormPaths.NinCertificatePath):
				{
					encryptedDigitalMatrix.Hashes.NinCertificate = encodedFileBytes;
					EdiForm.NinCertificatePath = fileResult.FullPath;
					OnPropertyChanged(nameof(EdiForm));
					break;
				}
			case nameof(EdiFormPaths.BirthCertificatePath):
				{
					encryptedDigitalMatrix.Hashes.BirthCertificate = encodedFileBytes;
					EdiForm.BirthCertificatePath = fileResult.FullPath;
					OnPropertyChanged(nameof(EdiForm));
					break;
				}
			case nameof(EdiFormPaths.TelephoneBillPath):
				{
					encryptedDigitalMatrix.Hashes.TelephoneBill = encodedFileBytes;
					EdiForm.TelephoneBillPath = fileResult.FullPath;
					OnPropertyChanged(nameof(EdiForm));
					break;
				}
			case nameof(EdiFormPaths.OtherPath):
				{
					encryptedDigitalMatrix.Hashes.Other = encodedFileBytes;
					EdiForm.OtherPath = fileResult.FullPath;
					OnPropertyChanged(nameof(EdiForm));
					break;
				}
			case nameof(EdiFormPaths.VoicePath):
				{
					encryptedDigitalMatrix.Hashes.Voice = encodedFileBytes;
					EdiForm.VoicePath = fileResult.FullPath;
					OnPropertyChanged(nameof(EdiForm));
					break;
				}
		}
	}
	[RelayCommand]
	private async Task Submit()
	{
		if ((string.IsNullOrEmpty(encryptedDigitalMatrix.Hashes.DrivingLicense) || string.IsNullOrEmpty(encryptedDigitalMatrix.Hashes.IdentityCard) || string.IsNullOrEmpty(encryptedDigitalMatrix.Hashes.Passport)))
		{
			return;
		}

		var result = _encryptedDigitalIdentityService.Generate(encryptedDigitalMatrix);

		await _secureRepository.SetAsync("edi", result);
	}
}
