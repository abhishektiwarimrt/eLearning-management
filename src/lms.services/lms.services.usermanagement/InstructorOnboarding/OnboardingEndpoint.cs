using lms.buildingblocks.RequestResponse;

namespace lms.services.usermanagement.InstructorOnboarding
{
    // ── DTOs ─────────────────────────────────────────────────────────────────

    public record SaveOnboardingStepRequest(
        string UserEmail,
        int LastStep,
        bool IsComplete,
        // Step 1
        int YearsExperience,
        string? PriorCourses,
        // Step 2
        string? Specialties,
        bool HasRecordingEquipment,
        // Step 3
        string? CoursePlan,
        string? Headline,
        string? Bio,
        // Step 4
        string? Language,
        string? PayoutMethod
    );

    public record SaveOnboardingStepResponse(bool Saved, string Status);

    public record GetOnboardingStatusResponse(
        bool Exists,
        string Status,
        int LastStep,
        // Step data returned so the web can pre-fill the form on resume
        int YearsExperience,
        string? PriorCourses,
        string? Specialties,
        bool HasRecordingEquipment,
        string? CoursePlan,
        string? Headline,
        string? Bio,
        string? Language,
        string? PayoutMethod
    );

    // ── Endpoints ────────────────────────────────────────────────────────────

    /// <summary>
    /// POST /api/v1/user/{UserEmail}/onboarding  — save/update a step
    /// GET  /api/v1/user/{UserEmail}/onboarding  — check status and resume data
    /// </summary>
    public class OnboardingEndpoint(ILogger<OnboardingEndpoint> logger) : VersionedCarterModule
    {
        protected override ApiVersion ApiVersion => new(1, 0);
        protected override string ApiName => "User";

        protected override void ConfigureApi(RouteGroupBuilder group)
        {
            // Save step
            group.MapPost("/{UserEmail}/onboarding",
                async (string UserEmail,
                       SaveOnboardingStepRequest request,
                       ISender sender,
                       HttpContext context) =>
                {
                    string email = context.Request.RouteValues["UserEmail"]?.ToString();
                    if (string.IsNullOrWhiteSpace(email))
                        return Results.BadRequest("Invalid UserEmail.");

                    var result = await sender.Send(new SaveOnboardingStepCommand(email, request));

                    return Results.Ok(new ApiResponse<SaveOnboardingStepResponse>
                    {
                        Status = "success",
                        Data = new SaveOnboardingStepResponse(result.Saved, result.Status),
                        Metadata = new Metadata { Timestamp = DateTime.UtcNow, Version = ApiVersion.ToString() }
                    });
                })
                .MapToApiVersion(1, 0)
                .Produces<ApiResponse<SaveOnboardingStepResponse>>(StatusCodes.Status200OK)
                .ProducesProblem(StatusCodes.Status400BadRequest)
                .WithSummary("Save Onboarding Step")
                .WithDescription("Upserts onboarding progress. Set IsComplete=true on final step to assign Instructor role.");

            // Get status
            group.MapGet("/{UserEmail}/onboarding",
                async (string UserEmail, ISender sender, HttpContext context) =>
                {
                    string email = context.Request.RouteValues["UserEmail"]?.ToString();
                    if (string.IsNullOrWhiteSpace(email))
                        return Results.BadRequest("Invalid UserEmail.");

                    var result = await sender.Send(new GetOnboardingStatusQuery(email));

                    return Results.Ok(new ApiResponse<GetOnboardingStatusResponse>
                    {
                        Status = "success",
                        Data = result,
                        Metadata = new Metadata { Timestamp = DateTime.UtcNow, Version = ApiVersion.ToString() }
                    });
                })
                .MapToApiVersion(1, 0)
                .Produces<ApiResponse<GetOnboardingStatusResponse>>(StatusCodes.Status200OK)
                .WithSummary("Get Onboarding Status")
                .WithDescription("Returns onboarding status and step data so the web can resume where the user left off.");
        }
    }
}
