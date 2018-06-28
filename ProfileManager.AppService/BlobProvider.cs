using System;
using System.Threading.Tasks;
using E = ProfileManager.Entities;

namespace ProfileManager.AppService
{

    public class BlobProvider : IBlobProvider
    {
        public Task<Uri> AddBlob(byte[] blobData)
        {
            throw new NotImplementedException();
        }

        public Task<Uri> GetReadSasForBlob(string blobUri)
        {
            throw new NotImplementedException();
        }

        public Task<Uri> GetWriteSasForBlob(string blobUri)
        {
            throw new NotImplementedException();
        }
    }

    public interface IBlobProvider
    {
        Task<Uri> AddBlob(byte[] blobData);
        Task<Uri> GetReadSasForBlob(string blobUri);
        Task<Uri> GetWriteSasForBlob(string blobUri);
    }
}
