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
        true,
        "public, admin, references">

type Context = Db.dataContext option

type UserCard = Db.dataContext.``public.user_cardsEntity``
type User = Db.dataContext.``public.usersEntity``
type Tool = Db.dataContext.``public.toolsEntity``
type ToolCheckout = Db.dataContext.``public.tool_checkoutEntity``
type AccessLog = Db.dataContext.``public.access_logEntity``

let getContext ctxt = Option.defaultWith (Db.GetDataContext) ctxt
