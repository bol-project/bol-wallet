#!/bin/sh

dotnet publish -f net10.0-ios -c Release -p:ArchiveOnBuild=true ./../../BolWallet/BolWallet.csproj