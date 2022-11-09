namespace BolWallet.Models
{
	public partial class EdiForm : ObservableObject
	{
        public byte[] IdentityCard { get; set; }
        public byte[] Passport { get; set; }
        public byte[] DrivingLicense { get; set; }
        public byte[] Photo { get; set; }
        public byte[] NinCertificate { get; set; }
        public byte[] BirthCertificate { get; set; }
        public byte[] TelephoneBill { get; set; }
        public byte[] Other { get; set; }
        public byte[] Voice { get; set; }
		public byte[] TextInfo { get; set; }
    }
}
