namespace BolWallet.Models
{
	public partial class EdiFormPaths : ObservableObject
	{
		public string IdentityCardPath { get; set; }
		public string PassportPath { get; set; }
		public string DrivingLicencePath { get; set; }
		public string PhotoPath { get; set; }
		public string NinCertificatePath { get; set; }
		public string BirthCertificatePath { get; set; }
		public string TelephoneBillPath { get; set; }
		public string OtherPath { get; set; }
		public string VoicePath { get; set; }
	}
}
