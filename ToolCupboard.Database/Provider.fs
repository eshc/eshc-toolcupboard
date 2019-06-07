module ToolCupboard.Database.Provider

open FSharp.Data.Sql

let [<Literal>] ResolutionPath = __SOURCE_DIRECTORY__ + "/libs"

type sql = SqlDataProvider<
    Common.DatabaseProviderTypes.POSTGRESQL,
    ConnectionString.connString,
    "ToolcupboardDatabase",
    ResolutionPath,
    1000,
    true,
    "public, admin, references">

type Context = Db.dataContext option

type UserCard = Db.dataContext.``public.user_cardsEntity``
type User = Db.dataContext.``public.usersEntity``