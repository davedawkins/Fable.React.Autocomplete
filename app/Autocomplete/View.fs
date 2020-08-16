module Components.Autocomplete.View

open Components.Autocomplete.Types

open Fable.React
open Fable.React.Props
open Fable.Import
open System
open AutoComplete

let slackCode s =
    span [ Style [
               FontFamily "Monaco;Consolas;monospace"
               Color "#e01e5a"
           ] ] [
        str s
    ]

let currentSelection state =
    let currentLabel =
        match state.SelectedItem with
        | "" -> "No selection "
        | _ ->  "Selection is "

    div [ ClassName "mt-2"; Style [ Color "gray"; FontSize "80%" ] ] 
      [ 
          str currentLabel
          em [ ] [ str state.SelectedItem ] 
      ]

let renderExample1 (state: State) dispatch =
    let current =
        match state.SelectedItem with
        | "" -> "(none)"
        | _ -> state.SelectedItem

    div [] [
        h3 [ ClassName "title is-3" ] [
            str "Example #1 - Basic"
        ]
        p [] [
            str "This uses the simple "
            slackCode "autocompleteBasic"
            str " function, which minimises the amount of configuration required to get started."
        ]
        div [ Style [ Margin "20px" ] ] [
            div [] [
                label [ ClassName "label" ] [
                    str "Make Selection"
                ]
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
            ]
            currentSelection state
        ]
        p [] [ str "" ]
        h4 [ ClassName "title is-4" ] [
            str "Example Code"
        ]
        div [] [
            Common.highlight """
            AutoComplete.autocompleteBasic {
              Items = [
                  "99 Red Balloons"
                  "Are You Gonna Be My Girl"
                  "Are You Gonna Go My Way"
                  "Dakota"
                  "Thing Called Love"
                  "The Lemon Song"
                  "Black Dog"
                  "Immigrant Song"
                  "Whole Lotta Love"
                  "Whole Lotta Rosie"
                  ]
              Model = state.SelectedItem
              Dispatch = UpdateSelectedItem >> dispatch
              }
            """
        ]
    ]

// ----------------------------------------------------------------------------
// Example #2

type MenuItem = { Key: string; Label: string }

let renderExample2 (state: State) dispatch =
    div [] [
        h3 [ ClassName "title is-3" ] [
            str "Example #2 - Advanced"
        ]
        p [] [
            str "This uses the more advanced "
            slackCode "autocomplete"
            str " function, provides access to the full API of the underlying react-autocomplete component"
        ]
        div [ Style [ Margin "20px" ] ] [
            div [] [
                label [ ClassName "label" ] [
                    str "Make Selection"
                ]
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
                    InputProps {| ``class`` = "input is-primary" |}
                ]
            ]
            currentSelection state
        ]
        p [] [ str "" ]
        h4 [ ClassName "title is-4" ] [
            str "Example Code"
        ]
        div [] [
            Common.highlight """
            AutoComplete.autocomplete {
              }
            """
        ]
    ]

let render (state: State) dispatch =
    div [] [
        h2 [ ClassName "title is-2" ] [
            str "Autocomplete Demo"
        ]
        renderExample1 state dispatch
        hr []
        renderExample2 state dispatch
    ]
