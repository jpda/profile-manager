using System;
using System.Threading.Tasks;

namespace ProfileManager.AppService
{
    public interface IBlobProvider
    {
        Task<Uri> AddBlob(byte[] blobData, string name);
        Uri GetReadSasForBlob(Uri blobUri, int validForSeconds = 300);
        Uri GetWriteSasForBlob(Uri blobUri, int validForSeconds = 300);
    }
}
