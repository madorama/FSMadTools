namespace FSMadTools

open MadLib
open FsUnity
open UnityEngine
open UnityEditor
open UniRx
open FSMadTools.Finger

type FingerEditor() =
  inherit ToolBase()
  static let templatesPath = Path.dllProjectRelativeDirectory + "finger_templates.json"
  let gameObject = new ReactiveProperty<GameObject option>()
  let selectedClip = new IntReactiveProperty(0)
  let mutable clips = [||]
  let mutable editClip = None
  let mutable templates = new ReactiveCollection<FingerTemplate>()
  let mutable templatesList = new ReorderableList<FingerTemplate>(templates |> List.ofSeq, true, true, false, false)
  let mutable scrollPosition = new Vector2(0.f, 0.f)
  let selectedHand = new IntReactiveProperty(0)
  let isPreviewMode = new BoolReactiveProperty(false)
  let leftHand = Finger.createHand(HandType.LeftHand)
  let rightHand = Finger.createHand(HandType.RightHand)

  let init (go : GameObject) =
    leftHand |> Array.iter Finger.init
    rightHand |> Array.iter Finger.init
    clips <- AnimationUtility.GetAnimationClips(go)
    selectedClip.SetValueAndForceNotify(if clips.Length > 0 then 0 else -1)

  let initEditorFingers (clip : AnimationClip) =
    editClip <- Some clip

    let fingerInit (curve : EditorCurveBinding) (finger : Finger) =
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

  let updateFingers () =
    let aux go editClip =
      leftHand
        |> Array.iter (fun x -> if x.enabled then Finger.setFingerCurve editClip x)

      rightHand
        |> Array.iter (fun x -> if x.enabled then Finger.setFingerCurve editClip x)

      EditorApplication.RepaintAnimationWindow()
      if not isPreviewMode.Value then AnimationMode.StartAnimationMode()
      AnimationMode.BeginSampling()
      AnimationMode.SampleAnimationClip(go, editClip, 0.f)
      AnimationMode.EndSampling()
      if not isPreviewMode.Value then AnimationMode.StopAnimationMode()

    gameObject.Value |> Option.iter (fun go -> editClip |> Option.iter (aux go))

  do
    gameObject.Subscribe(Option.iter init) |> ignore

    selectedClip.Subscribe(fun x -> clips |> function
      | [||] -> ()
      | clips ->
        leftHand |> Array.iter Finger.init
        rightHand |> Array.iter Finger.init
        initEditorFingers clips.[x]
        if isPreviewMode.Value then updateFingers ()
    ) |> ignore

    isPreviewMode.Subscribe(fun x ->
      if x then
        AnimationMode.StartAnimationMode()
        updateFingers ()
      else
        AnimationMode.StopAnimationMode()
    ) |> ignore

    selectedHand.Subscribe(fun _ ->
      GUI.FocusControl("")
    ) |> ignore

    let updateTemplatesList () =
      templatesList.List <- templates |> List.ofSeq

    templates.ObserveCountChanged().Subscribe(fun _ -> updateTemplatesList ()) |> ignore
    templates.ObserveReplace().Subscribe(fun _ -> updateTemplatesList ()) |> ignore

    let addTemplate (t : FingerTemplate) =
      templates.Add(FingerTemplate.create t.Name t.LeftHand t.RightHand)

    if templatesPath |> System.IO.File.Exists then
      let reader = new System.IO.StreamReader(templatesPath)
      let json = reader.ReadToEnd()
      reader.Close()
      let loadedTemplates = JsonUtility.FromJson<JsonTemplates>(json) |> FingerTemplate.ofJsonTemplates;
      loadedTemplates.Templates |> List.iter addTemplate
    else
      FingerTemplate.defaultTemplates |> List.iter addTemplate

    templatesList.addDrawHeader (fun rect ->
      EditorGUI.LabelField(rect, "Templates")
    )

    templatesList.addDrawElement (fun rect i _ _ ->
      templates.[i].Name <- EditorGUI.TextField(rect, templates.[i].Name)
    )

    templatesList.addOnReorder (fun _ oldId newId ->
      templates.Move(oldId, newId)
    )

  let drawGUI (go : GameObject) =
    let drawTemplates () =
      let listButtons () =
        if GUILayout.Button("Add") then
          templates.Add(new FingerTemplate("new finger"))

        if GUILayout.Button("Remove") then
          let selectIndex = templatesList.Index
          templates.RemoveAt(selectIndex)
          templatesList.Index <- min selectIndex (templatesList.Count - 1)

      let templateSaveLoadButtons () =
        let selIndex = templatesList.Index

        if GUILayout.Button("Save") && selIndex >= 0 then
          Finger.copyHand leftHand templates.[selIndex].LeftHand
          Finger.copyHand rightHand templates.[selIndex].RightHand
          let ft = ScriptableObject.CreateInstance<FingerTemplates>()
          ft.Templates <-
            templates
              |> List.ofSeq
          AssetDatabase.CreateAsset(ft, templatesPath)
          let writer = new System.IO.StreamWriter(templatesPath, false)
          JsonUtility.ToJson(ft |> FingerTemplate.toJsonTemplates ,true)
            |> writer.Write
          writer.Flush()
          writer.Close()

        if GUILayout.Button("Load") && selIndex >= 0 then
          Finger.copyHand templates.[selIndex].LeftHand leftHand
          Finger.copyHand templates.[selIndex].RightHand rightHand
          if isPreviewMode.Value then updateFingers ()

      CustomGUILayout.optionsVerticalScope [| GUILayout.ExpandHeight(true); GUILayout.Width(200.f) |] (fun _ ->
        CustomGUILayout.horizontalScope (fun _ -> listButtons ())
        using(new GUILayout.ScrollViewScope(scrollPosition, GUILayout.MinHeight(551.f))) (fun s ->
          scrollPosition <- s.scrollPosition
          templatesList.DoLayoutList ()
        )
        CustomGUILayout.horizontalScope (fun _ -> templateSaveLoadButtons ())
      )

    let drawFingerEditor () =
      CustomGUILayout.verticalScope (fun _ ->
        let clipNames = clips |> Array.map (fun x -> x.name)
        selectedClip.Value <- EditorGUILayout.Popup("Clip", max 0 selectedClip.Value, clipNames)

        isPreviewMode.Value <- GUILayout.Toggle(isPreviewMode.Value, "Preview", GUI.skin.button)
        if GUILayout.Button("Apply") then updateFingers ()

        selectedHand.Value <- GUILayout.Toolbar(selectedHand.Value, [| "LeftHand"; "RightHand" |], EditorStyles.toolbarButton)

        EditorGUI.BeginChangeCheck()

        match selectedHand.Value with
        | 0 -> leftHand |> Array.iter Finger.drawAndUpdate
        | 1 -> rightHand |> Array.iter Finger.drawAndUpdate
        | _ -> ()

        CustomGUILayout.horizontalScope (fun _ ->
          if GUILayout.Button("Copy hand(left to right)") then
            Finger.copyHand leftHand rightHand
          if GUILayout.Button("Copy hand(right to left)") then
            Finger.copyHand rightHand leftHand
        )

        if EditorGUI.EndChangeCheck() then
          if isPreviewMode.Value then updateFingers ()
      )

    CustomGUILayout.horizontalScope (fun _ ->
      drawTemplates ()
      drawFingerEditor ()
    )

  override __.draw () =
    gameObject.Value <- Selection.activeGameObject ()

    match gameObject.Value with
    | None -> EditorError.pleaseSelectGameObject ()
    | Some go ->
      clips <- AnimationUtility.GetAnimationClips(go)
      if clips.Length > 0 then drawGUI go
      else EditorError.error "アニメーションを作成してください"