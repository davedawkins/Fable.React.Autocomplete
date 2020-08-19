# Fable.React.Autocomplete [![Build Status](https://travis-ci.org/DaveDawkins/Fable.React.Autocomplete.svg?branch=master)](https://travis-ci.org/DaveDawkins/Fable.React.Autocomplete) [![Build status](https://ci.appveyor.com/api/projects/status/9ihe9vmw3k37u72r?svg=true)](https://ci.appveyor.com/project/DaveDawkins/fable-react-autocomplete) [![Nuget](https://img.shields.io/nuget/v/Fable.React.Autocomplete.svg?maxAge=0&colorB=brightgreen)](https://www.nuget.org/packages/Fable.React.Autocomplete)

A complete binding for [react-autocomplete](https://github.com/reactjs/react-autocomplete) that is ready to use within [Elmish](https://github.com/fable-elmish/elmish) applications

## Installation
- Install this library from nuget
```
paket add Fable.React.Autocomplete --project /path/to/Project.fsproj
```
- Install the actual Autocomplete library from npm
```
npm install react-autocomplete --save
```

## Documentation

There are two basic functions available: `autocompleteBasic` and `autocomplete`.

Use `autocompleteBasic` for the simplest experience, when all you need is to choose from a list of strings. You supply the list of strings, the current value and a dispatch function, as a `record`.

Use `autocomplete` for access to the complete API supplied by [`react-autocomplete`](). Here you supply a list of option values to configure the component. Use this version if your data is more complex than a list of strings, or you wish to tweak styling or event handling.

[Live Demo with examples](https://davedawkins.github.io/Fable.React.Autocomplete/)

### Autocomplete.autocompleteBasic

#### Example

```fs
type State = { SelectedItem : string }

type Msg = UpdateSelectedItem of string 

let init() = { SelectedItem = "" }, Cmd.none

let update msg state = 
    match msg with 
    | UpdateSelectedItem item ->
        let nextState = { state with SelectedItem = item }
        nextState, Cmd.none

let render state dispatch = 
    AutoComplete.autocompleteBasic
        { Items =
                [ "99 Red Balloons"
                "Are You Gonna Be My Girl"
                "Are You Gonna Go My Way"
                "Dakota"
                "Thing Called Love"
                "The Lemon Song"
                "Black Dog"
                "Immigrant Song"
                "Whole Lotta Love"
                "Whole Lotta Rosie" ]
            Model = state.SelectedItem
            Dispatch = UpdateSelectedItem >> dispatch }

```

The argument to `autocompleteBasic` is a record of type `BasicProps`:

```fs
type BasicProps = {
    Items           : string list
    Model           : string
    Dispatch        : string -> unit
}
```

| BasicProp   | Description                                                                                |
| ----------- | ------------------------------------------------------------------------------------------ |
| Items       | List of strings to be offered to user                                                      |
| Model       | The current value of the selection                                                         |
| Dispatch    | Function to call when user makes a selection. Also called while typing into input element  |


### Autocomplete.autocomplete

#### Example
```fs
let render state dispatch = 
    AutoComplete.autocomplete [
        Items [|
            { Key = "0"; Label = "99 Red Balloons" }
            { Key = "1"; Label = "Are You Gonna Be My Girl" }
            { Key = "2"; Label = "Are You Gonna Go My Way" }
            { Key = "3"; Label = "Dakota" }
            { Key = "4"; Label = "Thing Called Love" }
            { Key = "5"; Label = "The Lemon Song" }
            { Key = "6"; Label = "Black Dog" }
            { Key = "7"; Label = "Immigrant Song" }
            { Key = "8"; Label = "Whole Lotta Love" }
            { Key = "9"; Label = "Whole Lotta Rosie" }
        |]
        Value state.SelectedItem
        RenderItem(fun item highlight ->
            div [ Prop.Key <| item.Key // Keep React happy.
                    Props.Style [ Background(if highlight then "gray" else "none") ] ] [
                str item.Label
            ])
        GetItemValue(fun item -> item.Label)
        OnSelect(UpdateSelectedItem >> dispatch)
        ShouldItemRender(fun item value -> item.Label.ToLower().Contains(value.ToLower()))
        OnChange(fun e v -> v |> UpdateSelectedItem |> dispatch)
        InputProps [ ClassName "input is-primary" ]
    ]
```

The argument to `autocomplete` is a `list` of `AutoCompleteProps<'Item>`. The generic type argument `'Item` allows you to show an array of any record type.

```fs
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
  | SortItems of ('Item -> 'Item -> string -> int)
  | Value of string
  | WrapperProps of HTMLAttr list
  | WrapperStyle of CSSProp list
```

See original documentation for [react-autocomplete](https://github.com/reactjs/react-autocomplete) for a detailed
explanation of these configuration items. The following table discusses any F# aspects of their binding implementations.

| AutoCompleteProps | Description                                                                                |
| ---------------- | ------------------------------------------------------------------------------------------ |
| GetItemValue | Return the string value of the given `item` record |
| Items | Array of `item` records |
| RenderItem | Render `item` as a `ReactElement`. Second argument is `true` if `item` should be highlighted |
| AutoHighlight |  |
| InputProps | Properties for the <input> element. List of HTMLAttr |
| IsItemSelectable | |
| MenuStyle | Style properties for the menu. A list of CSSProp |
| OnChange | |
| OnMenuVisibilityChange | |
| OnSelect | |
| Open | |
| RenderInput | |
| RenderMenu | |
| SelectOnBlur | |
| ShouldItemRender | |
| SortItems |  Compare function for Array.sort. string argument is current value |
| Value | |
| WrapperProps | List of HTMLAttr |
| WrapperStyle | List of CSSProp |

### ofBasic

Use this to convert a `BasicProps` record into a list of `AutoCompleteProps`, to which you can append other `AutoCompleteProps`, such as `MenuStyle`, `InputProps`, `WrapperProps`, `WrapperStyle`.

For example

```fs
    AutoComplete.autocomplete <|
        InputProps [ ClassName "input is-primary" ] ::
        ofBasic { 
            Items = songs
            Model = state.SelectedItem
            Dispatch = UpdateSelectedItem >> dispatch 
        }
```

The definition of `ofBasic` is as follows

```fs
let ofBasic (basicProps : BasicProps) = // BasicProps -> AutoCompleteProps<string>
    let props = [
      Items (List.toArray basicProps.Items)
      RenderItem (fun item highlight -> 
        div [
                Prop.Key <| string item // Keep React happy.
                Props.Style [ Background (if highlight then "gray" else "none") ] 
            ] 
            [ str item  ]
      )
      GetItemValue id
      OnSelect basicProps.Dispatch
      ShouldItemRender (fun item value -> item.ToLower().Contains( value.ToLower() ))
      Value basicProps.Model
      OnChange (fun e v -> v |> basicProps.Dispatch)
    ]
    props

```

## Issues
- `react-autocomplete` doesn't itself appear to be maintained
- Working around an issue in FunctionComponent that appears to lose F# metadata from properties
passed through ReactJS (issue logged) - see `makeList` in Autocomplete.fs
