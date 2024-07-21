using Bunit;
using Xunit;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.Extensions.DependencyInjection;
using Moq;
using System.Security.Claims;
using ExamRegistrationUoJ.Components.Layout; 

namespace ExamRegistrationHeader.Tests
{
    public class HeaderComponentTests : TestContext
    {
        [Fact]
        public void HeaderComponent_ShouldRenderCorrectly_ForUnauthorizedUser()
        {
            // Arrange
            Services.AddAuthorizationCore();
            var authContext = new AuthenticationState(new ClaimsPrincipal(new ClaimsIdentity()));
            Services.AddSingleton(new Mock<AuthenticationStateProvider>(MockBehavior.Strict).Object);

            var cut = RenderComponent<Header>();

            // Act & Assert
            cut.MarkupMatches(@"
                <div class=""header"" id=""headerbar"">
                    <div class=""logo-bar"">
                        <div class=""container"">
                            <img src=""Assets/Images/UoJ_logo.png"" class=""logo"" />
                        </div>
                        <div class=""head-text"">
                            <span class=""title light-blue"">FACULTY OF ENGINEERING</span><br/>
                            <span class=""title"">UNIVERSITY OF JAFFNA</span>
                        </div>
                    </div>
                    <div class=""nav-bar"">
                        <div class=""nav-item""><a href=""login/microsoft?RedirectUri=/"" title=""Login from microsoft account"">Login</a></div>
                        <div class=""nav-item""><a href=""/contact"">Contact</a></div>
                        <div class=""nav-item""><a href=""/about"">About</a></div>
                        <div class=""nav-item""><a href="""">Home</a></div>
                    </div>
                </div>");
        }

        [Fact]
        public void HeaderComponent_ShouldRenderCorrectly_ForAuthorizedUser()
        {
            // Arrange
            var identity = new ClaimsIdentity(new[]
            {
                new Claim(ClaimTypes.Name, "Panda Admin")
            }, "Test authentication type");

            var user = new ClaimsPrincipal(identity);

            Services.AddAuthorizationCore();
            Services.AddSingleton<AuthenticationStateProvider>(new TestAuthStateProvider(user));

            var cut = RenderComponent<Header>();

            // Act
            cut.Find("a[title='Logged in as Panda admin']").Click();

            // Assert
            cut.Find("div#add-item").MarkupMatches(@"
                <div style=""display:none"" class=""msg blocking"" id=""add-item"">
                    <div class=""deselect-plane""></div>
                    <div class=""hover-box"">
                        <h1>Confirm Logout</h1>
                        <p class=""label"">Are you sure you want to logout from: <span class=""bold"">Test User?</span></p>
                        <button>No</button>
                        <a href=""login/logout""><button>Yes</button></a>
                    </div>
                </div>");
        }
    }

    public class TestAuthStateProvider : AuthenticationStateProvider
    {
        private readonly ClaimsPrincipal _user;

        public TestAuthStateProvider(ClaimsPrincipal user)
        {
            _user = user;
        }

        public override Task<AuthenticationState> GetAuthenticationStateAsync()
        {
            return Task.FromResult(new AuthenticationState(_user));
        }
    }
}

