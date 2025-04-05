using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Restaurants.Domain.Interfaces
{
    public interface IBlobStorageService
    {
        string? GetBlobSasUrl(string? blobUrl);
        Task<string> UploadToBlobAsync(Stream file, string fileName);

    }
}
