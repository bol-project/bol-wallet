using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using BolWallet.Bolnformation;
using BolWallet.Components;
using System.Threading.Tasks;
using CommunityToolkit.Maui.Alerts;
using Microsoft.Extensions.Logging;
using MudBlazor;

namespace BolWallet.ViewModels
{
    public partial class DownloadCertificationDocumentsViewModel : BaseViewModel
    {
        private readonly ISecureRepository _secureRepository;
        private readonly IFileDownloadService _fileDownloadService;
        private readonly ILogger<DownloadCertificationDocumentsViewModel> _logger;
        private readonly IDialogService _dialogService;

        public DownloadCertificationDocumentsViewModel(
            INavigationService navigationService,
            ISecureRepository secureRepository,
            IFileDownloadService fileDownloadService,
            ILogger<DownloadCertificationDocumentsViewModel> logger,
            IDialogService dialogService
            ) : base(navigationService)
        {
            _secureRepository = secureRepository;
            _fileDownloadService = fileDownloadService;
            _logger = logger;
            _dialogService = dialogService;
        }

        [ObservableProperty] private List<FileItem> _files;

        public async Task OnInitializeAsync(CancellationToken cancellationToken = default)
        {
            try
            {
                userData = await _secureRepository.GetAsync<UserData>("userdata");

                if (userData.IsIndividualRegistration)
                    Files = _fileDownloadService.CollectIndividualFilesForDownload(userData);
                else
                    Files = _fileDownloadService.CollectCompanyFilesForDownload(userData);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error initializing DownloadCertificationDocumentsViewModel");
                await Toast.Make(ex.Message).Show(cancellationToken);
            }
        }

        [RelayCommand]
        public async Task DownloadAllDocuments(List<FileItem> files)
        {
            var ediZipFiles = await _fileDownloadService.CreateZipFileAsync(files);

            await _fileDownloadService.SaveZipFileAsync(ediZipFiles);
        }

        [RelayCommand]
        public async Task DownloadDocument(FileItem file)
        {
            await _fileDownloadService.DownloadDataAsync(file.Content, file.FileName);
        }

        public void OpenGeneratedDocumentsMoreInfo()
        {
            var parameters = new DialogParameters<MoreInfoDialog>
        {
            { x => x.Title, GeneratedDocumentsInformation.Title },
            { x => x.Paragraph1, GeneratedDocumentsInformation.Description },
            { x => x.Paragraph2, GeneratedDocumentsInformation.Content }
        };

            var options = new DialogOptions { CloseOnEscapeKey = true };

            _dialogService.Show<MoreInfoDialog>("", parameters, options);
        }
    }
}
