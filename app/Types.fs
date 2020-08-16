module App.Types

type Page =
    | Introduction
    | Usage

type State = {
    CurrentPage : Page
    Autocomplete : Components.Autocomplete.Types.State
}

type Msg =
    | View of Page
    | AutocompleteMsg of Components.Autocomplete.Types.Msg
