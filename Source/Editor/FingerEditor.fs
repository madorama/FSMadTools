namespace FSMadTools

open FSMadTools.Utility
open UnityEngine
open UnityEditor
open UniRx

type FingerEditor() =
  inherit ToolBase()
  let gameObject = new ReactiveProperty<GameObject option>()
  let selectedClip = new IntReactiveProperty(0)
  let mutable clips = [||]
  let mutable editClip = None
  let mutable selectedHand = 0
  let isPreviewMode = new BoolReactiveProperty(false)
  let leftHand = Finger.names |> List.map (fun name -> Finger.create name Finger.LeftHand) |> List.toArray
  let rightHand = Finger.names |> List.map (fun name -> Finger.create name Finger.RightHand) |> List.toArray

  let init (go : GameObject) =
    leftHand |> Array.iter Finger.init
    rightHand |> Array.iter Finger.init
    clips <- AnimationUtility.GetAnimationClips(go)
    selectedClip.SetValueAndForceNotify(if clips.Length > 0 then 0 else -1)

  let initEditorFingers (clip : AnimationClip) =
    editClip <- Some clip

    let fingerInit (curve : EditorCurveBinding) (finger : Finger.Finger) =
      finger.fingers <-
        finger.fingers
        |> Array.mapi (fun i x ->
          if curve.propertyName.Contains(Finger.getAnimatorName i finger) then
            finger.enabled <- true
            AnimationUtility.GetEditorCurve(clip, curve).keys.[0].value
          else x
        )
      
    AnimationUtility.GetCurveBindings(clip) |> Array.filter (fun curve ->
      curve.propertyName.StartsWith("LeftHand.") || curve.propertyName.StartsWith("RightHand.")
    ) |> Array.iter (fun curve ->
      leftHand |> Array.iter (fingerInit curve)
      rightHand |> Array.iter (fingerInit curve)
    )

  do
    gameObject.Subscribe(Option.iter init) |> ignore
    selectedClip.Subscribe(fun x -> clips |> function
      | [||] -> ()
      | clips ->
        leftHand |> Array.iter Finger.init
        rightHand |> Array.iter Finger.init
        initEditorFingers clips.[x]
    ) |> ignore
    isPreviewMode.Subscribe(fun x ->
      match x with
      | true -> if AnimationMode.InAnimationMode() |> not then AnimationMode.StartAnimationMode()
      | false -> if AnimationMode.InAnimationMode() then AnimationMode.StopAnimationMode()
    ) |> ignore

  let drawFingers () =
    match selectedHand with
    | 0 -> leftHand |> Array.iter Finger.drawEditor
    | 1 -> rightHand |> Array.iter Finger.drawEditor
    | _ -> ()

    CustomGUILayout.horizontalScope (fun _ ->
      let copyHand (src : Finger.Finger[]) (dest : Finger.Finger[]) =
        let fingers = src |> Array.map (fun f -> (f.enabled, f.fingers |> Array.copy))
        dest |> Array.iteri (fun i f ->
          let (enabled, fingers) = fingers.[i]
          f.enabled <- enabled
          f.fingers <- fingers
        )

      if GUILayout.Button("Copy hand(left to right)") then
        copyHand leftHand rightHand
      if GUILayout.Button("Copy hand(right to left)") then
        copyHand rightHand leftHand
    )
  
  let updateFingers (go : GameObject) (editClip : AnimationClip) =
    leftHand |> Array.iter (fun x -> if x.enabled then Finger.setFingerCurve editClip x)
    rightHand |> Array.iter (fun x -> if x.enabled then Finger.setFingerCurve editClip x)
    EditorApplication.RepaintAnimationWindow()
    if isPreviewMode.Value |> not then AnimationMode.StartAnimationMode()
    AnimationMode.BeginSampling()
    AnimationMode.SampleAnimationClip(go, editClip, 0.f)
    AnimationMode.EndSampling()
    if isPreviewMode.Value |> not then AnimationMode.StopAnimationMode()

  let drawGUI (go : GameObject) =
    isPreviewMode.Value <- GUILayout.Toggle(isPreviewMode.Value, "Preview", GUI.skin.button)

    if GUILayout.Button("Apply") then
      editClip |> Option.iter (updateFingers go)

    selectedHand <- GUILayout.Toolbar(selectedHand, [| "LeftHand"; "RightHand" |], EditorStyles.toolbarButton)

    EditorGUI.BeginChangeCheck()

    drawFingers ()

    if EditorGUI.EndChangeCheck() then
      editClip |> Option.iter(fun editClip ->
        if isPreviewMode.Value then updateFingers go editClip
      )

  let drawClips (go : GameObject) =
    match AnimationUtility.GetAnimationClips(go) with
    | [||] -> false
    | cs ->
      clips <- cs
      let clipNames = clips |> Array.map (fun x -> x.name)
      selectedClip.Value <- EditorGUILayout.Popup("Clip", max 0 selectedClip.Value, clipNames)
      true

  override __.draw () =
    gameObject.Value <- Unity.Selection.activeGameObject ()

    match gameObject.Value with
    | None -> EditorError.pleaseSelectGameObject ()
    | Some go ->
      match drawClips go with
      | false -> EditorError.error "アニメーションを作成してください"
      | true -> drawGUI go