// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

namespace BolWallet.Services.Abstractions;

public interface IFileDownloadService
{
    Task<byte[]> CreateZipFileAsync(IEnumerable<FileItem> files, CancellationToken cancellationToken = default);
    List<FileItem> CollectIndividualFilesForDownload(UserData userdata);
    Task SaveZipFileAsync(byte[] ediZipFiles, CancellationToken cancellationToken);
    Task DownloadDataAsync<T>(T data, string fileName, CancellationToken cancellationToken = default);
    List<FileItem> CollectCompanyFilesForDownload(UserData userdata);
}
