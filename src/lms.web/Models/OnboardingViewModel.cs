namespace lms.web.Models
{
    public class OnboardingViewModel
    {
        public int CurrentStep { get; set; }
        public string[] Steps { get; set; } = new[] { "Experience", "Topics", "Commitment", "Goals" };

        // Make these nullable / non‑required so empty values are still valid
        public string? Experience { get; set; }
        public string? Topics { get; set; }
        public string? TimePerWeek { get; set; }
        public string? Goals { get; set; }
        public int YearsExperience { get; set; }
        public string PriorCourses { get; set; } = "";
        public string Specialties { get; set; } = "";
        public bool HasRecordingEquipment { get; set; }
        public string CoursePlan { get; set; } = "";
        public string Headline { get; set; } = "";
        public string Bio { get; set; } = "";
        public string Language { get; set; } = "English";
        public string PayoutMethod { get; set; } = "";

    }
}
