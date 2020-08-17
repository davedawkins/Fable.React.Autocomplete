module AutoComplete

open Fable.React
open Fable.React.Props
open Fable.Core.JsInterop
open Fable.Core
open Browser

// Minimal configuration properties used by autocompleteBasic. Use `ofBasic` to turn this
// into a list of AutoCompleteProp
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
  | InputProps of HTMLAttr list
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
  | WrapperProps of HTMLAttr list
  | WrapperStyle of CSSProp list

// Default menu style from reactjs-autocomplete
let DefaultMenuStyle = [
    BorderRadius        "3px"
    BoxShadow           "0 2px 12px rgba(0, 0, 0, 0.1)"
    Background          "rgba(255, 255, 255, 0.9)"
    Padding             "2px 0"
    FontSize            "90%"
    Position            PositionOptions.Fixed
    //OverflowStyle       OverflowOptions.Auto // Seems to generate incorrect style name
    OverflowY( OverflowOptions.Auto ) 
    OverflowX( OverflowOptions.Auto )          
    MaxHeight           "50%" // TODO: don't cheat, let it flow to the bottom
    ]

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


// Turn a list of DUs into a JS object
let makePojo<'T> : seq<'T> -> obj = 
    keyValueList CaseRules.LowerFirst

// ----------------------------------------------------------------------------
// Cannot find the Fable way to do this, so emitting some JS
// https://stackoverflow.com/questions/455338/how-do-i-check-if-an-object-has-a-key-in-javascript
//
[<Emit("($1 && $1.hasOwnProperty($0))")>]
let private pojoHasOwnProperty (key : string) (pojo : obj) : bool = jsNative

[<Emit("$1[$0]")>]
let private pojoGet (key : string) (pojo : obj) : obj = jsNative

let private pojoGetDefault (key : string) (pojo : obj) (value:obj): obj =
    match pojoHasOwnProperty key pojo with
    | true -> pojoGet key pojo
    | false -> value

// Kind of like "{ pojo with name = value }"
// However this just mutates pojo
let pojoWith (pojo:obj) (name:string) value =
    pojo?(name) <- value // Would prefer {| propsObj with menuStyle = makePojo |}
    pojo

// ----------------------------------------------------------------------------
// If no "menuStyle" property specified by caller then add in the default
// with the zIndex fix (see definition of DefaultMenuStyle)
let private fixMenuStyle (propsObj : obj) : obj =
    let ms = pojoGetDefault "menuStyle" propsObj DefaultMenuStyle
    pojoWith propsObj "menuStyle" <| makePojo !! ms

let private fixNested (name: string) (propsObj : obj) : obj =
    let ms = pojoGetDefault name propsObj null
    match ms with
    | null -> propsObj
    | _    -> pojoWith propsObj name <| makePojo !! ms


// ----------------------------------------------------------------------------
// Manufacture a minimal set of AutoCompleteProps from a BasicProps record
//
let ofBasic (basicProps : BasicProps) =
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
    props

// ----------------------------------------------------------------------------
// Entry point into react-autocomplete
//
let private reactAutocomplete<'Item> (props : AutoCompleteProps<'Item> list) = 
    let propsPojo = 
        props 
        |> makeList // Restore list metadata lost after passing through ReactJS
        |> makePojo // Top-level convert
        //|> fixMenuStyle  // Disabled this injection of the standard menu style
        |> fixNested "menuStyle" // Nested pojos
        |> fixNested "inputProps" 
        |> fixNested "wrapperProps" 
        |> fixNested "wrapperStyle"
//    System.Console.WriteLine(propsPojo)
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

let autocompleteBasic = ofBasic >> autocomplete
