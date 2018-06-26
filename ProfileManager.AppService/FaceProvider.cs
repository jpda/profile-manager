using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace ProfileManager.AppService
{
    class FaceProvider : IFaceProvider
    {
    }

    public interface IFaceProvider
    {
        Task GetFacesFromPhoto(Uri photoUri);
        Task CheckFace(Uri photoUri, Uri liveUri);
    }

    public class BlobProvider : IBlobProvider
    {

    }

    public interface IBlobProvider
    {
        Task<Uri> AddBlob(byte[] blobData);
        Task<Uri> GetReadSasForBlob(string blobUri);
        Task<Uri> GetWriteSasForBlob(string blobUri);
    }
}
