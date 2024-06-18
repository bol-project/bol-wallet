namespace BolWallet.Models;

public class FileItem
{
    public byte[] Content { get; set; }
    public string FileName { get; set; }
}

public class GenericHashTableFiles
{
    public FileItem DrivingLicense { get; set; }

    public FileItem OtherIdentity { get; set; }

    public FileItem FacePhoto { get; set; }

    public FileItem PersonalVoice { get; set; }

    public FileItem ProofOfCommunication { get; set; }

    public FileItem ProofOfResidence { get; set; }
}

public class CompanyHashFiles
{
    public FileItem IncorporationCertificate { get; set; }
    public FileItem MemorandumAndArticlesOfAssociation { get; set; }
    public FileItem RepresentationCertificate { get; set; }
    public FileItem TaxRegistrationCertificate { get; set; }
    public FileItem ChambersRecords { get; set; }
    public FileItem RegisterOfShareholders { get; set; }
    public FileItem ProofOfVatNumber { get; set; }
    public FileItem ProofOfAddress { get; set; }
}
