using Newtonsoft.Json;
using System;

namespace ProfileManager.Entities
{
    public class Employee
    {
        public string CompanyId { get; set; }
        [JsonProperty("id")]
        public string ImmutableId { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Department { get; set; }
        public Uri PhotoPath { get; set; }
        public Guid PersonGroupId { get; set; }
        public Guid PersonGroupPersonId { get; set; }
        // todo: make this a collection, since the face API supports (and likely needs, for training) multiple
        public Guid PersistedFaceId { get; set; }
        [JsonIgnore]
        public byte[] PhotoBytes { get; set; }
        //todo: another reason to use a model proxy here
        [JsonIgnore]
        public Uri PhotoPathSas { get; set; }
    }
}
