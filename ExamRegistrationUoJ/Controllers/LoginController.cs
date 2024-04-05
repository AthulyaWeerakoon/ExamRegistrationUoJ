using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.MicrosoftAccount;
using Microsoft.AspNetCore.Mvc;

namespace BlazorApp1.Controllers
{
    [Route("/[controller]")]
    [ApiController]
    public class LoginController : ControllerBase
    {
        [HttpGet("microsoft")]
        public async Task<ActionResult> Login(string RedirectUri) {
            var props = new AuthenticationProperties
            {
                RedirectUri = RedirectUri
            };
            return Challenge(props, MicrosoftAccountDefaults.AuthenticationScheme);
        }

        [HttpGet("logout")]
        public async Task<ActionResult> Logout() {
            await HttpContext.SignOutAsync();
            return Redirect("/");
        }
    }
}
