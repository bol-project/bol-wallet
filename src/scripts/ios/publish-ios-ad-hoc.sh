#!/bin/sh

dotnet publish -f net8.0-ios -c Release -p:ArchiveOnBuild=true ./../../BolWallet/BolWallet.csproj