module Components.Autocomplete.View

open Components.Autocomplete.Types

open Fable.React
open Fable.React.Props
open Fable.Import
open System
open AutoComplete

let songs = [
  "99 Red Balloons"
  "Are You Gonna Be My Girl"
  "Are You Gonna Go My Way"
  "Black Dog"
  "Dakota"
  "Immigrant Song"
  "Thing Called Love"
  "The Lemon Song"
  "Whole Lotta Love"
  "Whole Lotta Rosie" 
]

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
        | _ -> "Selection is "

    div [ ClassName "mt-2"; Style [ Color "gray"; FontSize "80%" ] ] [
        str currentLabel
        em [] [ str state.SelectedItem ]
    ]

let renderTemplate title description autocomplete code state =
    div [] [
        h3 [ ClassName "title is-3" ] [
            str title
        ]
        description
        div [ Style [ Margin "20px" ] ] [
            div [] [
                label [ ClassName "label" ] [
                    str "Make Selection"
                ]
                autocomplete
            ]
            currentSelection state
        ]
        p [] [ str "" ]
        h4 [ ClassName "title is-4" ] [
            str "Code"
        ]
        div [] [ Common.highlight code ]
    ]


let renderExample1 (state:State) dispatch =
    renderTemplate
      "Example #1 - Basic"
      (div [] [
        str "This uses the simple "
        slackCode "autocompleteBasic"
        str " function, which minimises the amount of configuration required to get started."
      ])
      (AutoComplete.autocompleteBasic { 
        Items = songs
        Model = state.SelectedItem
        Dispatch = UpdateSelectedItem >> dispatch 
      })
      """
    AutoComplete.autocompleteBasic {
      Items = songs
      Model = state.SelectedItem
      Dispatch = UpdateSelectedItem >> dispatch
      }
      """
      state

let renderExample1a (state:State) dispatch =
    renderTemplate
      "Example #1a - Basic + Styling"
      (div [] [
        str """
        We can make use of the ofBasic function to get the benefits of autocompleteBasic, but then append 
        more advanced style options to pass into the more advanced autocomplete function. 
        This example adds Bulma styling to the <input> record.
        The ofBasic function takes a BasicProps record and converts it to a list of AutocompleteProps<string>
        properties.
        """
      ])
      (AutoComplete.autocomplete <|
        InputProps [ ClassName "input is-primary" ] ::
        ofBasic { 
            Items = songs
            Model = state.SelectedItem
            Dispatch = UpdateSelectedItem >> dispatch 
        }
      )
      """
    AutoComplete.autocomplete <|
        InputProps [ ClassName "input is-primary" ] ::
        ofBasic { 
            Items = songs
            Model = state.SelectedItem
            Dispatch = UpdateSelectedItem >> dispatch 
        }
      """
      state

// ----------------------------------------------------------------------------
// Example #2

type MenuItem = { Key: string; Label: string }

let example2Options state dispatch = [ 
    Items (songs |> List.mapi (fun i v -> { Key = string i; Label = v }) |> List.toArray)
    Value state.SelectedItem
    GetItemValue(fun item -> item.Label)
    OnSelect(UpdateSelectedItem >> dispatch)
    ShouldItemRender(fun item value -> item.Label.ToLower().Contains(value.ToLower()))
    OnChange(fun e v -> v |> UpdateSelectedItem |> dispatch)
    InputProps [ ClassName "input is-primary"; Placeholder "Choose a song" ]
    RenderItem(fun item highlight ->
        div [ Prop.Key <| item.Key // Keep React happy.
              Props.Style [ Background(if highlight then "gray" else "none") ] ] [
            str item.Label
        ])
  ]

