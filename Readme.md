# ToDoList API

**Author**: Joshua Taylor
**Version**: 1.0.0

## Overview

ToDoList API is a simple RESTful API connected to a backend database which
allows users to create to-do items, view existing to-do items,
retrieve all to-do items, and to delete to-do items. All responses are
provided in JSON format.

## Getting Started

ToDoList API targets the .NET Core 2.0 platform, ASP.NET Core, Entity
Framework Core and the MVC Framework. The .NET Core 2.0 SDK can be downloaded 
from the following URL for Windows, Linux, and macOS:

https://www.microsoft.com/net/download/

Additionally, the Entity Framework tools will need to be installed via the
NuGet Package Manager Console in order to create a migration for the local,
development database (run from the solution root):

    Install-Package Microsoft.EntityFrameworkCore.Tools
	Add-Migration Initial
	Update-Database

The dotnet CLI utility would then be used to build and run the application:

    cd ToDoList
    dotnet build
    dotnet run

The _dotnet run_ command will start an HTTP server on localhost using Kestrel
which can be accessed by HTTP requests to the defined API endpoints.

Additionally, users can build and run ToDoList using Visual Studio
2017 or greater by opening the solution file at the root of this repository.
All dependencies are referenced via NuGet and should be brought in during
the restore process. If this does not occur, the following will download all
needed dependencies (other than the Entity Framework tools):

    dotnet restore

## Data Model

To-do list items are represented in state transfers as a simple JSON object
containing the following fields:

### id (int)

Primary key for each to-do list item. If included in request body JSON, this
will need to match the id specified in routing.

### title (string)

A JSON string containing the title of each to-do list item.

### isComplete (bool)

A boolean JSON value representing whether the task has been completed.

## Example JSON Request / Response Body

__Note:__ Remove the "id" field for requests _or_ ensure that this field
matches with the id provided via routing.

    {
        "id": 3,
        "title": "Write README documentation",
        "isComplete": "false"
    }

## End-Points

### GET /api/todo

Retrieves all to-do list items stored within the backend database in JSON
format as an array.

### GET /api/todo/{id:int}

Retrieves the specified to-do list item by its numeric id primary key. A
successful response can be identified with code 200 with the requested item
in the response body in JSON format.

### POST /api/todo

Adds a new to-do list item using the JSON provided in the request body. Will
return code 201 on successful item creation along with the new item in the
response body in JSON format.

### PUT /api/todo/{id:int}

Updates the to-do list item with a numeric id primary key matching the id
passed from routing using the JSON representational state provided in the
request body. This method will update __all__ fields of the existing to-do
list item specified. Returns an empty 204 response on success per _RFC 2616 Section 10.2.5_.

### DELETE /api/todo/{id:int}

Deletes the existing to-do list item with the numeric id primary key matching
the id provided in routing. Provides an empty 204 response on success.

## Architecture

### TodoController

_TodoController_ provides all of the API logic for each of the above
documented API endpoints. TodoController obtains its database context
through dependency injection.

### TodoItem

TodoItem corresponds to the code-side representation of a to-do list
item described in the _Data Model_ section of this document. This
class is used for code-first migrations to the API's backend database
through the Entity Framework migration tools.

    public int Id           // Primary Key
    public string Title     // "title" from JSON
    public bool IsComplete  // "isComplete" from JSON

## Change Log

* 4.10.2018 [Joshua Taylor](mailto:taylor.joshua88@gmail.com) - Initial
release.