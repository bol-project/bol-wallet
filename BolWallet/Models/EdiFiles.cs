namespace BolWallet.Models;

public class EdiFileItem
{
    public byte[] Content { get; set; }
    public string FileName { get; set; }
}

public class EdiFiles
{
    public EdiFileItem IdentityCard { get; set; }

    public EdiFileItem Passport { get; set; }

    public EdiFileItem DrivingLicense { get; set; }

    public EdiFileItem Photo { get; set; }

    public EdiFileItem Voice { get; set; }

    public EdiFileItem NinCertificate { get; set; }

    public EdiFileItem BirthCertificate { get; set; }

    public EdiFileItem TelephoneBill { get; set; }

    public EdiFileItem Other { get; set; }
}
