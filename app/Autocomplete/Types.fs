module Components.Autocomplete.Types

open System 

type State = {
    SelectedItem : string
    MenuVisible : bool option
}

type Msg = 
    | UpdateSelectedItem of string 
    | MenuVisibilityChanged of bool
