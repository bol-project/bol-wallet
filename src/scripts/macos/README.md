# BoLWallet Mac Catalyst Publishing Guide

- [BoLWallet Mac Catalyst Publishing Guide](#bolwallet-mac-catalyst-publishing-guide)
  - [Publish BolWallet for Mac Catalyst](#publish-bolwallet-for-mac-catalyst)
  - [Verifying app bundle and installer package work](#verifying-app-bundle-and-installer-package-work)
  - [Notarization of installer package](#notarization-of-installer-package)
  - [Notarization ticket stapling](#notarization-ticket-stapling)
  - [Notarization validation](#notarization-validation)
  - [Resources](#resources)

## Publish BolWallet for Mac Catalyst

`maccatalyst` supports both `x64` and `arm64` platforms.

Verify development/distribution/provisioning profiles are installed on the local machine and then use the `publish-maccatalyst*.sh` scripts to publish the project to generate the app bundle (`.app`) and an installer package (`.pkg`).

To publish a universal maccatalyst app bundle and installer package use `publish-maccatalyst.sh`. This will generate the files under `./../../bin/Release/net8.0-maccatalyst`.

Default publishing based on project settings for `Release` configuration and the `maccatalyst` platform creates an installer to distribute outside the App Store. If using this, then follow the guide below to do the necessary steps of notarizing the package installer and stapling the notarization ticket before distributing the installer.

If `publish-maccatalyst-x64.sh` or `publish-maccatalyst-arm64.sh` are used to generate an app bundler and installer package that targets the specific CPU architecture, then the files will be generated under `./../../bin/Release/net8.0-maccatalyst/maccatalyst-x64` and `./../../bin/Release/net8.0-maccatalyst/maccatalyst-arm64` respectively.

## Verifying app bundle and installer package work

Running the installer package will verify the app installs and runs as expected.

However, **while the installer package will work on other Macs**, the build machine won't show the `BolWallet.app` under `/Applications` even though the installation completes without issues.

For the app to appear as expected, copy the installer package outside the `bin` folder, for example in `~/Downloads` and then delete the `bin` folder.

## Notarization of installer package

`xcrun notary submit` is used to notarize the installer package. Since this requires setting AppStore Connect credentials, the `notary-store-creds.sh` script can be used to store a keychain profile with the required credentials under the profile name `bolwallet-notarytool-password`.

Then, to submit the installer package for notarization use `notary-submit-package.sh`, passing the path to the installer package (.pkg) file. A submission id will be created as the result of a successful notarization request. The process takes some time to complete.

To check on progress, use `notary-info.sh`, passing the submission id.

Alternatively, `notary-submit-package-and-wait.sh` can be used to both submit the notarization request and wait until it's completed.

After notarization has been completed, `notary-log.sh` can be used to check on the notarization result, passing the submision id as an argument. It's recommended to check the log even if notarization completed successfully.

## Notarization ticket stapling

After a successful notarization, a staple ticket is generated which can be used to staple the installer package.

This is an optional step, since Apple's Gatekeeper will be able to find the ticket online when the app runs for the first time after installation.

However, attaching the ticket to the installer package allows the Gatekeeper to find the ticket even when a network connection isn't available.

To attach the ticket use `notary-staple-ticket.sh` passing the installer package file path as an argument. This requires internet connectivity since the stapler retrieves the ticket online and then attached it to the installer package.

## Notarization validation

As an optional step, the installer package notarization can be validated using the `notary-validate.sh` script, passing the installer package file path as an argument.

## Resources

- [Publish a Mac Catalyst app outside the App Store](https://learn.microsoft.com/en-us/dotnet/maui/mac-catalyst/deployment/publish-outside-app-store?view=net-maui-8.0)
- [Customizing the notarization workflow](https://developer.apple.com/documentation/security/notarizing_macos_software_before_distribution/customizing_the_notarization_workflow)
