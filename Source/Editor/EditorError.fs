namespace FSMadTools

open UnityEditor

module EditorError =
  let inline error (str : string) =
    EditorGUILayout.HelpBox(str, MessageType.Error)

  let inline warn (str : string) =
    EditorGUILayout.HelpBox(str, MessageType.Warning)

  let inline pleaseSelectGameObject () =
    error "ゲームオブジェクトを選択してください"

  let inline avatarDescriptorDoesNotExist () =
    error "VRC_AvatarDescriptor コンポーネントがありません"
