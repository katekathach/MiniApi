using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddDbContext<TodoDb>(opt => opt.UseInMemoryDatabase("TodoList"));//DI 
builder.Services.AddDatabaseDeveloperPageExceptionFilter(); //DI
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();


app.UseSwagger();
app.UseSwaggerUI();

app.MapGet("/", () => "Hello World!");

///********************************************************************//
/// TIME COVERSION mini api
//*******************************************************************//
app.MapGet("/TimeConversion/{input}", (string input) =>
{
    List<Time> time = new();
    if (input.Contains('-'))
    {
        var localDateTime = DateTime.Parse(input);
        var univDateTime = localDateTime.ToUniversalTime();
        Int32 unixTimestamp = (Int32)(DateTime.Now.Subtract(new DateTime(1970, 1, 1))).TotalSeconds;
        var UnixTiemStamp = DateTimeOffset.Parse(input).ToUnixTimeSeconds();
        time.Add(new Time { UNIX = UnixTiemStamp.ToString(), UTC = univDateTime.ToString() });
    }
    else
    {
        long localDateTime = Int64.Parse(input);
        System.DateTime dtDateTime = new DateTime(1970, 1, 1, 0, 0, 0, 0, System.DateTimeKind.Utc);
        dtDateTime = dtDateTime.AddMilliseconds(localDateTime).ToLocalTime();       
        time.Add(new Time { UNIX = localDateTime.ToString(), UTC = dtDateTime.ToString()});
    }
    return Results.Ok(time.ToList());
});

app.MapGet("/TimeConversion/", () => Results.Ok(new 
{
   thisDate =  DateTime.Now.ToString("D")


}));

app.MapGet("ok-object", () => Results.Json(new
{
    thisDate = DateTime.Now.ToString("D")


}));

app.MapGet("/HrsToDays/{hours}", (double hours) =>
{
    TimeSpan result = TimeSpan.FromHours(hours);

    return Results.Ok(result.TotalDays + " Days");
});

app.MapGet("/DaysToHrs/{Days}", (double Days) =>
{
    
    TimeSpan day = TimeSpan.FromDays(Days);

    return Results.Ok(day.TotalHours + " Hours");
});


/***********************************************************************/
// mini api map get with param with route constraints
/***********************************************************************/
app.MapGet("get-params/{age:int}", (int age) => { return $"Age provided was {age}"; }); //explicit map integer to params

app.MapGet("cars/{carId:regex(^[a-z0-9]+$)}", (string carId) => { return $"Car id provide was: {carId}"; });

app.MapGet("BooksISBN/{ISBN:length(13)}", (string ISBN) => { return $"ISBN provide was: {ISBN}"; });

///********************************************************************//
///  TODO list mini api
///*******************************************************************//
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