using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using Microsoft.ProjectOxford.Face.Contract;

namespace ProfileManager.AppService
{
    // this is using the Oxford entities, which makes this whole interface pretty tightly bound to the Oxford SDK and by extension, the Face API.
    // Should prob launder the objects through my own entities but time is of the essence
    public interface IFaceInfoProvider
    {
        Task<IList<Face>> DetectFacesFromPhotoAsync(Uri photoUri);
        Task<IList<Face>> DetectFacesFromPhotoAsync(byte[] fileData);
        Task<Guid> AddPersonFaceAsync(Guid personId, byte[] fileData, string groupId = "");
        Task<Guid> CreatePersonInPersonGroupAsync(string employeeObjectId, string employeeId, string personName, string groupId = "");
        Task IdentifyFaceAsync(byte[] fileData, string groupId = "");
        Task DeletePersonFromPersonGroupAsync(Guid personGroupPersonId, string groupId = "");
    }
}