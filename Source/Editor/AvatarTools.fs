namespace FSMadTools

open FSMadTools.Utility
open UnityEngine
open UnityEditor
open UniRx

type Tags =
  | TagScalingEditor
  | TagLipSyncAttacher
  | TagFingerEditor
  with
    member x.toolbarName = x |> function
      | TagScalingEditor -> "Scaling"
      | TagLipSyncAttacher -> "LipSync Attacher"
      | TagFingerEditor -> "FingerEditor"
    
type AvatarTools() as x =
  inherit EditorWindow()

  let tags = [
    TagScalingEditor
    TagLipSyncAttacher
    TagFingerEditor
  ]

  let mutable skin = None
  let mutable style = None
  let selectTab = new ReactiveProperty<Tags>(TagScalingEditor)
  let mutable selectEditor : ToolBase option = None

  let OnDisable () =
    if AnimationMode.InAnimationMode() then
      AnimationMode.StopAnimationMode()

  [<MenuItem("MadTools/Avatar Tools")>]
  static let init () =
    EditorWindow.GetWindow<AvatarTools>("Avatar Tools") |> ignore

  let OnEnable () =
    let skinPath =
      System.Reflection.Assembly.GetExecutingAssembly().Location
      |> System.IO.Path.GetDirectoryName
      |> (fun x -> x.Replace("\\", "/") + "/GUISkin.guiskin")
      |> FileUtil.GetProjectRelativePath

    skin <-
      Unity.AssetDatabase.loadAssetAtPath<GUISkin>(skinPath) |> Option.apply (fun s ->
        style <- Some <| s.GetStyle("Tab")
      )
    selectTab.Subscribe(fun st ->
      if AnimationMode.InAnimationMode() then
        AnimationMode.StopAnimationMode()
      selectEditor <-
        match st with
        | TagScalingEditor -> Some (new ScalingAvatar() :> ToolBase)
        | TagLipSyncAttacher -> Some (new LipSyncAttacher() :> ToolBase)
        | TagFingerEditor -> Some (new FingerEditor() :> ToolBase)
    )
  
  let OnGUI () =
    match skin, style with
    | Some skin, Some style ->
      let oldSkin = GUI.skin

      GUI.skin <- skin
      selectTab.Value <- CustomGUILayout.styledToolbar selectTab.Value tags style

      GUI.skin <- oldSkin

      selectEditor |> Option.iter (fun x -> x.draw ())
    | _ -> ()
    x.Repaint()