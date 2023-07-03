module rec Data.Todo

open Microsoft.EntityFrameworkCore
open Domain
open Microsoft.EntityFrameworkCore.Metadata.Builders
open EntityFrameworkCore.FSharp.Extensions
open EntityFrameworkCore.FSharp.DbContextHelpers



module DB =

    [<CLIMutable>]
    type TodoDB =
        { Id: int
          Title: string
          Description: string
          State: string }

    let private stateToString (state: Entities.TodoState) =
        match state with
        | Entities.TodoState.Backlog -> "backlog"
        | Entities.TodoState.InProgress -> "in_progress"
        | Entities.TodoState.Completed -> "completed"

    let private stringToState (state: string) =
        match state with
        | "backlog" -> Entities.TodoState.Backlog
        | "in_progress" -> Entities.TodoState.InProgress
        | "completed" -> Entities.TodoState.Completed
        | _ -> Entities.TodoState.Backlog


    let fromDomain (domain: Entities.Todo) =
        { Id =
            match domain.Id with
            | Some id -> id
            | None -> 0
          Title = domain.Title
          Description = domain.Description
          State = stateToString domain.State }

    let toDomain (db: TodoDB) : Entities.Todo =
        { Id = Some db.Id
          Title = db.Title
          Description = db.Description
          State = stringToState db.State }


type Context(options: DbContextOptions<Context>) =
    inherit DbContext(options)

    [<DefaultValue>]
    val mutable private Todos: DbSet<DB.TodoDB>

    member this._Todos
        with public get () = this.Todos
        and public set (value) = this.Todos <- value


    override this.OnModelCreating(modelBuilder) =
        modelBuilder.RegisterOptionTypes() |> ignore
        modelBuilder.ApplyConfiguration(Mapping()) |> ignore

    member this.FindById(id: int) =
        async {
            match! tryFindEntityAsync this id with
            | Some todo -> return Some(DB.toDomain todo)
            | None -> return None
        }

    member this.FindAll() =
        async {
            let! result = toListAsync this.Todos

            return result |> List.map DB.toDomain |> List.toSeq
        }
    
    member this.Add(todo: Entities.Todo) =
        async {
            let dbTodo = DB.fromDomain todo
            
            let! saved = addEntityAsync' this dbTodo
            
            saveChanges this
            
            return saved.Entity.Id
        }

type Mapping() =
    interface IEntityTypeConfiguration<DB.TodoDB> with

        member this.Configure(builder: EntityTypeBuilder<DB.TodoDB>) =
            builder.ToTable("todos") |> ignore

            builder.HasKey(fun x -> x.Id :> obj) |> ignore

            builder
                .Property(fun x -> x.Id)
                .HasColumnName("id")
                .ValueGeneratedOnAdd() |> ignore

            builder.Property(fun x -> x.Title).HasColumnName("title").IsRequired() |> ignore

            builder
                .Property(fun x -> x.Description)
                .HasColumnName("description")
                .IsRequired()
            |> ignore

            builder.Property(fun x -> x.State).HasColumnName("state").IsRequired() |> ignore
