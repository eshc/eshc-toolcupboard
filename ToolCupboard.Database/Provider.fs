module ToolCupboard.Database.Provider

open FSharp.Data.Sql

let [<Literal>] ResolutionPath = __SOURCE_DIRECTORY__ + "/libs"

type Db = 
    SqlDataProvider<
        Common.DatabaseProviderTypes.POSTGRESQL,
        ConnectionString.ConnectionString,
        "ToolcupboardDatabase",
        ResolutionPath,
        1000,
        false,
        "public, admin, references">

type Context = Db.dataContext option

type UserCard = Db.dataContext.``public.user_cardsEntity``
type User = Db.dataContext.``public.usersEntity``
type Tool = Db.dataContext.``public.toolsEntity``
type ToolCard = Db.dataContext.``public.tool_cardsEntity``
type ToolCheckout = Db.dataContext.``public.tool_checkoutEntity``
type AccessLog = Db.dataContext.``public.access_logEntity``

let defaultConnectionString : string option ref = ref None

let makeContext () =
    match !defaultConnectionString with
    | None -> Db.GetDataContext()
    | Some c -> Db.GetDataContext(c)

let getContext ctxt = 
    Option.defaultWith makeContext ctxt
