using Azure.Core;
using BarcodeApi.Models;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using Microsoft.IdentityModel.Tokens;
using OpenIddict.Abstractions;
using OpenIddict.Server.AspNetCore;
using System.Security.Claims;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;
using static OpenIddict.Abstractions.OpenIddictConstants;
//https://www.c-sharpcorner.com/article/web-ap/
namespace BarcodeApi.Controllers
{
    [Authorize]
    [Route("[controller]/api")]
    [ApiController]
    public class AccountController : ControllerBase
    {
        private readonly IOpenIddictApplicationManager _applicationManager;
        private readonly IOpenIddictAuthorizationManager _authorizationManager;
        private readonly IOpenIddictScopeManager _scopeManager;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly ApplicationDbContext _applicationDbContext;
        private static bool _databaseChecked;

        public AccountController(
              IOpenIddictApplicationManager applicationManager,
        IOpenIddictAuthorizationManager authorizationManager,
        IOpenIddictScopeManager scopeManager,
            UserManager<ApplicationUser> userManager,
             SignInManager<ApplicationUser> signInManager,
            ApplicationDbContext applicationDbContext)
        {
            _applicationManager = applicationManager;
            _authorizationManager = authorizationManager;
            _scopeManager = scopeManager;
            _userManager = userManager;
            _signInManager = signInManager;
            _applicationDbContext = applicationDbContext;
        }

        // POST: /Account/Register
        [HttpPost]
        [AllowAnonymous]
        [Route("Register")]
        public async Task<IActionResult> Register()
        {
            var formCollection = await Request.ReadFormAsync();
            if (formCollection.TryGetValue("UserName", out var UserNamevar) && !string.IsNullOrEmpty(UserNamevar) &&
                formCollection.TryGetValue("Email", out var Emailvar) && !string.IsNullOrEmpty(Emailvar) &&
                formCollection.TryGetValue("Password", out var Passwordvar) && !string.IsNullOrEmpty(Passwordvar))
            {
                string UserName = UserNamevar.ToString(), Email = Emailvar.ToString(), Password = Passwordvar.ToString();
                EnsureDatabaseCreated(_applicationDbContext);
                if (ModelState.IsValid)
                {
                    var user = await _userManager.FindByNameAsync(Email);
                    if (user != null)
                    {
                        return StatusCode(StatusCodes.Status409Conflict);
                    }

                    user = new ApplicationUser { UserName = UserName, Email = Email };
                    var result = await _userManager.CreateAsync(user, Password);
                    if (result.Succeeded)
                    {
                        return Ok();
                    }
                    else
                    {
                        return BadRequest(string.Join(Environment.NewLine, result.Errors.Select(x => x.Description)));
                    }
                }
                
                // AddErrors(result);
            }

            // If we got this far, something failed.
            return BadRequest(ModelState);
        }

     

        private static void EnsureDatabaseCreated(ApplicationDbContext context)
        {
            if (!_databaseChecked)
            {
                _databaseChecked = true;
                context.Database.EnsureCreated();
            }
        }

        
    }
}
