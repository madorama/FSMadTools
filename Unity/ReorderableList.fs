namespace FsUnity

open MadLib
open UnityEditorInternal
open System.Collections.Generic
open System.Linq

type ReorderableList<'a>
  ( items : 'a list, ?draggable, ?displayHeader, ?displayAddButton, ?displayRemoveButton ) as this =
  let rl =
    new ReorderableList
      ( new List<'a>(items)
      , typeof<'a>
      , defaultArg draggable true
      , defaultArg displayHeader true
      , defaultArg displayAddButton true
      , defaultArg displayRemoveButton true
      )

  let mutable oldIndex = 0

  do
    this.addOnSelect (fun i -> if oldIndex <> i then oldIndex <- i)

  member __.Count with get () = rl.count
  member __.Draggable with get () = rl.draggable
  member __.Index
    with get () = rl.index
    and set value = rl.index <- value
  member __.List
    with get () = rl.list.Cast<'a>() |> List.ofSeq
    and set (xs : 'a list) = rl.list <- new List<'a>(xs)
  member __.SerializedProperty with get () = rl.serializedProperty
  member __.DisplayAdd
    with get () = rl.displayAdd
    and set value = rl.displayAdd <- value

  member __.DisplayRemove
    with get () = rl.displayRemove
    and set value = rl.displayRemove <- value

  member __.DoList rect = rl.DoList(rect)
  member __.DoLayoutList () = rl.DoLayoutList()

  member __.addDrawHeader f =
    rl.drawHeaderCallback <-
      rl.drawHeaderCallback
      |> Delegate.add (ReorderableList.HeaderCallbackDelegate f)

  member __.addDrawFooter f =
    rl.drawFooterCallback <-
      rl.drawFooterCallback
      |> Delegate.add (ReorderableList.FooterCallbackDelegate f)

  member __.addDrawElement f =
    rl.drawElementCallback <-
      rl.drawElementCallback
      |> Delegate.add (ReorderableList.ElementCallbackDelegate f)

  member __.addDrawElementBackground f =
    rl.drawElementBackgroundCallback <-
      rl.drawElementBackgroundCallback
      |> Delegate.add (ReorderableList.ElementCallbackDelegate f)

  member __.addOnElementHeight f =
    rl.elementHeightCallback <-
      rl.elementHeightCallback
      |> Delegate.add (ReorderableList.ElementHeightCallbackDelegate f)

  member __.addOnReorder f =
    rl.onReorderCallback <-
      rl.onReorderCallback
      |> Delegate.add (ReorderableList.ReorderCallbackDelegate (fun x -> f x oldIndex x.index ))

  member __.addOnSelect f =
    rl.onSelectCallback <-
      rl.onSelectCallback
      |> Delegate.add (ReorderableList.SelectCallbackDelegate (fun x -> f x.index))

  member __.addOnAdd f =
    rl.onAddCallback <-
      rl.onAddCallback
      |> Delegate.add (ReorderableList.AddCallbackDelegate f)

  member __.addOnAddDropdown f =
    rl.onAddDropdownCallback <-
      rl.onAddDropdownCallback
      |> Delegate.add (ReorderableList.AddDropdownCallbackDelegate f)

  member __.addOnRemove f =
    rl.onRemoveCallback <-
      rl.onRemoveCallback
      |> Delegate.add (ReorderableList.RemoveCallbackDelegate f)

  member __.addOnMouseUp f =
    rl.onMouseUpCallback <-
      rl.onMouseUpCallback
      |> Delegate.add (ReorderableList.SelectCallbackDelegate f)

  member __.addOnCanRemove f =
    rl.onCanRemoveCallback <-
      rl.onCanRemoveCallback
      |> Delegate.add (ReorderableList.CanRemoveCallbackDelegate f)

  member __.addOnCanAdd f =
    rl.onCanAddCallback <-
      rl.onCanAddCallback
      |> Delegate.add (ReorderableList.CanAddCallbackDelegate f)

  member __.addOnChanged f =
    rl.onChangedCallback <-
      rl.onChangedCallback
      |> Delegate.add (ReorderableList.ChangedCallbackDelegate f)
