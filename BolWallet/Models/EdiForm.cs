namespace BolWallet.Models
{
	public partial class EdiForm : ObservableObject
	{
		public string IdentityCard { get; set; }
		public string Passport { get; set; }
		public string DrivingLicense { get; set; }
		public string Photo { get; set; }
		public string NinCertificate { get; set; }
		public string BirthCertificate { get; set; }
		public string TelephoneBill { get; set; }
		public string Other { get; set; }
		public string Voice { get; set; }
	}
}
