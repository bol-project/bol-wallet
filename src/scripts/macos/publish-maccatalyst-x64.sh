#!/bin/sh

dotnet build -f net10.0-maccatalyst -c Release -p:RuntimeIdentifier=maccatalyst-x64 ./../../BolWallet.csproj