module Domain.Repositories

open Domain.Entities

type ITodoRepository =
    abstract member FindByIdAsync : int -> Async<Todo option>
    abstract member FindAllAsync : unit -> Async<Todo seq>
    abstract member AddTodo : todo:Todo -> Async<int>
    abstract member UpdateTodo : todo:Todo -> Async<Todo>