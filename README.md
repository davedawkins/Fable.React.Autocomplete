# Fable.React.Autocomplete [![Build Status](https://travis-ci.org/DaveDawkins/Fable.React.Autocomplete.svg?branch=master)](https://travis-ci.org/DaveDawkins/Fable.React.Autocomplete) [![Build status](https://ci.appveyor.com/api/projects/status/9ihe9vmw3k37u72r?svg=true)](https://ci.appveyor.com/project/DaveDawkins/fable-react-autocomplete) [![Nuget](https://img.shields.io/nuget/v/Fable.React.Autocomplete.svg?maxAge=0&colorB=brightgreen)](https://www.nuget.org/packages/Fable.React.Autocomplete)


A complete binding for [react-autocomplete](https://github.com/coderhaoxin/react-autocomplete) that is ready to use within [Elmish](https://github.com/fable-elmish/elmish) applications

## Installation
- Install this library from nuget
```
paket add Fable.React.Autocomplete --project /path/to/Project.fsproj
```
- Install the actual Autocomplete library from npm
```
npm install autocomplete react-autocomplete --save
```

## Usage 

[Live Demo with examples](https://davedawkins.github.io/Fable.React.Autocomplete/)

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
## Documentation

There are two basic functions available: `autocompleteBasic` and `autocomplete`.

Use `autocompleteBasic` for the simplest experience, when all you need is to choose from a list of strings. You supply the list of strings, the current value and a dispatch function, as a `record`.

Use `autocomplete` for access to the complete API supplied by [`react-autocomplete`](). Here you supply a list of DU options