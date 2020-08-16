module Components.Autocomplete.State

open System
open Elmish
open Components.Autocomplete.Types

let init() = { SelectedItem = ""}, Cmd.none

let update msg state = 
    match msg with 
    | UpdateSelectedItem item ->
        let nextState = { state with SelectedItem = item }
        nextState, Cmd.none