# DocumentDB StoredProcs in F# via Fable 

This repositry is an exploration of how to write DocumentDB stored procedures using F#. This scenario is not supported by Microsoft, The idea is to write a stored procedure in F#, use Fable to compile to JavaScript and register it with DocumentDB using the standard API for this. 

## DocumentDB
[DocumentDB](https://azure.microsoft.com/en-us/services/documentdb/) is a Microsoft NonSQL database for the storage, reteival and manipulation of JSON based documents. Documents may be added, modified and removed using a number of client side APIs. This means any languages that support those APIs may be used, [for example](https://docs.microsoft.com/en-us/dotnet/articles/fsharp/using-fsharp-on-azure/#using-azure-documentdb-with-f). However to make transactional changes to multiple documents the changes must be done using a [JavaScript stored procedure](https://www.documentdb.com/javascript/tutorial). The workflow is to write the procedure in Java Script and use DocumentDB calls to register and invoke the stored procedure. If done using the .Net SDK that will look something like this:


```
var helloWorld = new StoredProcedure { Id = "HelloWorld", Body = @"function () {
        var context = getContext();
        var response = context.getResponse();
        response.setBody("Hello, World");
    }"
};
// register stored procedure
StoredProcedure createdStoredProcedure = await client.CreateStoredProcedureAsync(collection.SelfLink, helloWorld);
// execute stored procedure
Document createdDocument = await client.ExecuteStoredProcedureAsync<String>(hellowWorld.SelfLink);
```

## Fable
[Fable]() is a F# to JavaScript compiler. While it is a general purpose tool most of the examples and activity is focused on browser based programming. 

To get a feel for Fable I down loaded [this](https://github.com/Pauan/fable-getting-started) simple example. If you download, configure and build this example you will see it  compiles the following F#
```
let main () =
    Browser.console.log message

do
    main ()
```

to the following JavaScript embeded in an index.html file.
```
function main() {
    console.log(message$$1);
}
main();
```
Along with cross complied JavaScript function there is lot of other JavaScript in the file presumable to support the runtime, ECMAScript shims, browser specific hacks, possibly libraries for F# features etc.


## F# Stored procedures
To write F# stored procedures we need to compile the F# code to JavaScript, then register the stored procedure using the .net SDK. 

### Compilation
Going back to [`Hello World`](https://www.documentdb.com/javascript/tutorial). We can write the equivalent "Hello World" in F# like this
```
let helloWorld =
    let context = getContext
    let response = context.getResponse
    response.setBody("Hello World")
```
What we would like is for the tooling to generate the same JavaScript as in the example. 

```
function () {
        var context = getContext();
        var response = context.getResponse();
        response.setBody("Hello, World");
    }
```

But of course the F# code will not compile without `getContext` and other referenced types. The simplest thing I could think of was to write some F# scaffolding to implement `getContext` and other elements of the DocumentDB SDK referenced. I imagine referening that SDK would also work. So after adding

```
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
```
to main.fs it compiled. Buiding and the app results in this in the main.js file:
![helloWorld function](docs\hello.png)

**Success!** - well perhaps not quite.  

## Next Steps

For one thing there is that `();` at the end of function. But that aside this is just HelloWorld and it has not even been registered and executed. The above is not even a proof of concept. Proof of plausibility? Perhaps. Some next steps are:

* Figure out how to have Fable generate the JavaScript for a single function as a string rather than writing it to the index.html file.
* Determine if it is possible to extend the JavaScript environment on the DocumentDB side with the various shims that will be needed as the F# code gets more complex.
* Test registering the stored procedure and then executing it
* Figure out how document parameters work and how to map between the F# types and the stored procedure types.

## Is it Worthwhile

Why bother with this? Afterall the JavaScript in most stored procedures will be simple and not too hard to write. One reason is of course to have F# everywhere. A second is that programming DocumentDB in F# adds backs of the type saftey lost when going to NoSQL. A kind of middle ground between no schema and a SQL schema. But the biggest reason is because this is the last piece needed to create a F# end-to-end stack:

* F# is now well established on the browser
* F# works with Azure functions 

Adding F# to DocumentDB stored procedures should make it possible to flow the same types from the browser through HTTP triggered Azure functions to DocumentDB. Adding F# stored proceedures means the transactional parts of the application running on DocumentDB can also be written in F#.



Comments, ideas or help are very welcome. You can reach me:

Twitter: jbeeko

GitHub: jbeeko
