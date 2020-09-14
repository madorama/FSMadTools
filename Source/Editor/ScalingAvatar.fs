namespace FSMadTools

open MadLib
open UnityEngine
open UnityEditor
open UniRx
open VRC.SDK3.Avatars.Components

module Scaling =
  type ScaleMode
    = All
    | Split

  type ScalingAvatar() =
    inherit ToolBase()

    let gameObject = new ReactiveProperty<GameObject option>()
    let mutable defaultViewPos = Vector3(0.f, 1.6f, 0.f)
    let mutable scale = Vector3(1.f, 1.f, 1.f)
    let mutable descriptor = None
    let mutable scaleMode = All
    let mutable isSplitMode = new ReactiveProperty<bool>(false)

    do
      gameObject.Subscribe(
        Option.iter (fun (go : GameObject) ->
          scale <- go.transform.localScale
          descriptor <- go |> FsUnity.GameObject.getComponent<VRCAvatarDescriptor>
          descriptor |> Option.iter (fun desc ->
            let viewPos = desc.ViewPosition
            defaultViewPos <- Vector3(viewPos.x / scale.x, viewPos.y / scale.y, viewPos.z / scale.z)
          )
        )
      ) |> ignore

      isSplitMode.Subscribe(fun x ->
        scaleMode <- if x then Split else All
        scale.x <- scale.y
        scale.z <- scale.y
      ) |> ignore

    let drawGUI (go : GameObject) (desc : VRCAvatarDescriptor) = 
      defaultViewPos <- EditorGUILayout.Vector3Field("Default ViewPosition(scale 1,1,1)", defaultViewPos)

      isSplitMode.Value <- EditorGUILayout.Toggle("Split Scale Mode",
          match scaleMode with
          | All -> false
          | Split -> true
        )

      match scaleMode with
      | All ->
        scale.x <- EditorGUILayout.FloatField("Scale", scale.y)
        scale.y <- scale.x
        scale.z <- scale.x
      | Split ->
        scale <- EditorGUILayout.Vector3Field("Scale", scale)

      if GUILayout.Button("Change Scale") then
        go.transform.localScale <- scale
        desc.ViewPosition <- Vector3.Scale(defaultViewPos, scale)
        SceneView.RepaintAll()

    override __.draw () =
      gameObject.Value <- FsUnity.Selection.activeGameObject ()

      match gameObject.Value with
      | None -> EditorError.pleaseSelectGameObject ()
      | Some go ->
        match descriptor with
        | None -> EditorError.avatarDescriptorDoesNotExist ()
        | Some desc -> drawGUI go desc
