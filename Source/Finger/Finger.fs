namespace FSMadTools.Finger

open UnityEngine
open UnityEditor

[<System.Serializable>]
type HandType
  = LeftHand
  | RightHand
  with
    member u.toString () = u |> function
      | LeftHand -> "LeftHand"
      | RightHand -> "RightHand"

    member u.toInt () = u |> function
      | LeftHand -> 0
      | RightHand -> 1

    static member ofInt (n : int) = n |> function
      | 0 -> Some LeftHand
      | 1 -> Some RightHand
      | _ -> None


type Finger = {
  name : string
  handType : HandType
  mutable enabled : bool
  mutable fingers : float32[]
}

[<System.Serializable>]
type JsonFinger = {
  mutable name : string
  mutable handType : int
  mutable enabled : bool
  mutable fingers : float32[]
}  

[<CompilationRepresentation(CompilationRepresentationFlags.ModuleSuffix)>]
module Finger =
  let names = [
    "Thumb"
    "Index"
    "Middle"
    "Ring"
    "Little"
  ]

  let toJsonFinger (finger : Finger) : JsonFinger = {
    name = finger.name
    handType = finger.handType.toInt ()
    enabled = finger.enabled
    fingers = finger.fingers |> Array.copy
  }
  
  let ofJsonFinger (jsonFinger : JsonFinger) : Finger = {
    name = jsonFinger.name
    handType =
      match HandType.ofInt jsonFinger.handType with
      | Some t -> t
      | None -> LeftHand
    enabled = jsonFinger.enabled
    fingers = jsonFinger.fingers |> Array.copy
  }

  let inline getFingerName (n : int) (finger : Finger) =
    sprintf "%s %s %s" (finger.handType.toString()) finger.name (if n = 0 then "Spread" else n.ToString())

  let inline getAnimatorName (n : int) (finger : Finger) =
    sprintf "%s.%s.%s" (finger.handType.toString()) finger.name (if n = 0 then "Spread" else n.ToString() + " Stretched")

  let inline getFinger (i : int) (finger : Finger) : float32 =
    finger.fingers.[i]

  let copyHand (src : Finger[]) (dest : Finger[]) =
    let fingers = src |> Array.map (fun f -> (f.enabled, f.fingers |> Array.copy))
    dest |> Array.iteri (fun i f ->
      let (enabled, fingers) = fingers.[i]
      f.enabled <- enabled
      f.fingers <- fingers
    )

  let init (finger : Finger) =
    finger.enabled <- false
    finger.fingers <- Array.zeroCreate 4

  let create (name : string) (handType : HandType) : Finger = {
    name = name
    handType = handType
    enabled = false
    fingers = Array.zeroCreate 4
  }

  let createHand (handType : HandType) =
    names |> List.map (fun name -> create name handType) |> List.toArray

  let drawAndUpdate (finger : Finger) =
    using(new EditorGUILayout.VerticalScope(GUI.skin.box)) (fun _ ->
      use g = new EditorGUILayout.ToggleGroupScope(finger.handType.ToString() + " " + finger.name, finger.enabled)
      finger.enabled <- g.enabled

      [| 1..3 |] |> Array.iter (fun i ->
        finger.fingers.[i] <- EditorGUILayout.Slider((getFingerName i finger), (getFinger i finger), -1.f, 1.f)
      )

      finger.fingers.[0] <- EditorGUILayout.Slider((getFingerName 0 finger), (getFinger 0 finger), -2.f, 2.f)
    )

  let setFingerCurve (clip : AnimationClip) (finger : Finger) =
    let endKeyFrame = 1.f / clip.frameRate
    finger.fingers |> Array.iteri (fun i value ->
      let binding = EditorCurveBinding.PPtrCurve("", typeof<Animator>, finger |> getAnimatorName i)
      let curve = AnimationCurve.Linear(0.f, value, endKeyFrame, value)
      AnimationUtility.SetEditorCurve(clip, binding, curve)
    )
