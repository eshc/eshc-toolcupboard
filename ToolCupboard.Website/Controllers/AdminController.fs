namespace ToolCupboard.Website.Controllers

open System
open FSharp.Data.Sql
open Microsoft.AspNetCore.Mvc
open Microsoft.AspNetCore.Authorization
open ToolCupboard.Database
open ToolCupboard.Database.Provider
open ToolCupboard.Website.Models
open ToolCupboard.Website.Services

type IdRoute = {
    id : int64
}

[<Authorize(Policy = "AdminOnly")>]
type AdminController (ldapService : ILdapService) =
    inherit Controller()

    member private this.ErrorUnknownUser(who : string) =
        this.View("UnknownUserError", who) :> ActionResult

    member private this.ErrorUnknownTool(who : string) =
        this.View("UnknownToolError", who) :> ActionResult

    member private this.ErrorUnknownUserCard(cardId : string) =
        this.View("UnknownUserCardError", id) :> ActionResult

    member private this.ErrorUnknownToolCard(cardId : string) =
        this.View("UnknownToolCardError", id) :> ActionResult

    member private this.RedirectToEditUser(userId) =
        this.RedirectToAction("EditUser", { id = userId }) :> ActionResult

    member private this.RedirectToEditTool(toolId) =
        this.RedirectToAction("EditTool", { id = toolId }) :> ActionResult

    member private this.RedirectToUnknownCards() =
        this.RedirectToAction("UnknownCards") :> ActionResult

    member private this.LastUnknownCard(ctxt) = 
        async { 
            let ctxt = getContext ctxt
            let last =
                query {
                    for v in ctxt.Public.AccessLog do
                    sortByDescending v.AccessId
                    select v
                }
            let card = Seq.tryHead last
            match card with 
            | None -> return ""
            | Some card ->
                let hasToolCard = 
                    query {
                        for c in ctxt.Public.ToolCards do
                        where (c.CardId = card.CardId)
                        select c
                    } |> Seq.tryHead |> Option.isSome
                let hasUserCard = 
                    query {
                        for c in ctxt.Public.UserCards do
                        where (c.CardId = card.CardId)
                        select c
                    } |> Seq.tryHead |> Option.isSome
                if hasToolCard || hasUserCard 
                then return "" 
                else return card.CardId
        }

    member this.ToolBorrower(ctxt : Provider.Db.dataContext, toolId) =
        query {
            for ck in ctxt.Public.ToolCheckout do
            join user in ctxt.Public.Users on (ck.UserId = user.UserId)
            where (ck.ToolId = toolId)
            select (user.Name)
            headOrDefault 
        }

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
        let ctxt = Provider.makeContext ()
        async {
            let! tools = 
                query {
                    for tool in ctxt.Public.Tools do
                    select (
                        ViewToolsToolViewModel(
                            tool.ToolId, 
                            tool.Name,
                            this.ToolBorrower(ctxt, tool.ToolId)))
                } |> Seq.executeQueryAsync
            let vm = ViewToolsViewModel(tools)
            return this.View(vm)
        }

    member this.ViewUsers() =
        let ctxt = Provider.makeContext ()
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
            let ctxt = Provider.makeContext ()
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
                    let user = 
                        ctxt.Public.Users.Create(
                            added=DateTime.Now, 
                            email=luser.email, 
                            enabled=true, 
                            note="",
                            login=luser.username, 
                            name=luser.displayName)
                    do! ctxt.SubmitUpdatesAsync()
                    return this.RedirectToEditUser (user.UserId)
                | Result.Error err -> return this.ErrorUnknownUser(username)
            | Some user -> return this.RedirectToEditUser (user)
        }

    member this.LdapSyncUser(id : int64) =
        async {
            let ctxt = Provider.makeContext ()
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

    member private this.EditToolCards(ctxt, id : int64) =
        async {
            let ctxt = Provider.getContext ctxt
            let! cards = 
                query {
                    for card in ctxt.Public.ToolCards do
                    where (card.ToolId = id)
                    select card
                } |> Seq.executeQueryAsync

            return cards |> Seq.map (fun card ->
                EditToolCardViewModel(CardId = card.CardId, Description = card.Description, Added = card.Added))
        }
 
    member this.EditUser(id : int64) =
        async {
            let! user = Users.GetUserAsync None id
            match user with
            | None -> return this.EditUserNotFound id
            | Some user -> 
                let! cards = this.EditUserCards(None, id)
                let vm = EditUserViewModel.OfUser user cards
                let! lc = this.LastUnknownCard(None)
                vm.LastUnknownCard <- lc
                return this.View(vm)
        }
        
    [<HttpPost>]
    member this.EditUser(id : int64, model : EditUserViewModel) =
        async {
            let ctxt = Provider.makeContext ()
            let! user = Users.GetUserAsync (Some ctxt) id
            match user with
            | None -> return this.EditUserNotFound id
            | Some user -> 
                let! cards = this.EditUserCards(Some ctxt, id)
                let! lc = this.LastUnknownCard(Some ctxt)
                model.Cards <- cards
                model.LastUnknownCard <- lc
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
            let ctxt = Provider.makeContext ()
            let! user = Users.GetUserAsync (Some ctxt) id
            match user with 
            | None -> return this.ErrorUnknownUser (string id)
            | Some user ->
                let card = ctxt.Public.UserCards.Create(DateTime.Now, description, id)
                card.CardId <- cardid
                do! ctxt.SubmitUpdatesAsync()
                return this.RedirectToEditUser(card.UserId)
        }

    member this.AddToolCard(id : int64, cardid : string, description : string) =
        async { 
            let ctxt = Provider.makeContext ()
            let! tool = Tools.GetToolAsync (Some ctxt) id
            match tool with 
            | None -> return this.ErrorUnknownTool (string id)
            | Some tool ->
                let card = ctxt.Public.ToolCards.Create(DateTime.Now, description, id)
                card.CardId <- cardid
                do! ctxt.SubmitUpdatesAsync()
                return this.RedirectToEditTool(card.ToolId)
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
            let ctxt = Provider.makeContext ()
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
            let ctxt = Provider.makeContext ()
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
            let ctxt = Provider.getContext None
            let! tool = Tools.GetToolAsync (Some ctxt) id
            match tool with
            | None -> return this.EditToolNotFound(id)
            | Some tool ->
                let! cards = this.EditToolCards(Some ctxt, id)
                let! lc = this.LastUnknownCard(Some ctxt)
                let vm = EditToolViewModel.OfTool tool cards
                vm.BorrowedBy <- this.ToolBorrower(ctxt, tool.ToolId)
                vm.LastUnknownCard <- lc
                return this.View(vm)
        }

    [<HttpPost>]
    member this.EditTool(id : int64, model : EditToolViewModel) =
        async {
            let ctxt = Provider.makeContext ()
            let! tool = Tools.GetToolAsync (Some ctxt) id
            match tool with
            | None -> return this.EditToolNotFound(id)
            | Some tool ->
                let! cards = this.EditToolCards(None, id)
                let! lc = this.LastUnknownCard(None)
                model.Cards <- cards
                model.LastUnknownCard <- lc
                model.BorrowedBy <- this.ToolBorrower(ctxt, tool.ToolId)
                model.ToolId <- tool.ToolId
                if this.ModelState.IsValid then
                    model.UpdateTool(tool)
                    do! ctxt.SubmitUpdatesAsync()
                    return this.View(model)
                else
                    return this.View(model)
        }

    member this.EditToolCard(id : string) =
        async { 
            let ctxt = Provider.makeContext ()
            let card =
                query { 
                    for card in ctxt.Public.ToolCards do
                    where (card.CardId = id)
                    select card
                    exactlyOneOrDefault
                } |> Option.ofObj
            match card with
            | Some card ->
                let vm = 
                    EditToolCardViewModel(
                        CardId = card.CardId, 
                        Description = card.Description, 
                        Added = card.Added,
                        Tool = card.ToolId)
                return this.View(vm) :> ActionResult
            | None -> return this.ErrorUnknownUserCard(id)
        }

    [<HttpPost>]
    member this.EditToolCard(id : string, model : EditToolCardViewModel) =
        async {
            let ctxt = Provider.makeContext ()
            let tool = 
                query {
                    for tool in ctxt.Public.Tools do
                    where (tool.ToolId = model.Tool)
                    select tool
                    headOrDefault
                }
            if isNull tool 
            then this.ModelState.AddModelError("Tool", "Tool does not exist.")

            if this.ModelState.IsValid then
                let ctxt = Provider.makeContext ()
                let card = 
                    query {
                        for card in ctxt.Public.ToolCards do
                        where (card.CardId = id)
                        select (card)
                        exactlyOneOrDefault
                    } |> Option.ofObj
                match card with
                | None -> return this.ErrorUnknownUserCard(id)
                | Some card ->
                    card.Description <- model.Description
                    card.ToolId <- tool.ToolId
                    do! ctxt.SubmitUpdatesAsync()
                    return this.RedirectToEditTool(tool.ToolId)
            else
                return this.View(model) :> ActionResult
        }

    member this.DeleteToolCard(id : string) =
        async {
            let! card = Tools.DeleteToolCardAsync None id
            match card with 
            | None -> return this.ErrorUnknownToolCard(id)
            | Some card -> return this.RedirectToEditTool(card.ToolId)
        }

    member this.LastCard() =
        async { 
            let ctxt = Provider.makeContext ()
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
