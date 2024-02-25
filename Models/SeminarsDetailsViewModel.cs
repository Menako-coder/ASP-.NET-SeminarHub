using Microsoft.AspNetCore.Identity;
using SeminarHub.Data.DataModels;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace SeminarHub.Models
{
    public class SeminarsDetailsViewModel
    {
        public int Id { get; set; }
        public string Topic { get; set; }  
        public string Lecturer { get; set; }
        public string Details { get; set; } 
        public string DateAndTime { get; set; }
        public int Duration { get; set; }
        public string Category { get; set; }
        public string Organizer { get; set; }
    }
}
