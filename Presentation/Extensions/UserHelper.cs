using System.Security.Claims;
using System.Security.Principal;

namespace Presentation.Extensions
{
    public static class UserHelper
    {
        public static int GetUserId(this IPrincipal principal)
        {
            var claimsIdentity = (ClaimsIdentity)principal.Identity;
            var claim = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier);
            return int.Parse(claim.Value);
        }
    }
}
