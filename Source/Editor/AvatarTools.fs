namespace FSMadTools

open FSMadTools.Utility
open UnityEngine
open UnityEditor
open UniRx
open CustomGUI

type TagScalingEditor() = inherit ToolbarTag("Scaling")
type TagFingerEditor() = inherit ToolbarTag("Finger Editor")
type TagLipSyncAttacher() = inherit ToolbarTag("LipSync Attacher")

type AvatarTools() as x =
  inherit EditorWindow()

  let tabs : ToolbarTag list = [
    new TagScalingEditor()
    new TagLipSyncAttacher()
    new TagFingerEditor()
  ]

  let mutable skin = None
  let mutable style = None
  let selectTab = new ReactiveProperty<ToolbarTag>(new TagScalingEditor() :> ToolbarTag)
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
    selectTab.SkipLatestValueOnSubscribe().Subscribe(fun st ->
      if AnimationMode.InAnimationMode() then
        AnimationMode.StopAnimationMode()
      selectEditor <-
        match st with
        | :? TagScalingEditor -> Some (new ScalingAvatar() :> ToolBase)
        | :? TagLipSyncAttacher -> Some (new LipSyncAttacher() :> ToolBase)
        | :? TagFingerEditor -> Some (new FingerEditor() :> ToolBase)
        | _ -> None
    )
  
  let OnGUI () =
    match skin, style with
    | Some skin, Some style ->
      let oldSkin = GUI.skin

      GUI.skin <- skin
      selectTab.Value <- CustomGUILayout.styledToolbar selectTab.Value tabs style

      GUI.skin <- oldSkin

      selectEditor |> Option.iter (fun x -> x.draw ())
    | _ -> ()
    x.Repaint()