let renderExample2 state dispatch =
    renderTemplate
        "Example #2 - Advanced"
        (div [] [
            str "This uses the more advanced "
            slackCode "autocomplete"
            str " function, providing access to the full API of the underlying react-autocomplete component"
            br []
            br []
            str """
            Note how we're using a MenuItem as the data element for Items, and then handling the member fields
            in GetItemValue, ShouldItemRender, RenderItem.
                """
         ])
        (AutoComplete.autocomplete (example2Options state dispatch))
        """
    type MenuItem = { Key: string; Label: string }

    Autocomplete.autocomplete [
      Items (songs |> List.mapi (fun i v -> { Key = string i; Label = v }) |> List.toArray)
      Value state.SelectedItem
      GetItemValue(fun item -> item.Label)
      OnSelect(UpdateSelectedItem >> dispatch)
      ShouldItemRender(fun item value -> item.Label.ToLower().Contains(value.ToLower()))
      OnChange(fun e v -> v |> UpdateSelectedItem |> dispatch)
      InputProps [ ClassName "input is-primary"; Placeholder "Choose a song" ]  // Adds styling to <input>
      RenderItem(fun item highlight ->
          div [ Prop.Key <| item.Key // Keep React happy.
                Props.Style [ Background(if highlight then "gray" else "none") ] ] [
              str item.Label
          ])
    ]
        """
        state

let example3Options state dispatch =
  (OnMenuVisibilityChange (MenuVisibilityChanged >> dispatch)) 
    :: (example2Options state dispatch)

let renderExample3 state dispatch = 
  renderTemplate
    "Example #3 - OnMenuVisibilityChanged"
    (div [] [
        str "A simple notification on the visibility of the menu. In the example below we are dispatching the message "
        slackCode "MenuVisibilityChanged of bool"
        br []
        br []
        str "Menu is visible: "
        (Option.defaultValue false state.MenuVisible) |> string |> str
    ])
    (AutoComplete.autocomplete (example3Options state dispatch))
    """
    OnMenuVisibilityChange (MenuVisibilityChanged >> dispatch)
    // ... other options
    """
    state

let example4Options state dispatch =
  (SortItems (fun a b v -> String.Compare(b.Label, a.Label) )) 
    :: (example2Options state dispatch)

let renderExample4 state dispatch = 
  renderTemplate
    "Example #4 - Sorting"
    (div [] [
        str "A compare function that also tells you what the current value is"
    ])
    (AutoComplete.autocomplete (example4Options state dispatch))
    """
    SortItems (fun a b v -> String.Compare(b.Label, a.Label))
    // ... other options
    """
    state

let example5Options state dispatch =
  (IsItemSelectable (fun item -> String.Compare( item.Label.ToUpper(), "I" ) <= 0 )) 
    :: (example2Options state dispatch)

let renderExample5 state dispatch = 
  renderTemplate
    "Example #5 - IsItemSelectable"
    (div [] [
        str "In the example below, notice how any item from 'I' onwards is displayed but cannot be selected"
    ])
    (AutoComplete.autocomplete (example5Options state dispatch))
    """
    IsItemSelectable (fun item -> String.Compare( item.Label.ToUpper(), "I" ) <= 0 )
    // ... other options
    """
    state


let render (state: State) dispatch =
    div [] [
        h2 [ ClassName "title is-2" ] [
            str "Autocomplete Demo"
        ]
        p [] [
            str "The following examples make use of these type definitions:"            
        ]
        div [] [ Common.highlight
            """
    let songs = [
      "99 Red Balloons"
      "Are You Gonna Be My Girl"
      "Are You Gonna Go My Way"
      "Black Dog"
      "Dakota"
      "Immigrant Song"
      "Thing Called Love"
      "The Lemon Song"
      "Whole Lotta Love"
      "Whole Lotta Rosie" 
    ]

    type State = {
        SelectedItem : string
        MenuVisible : bool option
    }

    type Msg = 
        | UpdateSelectedItem of string 
        | MenuVisibilityChanged of bool

            """
        ]
        renderExample1 state dispatch
        hr []
        renderExample1a state dispatch
        hr []
        renderExample2 state dispatch
        hr []
        renderExample3 state dispatch
        hr []
        renderExample4 state dispatch
        hr []
        renderExample5 state dispatch
    ]
