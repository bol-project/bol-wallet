namespace BolWallet.Models.Messages;

internal record DisplayErrorMessage(string Message, Exception Exception = null);
