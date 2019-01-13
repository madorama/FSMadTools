namespace FSMadTools.Utility

[<CompilationRepresentation(CompilationRepresentationFlags.ModuleSuffix)>]
module String =
  [<CompiledName "Lowercase">]
  let inline lowercase (x : string) = x.ToLower()

  [<CompiledName "Uppercase">]
  let inline uppercase (x : string) = x.ToUpper()