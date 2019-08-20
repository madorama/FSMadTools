namespace FSMadTools.Finger

open UnityEngine

type FingerTemplate(name, ?left, ?right) =
  let leftHand = defaultArg left (Finger.createHand LeftHand)
  let rightHand = defaultArg right (Finger.createHand RightHand)

  member val Name = name with get, set
  member __.LeftHand with get () = leftHand
  member __.RightHand with get () = rightHand

[<System.Serializable>]
type JsonFingerTemplate = {
  mutable name : string
  mutable leftHand : JsonFinger[]
  mutable rightHand : JsonFinger[]
}

[<AllowNullLiteral>]
type FingerTemplates() =
  inherit ScriptableObject()
  [<SerializeField>]
  member val Templates : FingerTemplate list = [] with get, set

[<System.Serializable>]
type JsonTemplates = {
  mutable templates : JsonFingerTemplate[]
}

[<CompilationRepresentation(CompilationRepresentationFlags.ModuleSuffix)>]
module FingerTemplate =

  let create name leftHand rightHand =
    new FingerTemplate(name, leftHand |> Array.copy, rightHand |> Array.copy)

  let toJsonFingerTemplate (ft : FingerTemplate) : JsonFingerTemplate = {
    name = ft.Name
    leftHand = ft.LeftHand |> Array.copy |> Array.map Finger.toJsonFinger
    rightHand = ft.RightHand |> Array.copy |> Array.map Finger.toJsonFinger
  }

  let ofJsonFingerTemplate (jft : JsonFingerTemplate) : FingerTemplate =
    create jft.name (jft.leftHand |> Array.map Finger.ofJsonFinger) (jft.rightHand |> Array.map Finger.ofJsonFinger)

  let toJsonTemplates (templates : FingerTemplates) : JsonTemplates = {
    templates =
      templates.Templates |> List.map toJsonFingerTemplate |> Array.ofList
  }

  let ofJsonTemplates (jt : JsonTemplates) : FingerTemplates =
    let ft = new FingerTemplates()
    ft.Templates <- jt.templates |> List.ofArray |> List.map ofJsonFingerTemplate
    ft

  type fingers = {
    thumb : float32[]
    index : float32[]
    middle : float32[]
    ring : float32[]
    little : float32[]
  }

  let defaultTemplates =
    let createTemplate name fingers =
      let ft = new FingerTemplate(name)
      ft.LeftHand.[0].fingers <- fingers.thumb
      ft.LeftHand.[1].fingers <- fingers.index
      ft.LeftHand.[2].fingers <- fingers.middle
      ft.LeftHand.[3].fingers <- fingers.ring
      ft.LeftHand.[4].fingers <- fingers.little
      ft.LeftHand |> Array.iter (fun x -> x.enabled <- true)
      Finger.copyHand ft.LeftHand ft.RightHand
      ft

    let openF = [| 0.f; 0.8f; 0.8f; 0.8f |]
    let closeF = [| 0.f; -0.8f; -0.8f; -0.8f |]

    let fistFingers = {
      thumb = closeF
      index = closeF
      middle = closeF
      ring = closeF
      little = closeF
    }

    let handOpenFingers = {
      thumb = openF
      index = openF
      middle = openF
      ring = openF
      little = openF
    }

    let victoryFingers = {
      fistFingers with
        index = openF
        middle = openF
    }

    let thumbsUpFingers = {
      fistFingers with
        thumb = openF
    }

    let fingerPointFingers = {
      fistFingers with
        index = openF
    }

    let handgunFingers = {
      fistFingers with
        thumb = openF
        index = openF
    }

    let rnrFingers = {
      fistFingers with
        index = openF
        little = openF
    }

    [
      createTemplate "Fist" fistFingers
      createTemplate "Hand open" handOpenFingers
      createTemplate "Victory" victoryFingers
      createTemplate "Thumbs up" thumbsUpFingers
      createTemplate "Finger point" fingerPointFingers
      createTemplate "Handgun" handgunFingers
      createTemplate "Rock'n'roll" rnrFingers
    ]
