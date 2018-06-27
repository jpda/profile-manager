using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace ProfileManager.Entities
{
    public class Face
    {
        [JsonProperty("faceId")]
        public string FaceId { get; set; }
        [JsonProperty("faceRectangle")]
        public Dictionary<string, int> FaceDimensions { get; set; }
        [JsonProperty("faceAttributes")]
        public dynamic FaceAttributes { get; set; }
    }
}
