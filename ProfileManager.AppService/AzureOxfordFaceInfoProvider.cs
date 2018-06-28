using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.ProjectOxford.Face;
using Microsoft.ProjectOxford.Face.Contract;
using System.Linq;
using System.IO;

namespace ProfileManager.AppService
{
    /// <summary>
    /// Uses the ProjectOxford client library instead of hand-rolled artisan HttpRequests. Default group ID is sent in at instantiation time.
    /// </summary>
    public class AzureOxfordFaceInfoProvider : IFaceInfoProvider
    {
        // todo: decide if group ID will be hidden, overridable per call or required and ditch the default
        private readonly IFaceServiceClient _client;
        private readonly string _defaultPersonGroupId;

        public AzureOxfordFaceInfoProvider(IFaceServiceClient client, string personGroupId)
        {
            _client = client;
            _defaultPersonGroupId = personGroupId;
        }

        public async Task IdentifyFaceAsync(byte[] fileData, string groupId = "")
        {
            var incomingFaces = await DetectFacesFromPhotoAsync(fileData);
            var incomingFace = incomingFaces.SingleOrDefault();
            if (incomingFaces == null) { throw new Exception("number of faces in source doesn't equal 1"); }

            var targetGroup = string.IsNullOrEmpty(groupId) ? _defaultPersonGroupId : groupId;
            var group = await GetPersonGroupOrCreateAsync(targetGroup);
            var result = await _client.IdentifyAsync(new[] { incomingFace.FaceId }, personGroupId: group.PersonGroupId, confidenceThreshold: 0.75f);
        }

        /// <summary>
        /// Used to detect number of faces in the photo. 
        /// </summary>
        /// <param name="photoUri">URI to photo</param>
        /// <returns></returns>
        public async Task<IList<Face>> DetectFacesFromPhotoAsync(Uri photoUri)
        {
            var faces = await _client.DetectAsync(photoUri.ToString(), true, false, new[] { FaceAttributeType.Occlusion });
            return faces.ToList();
        }

        /// <summary>
        /// Used to detect number of faces in the photo.
        /// </summary>
        /// <param name="fileData">Photo data</param>
        /// <returns></returns>
        public async Task<IList<Face>> DetectFacesFromPhotoAsync(byte[] fileData)
        {
            using (var ms = new MemoryStream(fileData))
            {
                var faces = await _client.DetectAsync(ms, true, false, new[] { FaceAttributeType.Occlusion });
                return faces;
            }
        }

        /// <summary>
        /// Adds a person to a group - supports identifying them later
        /// </summary>
        /// <param name="personId"></param>
        /// <param name="fileData"></param>
        /// <param name="groupId"></param>
        /// <returns></returns>
        public async Task AddPersonAndPhotoToGroupAsync(Guid personId, string personName, byte[] fileData, string groupId = "")
        {
            var targetGroup = string.IsNullOrEmpty(groupId) ? _defaultPersonGroupId : groupId;
            var group = await GetPersonGroupOrCreateAsync(targetGroup);

            try
            {
                Person person;
                var people = await _client.ListPersonsInPersonGroupAsync(group.PersonGroupId);
                if (!people.Any(x => x.PersonId == personId))
                {
                    var creationResult = await _client.CreatePersonInPersonGroupAsync(group.PersonGroupId, personName);
                    personId = creationResult.PersonId;
                }

                person = await _client.GetPersonInPersonGroupAsync(group.PersonGroupId, personId);

                using (var ms = new MemoryStream(fileData))
                {
                    var result = await _client.AddPersonFaceInPersonGroupAsync(group.PersonGroupId, personId, ms);
                }

                await _client.TrainPersonGroupAsync(group.PersonGroupId);
            }
            catch (Exception)
            {
                // todo: there are multiple discrete transactions here, so on error there is a lot to compensate for. fix it
                throw;
            }
        }

        /// <summary>
        /// Adds person to group. If group doesn't exist, creates group.
        /// </summary>
        /// <param name="personId">Unique person ID</param>
        /// <param name="photoData"></param>
        /// <param name="groupId"></param>
        /// <returns></returns>
        public async Task<AddPersistedFaceResult> AddPersonToGroupAsync(Guid personId, byte[] photoData, string groupId = "")
        {
            var targetGroup = string.IsNullOrEmpty(groupId) ? _defaultPersonGroupId : groupId;
            var group = await GetPersonGroupOrCreateAsync(targetGroup);
            using (var ms = new MemoryStream(photoData))
            {
                return await AddPersonToGroupAsync(personId, photoData, group.PersonGroupId);
            }
        }

        public async Task<AddPersistedFaceResult> AddPersonToGroupAsync(Guid personId, Stream photoData, string groupId = "")
        {
            var targetGroup = string.IsNullOrEmpty(groupId) ? _defaultPersonGroupId : groupId;
            var group = await GetPersonGroupOrCreateAsync(targetGroup);
            // AddPersonFaceInPersonGroupAsync calls DetectAsync as part of its implementation, so no need to do that separately
            var result = await _client.AddPersonFaceInPersonGroupAsync(group.PersonGroupId, personId, photoData);
            await _client.TrainPersonGroupAsync(group.PersonGroupId);
            return result;
        }

        #region PersonGroup creation

        private async Task<PersonGroup> GetPersonGroupAsync(string groupId)
        {
            return await _client.GetPersonGroupAsync(groupId);
            // todo: throw exception or let it bubble from the client call?
            var groups = await _client.ListPersonGroupsAsync();
            if (groups.Any(x => x.PersonGroupId == groupId))
            {
                return groups.Single(x => x.PersonGroupId == groupId);
            }
            throw new Exception("Person group not found");
        }

        private async Task<PersonGroup> GetPersonGroupOrCreateAsync(string groupId)
        {
            var groups = await _client.ListPersonGroupsAsync();
            if (groups.Any(x => x.PersonGroupId == groupId))
            {
                return groups.Single(x => x.PersonGroupId == groupId);
            }
            await _client.CreatePersonGroupAsync(groupId, groupId);
            return await GetPersonGroupAsync(groupId);
        }

        #endregion
    }
}
