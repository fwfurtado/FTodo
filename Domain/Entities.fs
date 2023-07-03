module Domain.Entities

open Commons.Monads

type TodoState =
    Backlog
    | InProgress
    | Completed

type TodoId = int option

type Todo = {
    Id: TodoId
    Title: string
    Description: string
    State: TodoState
}

type TodoList = Todo list

let createTodo title description =
    { Id = None; Title = title; Description = description; State = Backlog }

let setId todo id =
    if id < 0 then
        Fail "Id must be greater than 0"
    else
        Success { todo with Id = Some id }

let transitionForward todo =
    match todo.State with
    | Backlog -> { todo with State = InProgress }
    | InProgress -> { todo with State = Completed }
    | Completed -> todo

let transitionBackward todo =
    match todo.State with
    | Backlog -> todo
    | InProgress -> { todo with State = Backlog }
    | Completed -> { todo with State = InProgress }