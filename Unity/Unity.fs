namespace FsUnity

open MadLib
open UnityEngine
open UnityEditor

[<CompilationRepresentation(CompilationRepresentationFlags.ModuleSuffix)>]
module Debug =
  [<CompiledName "Log">]
  let log (message :'a) : 'a =
    Debug.Log(message)
    message

[<CompilationRepresentation(CompilationRepresentationFlags.ModuleSuffix)>]
module AssetDatabase =
  [<CompiledName "LoadAssetAtPath">]
  let inline loadAssetAtPath< ^a when ^a :> Object and ^a : null > path =
    AssetDatabase.LoadAssetAtPath< ^a >(path) |> Option.ofObj

[<CompilationRepresentation(CompilationRepresentationFlags.ModuleSuffix)>]
module Selection =
  [<CompiledName "ActiveGameObject">]
  let inline activeGameObject () =
    Selection.activeGameObject |> Option.ofObj

[<CompilationRepresentation(CompilationRepresentationFlags.ModuleSuffix)>]
module GameObject =
  [<CompiledName "GetComponent">]
  let inline getComponent< ^a when ^a :> Component and ^a : null> (gameObject : GameObject) : ^a option =
    gameObject.GetComponent() |> Option.ofObj

[<CompilationRepresentation(CompilationRepresentationFlags.ModuleSuffix)>]
module Mesh =
  [<CompiledName "BlendShapeName">]
  let inline blendShapeName (index : int) (mesh : Mesh) =
    mesh.GetBlendShapeName(index)

  [<CompiledName "BlendShapeIndex">]
  let inline blendShapeIndex (name : string) (mesh : Mesh) =
    mesh.GetBlendShapeIndex(name)

  [<CompiledName "BlendShapeNames">]
  let inline blendShapeNames (mesh : Mesh) =
    [0..mesh.blendShapeCount - 1] |> List.map (fun i -> blendShapeName i mesh)
