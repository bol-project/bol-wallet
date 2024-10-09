using System.ComponentModel.DataAnnotations;

namespace BolWallet.Models
{
    public partial class GenericHashTableForm : ObservableObject
    {
        public string IdentityCard { get; set; }
        public string Passport { get; set; }
        public string ProofOfNin { get; set; }
        public string BirthCertificate { get; set; }

        public string DrivingLicense { get; set; }
        public string OtherIdentity { get; set; }
        public string FacePhoto { get; set; }
        public string PersonalVoice { get; set; }
        public string ProofOfCommunication { get; set; }
        public string ProofOfResidence { get; set; }
    }

    public class GenericSHA256TableForm : ObservableObject
    {
        private string _drivingLicense;

        [RegularExpression("^[A-F0-9]*$", ErrorMessage = "Only capital letters and numbers are allowed.")]
        [StringLength(64, MinimumLength = 64, ErrorMessage = "The SHA-256 hash must be exactly 64 characters.")]
        public string DrivingLicense
        {
            get => _drivingLicense;
            set => _drivingLicense = value?.ToUpper();
        }

        private string _otherIdentity;

        [RegularExpression("^[A-F0-9]*$", ErrorMessage = "Only capital letters and numbers are allowed.")]
        [StringLength(64, MinimumLength = 64, ErrorMessage = "The SHA-256 hash must be exactly 64 characters.")]
        public string OtherIdentity
        {
            get => _otherIdentity;
            set => _otherIdentity = value?.ToUpper();
        }

        private string _facePhoto;

        [RegularExpression("^[A-F0-9]*$", ErrorMessage = "Only capital letters and numbers are allowed.")]
        [StringLength(64, MinimumLength = 64, ErrorMessage = "The SHA-256 hash must be exactly 64 characters.")]
        public string FacePhoto
        {
            get => _facePhoto;
            set => _facePhoto = value?.ToUpper();
        }

        private string _personalVoice;

        [RegularExpression("^[A-F0-9]*$", ErrorMessage = "Only capital letters and numbers are allowed.")]
        [StringLength(64, MinimumLength = 64, ErrorMessage = "The SHA-256 hash must be exactly 64 characters.")]
        public string PersonalVoice
        {
            get => _personalVoice;
            set => _personalVoice = value?.ToUpper();
        }

        private string _proofOfCommunication;

        [RegularExpression("^[A-F0-9]*$", ErrorMessage = "Only capital letters and numbers are allowed.")]
        [StringLength(64, MinimumLength = 64, ErrorMessage = "The SHA-256 hash must be exactly 64 characters.")]
        public string ProofOfCommunication
        {
            get => _proofOfCommunication;
            set => _proofOfCommunication = value?.ToUpper();
        }

        private string _proofOfResidence;

        [RegularExpression("^[A-F0-9]*$", ErrorMessage = "Only capital letters and numbers are allowed.")]
        [StringLength(64, MinimumLength = 64, ErrorMessage = "The SHA-256 hash must be exactly 64 characters.")]
        public string ProofOfResidence
        {
            get => _proofOfResidence;
            set => _proofOfResidence = value?.ToUpper();
        }
    }
}
