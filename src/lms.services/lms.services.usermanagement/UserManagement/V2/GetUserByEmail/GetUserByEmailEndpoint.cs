namespace lms.services.usermanagement.UserManagement.V2.GetUserByEmail
{
    public record GetUserByEmailResponse(UserDto User);
    public class GetUserByEmailEndpoint : VersionedCarterModule
    {
        protected override ApiVersion ApiVersion => new ApiVersion(2, 0);
        protected override string ApiName => "User";

        protected override void ConfigureApi(RouteGroupBuilder group)
        {


            group.MapGet("/",
            async (string UserEmail,
           ISender sender, HttpContext context) =>
           {
               var userEmail = context.Request.Query["UserEmail"].ToString();
               var result = await sender.Send(new GetUserByEmailQuery(userEmail));

               var response = result.Adapt<GetUserByEmailResponse>();

               return Results.Ok(response);

           })
           .MapToApiVersion(2)
           .Produces<GetUserByEmailResponse>(StatusCodes.Status200OK)
           .ProducesProblem(StatusCodes.Status400BadRequest)
           .WithSummary("Get User By Email")
           .WithDescription("Get User Email");


        }
    }
}

