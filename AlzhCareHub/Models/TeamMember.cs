﻿namespace AlzhCareHub.Models
{
    using Microsoft.AspNetCore.Http;
    using Newtonsoft.Json;
    using System.ComponentModel.DataAnnotations.Schema;

    public class TeamMember
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Location { get; set; }
        public string PhoneNumber { get; set; }
        public string ImageUrl { get; set; }

        [JsonIgnore] // Prevent this from being deserialized from Firebase
        public IFormFile ImageFile { get; set; }
    }
}

