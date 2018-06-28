using System;
using System.Threading.Tasks;
using Microsoft.Extensions.Options;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Blob;

namespace ProfileManager.AppService
{
    public interface IBlobProvider
    {
        Task<Uri> AddBlob(byte[] blobData, string name);
        Task<Uri> GetReadSasForBlob(string blobUri);
        Task<Uri> GetWriteSasForBlob(string blobUri);
    }

    public class AzureStorageBlobProvider : IBlobProvider
    {
        private CloudStorageAccount _account;
        private CloudBlobContainer _container;

        public AzureStorageBlobProvider(IOptions<BlobProviderOptions> options) : this(options.Value.ConnectionString, options.Value.UploadContainer) { }

        public AzureStorageBlobProvider(string connectionString, string containerName)
        {
            _account = CloudStorageAccount.Parse(connectionString);
            _container = _account.CreateCloudBlobClient().GetContainerReference(containerName);
            _container.CreateIfNotExistsAsync().Wait();
        }

        public async Task<Uri> AddBlob(byte[] blobData, string name)
        {
            var blob = _container.GetBlockBlobReference(name);
            await blob.UploadFromByteArrayAsync(blobData, 0, blobData.Length);
            return blob.Uri;
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
}
