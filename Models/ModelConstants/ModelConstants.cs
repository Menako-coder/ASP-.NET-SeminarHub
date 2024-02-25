namespace SeminarHub.Models.ModelConstants
{
    public static class ModelConstants
    {
        public const int SeminarTopicMaxLength = 100;
        public const int SeminarTopicMinLength = 3;

        public const int SeminarLecturerMaxLength = 60;
        public const int SeminarLecturerMinLength = 5;

        public const int SeminarDetailsMaxLength = 500;
        public const int SeminarDetailsMinLength = 10;

        public const string DateTimeFormat = "dd/MM/yyyy HH:mm";

        public const int CategoryNameMaxLength = 50;
        public const int CategoryNameMinLength = 3;

        public const string RequireErrorMessage = "The field {0} is required";
        public const string StringLengthErrorMessage = "The field {0} must be between {2} and {1} characters long";
    }
}
