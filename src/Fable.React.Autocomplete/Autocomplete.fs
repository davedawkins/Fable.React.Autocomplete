module AutoComplete

open Fable.React
open Fable.React.Props
open Fable.Core.JsInterop
open Fable.Core
open Browser

// Minimal configuration properties used by autocompleteBasic
type BasicProps = {
    Items           : string list
    Model           : string
    Dispatch        : string -> unit
}

// Full API properties used by autocomplete
type AutoCompleteProps<'Item> = 
  | GetItemValue of ('Item -> string)
  | Items of 'Item array
  | RenderItem of ('Item -> bool -> ReactElement)
  | AutoHighlight of bool
  | InputProps of obj
  | IsItemSelectable of ('Item -> bool)
  | MenuStyle of CSSProp list
  | OnChange of (Browser.Types.Event -> string -> unit)
  | OnMenuVisibilityChange of (bool -> unit)
  | OnSelect of (string -> unit)
  | Open of bool
  | RenderInput of (obj -> ReactElement)
  | RenderMenu of ('Item array -> string -> obj -> ReactElement)
  | SelectOnBlur of bool
  | ShouldItemRender of ('Item -> string -> bool)
  | SortItems of ('Item -> 'Item -> string -> int) // Compare function for Array.sort. string argument is current value
  | Value of string
  | WrapperProps of obj
  | WrapperStyle of obj

let tmp = {| a = 10 |}

// Default menu style from reactjs-autocomplete with addition of ZIndex to bring menu to front. Otherwise
// seems to be buried under other components
let DefaultMenuStyle = [
    ZIndex              1
    BorderRadius        "3px"
    BoxShadow           "0 2px 12px rgba(0, 0, 0, 0.1)"
    Background          "rgba(255, 255, 255, 0.9)"
    Padding             "2px 0"
    FontSize            "90%"
    Position            PositionOptions.Fixed
    OverflowStyle       OverflowOptions.Auto
    MaxHeight           "50%" // TODO: don't cheat, let it flow to the bottom
    ]

let makePojo<'T> : seq<'T> -> obj = 
    keyValueList CaseRules.LowerFirst

let DefaultMenuStyleAsPojo =
    makePojo DefaultMenuStyle

// ----------------------------------------------------------------------------
// Duplicates list-like object to give it back iteration capability
// Workaround for issue logged against fable react

// My first attempt. Works, preserves ordering.
let rec private makeList list = 
    match list with
    | [] -> []
    | x::xs -> x :: (makeList xs)

// Found this on StackOverflow. I suspect it is a better implementation
// even though it builds in reverse and then has to reverse as a final
// operation. Something for me to investigate. We have so few items that
// I'm not too worried about it for now
// https://stackoverflow.com/questions/30803581/f-recursive-function-to-copy-a-list

//let private copy input =
//  let rec copy acc input =
//    match input with
//    | [] -> List.rev acc
//    | x::xs -> copy (x::acc) xs
//  copy [] input

// ----------------------------------------------------------------------------
// Cannot find the Fable way to do this, so emitting some JS
// https://stackoverflow.com/questions/455338/how-do-i-check-if-an-object-has-a-key-in-javascript
//
[<Emit("($1 && $1.hasOwnProperty($0))")>]
let private pojoHasOwnProperty (key : string) (pojo : obj) : bool = jsNative

// ----------------------------------------------------------------------------
// If no "menuStyle" property specified by caller then add in the default
// with the zIndex fix (see definition of DefaultMenuStyle)
let private fixMenuStyle (propsObj : obj) : obj =
    match (pojoHasOwnProperty "menuStyle" propsObj) with
    | true  -> propsObj
    | false -> 
        propsObj?menuStyle <- DefaultMenuStyleAsPojo
        propsObj

// ----------------------------------------------------------------------------
// Entry point into react-autocomplete
//
let private reactAutocomplete<'Item> (props : AutoCompleteProps<'Item> list) = 
    let propsPojo = props |> makeList |> makePojo |> fixMenuStyle
    ofImport "default" "react-autocomplete" propsPojo []

// ----------------------------------------------------------------------------
// Entry point to react-autocomplete mounted as a function component
//
let autocomplete<'Item>  =
    FunctionComponent.Of( reactAutocomplete<'Item>, "autocompleteView", memoEqualsButFunctions)

// ----------------------------------------------------------------------------
// Batteries-included version of autocomplete that plays nicely with Elmish
// Just supply a list of options to choose from, the current value and a way 
// to dispatch a new value and you're good to go.
//
let autocompleteBasic =
    FunctionComponent.Of( fun (basicProps:BasicProps) ->
        let props = [
          Items (List.toArray basicProps.Items)
          RenderItem (fun item highlight -> 
            div [
                    Prop.Key <| string item // Keep React happy.
                    Props.Style [ Background (if highlight then "gray" else "none") ] 
                ] 
                [   
                    str item 
                ]
          )
          GetItemValue id
          OnSelect basicProps.Dispatch
          ShouldItemRender (fun item value -> item.ToLower().Contains( value.ToLower() ))
          Value basicProps.Model
          OnChange (fun e v -> v |> basicProps.Dispatch)
        ]
        reactAutocomplete<string> props
    )