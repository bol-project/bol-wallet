#!/bin/sh

dotnet publish -f net9.0-ios -c Release -p:ArchiveOnBuild=true ./../../BolWallet/BolWallet.csproj