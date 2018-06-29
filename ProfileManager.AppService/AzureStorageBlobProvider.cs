using System;
using System.Threading.Tasks;
using Microsoft.Extensions.Options;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Blob;

namespace ProfileManager.AppService
{
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
            // since the blob name matches the persisted face ID, there shouldn't be a collision - and if there is, it's because the same photo was uploaded
            // blob storage will overwrite if the names match
            await blob.UploadFromByteArrayAsync(blobData, 0, blobData.Length);
            return blob.Uri;
        }

        public async Task<bool> DeleteBlobAsync(Uri photoPath)
        {
            var fileName = System.IO.Path.GetFileName(photoPath.AbsolutePath);
            var blob = _container.GetBlockBlobReference(fileName);
            return await blob.DeleteIfExistsAsync();
        }

        //todo: consider longer timeouts for better caching
        public Uri GetReadSasForBlob(Uri blobUri, int validForSeconds = 300)
        {
            return new Uri($"{blobUri}{GenerateSas(blobUri, SharedAccessBlobPermissions.Read, validForSeconds)}");
        }

        public Uri GetWriteSasForBlob(Uri blobUri, int validForSeconds = 300)
        {
            return new Uri($"{blobUri}?{GenerateSas(blobUri, SharedAccessBlobPermissions.Write, validForSeconds)}");
        }

        private string GenerateSas(Uri blobUri, SharedAccessBlobPermissions operations, int validForSeconds)
        {
            var fileName = System.IO.Path.GetFileName(blobUri.AbsolutePath);
            var blob = _container.GetBlockBlobReference(fileName);
            return blob.GetSharedAccessSignature(new SharedAccessBlobPolicy()
            {
                Permissions = operations,
                SharedAccessExpiryTime = DateTime.UtcNow.AddMinutes(validForSeconds)
            });
        }
    }
}
