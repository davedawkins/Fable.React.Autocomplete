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
- You will also need css module loaders for Webpack because we are going to import the styles directly from npm `css-loader` and `style-loader`, install them :
```
npm install css-loader style-loader --save-dev
```
- Now from your Webpack, use the loaders:
```
{
    test: /\.(sa|c)ss$/,
    use: [
        "style-loader",
        "css-loader"
    ]
}
```

## Usage 

[Live Demo with examples](https://davedawkins.github.io/Fable.React.Autocomplete/)

```fs
type State = { SelectedTime : DateTime }

type Msg = UpdateSelectedTime of DateTime 

let init() = { SelectedTime = DateTime.Now }, Cmd.none

let update msg state = 
    match msg with 
    | UpdateSelectedTime time ->
        let nextState = { state with SelectedTime = time }
        nextState, Cmd.none

let render state dispatch = 
    Autocomplete.autocomplete 
        [ Autocomplete.Value state.SelectedTime 
          Autocomplete.OnChange (UpdateSelectedTime >> dispatch)
          Autocomplete.ClassName "input" ]


// Somewhere before you app starts
// you must import the CSS theme

importAll "autocomplete/dist/themes/material_green.css"

// or any of the other themes in the dist directory of autocomplete
```
