namespace FSMadTools

open FSMadTools.Utility
open UnityEngine
open UnityEditor
open VRCSDK2
open UniRx
open UniRx.Triggers

type ScalingAvatar() =
  inherit ToolBase()
  let gameObject = new ReactiveProperty<GameObject option>()
  let mutable defaultViewPos = Vector3(0.f, 1.6f, 0.f)
  let mutable scale = Vector3(1.f, 1.f, 1.f)
  let mutable descriptor = None

  do
    gameObject.Subscribe(
      Option.iter (fun (go : GameObject) ->
        scale <- go.transform.localScale
        descriptor <- go |> Unity.GameObject.getComponent<VRC_AvatarDescriptor>
        descriptor |> Option.iter (fun desc ->
          let viewPos = desc.ViewPosition
          defaultViewPos <- Vector3(viewPos.x / scale.x, viewPos.y / scale.y, viewPos.z / scale.z)
        )
      )
    ) |> ignore

  let drawGUI (go : GameObject) (desc : VRC_AvatarDescriptor) = 
    defaultViewPos <- EditorGUILayout.Vector3Field("Default ViewPosition(scale 1,1,1)", defaultViewPos)
    scale <- EditorGUILayout.Vector3Field("Scale", scale)
    if GUILayout.Button("Change Scale") then
      go.transform.localScale <- scale
      desc.ViewPosition <- Vector3.Scale(defaultViewPos, scale)
      SceneView.RepaintAll()

  override __.draw () =
    gameObject.Value <- Unity.Selection.activeGameObject ()

    match gameObject.Value with
    | None -> EditorError.pleaseSelectGameObject ()
    | Some go ->
      match descriptor with
      | None -> EditorError.avatarDescriptorDoesNotExist ()
      | Some desc -> drawGUI go desc
