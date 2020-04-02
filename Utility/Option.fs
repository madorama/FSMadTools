namespace MadLib

[<CompilationRepresentation(CompilationRepresentationFlags.ModuleSuffix)>]
module Option =
  open System

  [<CompiledName "Apply">]
  let inline apply f option = match option with None -> None | Some v -> f v; option
