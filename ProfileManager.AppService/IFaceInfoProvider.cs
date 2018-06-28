using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using ProfileManager.Entities;

namespace ProfileManager.AppService
{
    public interface IFaceInfoProvider
    {
        Task<IList<Face>> GetFacesFromPhotoAsync(Uri photoUri);
        Task<decimal> GetFaceMatchConfidenceAsync(Uri photoUri, Uri liveUri);
        Task<IList<Face>> GetFacesFromPhotoAsync(byte[] fileData);
    }
}
