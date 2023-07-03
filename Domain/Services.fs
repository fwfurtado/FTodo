module Domain.Services

open Domain.Repositories
open Domain.Entities
open Commons.Monads

type TodoServices(repository: ITodoRepository) =

    member _.GetTodosAsync() = repository.FindAllAsync()

    member _.GetTodoAsync(id: int) = repository.FindByIdAsync id

    member _.CreateTodoAsync(title: string, description: string) =
        async {
            let todo = createTodo title description
            let! id = repository.AddTodo todo

            return setId todo id
        }
    
    member _.MoveForwardAsync(id: int) =
        async {
            match! repository.FindByIdAsync id with
            | Some todo -> return! (todo >> transitionForward >> repository.UpdateTodo >> Success)
            | None -> return Fail "Todo not found"          
        }