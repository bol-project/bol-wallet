namespace BolWallet.Models;

public class GenericHashTableFileItem
{
    public byte[] Content { get; set; }
    public string FileName { get; set; }
}

public class GenericHashTableFiles
{
    public GenericHashTableFileItem DrivingLicense { get; set; }

    public GenericHashTableFileItem OtherIdentity { get; set; }

    public GenericHashTableFileItem FacePhoto { get; set; }

    public GenericHashTableFileItem PersonalVoice { get; set; }

    public GenericHashTableFileItem ProofOfCommunication { get; set; }

    public GenericHashTableFileItem ProofOfResidence { get; set; }
}
