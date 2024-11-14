namespace BolWallet.Services;

public class AppVersion : IAppVersion
{
    public string GetVersion() => $"{AppInfo.Current.VersionString} ({AppInfo.Current.BuildString})";
}
