using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
using ProfileManager.Entities;

namespace ProfileManager.AppService
{

    public class AzureFaceProvider : IFaceInfoProvider
    {
        // don't dispose this; it'll keep sockets open in TIME_WAIT status :/
        private readonly HttpClient _faceApiClient;
        private readonly string _faceApiEndpoint;

        public AzureFaceProvider(string endpoint, string key) : this(new HttpClient(), endpoint, key) { }

        public AzureFaceProvider(HttpClient client, string endpoint, string key)
        {
            client.DefaultRequestHeaders.Add("Ocp-Apim-Subscription-Key", key);
            _faceApiClient = client;
            _faceApiEndpoint = endpoint;
        }

        public Task<decimal> GetFaceMatchConfidenceAsync(Uri photoUri, Uri liveUri)
        {
            throw new NotImplementedException();
        }

        public async Task<IList<Face>> GetFacesFromPhotoAsync(Uri photoUri)
        {
            var httpClient = new HttpClient();
            return await GetFacesFromPhotoAsync(await httpClient.GetByteArrayAsync(photoUri));
            throw new NotImplementedException();
        }

        public async Task<IList<Face>> GetFacesFromPhotoAsync(byte[] fileData)
        {
            var uri = $"{_faceApiEndpoint}/detect?returnFaceId=true&returnFaceAttributes=occlusion";
            using (var content = new ByteArrayContent(fileData))
            {
                content.Headers.ContentType = new System.Net.Http.Headers.MediaTypeHeaderValue("application/octet-stream");
                try
                {
                    var rawFaceResponse = await _faceApiClient.PostAsync(uri, content);
                    if (rawFaceResponse.IsSuccessStatusCode)
                    {
                        var rawResponse = await rawFaceResponse.Content.ReadAsStringAsync();
                        var faceData = JsonConvert.DeserializeObject<List<Face>>(rawResponse);
                        return faceData;
                    }
                }
                catch (Exception ex)
                {
                    // todo: do something...log, retry, whatev
                    Console.WriteLine(ex.Message);
                    throw;
                }
            }

            return new List<Face>();
        }
    }

    public interface IFaceInfoProvider
    {
        Task<IList<Face>> GetFacesFromPhotoAsync(Uri photoUri);
        Task<decimal> GetFaceMatchConfidenceAsync(Uri photoUri, Uri liveUri);
        Task<IList<Face>> GetFacesFromPhotoAsync(byte[] fileData);
    }

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
