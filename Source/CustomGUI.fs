namespace FSMadTools

open UnityEngine
open UnityEditor

[<CompilationRepresentation(CompilationRepresentationFlags.ModuleSuffix)>]
module CustomGUILayout =
  open MadLib

  [<CompiledName "ObjectField">]
  let inline objectField (label : string) (obj : 'T :> Object) (allowSceneObjects : bool) : 'T option =
    EditorGUILayout.ObjectField("Face Mesh", obj, typeof<'T>, allowSceneObjects) :?> 'T
      |> Option.ofObj

  [<CompiledName "Toolbar">]
  let inline styledToolbar (sel : ^a) (tags : ^a list) (style : GUIStyle) : ^a when ^a : (member toolbarName : string) =
    let getTagName tag = (^a : (member toolbarName : string) tag)
    let names = tags |> List.map getTagName |> List.toArray
    let selId =
      tags
      |> List.mapi (fun i x -> (i, x))
      |> List.tryFind (fun (_, tag) -> sel = tag)
      |> function
        | None -> 0
        | Some (i, _) -> i
    let newSel = GUILayout.Toolbar(selId, names, style)
    tags.[newSel]

  [<CompiledName "Toolbar">]
  let inline toolbar (sel : ^a) (values : ^a list) : ^a when ^a : (member toolbarName : string) =
    styledToolbar sel values EditorStyles.toolbar

  [<CompiledName("HorizontalScope")>]
  let inline optionsHorizontalScope (options : GUILayoutOption[]) (f : GUILayout.HorizontalScope -> unit) =
    using(new GUILayout.HorizontalScope(options)) f

  [<CompiledName("HorizontalScope")>]
  let inline styledHorizontalScope (style : GUIStyle) (f : GUILayout.HorizontalScope -> unit) =
    using(new GUILayout.HorizontalScope(style)) f

  [<CompiledName("HorizontalScope")>]
  let inline horizontalScope (f : GUILayout.HorizontalScope -> unit) =
    using(new GUILayout.HorizontalScope()) f

  [<CompiledName("VerticalScope")>]
  let inline optionsVerticalScope (options : GUILayoutOption[]) (f : GUILayout.VerticalScope -> unit) =
    using(new GUILayout.VerticalScope(options)) f

  [<CompiledName("VerticalScope")>]
  let inline styledVerticalScope (style : GUIStyle) (f : GUILayout.VerticalScope -> unit) =
    using(new GUILayout.VerticalScope(style)) f

  [<CompiledName("VerticalScope")>]
  let inline verticalScope (f : GUILayout.VerticalScope -> unit) =
    using(new GUILayout.VerticalScope()) f