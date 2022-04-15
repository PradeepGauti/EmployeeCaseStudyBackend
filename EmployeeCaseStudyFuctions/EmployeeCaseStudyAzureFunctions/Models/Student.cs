using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace EmployeeCaseStudyAzureFunctions.Models
{
    public class Student
    {
        [JsonProperty("Name")]
        public string Name { get; set; }

        [JsonProperty("id")]
        public string StudentId { get; set; }

        [JsonProperty("Age")]
        public string DateOfBirth { get; set; }

        [JsonProperty("Class")]
        public string Class { get; set; }

        [JsonProperty("Contact")]
        public string PhoneNumber { get; set; }

        [JsonProperty("Email")]
        public string Email { get; set; }

        [JsonProperty("RollNumber")]
        public int RollNumber { get; set; }
    }
}
