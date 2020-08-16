module Components.Autocomplete.State

open System
open Elmish
open Components.Autocomplete.Types

let init() = { SelectedItem = ""; MenuVisible = None }, Cmd.none

let update msg state = 
    match msg with 
    | MenuVisibilityChanged visible ->
        { state with MenuVisible = Some visible }, Cmd.none
    | UpdateSelectedItem item ->
        { state with SelectedItem = item }, Cmd.none