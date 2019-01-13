namespace FSMadTools.Utility

[<CompilationRepresentation(CompilationRepresentationFlags.ModuleSuffix)>]
module Delegate =
  open System

  [<CompiledName "Add">]
  let inline add (a : ^T) (b : ^T) : ^T when ^T :> Delegate =
    Delegate.Combine(a, b) :?> ^T
