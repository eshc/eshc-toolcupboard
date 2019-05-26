namespace ToolCupboard.Website.Controllers

open System
open FSharp.Data.Sql
open Microsoft.AspNetCore.Mvc
open Microsoft.AspNetCore.Authorization
open ToolCupboard.Database
open ToolCupboard.Database.Provider
open ToolCupboard.Website.Models
open ToolCubpoard.Website.Services

type IdRoute = {
    id : int64
}

[<Authorize(Policy = "AdminOnly")>]
type AdminController (ldapService : ILdapService) =
    inherit Controller()

    member private this.ErrorUnknownUser(who : string) =
        this.View("UnknownUserError", who) :> ActionResult

    member private this.ErrorUnknownUserCard(cardId : string) =
        this.View("UnknownUserCardError", id) :> ActionResult

    member private this.RedirectToEditUser(userId) =
        this.RedirectToAction("EditUser", { id = userId }) :> ActionResult

    member private this.RedirectToEditTool(toolId) =
        this.RedirectToAction("EditTool", { id = toolId }) :> ActionResult

    member private this.RedirectToUnknownCards() =
        this.RedirectToAction("UnknownCards") :> ActionResult

    member this.UnknownCards() =
        async {
            let! cards = AccessLog.LastUnknownCards None 50
            let model = UnknownCardsModel(cards |> Seq.map (fun v -> UnknownCardModel(CardId = v.CardId, LastDate = v.LastDate)))
            return this.View(model)
        }

    member this.CreateTool(id : string) =
        async {
            let! tool = Tools.RegisterToolAsync None id
            return this.RedirectToAction("EditTool", { id = tool.ToolId })
        }

    member this.ViewTools() =
        let ctxt = Provider.Db.GetDataContext()
        async {
            let! tools = 
                query {
                    for tool in ctxt.Public.Tools do
                    select (ViewToolsToolViewModel(tool.ToolId, tool.Name))
                } |> Seq.executeQueryAsync
            let vm = ViewToolsViewModel(tools)
            return this.View(vm)
        }

    member this.ViewUsers() =
        let ctxt = Provider.Db.GetDataContext()
        async {
            let! users = 
                query {
                    for user in ctxt.Public.Users do
                    select (ViewUsersUserViewModel( user.UserId, user.Login, user.Name))
                } |> Seq.executeQueryAsync
            let vm = ViewUsersViewModel(users)
            return this.View(vm)
        }


    [<HttpPost>]
    member this.LoadLdapUser(username : string) =
        async {
            let ctxt = Provider.Db.GetDataContext()
            let! userId = 
                query {
                    for user in ctxt.Public.Users do
                    where (user.Login = username || user.Email = username)
                    select user.UserId
                } |> Seq.tryHeadAsync
            match userId with
            | None -> 
                match ldapService.GetUserInfo(username) with
                | Result.Ok luser ->
                    let user = ctxt.Public.Users.Create(DateTime.Now, luser.email, luser.username, luser.displayName)
                    do! ctxt.SubmitUpdatesAsync()
                    return this.RedirectToEditUser (user.UserId)
                | Result.Error err -> return this.ErrorUnknownUser(username)
            | Some user -> return this.RedirectToEditUser (user)
        }

    member this.LdapSyncUser(id : int64) =
        async {
            let ctxt = Provider.Db.GetDataContext()
            let! user = Users.GetUserAsync (Some ctxt) id
            match user with
            | None -> return this.EditUserNotFound id :> ActionResult
            | Some user -> 
                match ldapService.GetUserInfo user.Login with 
                | Result.Ok luser ->
                    user.Name <- luser.displayName
                    user.Email <- luser.email
                    do! ctxt.SubmitUpdatesAsync()
                | _ -> ()
                return this.RedirectToEditUser (user.UserId)
        }

    member private this.EditUserNotFound(id : int64) = 
        this.ModelState.AddModelError("", sprintf "A user with id %d does not exist." id)
        this.View()

    member private this.EditUserCards(ctxt, id : int64) =
        async {
            let ctxt = Provider.getContext ctxt
            let! cards = 
                query { 
                    for card in ctxt.Public.UserCards do
                    where (card.UserId = id)
                    select card
                } |> Seq.executeQueryAsync

            return cards |> Seq.map (fun card -> 
                EditUserCardViewModel(CardId = card.CardId, Description = card.Description, Added = card.Added))
        }
 
    member this.EditUser(id : int64) =
        async {
            let! user = Users.GetUserAsync None id
            match user with
            | None -> return this.EditUserNotFound id
            | Some user -> 
                let! cards = this.EditUserCards(None, id)
                let vm = EditUserViewModel.OfUser user cards
                return this.View(vm)
        }
        
    [<HttpPost>]
    member this.EditUser(id : int64, model : EditUserViewModel) =
        async {
            let ctxt = Provider.Db.GetDataContext()
            let! user = Users.GetUserAsync (Some ctxt) id
            match user with
            | None -> return this.EditUserNotFound id
            | Some user -> 
                let! cards = this.EditUserCards(None, id)
                model.Cards <- cards
                model.UserId <- user.UserId
                if this.ModelState.IsValid then
                    model.UpdateUser(user)
                    do! ctxt.SubmitUpdatesAsync()
                    return this.View(model)
                else
                    return this.View(model)
        }

    member this.AddUserCard(id : int64, cardid : string, description : string) =
        async { 
            let ctxt = Provider.Db.GetDataContext()
            let! user = Users.GetUserAsync (Some ctxt) id
            match user with 
            | None -> return this.ErrorUnknownUser (string id)
            | Some user ->
                let card = ctxt.Public.UserCards.Create(DateTime.Now, description, id)
                do! ctxt.SubmitUpdatesAsync()
                return this.RedirectToEditUser(card.UserId)
        }

    member this.DeleteUserCard(id : string) =
        async {
            let! card = Users.DeleteUserCardAsync None id
            match card with 
            | None -> return this.ErrorUnknownUserCard(id)
            | Some card -> return this.RedirectToEditUser(card.UserId)
        }

    member this.EditUserCard(id : string) =
        async { 
            let ctxt = Provider.Db.GetDataContext()
            let card =
                query { 
                    for card in ctxt.Public.UserCards do
                    where (card.CardId = id)
                    select card
                    exactlyOneOrDefault
                } |> Option.ofObj
            match card with
            | Some card -> 
                let user = 
                    query {
                        for user in card.``public.users by user_id`` do
                        exactlyOne
                    }
                let vm = 
                    EditUserCardViewModel(
                        CardId = card.CardId, 
                        Description = card.Description, 
                        Added = card.Added,
                        User = user.Login)
                return this.View(vm) :> ActionResult
            | None -> return this.ErrorUnknownUserCard(id)
        }

    [<HttpPost>]

    member this.EditUserCard(id : string, model : EditUserCardViewModel) =
        async {
            let ctxt = Provider.Db.GetDataContext()
            let user = 
                query {
                    for user in ctxt.Public.Users do
                    where (user.Login = model.User || user.Email = model.User)
                    select user
                    headOrDefault
                }
            if isNull user 
            then this.ModelState.AddModelError("User", "User does not exist.")
            else model.User <- user.Login

            if this.ModelState.IsValid then
                let ctxt = Provider.Db.GetDataContext()
                let card = 
                    query {
                        for card in ctxt.Public.UserCards do
                        where (card.CardId = id)
                        select (card)
                        exactlyOneOrDefault
                    } |> Option.ofObj
                match card with
                | None -> return this.ErrorUnknownUserCard(id)
                | Some card ->
                    card.Description <- model.Description
                    card.UserId <- user.UserId
                    do! ctxt.SubmitUpdatesAsync()
                    return this.RedirectToAction("EditUser", { id = user.UserId }) :> ActionResult
            else
                return this.View(model) :> ActionResult
        }


    member private this.EditToolNotFound(id : int64) = 
        this.ModelState.AddModelError("", sprintf "A tool with id %d does not exist." id)
        this.View()

    member this.EditTool(id : int64) =
        async {
            let! tool = Tools.GetToolAsync None id
            match tool with
            | None -> return this.EditToolNotFound(id)
            | Some tool ->
                let vm = ToolViewModel.OfTool(tool)
                return this.View(vm)
        }

    [<HttpPost>]
    member this.EditTool(id : int64, model : ToolViewModel) =
        async {
            let ctxt = Provider.Db.GetDataContext()
            let! tool = Tools.GetToolAsync (Some ctxt) id
            match tool with
            | None -> return this.EditToolNotFound(id)
            | Some tool ->
                if this.ModelState.IsValid then
                    model.UpdateTool(tool)
                    do! ctxt.SubmitUpdatesAsync()
                    return this.View(model)
                else
                    return this.View(model)
        }

    member this.LastCard() =
        async { 
            let ctxt = Provider.Db.GetDataContext()
            let card = 
                query {
                    for c in ctxt.Public.AccessLog do
                    sortByDescending c.Date
                    select c
                    headOrDefault
                } |> Option.ofObj
            match card with
            | None -> return this.RedirectToUnknownCards()
            | Some lastCard ->
                let user = 
                    query {
                        for c in ctxt.Public.UserCards do
                        where (c.CardId = lastCard.CardId)
                        select c
                        headOrDefault
                    } |> Option.ofObj
                match user with
                | Some user -> return this.RedirectToEditUser (user.UserId)
                | None ->
                    let tool =
                        query {
                            for c in ctxt.Public.ToolCards do
                            where (c.CardId = lastCard.CardId)
                            select c
                            headOrDefault
                        } |> Option.ofObj
                    match tool with
                    | Some tool -> return this.RedirectToEditTool (tool.ToolId)
                    | None -> return this.RedirectToUnknownCards()
                
        }
