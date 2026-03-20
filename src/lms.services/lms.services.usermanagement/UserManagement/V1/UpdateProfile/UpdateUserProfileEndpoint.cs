using lms.buildingblocks.RequestResponse;

namespace lms.services.usermanagement.UserManagement.V1.UpdateUserProfile
{
    /// <summary>
    /// PUT /api/v1/user/profile?userEmail={email}
    /// Updates mutable profile fields for an existing instructor/user.
    /// </summary>
    public class UpdateUserProfileEndpoint(ILogger<UpdateUserProfileEndpoint> logger)
        : VersionedCarterModule
    {
        protected override ApiVersion ApiVersion => new(1, 0);
        protected override string ApiName => "User";

        protected override void ConfigureApi(RouteGroupBuilder group)
        {
            group.MapPut("/{UserEmail}/profile",
                async (
                    UpdateUserProfileRequest request,
                    ISender sender,
                    HttpContext context) =>
                {
                    object? userEmail = context.Request.RouteValues["UserEmail"];

                    if (userEmail == null)
                    {
                        return Results.BadRequest("Invalid UserEmail!");
                    }

                    UpdateUserProfileCommand command = request.Adapt<UpdateUserProfileCommand>()
                    with
                    { Email = userEmail.ToString(), Data = request };

                    logger.LogInformation("UpdateProfile request for {Email}", userEmail);


                    bool result = await sender.Send(command);

                    ApiResponse<UpdateUserProfileResponse> apiResponse = new ApiResponse<UpdateUserProfileResponse>
                    {
                        Status = "success",
                        Data = new UpdateUserProfileResponse(result),
                        Metadata = new Metadata
                        {
                            Timestamp = DateTime.UtcNow,
                            Version = ApiVersion.ToString()
                        }
                    };

                    return Results.Ok(apiResponse);
                })
                .MapToApiVersion(1, 0)
                .Produces<ApiResponse<UpdateUserProfileResponse>>(StatusCodes.Status200OK)
                .ProducesProblem(StatusCodes.Status400BadRequest)
                .ProducesProblem(StatusCodes.Status404NotFound)
                .WithSummary("Update User Profile")
                .WithDescription("Updates instructor profile fields: FirstName, LastName, Headline, Bio, Website, LinkedInUrl, TwitterHandle, Language.");
        }
    }

    public record UpdateUserProfileResponse(bool Updated);
}


