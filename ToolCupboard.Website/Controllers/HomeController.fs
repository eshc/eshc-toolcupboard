namespace ToolCupboard.Website.Controllers

open System.Linq
open System.Security.Claims
open Microsoft.AspNetCore.Authentication
open Microsoft.AspNetCore.Authentication.Cookies
open Microsoft.AspNetCore.Mvc
open ToolCupboard.Website.Models
open ToolCubpoard.Website.Services
open Microsoft.AspNetCore.Authentication.Cookies
open Microsoft.AspNetCore.Authentication
open System
open ToolCupboard.LDAP
open Microsoft.AspNetCore.Mvc
open Microsoft.AspNetCore.Mvc
open Microsoft.AspNetCore.Mvc

type HomeController (ldapService : ILdapService) =
    inherit Controller()

    member this.Index () =
        this.View()

    [<HttpPost>]
    member this.Login(model : LoginModel) =
        async {
            if this.ModelState.IsValid then
                let result = ldapService.Authenticate model.Username model.Password
                match result with
                | Result.Ok user -> 
                    let identity = ClaimsIdentity(CookieAuthenticationDefaults.AuthenticationScheme, ClaimTypes.Name, ClaimTypes.Role)
                    identity.AddClaim(Claim(ClaimTypes.NameIdentifier, user.dn))
                    identity.AddClaim(Claim(ClaimTypes.Name, user.displayName))
                    identity.AddClaim(Claim("ToolAdmin", user.groups.Contains("ToolAdmin").ToString()))
                    let principal = ClaimsPrincipal(identity)
                    let expires = DateTimeOffset.UtcNow.AddDays(30.0) |> Nullable
                    let authProperties = AuthenticationProperties(IsPersistent = true, ExpiresUtc = expires) 
                    let! v = Async.AwaitTask(this.HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, principal, authProperties))
                    return this.RedirectToAction("Index") :> ActionResult
                | Result.Error error ->
                    match error with 
                    | LdapError.IncorrectPassword | LdapError.NoSingleEntry -> 
                        this.ModelState.AddModelError("", "Username or password is invalid.")
                    | _ -> this.ModelState.AddModelError("", "Internal error.")
                    return this.View() :> ActionResult
            else
                return this.View() :> ActionResult
        }

    member this.Privacy () =
        this.View()

    member this.Error () =
        this.View();
