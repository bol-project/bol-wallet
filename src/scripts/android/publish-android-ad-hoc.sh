#!/bin/sh

dotnet publish -f net9.0-android -c Release -p:AndroidKeyStore=true -p:AndroidSigningKeyStore=$1 -p:AndroidSigningKeyAlias=bolwallet -p:AndroidSigningKeyPass=file:$2 -p:AndroidSigningStorePass=file:$2 ./../../BolWallet/BolWallet.csproj
