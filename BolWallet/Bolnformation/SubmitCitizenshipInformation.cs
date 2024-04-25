namespace BolWallet.Bolnformation;

public static class SubmitCitizenshipInformation
{
    public const string Title = "Submit Citizenship Information";
    public const string Description =
        "Your files will remain exclusively on your device and will not be transferred " +
        "to any external servers or third parties. The only instance where your files will be shared " +
        "is directly with the certifying authority during the verification process. " +
        "No copies of your files will be made or stored elsewhere.";
    // public const string Content =
    //     "1. Identity Card (front): This is a mandatory document. Alternatively, you can submit your passport. \n" +
    //     "2. Identity Card (back): This is also a mandatory document if the Identity card is chosen. If it's not available, you can resubmit the front side of the identity card.  \n" +
    //     "3. Passport: This document can be used instead of the Identity Card. If you choose to use your passport, you do not need to provide the Identity Card.  \n" +
    //     "4. Birth Certificate: This document is used for registering children. If you're registering a child, please provide this document.  \n" +
    //     "You can always submit all the documents if you have them.  \n";
    public const string Content =
        "1. Identity Card (front): This is a mandatory document. Alternatively, you can submit your passport. <br/><br/>" +
        "2. Identity Card (back): This is also a mandatory document when the identity card is chosen. If it's not available, you can resubmit the front side of the identity card. <br/><br/>" +
        "3. Passport: This document can be used instead of the identity card. If you choose to use your passport, you do not need to provide the Identity Card. <br/><br/>" +
        "4. Birth Certificate: This document can also be used for registering children under 12. If you're registering a child, please provide this document. <br/><br/>" +
        "<strong>You can always submit all the documents if you have them.</strong> <br/><br/>";
}
