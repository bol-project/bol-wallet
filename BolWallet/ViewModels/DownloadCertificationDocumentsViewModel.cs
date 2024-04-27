using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CommunityToolkit.Maui.Alerts;
using Microsoft.Extensions.Logging;

namespace BolWallet.ViewModels
{
    public partial class DownloadCertificationDocumentsViewModel : BaseViewModel
    {
        private readonly ISecureRepository _secureRepository;
        private readonly IFileDownloadService _fileDownloadService;
        private readonly ILogger<DownloadCertificationDocumentsViewModel> _logger;

        public DownloadCertificationDocumentsViewModel(
            INavigationService navigationService,
            ISecureRepository secureRepository,
            IFileDownloadService fileDownloadService,
            ILogger<DownloadCertificationDocumentsViewModel> logger
            ) : base(navigationService)
        {
            _secureRepository = secureRepository;
            _fileDownloadService = fileDownloadService;
            _logger = logger;
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
    }
}
