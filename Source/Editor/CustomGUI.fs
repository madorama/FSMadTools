namespace FSMadTools

open UnityEngine
open UnityEditor

[<CompilationRepresentation(CompilationRepresentationFlags.ModuleSuffix)>]
module CustomGUI =
  type ToolbarTag(name : string) =
    member val name = name with get

[<CompilationRepresentation(CompilationRepresentationFlags.ModuleSuffix)>]
module CustomGUILayout =
  open CustomGUI
  open FSMadTools.Utility

  [<CompiledName "ObjectField">]
  let inline objectField (label : string) (obj : 'T :> Object) (allowSceneObjects : bool) : 'T option =
    EditorGUILayout.ObjectField("Face Mesh", obj, typeof<'T>, allowSceneObjects) :?> 'T
      |> Option.ofObj

  [<CompiledName "Toolbar">]
  let styledToolbar (sel : ToolbarTag) (tags : ToolbarTag list) (style : GUIStyle) : ToolbarTag =
    let names = tags |> List.map (fun tag -> tag.name) |> List.toArray
    let indexedTags = tags |> List.mapi (fun i x -> (i, x))
    let selId =
      tags
      |> List.mapi (fun i x -> (i, x))
      |> List.tryFind (fun (_, tag) -> sel.GetType() = tag.GetType())
      |> function
        | None -> 0
        | Some (i, _) -> i
    let newSel = GUILayout.Toolbar(selId, names, style)
    tags.[newSel]

  [<CompiledName "Toolbar">]
  let inline toolbar (sel : ToolbarTag) (values : ToolbarTag list) : ToolbarTag =
    styledToolbar sel values EditorStyles.toolbar

  [<CompiledName("HorizontalScope")>]
  let inline styledHorizontalScope (style : GUIStyle) (f : GUILayout.HorizontalScope -> unit) =
    using(new GUILayout.HorizontalScope(style)) (fun s -> f s)

  [<CompiledName("HorizontalScope")>]
  let inline horizontalScope (f : GUILayout.HorizontalScope -> unit) =
    using(new GUILayout.HorizontalScope()) (fun s -> f s)

  [<CompiledName("VerticalScope")>]
  let inline styledVerticalScope (style : GUIStyle) (f : GUILayout.VerticalScope -> unit) =
    using(new GUILayout.VerticalScope(style)) (fun s -> f s)

  [<CompiledName("VerticalScope")>]
  let inline verticalScope (f : GUILayout.VerticalScope -> unit) =
    using(new GUILayout.VerticalScope()) (fun s -> f s)