using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
using ProfileManager.Entities;
using Microsoft.Extensions.Options;

namespace ProfileManager.AppService
{

    public class AzureFaceInfoProvider : IFaceInfoProvider
    {
        private readonly HttpClient _faceApiClient;
        private readonly IOptions<FaceInfoProviderOptions> _options;

        // todo: using IOptions here feels pretty leaky
        public AzureFaceInfoProvider(IOptions<FaceInfoProviderOptions> options) : this(new HttpClient(), options) { }

        public AzureFaceInfoProvider(HttpClient client, IOptions<FaceInfoProviderOptions> options)
        {
            _options = options;
            _faceApiClient = client;
            _faceApiClient.DefaultRequestHeaders.Add("Ocp-Apim-Subscription-Key", _options.Value.Key);
        }

        public Task<decimal> GetFaceMatchConfidenceAsync(Uri photoUri, Uri liveUri)
        {
            throw new NotImplementedException();
        }

        public Task<decimal> GetFaceMatchConfidenceAsync(byte[] photo, byte[] otherPhoto)
        {
            // todo: face ID expires after 24 hours, so a potential optimization would be to store the faceId and the last-scanned timestamp
            // todo: or use the face persistence channels in the face API
            
            throw new NotImplementedException();
            var face1 = GetFacesFromPhotoAsync(photo);
            var face2 = GetFacesFromPhotoAsync(otherPhoto);

            var uri = $"{_options.Value.Endpoint}/verify";
            using (var content = new StringContent(JsonConvert.SerializeObject(new { faceId1 = face1, faceId2 = face2 })))
            {
                content.Headers.ContentType = new System.Net.Http.Headers.MediaTypeHeaderValue("application/json");
                //return await GetFacesFromApi(content, uri);
            }
        }

        public async Task<IList<Face>> GetFacesFromPhotoAsync(Uri photoUri)
        {
            var uri = $"{_options.Value.Endpoint}/detect?returnFaceId=true&returnFaceAttributes=occlusion";
            using (var content = new StringContent(JsonConvert.SerializeObject(new { url = photoUri.ToString() })))
            {
                content.Headers.ContentType = new System.Net.Http.Headers.MediaTypeHeaderValue("application/json");
                return await GetFacesFromApi(content, uri);
            }
        }

        public async Task<IList<Face>> GetFacesFromPhotoAsync(byte[] photoData)
        {
            var uri = $"{_options.Value.Endpoint}/detect?returnFaceId=true&returnFaceAttributes=occlusion";
            using (var content = new ByteArrayContent(photoData))
            {
                content.Headers.ContentType = new System.Net.Http.Headers.MediaTypeHeaderValue("application/octet-stream");
                return await GetFacesFromApi(content, uri);
            }
        }

        private async Task<IList<Face>> GetFacesFromApi(HttpContent content, string uri)
        {
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
                // todo: implement retry + log
                Console.WriteLine(ex.Message);
            }
            return new List<Face>();
        }
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
