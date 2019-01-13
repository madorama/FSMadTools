namespace FSMadTools.Utility

[<CompilationRepresentation(CompilationRepresentationFlags.ModuleSuffix)>]
module Option =
  open System

  [<CompiledName "Apply">]
  let inline apply f option = match option with None -> None | Some v -> f v; option

  [<CompiledName "Contains">]
  let inline contains value option = match option with None -> false | Some v -> v = value

  [<CompiledName "Flatten">]
  let inline flatten option = match option with None -> None | Some x -> x

  [<CompiledName "OfNullable">]
  let inline ofNullable (value : Nullable< ^a >) =
    if value.HasValue then Some value.Value else None

  [<CompiledName "OfObj">]
  let inline ofObj obj = match obj with null -> None | x -> Some x
