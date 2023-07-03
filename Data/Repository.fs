module Data.Repository

open Data.Todo
open Domain.Entities
open Domain.Repositories

type TodoRepository(context: Context) =
    interface ITodoRepository with
        member this.AddTodo(todo: Todo) =
            context.Add todo
        member this.FindAllAsync() =
            context.FindAll()
        member this.FindByIdAsync(id: int) =
            context.FindById id

        member this.UpdateTodo(todo) =
            context.UpdateState todo