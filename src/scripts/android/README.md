# BoLWallet Android Publishing Guide

- [BoLWallet Android Publishing Guide](#bolwallet-android-publishing-guide)
  - [Publish BolWallet for Android](#publish-bolwallet-for-android)
  - [Binaries](#binaries)
  - [Resources](#resources)

## Publish BolWallet for Android

A script is provided to publish the application in release mode for Android and produce a signed bundle. It requires providing two arguments, one of the keystore file location and one for the keystore password. The command is shown below:

```sh
dotnet publish -f net8.0-android -c Release -p:AndroidKeyStore=true -p:AndroidSigningKeyStore=$1 -p:AndroidSigningKeyAlias=bolwallet -p:AndroidSigningKeyPass=$2 -p:AndroidSigningStorePass=$2 ./../../BolWallet/BolWallet.csproj
```

It assumes that the keystore file used contains a certificate with the alias `bolwallet` and that both the keystore file and the certificate have the same password.

Assuming the keystore file path is `~/.keystore` and its password is `password`, execute the command below:

```sh
./publish-android-ad-hoc.sh ~/.keystore password
```

To avoid typing the plain password, it is recommended to store it in a file and use its path as the second argument using the `file:` prefix. For example if the password is contained in the under a hypothetical `myuser` user's folder, like `/Users/myuser/.keystore_password.txt` file the execute the command below:

```sh
./publish-android-ad-hoc.sh ~/.keystore file:/Users/myuser/.keystore_password.txt
```

> Note that when using `file:` the full path should be used. For example, using `~/.keystore_password.txt` won't be fully unrolled by the dotnet publish command. The keystore file path can still use the home directory shortcut `~` character.

## Binaries

Both an `apk` and `aab` bundle will be published under `/bin/Release/net8.0-android`. Look for the files using the `-Signed` suffix, for example:

- `<BolWallet_Project_Path>/bin/Release/net8.0-android/publish/org.bol.bolwallet-Signed.aab`
- `<BolWallet_Project_Path>/bin/Release/net8.0-android/publish/org.bol.bolwallet-Signed.apk`

## Resources

- [Publish an Android app using the command line](https://learn.microsoft.com/en-us/dotnet/maui/android/deployment/publish-cli?view=net-maui-8.0)
