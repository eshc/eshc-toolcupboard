namespace ToolCupboard.Website.Controllers

open System.Linq
open System.Security.Claims
open Microsoft.AspNetCore.Authentication
open Microsoft.AspNetCore.Authentication.Cookies
open Microsoft.AspNetCore.Mvc
open ToolCupboard.Website.Models
open ToolCupboard.Website.Services
open System
open ToolCupboard.LDAP

type AuthController (ldapService : ILdapService) =
    inherit Controller()

    member this.Login() =
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
                    identity.AddClaim(Claim("ToolAdmin", user.groups.Contains("ToolAdmins").ToString()))
                    let principal = ClaimsPrincipal(identity)
                    let expires = DateTimeOffset.UtcNow.AddDays(30.0) |> Nullable
                    let authProperties = AuthenticationProperties(IsPersistent = true, ExpiresUtc = expires) 
                    do! Async.AwaitTask(this.HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, principal, authProperties))
                    return this.RedirectToAction("Index", "Home") :> ActionResult
                | Result.Error error ->
                    match error with 
                    | LdapError.IncorrectPassword | LdapError.NoSingleEntry -> 
                        this.ModelState.AddModelError("", "Username or password is invalid.")
                    | _ -> 
                        eprintfn "An internal error occurred: %A" error
                        this.ModelState.AddModelError("", "Internal error.")
                    return this.View() :> ActionResult
            else
                return this.View() :> ActionResult
        }

    member this.Logout() =
        async {
            do! Async.AwaitTask(this.HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme))
            return this.RedirectToAction("Index", "Home")
        }