namespace ToolCupboard.Website.Controllers

open Microsoft.AspNetCore.Mvc
open ToolCubpoard.Website.Services
open Microsoft.AspNetCore.Http
open System.Security.Claims
open Microsoft.AspNetCore.Authorization

type HomeController (ldapService : ILdapService) =
    inherit Controller()

    member this.Index () =
        this.View()

    member this.Privacy () =
        this.View()

    member this.Error () =
        this.View();
