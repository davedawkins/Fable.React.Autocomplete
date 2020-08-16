module Components.Autocomplete.Types

open System 

type State = {
    SelectedItem : string
}

type Msg = UpdateSelectedItem of string 
