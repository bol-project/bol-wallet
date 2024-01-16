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

    public EdiFileItem ProofOfNin { get; set; }

    public EdiFileItem BirthCertificate { get; set; }

    public EdiFileItem DrivingLicense { get; set; }

    public EdiFileItem OtherIdentity { get; set; }

    public EdiFileItem FacePhoto { get; set; }

    public EdiFileItem PersonalVoice { get; set; }

    public EdiFileItem ProofOfCommunication { get; set; }

    public EdiFileItem ProofOfResidence { get; set; }
}
