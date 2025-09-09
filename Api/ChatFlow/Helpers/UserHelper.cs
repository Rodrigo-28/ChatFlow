using System.Security.Claims;

namespace ChatFlow.Helpers
{
    public class UserHelper
    {
        public static Guid GetUserId(ClaimsPrincipal user)
        {
            var userId = user.FindFirstValue(ClaimTypes.NameIdentifier);

            if (string.IsNullOrEmpty(userId))
            {
                throw new UnauthorizedAccessException("Id is missing");
            }

            return Guid.Parse(userId);
        }

    }
}
