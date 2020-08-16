module AutoComplete

open Fable.React
open Fable.React.Props
open Fable.Core.JsInterop
open Fable.Core
open Browser

type AutoCompleteProps<'Item> = 
  | Items of 'Item array
  | RenderItem of ('Item -> bool -> ReactElement)
  | Value of string
  | GetItemValue of ('Item -> string)
  | OnChange of (Browser.Types.Event -> string -> unit)
  | OnSelect of (string -> unit)
  | MenuStyle of CSSProp list
  | ShouldItemRender of ('Item -> string -> bool)
  | InputProps of obj

// Default menu style from reactjs-autocomplete with addition of ZIndex to bring menu to front. Otherwise
// seems to be buried under other components
let defaultMenuStyle = [
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

// Duplicates list-like object to give it back iteration capability
// Workaround for issue logged against fable react
let rec private makeList list = 
    match list with
    | [] -> []
    | x::xs -> x :: (makeList xs)


let private _ac<'Item> (props : AutoCompleteProps<'Item> list) = 
    let propsObj = (keyValueList CaseRules.LowerFirst (makeList props))

    propsObj?menuStyle <- keyValueList CaseRules.LowerFirst defaultMenuStyle

//    System.Console.WriteLine( propsObj )

    ofImport "default" "react-autocomplete" propsObj []

let autocomplete<'Item>  =
    FunctionComponent.Of( _ac<'Item>, "autocompleteView", memoEqualsButFunctions)

type BasicProps = {
    Items           : string list
    Model           : string
    Dispatch        : string -> unit
}

// Batteries-included version of autocomplete that plays nicely with Elmish
// Just supply a list of options to choose from, the current value and a way 
// to dispatch a new value and you're good to go.
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
        _ac<string> props
    )