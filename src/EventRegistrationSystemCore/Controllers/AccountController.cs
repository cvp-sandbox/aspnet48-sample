using EventRegistrationSystemCore.Identity;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using EventRegistrationSystemCore.Models;
using EventRegistrationSystemCore.Utils;

namespace EventRegistrationSystemCore.Controllers;

public class AccountController : Controller
{
    private SQLiteUserStore _userStore;
    public AccountController(SQLiteUserStore userStore)
    {
        _userStore = userStore;
    }

    //
    // GET: /Account/Login
    [AllowAnonymous]
    public ActionResult Login(string returnUrl)
    {
        ViewBag.ReturnUrl = returnUrl;
        return View();
    }


    [HttpPost]
    [AllowAnonymous]
    [ValidateAntiForgeryToken]
    public async Task<ActionResult> Login(LoginViewModel model, string returnUrl, CancellationToken cancellationToken)
    {

        //if (!ModelState.IsValid)
        //{
        //    return View(model);
        //}

        // Get the user by username
        var userStore = _userStore;
        var user = await userStore.FindByEmailAsync(model.UserName, cancellationToken);

        if (user == null)
        {
            ModelState.AddModelError("", "Invalid login attempt.");
            return View(model);
        }

        // Verify password
        var passwordHasher = new LegacyPasswordHasher();
        var result = passwordHasher.VerifyHashedPassword(user, user.PasswordHash!, model.Password);

        if (result == PasswordVerificationResult.Success)
        {
            // Create claims identity
            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, user.Id),
                new Claim(ClaimTypes.Name, user.UserName ?? user.Email)
            };

            if (!string.IsNullOrEmpty(user.Email))
            {
                claims.Add(new Claim(ClaimTypes.Email, user.Email));
            }


            // Add roles
            var roles = await _userStore.GetUserRolesAsync(user.Id, cancellationToken);

            foreach (var role in roles)
            {
                claims.Add(new Claim(ClaimTypes.Role, role));
            }


            // Create the identity and sign in
            var identity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
            var principal = new ClaimsPrincipal(identity);

            try
            {
                Console.WriteLine("Starting sign-in process");
                Console.WriteLine($"Principal claims: {string.Join(", ", principal.Claims.Select(c => $"{c.Type}={c.Value}"))}");

                await HttpContext.SignInAsync(
                    CookieAuthenticationDefaults.AuthenticationScheme,
                    principal,
                    new AuthenticationProperties
                    {
                        IsPersistent = true,
                        ExpiresUtc = DateTime.UtcNow.AddDays(30)
                    });

                Console.WriteLine("Sign-in completed");

                // IMPORTANT: Manually update the current HttpContext.User
                // This will make IsAuthenticated = true for the current request
                HttpContext.User = principal;

                // Log authentication status after manual update
                var isAuthenticated = HttpContext.User.Identity?.IsAuthenticated;
                Console.WriteLine($"Is authenticated after manual update: {isAuthenticated}");


                // Set a custom response header with auth info
                // This is just for the response, not a cookie
                Response.Headers.Append(AuthHeaders.UserName, user.UserName);
                Response.Headers.Append(AuthHeaders.UserId, user.Id);
                Response.Headers.Append(AuthHeaders.UserRoles, string.Join(",", roles));



                // Return a simple redirect
                return returnUrl != null && Url.IsLocalUrl(returnUrl)
                    ? Redirect(returnUrl)
                    : Redirect("/");

            }
            catch (Exception ex)
            {
                Console.WriteLine($"Sign-in failed: {ex.Message}");
                Console.WriteLine($"Exception stack trace: {ex.StackTrace}");
                ModelState.AddModelError("", $"Authentication error: {ex.Message}");
                return View(model);
            }
        }

       
        ModelState.AddModelError("", "Invalid login attempt.");
        return View(model);
    }

    [HttpPost]
    [Authorize]
    [ValidateAntiForgeryToken]
    public async Task<ActionResult> Logout()
    {
        await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
        return RedirectToAction("Index", "Home");
    }

    private ActionResult RedirectToLocal(string returnUrl)
    {
        if (Url.IsLocalUrl(returnUrl))
        {
            return Redirect(returnUrl);
        }
        return RedirectToAction("Index", "Home");
    }
}
