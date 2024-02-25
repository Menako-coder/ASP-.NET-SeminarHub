using System.ComponentModel.DataAnnotations;
using static SeminarHub.Models.ModelConstants.ModelConstants;

namespace SeminarHub.Models
{
    public class SeminarFormViewModel
    {
        [Required(ErrorMessage = RequireErrorMessage)]
        [StringLength(SeminarTopicMaxLength,
            MinimumLength = SeminarTopicMinLength,
            ErrorMessage = StringLengthErrorMessage)]
        public string Topic { get; set; } = string.Empty;
        
        [Required(ErrorMessage = RequireErrorMessage)]
        [StringLength(SeminarLecturerMaxLength,
            MinimumLength = SeminarLecturerMinLength,
            ErrorMessage = StringLengthErrorMessage)]
        public string Lecturer { get; set; } = string.Empty;
        
        [Required(ErrorMessage = RequireErrorMessage)]
        [StringLength(SeminarDetailsMaxLength,
            MinimumLength = SeminarDetailsMinLength,
            ErrorMessage = StringLengthErrorMessage)]
        public string Details { get; set; } = string.Empty;
        
        [Required(ErrorMessage = RequireErrorMessage)]
        public string DateAndTime { get; set; } = string.Empty;

        public int Duration { get; set; } 

        [Required(ErrorMessage = RequireErrorMessage)]
        public int CategoryId { get; set; }

        public IEnumerable<CategoryViewModel> Categories { get; set; } = new List<CategoryViewModel>();
    }
}
