using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddDbContext<TodoDb>(opt => opt.UseInMemoryDatabase("TodoList"));//DI 
builder.Services.AddDatabaseDeveloperPageExceptionFilter(); //DI

var app = builder.Build();

app.MapGet("/", () => "Hello World!");

///********************************************************************//
///  TODO TIME COVERSION mini api
//*******************************************************************//
app.MapGet("/TimeConversion/{input}", (string input) =>
{
    List<Time> time = new();
    if (input.Contains('-') || input == " ")
    {
        var localDateTime = DateTime.Parse(input);
        var univDateTime = localDateTime.ToUniversalTime().ToFileTimeUtc();
        
        time.Add(new Time { UNIX = univDateTime.ToString() , UTC = univDateTime.ToString() });
    }
    return Results.Ok(time);
});


///********************************************************************//
///  TODO list mini api
//*******************************************************************//
app.MapGet("/todoitems", async (TodoDb db) =>
    await db.Todos.ToListAsync());

app.MapGet("/todoitems/complete", async (TodoDb db) =>
    await db.Todos.Where(t => t.IsComplete).ToListAsync());


app.MapGet("/todoitems/{id}", async (int id, TodoDb db) => 
    await db.Todos.FindAsync(id)
        is Todo todo
            ? Results.Ok(todo)
            : Results.NotFound()
);



app.MapPost("/todoitems", async (Todo todo, TodoDb db) =>
{
    db.Todos.Add(todo);
    await db.SaveChangesAsync();

    return Results.Created($"/todoitems/{todo.Id}", todo);
});


app.MapPut("/todoitems/{id}", async (int id, Todo inputTodo, TodoDb db) =>
{
    var todo = await db.Todos.FindAsync(id);

    if (todo is null) return Results.NotFound();

    todo.Name = inputTodo.Name;
    todo.IsComplete = inputTodo.IsComplete;

    await db.SaveChangesAsync();

    return Results.NoContent();
});

app.MapDelete("/todoitems/{id}", async (int id, TodoDb db) =>
{
    if (await db.Todos.FindAsync(id) is Todo todo)
    {
        db.Todos.Remove(todo);
        await db.SaveChangesAsync();
        return Results.Ok(todo);
    }

    return Results.NotFound();
});

app.Run();
class Time{
 public string UTC { get; set; }
    public string UNIX { get; set; }

}
class Todo
{
    public int Id { get; set; }
    public string? Name { get; set; }
    public bool IsComplete { get; set; }
}

class TodoDb : DbContext
{
    public TodoDb(DbContextOptions<TodoDb> options)
        : base(options) { }

    public DbSet<Todo> Todos => Set<Todo>();
}