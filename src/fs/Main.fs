module App.Main

// This must be before everything else
Fable.Core.JsInterop.importAll("babel-polyfill") |> ignore

open Fable.Import
open App.Message

// Some mock values expected to be available to the procedure 
type HTTPBody = {
    mutable data : string
    }
    with
    member this.setBody data = this.data <- data
    member this.getBody = this.data

type Request = HTTPBody
type Response = HTTPBody

type Context = {
    response : Response;
    request : Request;
    }
    with 
        member this.getResponse = this.response
        member this.getRequest = this.request

let getContext = 
    {response = {data = ""};
    request = {data = "this is a request"}}

//The function to become the stored procedure.
let helloWorld =
    let context = getContext
    let response = context.getResponse
    response.setBody("Hello World")

let main () =
    Browser.console.log message

do
    main ()
