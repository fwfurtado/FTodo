namespace Api.Controllers

open Commons.Monads
open Domain.Entities
open Domain.Repositories
open Domain.Services
open Microsoft.AspNetCore.Http
open Microsoft.AspNetCore.Mvc
open Microsoft.Extensions.Logging

type TodoView =
    { Id: int option
      Title: string
      Description: string
      State: string }

type NewTodoRequest = { Title: string; Description: string }

[<ApiController>]
[<Route("todos")>]
type TodoController(logger: ILogger<TodoController>, service: TodoServices) =
    inherit ControllerBase()

    let toView (todo: Todo) =
        { Id = todo.Id
          Title = todo.Title
          Description = todo.Description
          State =
            match todo.State with
            | TodoState.Backlog -> "backlog"
            | TodoState.InProgress -> "in_progress"
            | TodoState.Completed -> "completed" }

    [<HttpGet>]
    member _.List() =
        async {
            logger.LogInformation("Listing all todos")
            let! result = service.GetTodosAsync()

            return Results.Ok(result |> Seq.map toView)
        }

    [<HttpGet("{id}", Name = "ShowTodo")>]
    member _.Show(id: int) =
        async {
            logger.LogInformation("Showing todo {id}", id)

            let! result = service.GetTodoAsync(id)

            return
                match result with
                | Some todo -> Results.Ok(toView todo)
                | None -> Results.NotFound()
        }
    
    [<HttpPost>]
    member _.Create([<FromBody>] request: NewTodoRequest) =
        async {
            logger.LogInformation("Creating todo {title} with description {description}", request.Title, request.Description)

            let! result = service.CreateTodoAsync(request.Title, request.Description)
            
            logger.LogInformation("Created todo result {result}", result)
            
            match result with
            | Error msg -> return Results.BadRequest({|Message = msg|})
            | Success result -> return Results.CreatedAtRoute("ShowTodo", {| id = result.Id.Value |}, toView result)
        }
