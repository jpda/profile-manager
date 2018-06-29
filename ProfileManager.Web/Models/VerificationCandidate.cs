using ProfileManager.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ProfileManager.Web.Models
{
    public class VerificationCandidate
    {
        public string PersonGroupId { get; set; }
        public Guid PersonGroupPersonId { get; set; }
        public double Confidence { get; set; }
        public Uri CandidateEmployeePhotoUri { get; set; }
        public Employee Employee { get; set; }
    }

    public class VerificationCandidateViewModel
    {
        public string EncodedOriginalPhotoData { get; set; }
        public IList<VerificationCandidate> Candidates { get; set; }
    }
}
