namespace ToolCupboard.UIHelpers

open System.Collections.Generic
open Avalonia.Controls
open Avalonia.VisualTree

module VisualTreeExtensions =
  type IVisual with
      member this.Ancestor<'a when 'a :> IVisual>() =
          let parent = this.GetVisualParent()
          if isNull parent
          then None
          elif parent :? 'a
          then parent :?> 'a |> Some
          else parent.Ancestor<'a>()

      member this.BreadthFirstFind<'a when 'a :> IVisual>(f) =
          let q = new Queue<IVisual>(this.VisualChildren)
          let rec tryNext () =
              if q.Count = 0
              then None
              else
                  let v = q.Dequeue()
                  if v :? 'a && f (v :?> 'a)
                  then Some (v :?> 'a)
                  else
                      v.VisualChildren |> Seq.iter q.Enqueue
                      tryNext ()
          tryNext ()

      member this.BreadthFirstFindByName<'a when 'a :> Control>(name) =
          this.BreadthFirstFind<'a>(fun v -> v.Name = name)


