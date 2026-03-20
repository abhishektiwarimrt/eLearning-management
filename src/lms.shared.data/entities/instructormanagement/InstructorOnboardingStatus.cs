using lms.shared.data.entities.usermanagement;

namespace lms.shared.data.entities.instructormanagement
{
    /// <summary>
    /// Persists instructor onboarding progress.
    /// One row per user, upserted on every step save.
    /// Status transitions: InProgress → Completed (never goes back).
    /// </summary>
    public class InstructorOnboardingStatus
    {
        public int Id { get; set; }   // PK
        public int UserId { get; set; }   // FK → AspNetUsers.Id (unique)
        public User User { get; set; } = null!;

        /// <summary>
        /// "InProgress" | "Completed"
        /// </summary>
        public string Status { get; set; } = OnboardingStatusValue.InProgress;

        /// <summary>
        /// Which step the user last saved (1–4).
        /// Lets us resume exactly where they left off.
        /// </summary>
        public int LastStep { get; set; } = 1;

        // ── Step data stored as individual columns (easy to query/audit) ──

        // Step 1 — Experience
        public int YearsExperience { get; set; }
        public string? PriorCourses { get; set; }

        // Step 2 — Topics
        public string? Specialties { get; set; }
        public bool HasRecordingEquipment { get; set; }

        // Step 3 — Commitment
        public string? CoursePlan { get; set; }
        public string? Headline { get; set; }
        public string? Bio { get; set; }

        // Step 4 — Goals
        public string? Language { get; set; }
        public string? PayoutMethod { get; set; }

        // ── Timestamps ────────────────────────────────────────────────────
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
        public DateTime? CompletedAt { get; set; }
    }

    public static class OnboardingStatusValue
    {
        public const string InProgress = "InProgress";
        public const string Completed = "Completed";
    }
}
