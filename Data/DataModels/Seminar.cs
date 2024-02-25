using Microsoft.AspNetCore.Identity;
using Microsoft.Build.Graph;
using Microsoft.CodeAnalysis.Operations;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Security.Policy;
using static SeminarHub.Data.DataConstants.DataConstants;

namespace SeminarHub.Data.DataModels
{
    public class Seminar
    {
        [Key]
        public int Id { get; set; }
        
        [Required]
        [MaxLength(SeminarTopicMaxLength)]
        public string Topic { get; set; } = string.Empty;
        
        [Required]
        [MaxLength(SeminarLecturerMaxLength)]
        public string Lecturer { get; set; } = string.Empty;
        
        [Required]
        [MaxLength(SeminarDetailsMaxLength)]
        public string Details { get; set; } = string.Empty;
        
        [Required]
        public string OrganizerId { get; set; } = string.Empty;

        [Required]
        [ForeignKey(nameof(OrganizerId))]
        public IdentityUser Organizer { get; set; } = null!;

        [Required]
        public DateTime DateAndTime { get; set; }

        public int Duration { get; set; }

        [Required]
        public int CategoryId { get; set; }

        [Required]
        [ForeignKey(nameof(CategoryId))]
        public Category Category { get; set; } = null!;

        public List<SeminarParticipant> SeminarsParticipants { get; set; } = new List<SeminarParticipant>();
    }
}

