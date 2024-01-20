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
}
