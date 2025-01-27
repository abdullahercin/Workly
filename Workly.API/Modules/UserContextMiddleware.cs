namespace Workly.API.Modules
{
    public class UserContextMiddleware(RequestDelegate next)
    {
        public async Task InvokeAsync(HttpContext context)
        {
            if (context.User.Identity.IsAuthenticated)
            {
                // Kullanıcının RecId'sini al
                var userIdClaim = context.User.FindFirst("RecId");
                if (userIdClaim != null)
                {
                    context.Items["UserId"] = int.Parse(userIdClaim.Value);
                }
            }

            await next(context);
        }
    }
}
