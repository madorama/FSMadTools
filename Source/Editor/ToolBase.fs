namespace FSMadTools

open UnityEngine
open UniRx

[<AbstractClass>]
type ToolBase() =
  abstract draw : unit -> unit
