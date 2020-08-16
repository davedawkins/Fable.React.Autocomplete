module App.State 

open App.Types
open Elmish

let init() = 
    let initAutocompleteState, initAutocompleteCmd = Components.Autocomplete.State.init()
    
    { CurrentPage = Introduction
      Autocomplete = initAutocompleteState }, Cmd.map AutocompleteMsg initAutocompleteCmd

let update msg state = 
    match msg with 
    | View page -> { state with CurrentPage = page }, Cmd.none
    | AutocompleteMsg msg -> 
        let nextAutocompleteState, nextAutocompleteCmd = 
            Components.Autocomplete.State.update msg state.Autocomplete 
        let nextState = { state with Autocomplete = nextAutocompleteState }
        nextState, Cmd.map AutocompleteMsg nextAutocompleteCmd