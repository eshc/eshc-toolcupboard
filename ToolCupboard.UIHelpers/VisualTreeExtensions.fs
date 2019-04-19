module UIHelpers.VisualTreeExtensions

open Avalonia.VisualTree

type IVisual with
    member this.Ancestor<'a when 'a :> IVisual>() =
        let parent = this.GetVisualParent()
        if parent :? 'a
        then parent :?> 'a
        else parent.Ancestor<'a>()
         
