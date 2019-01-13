namespace FSMadTools

[<CompilationRepresentation(CompilationRepresentationFlags.ModuleSuffix)>]
module Finger =
  open UnityEngine
  open UnityEditor

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

  type Finger =
    { name : string
    ; handType : HandType
    ; mutable enabled : bool
    ; mutable fingers : float32[]
    }

  let names = [
    "Thumb";
    "Index";
    "Middle";
    "Ring";
    "Little"
  ]

  let inline getFingerName (n : int) (finger : Finger) =
    sprintf "%s %s %s" (finger.handType.toString ()) finger.name (if n = 0 then "Spread" else n.ToString())

  let inline getAnimatorName (n : int) (finger : Finger) =
    sprintf "%s.%s.%s" (finger.handType.toString ()) finger.name (if n = 0 then "Spread" else n.ToString() + " Stretched")

  let inline getFinger (i : int) (finger : Finger) : float32 =
    finger.fingers.[i]

  let init (finger : Finger) =
    finger.enabled <- false
    finger.fingers <- Array.zeroCreate 4

  let create (name : string) (handType : HandType) : Finger =
    { name = name
    ; handType = handType
    ; enabled = false
    ; fingers = Array.zeroCreate 4
    }

  let drawEditor (finger : Finger) =
    use g = new EditorGUILayout.ToggleGroupScope(finger.handType.toString () + " " + finger.name, finger.enabled)
    finger.enabled <- g.enabled

    [| 1..3 |] |> Array.iter (fun i ->
      finger.fingers.[i] <- EditorGUILayout.Slider((getFingerName i finger), (getFinger i finger), -1.f, 1.f)
    )

    finger.fingers.[0] <- EditorGUILayout.Slider((getFingerName 0 finger), (getFinger 0 finger), -2.f, 2.f)

  let setFingerCurve (clip : AnimationClip) (finger : Finger) =
    let endKeyFrame = 1.f / clip.frameRate
    finger.fingers |> Array.iteri (fun i value ->
      let binding = EditorCurveBinding.PPtrCurve("", typeof<Animator>, finger |> getAnimatorName i)
      let curve = AnimationCurve.Linear(0.f, value, endKeyFrame, value)
      AnimationUtility.SetEditorCurve(clip, binding, curve)
    )
