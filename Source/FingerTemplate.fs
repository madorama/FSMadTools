namespace FSMadTools.Finger

open UnityEngine

type FingerTemplate(name, ?left, ?right) =
  let leftHand = defaultArg left (Finger.createHand LeftHand)
  let rightHand = defaultArg right (Finger.createHand RightHand)

  member val Name = name with get, set
  member __.LeftHand with get () = leftHand
  member __.RightHand with get () = rightHand

[<AllowNullLiteral>]
type FingerTemplates() =
  inherit ScriptableObject()
  [<SerializeField>]
  member val Templates : FingerTemplate list = [] with get, set

[<CompilationRepresentation(CompilationRepresentationFlags.ModuleSuffix)>]
module FingerTemplate =
  let create name leftHand rightHand =
    new FingerTemplate(name, leftHand |> Array.copy, rightHand |> Array.copy)

  let defaultTemplates =
    let createTemplate name thumb index middle ring little =
      let ft = new FingerTemplate(name)
      ft.LeftHand.[0].fingers <- thumb
      ft.LeftHand.[1].fingers <- index
      ft.LeftHand.[2].fingers <- middle
      ft.LeftHand.[3].fingers <- ring
      ft.LeftHand.[4].fingers <- little
      ft.LeftHand |> Array.iter (fun x -> x.enabled <- true)
      Finger.copyHand ft.LeftHand ft.RightHand
      ft

    let openF = [| 0.f; 0.8f; 0.8f; 0.8f |]
    let closeF = [| 0.f; -0.8f; -0.8f; -0.8f |]

    let fist = createTemplate "Fist" closeF closeF closeF closeF closeF
    let handOpen = createTemplate "Hand Open" openF openF openF openF openF
    let victory = createTemplate "Victory" closeF openF openF closeF closeF
    let thumbsUp = createTemplate "Thumbs up" openF closeF closeF closeF closeF
    let fingerPoint = createTemplate "Finger Point" closeF openF closeF closeF closeF
    let handgun = createTemplate "Handgun" openF openF closeF closeF closeF
    let rnr = createTemplate "Rock'n'roll" closeF openF closeF closeF openF

    [ fist; handOpen; victory; thumbsUp; fingerPoint; handgun; rnr ]
