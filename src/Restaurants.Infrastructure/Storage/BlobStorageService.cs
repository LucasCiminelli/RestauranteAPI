using Azure.Storage.Blobs;
using Azure.Storage.Sas;
using Microsoft.Extensions.Options;
using Restaurants.Domain.Interfaces;
using Restaurants.Infrastructure.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Restaurants.Infrastructure.Storage
{
    public class BlobStorageService : IBlobStorageService
    {

        private readonly BlobStorageSettings _settings;

        public BlobStorageService(IOptions<BlobStorageSettings> settings)
        {
            _settings = settings.Value;
        }

        public async Task<string> UploadToBlobAsync(Stream file, string fileName)
        {
            var blobServiceClient = new BlobServiceClient(_settings.ConnectionString);

            var containerClient = blobServiceClient.GetBlobContainerClient(_settings.LogosContainerName);

            var blobClient = containerClient.GetBlobClient(fileName);

            await blobClient.UploadAsync(file);

            var blobUrl = blobClient.Uri.ToString();

            return blobUrl;

        }

        public string? GetBlobSasUrl(string? blobUrl)
        {
            if(blobUrl == null)
            {
                return null;
            }

            var sasBuilder = new BlobSasBuilder()
            {
                BlobContainerName = _settings.LogosContainerName,
                Resource = "b",
                StartsOn = DateTime.UtcNow,
                ExpiresOn = DateTime.UtcNow.AddMinutes(30),
                BlobName = GetBlobNameFromUrl(blobUrl)
            };


            sasBuilder.SetPermissions(BlobSasPermissions.Read);

            var blobServiceClient = new BlobServiceClient(_settings.ConnectionString);


            var sasToken = sasBuilder
                .ToSasQueryParameters(new Azure.Storage.StorageSharedKeyCredential(blobServiceClient.AccountName, _settings.AccountKey))
                .ToString();

            var sasTokenUrl = $"{blobUrl}?{sasToken}";

            return sasTokenUrl;

        }

        private string GetBlobNameFromUrl(string blobUrl) 
        {
            var uri = new Uri(blobUrl);
            return uri.Segments.Last();
        
        }

    }
}
