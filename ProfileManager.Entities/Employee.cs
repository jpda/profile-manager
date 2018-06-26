using System;

namespace ProfileManager.Entities
{
    public class Employee
    {
        public string Id { get; set; }
        public string ImmutableId { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Department { get; set; }
        public Uri PhotoPath { get; set; }
    }
}
