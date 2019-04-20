namespace ToolCupboard.UIHelpers

open Avalonia.VisualTree

module VisualTreeExtensions =
  type IVisual with
      member this.Ancestor<'a when 'a :> IVisual>() =
          let parent = this.GetVisualParent()
          if parent :? 'a
          then parent :?> 'a
          else parent.Ancestor<'a>()

