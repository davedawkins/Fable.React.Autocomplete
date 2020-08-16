module App.View

open App.Types

open Fulma
open Fable.React
open Fable.React.Props
open Fable.Core.JsInterop

let introduction =
    div [ ]
        [ h1 [ Style [ FontSize 30 ] ] [ str "Fable.React.Autocomplete" ]
          p  [ ] [ str "Fable binding for react-autocomplete that is ready to use within Elmish applications" ]
          br [ ]
          Common.highlight """
          """
          br [ ]
          hr [ ]
          h1 [ Style [ FontSize 30 ] ] [ str "Examples and configurations" ]
          br [ ] ]

let spacing = Props.Style [ Props.Padding 20 ]

let render (state: State) dispatch =
  div [ Style [ Padding 20.0 ] ] [
    Components.Autocomplete.View.render state.Autocomplete (AutocompleteMsg >> dispatch)
  ]
  