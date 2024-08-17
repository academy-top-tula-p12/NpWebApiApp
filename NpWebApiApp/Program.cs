using NpWebApiApp;

List<Employee> employees = new()
{
    new("Bobby", 35),
    new("Jimmy", 22),
    new("Tommy", 29),
};


var builder = WebApplication.CreateBuilder(args);
var app = builder.Build();

app.MapGet("/", () => "Hello World!");

// Get list of employees
app.MapGet("/api/empl", () => employees);

// Get one employee
app.MapGet("/api/empl/{id}", (int id) => 
{
    Employee? employee = employees.FirstOrDefault(e => e.Id == id);
    if (employee is not null)
        return Results.Json(employee);
    else
        return Results.NotFound(new { Message = "Employee not found (ID incorrect)"});
    
});

// Add employee
app.MapPost("/api/empl", (Employee employee) =>
{
    employee.Id = Employee.idGlobal++;
    employees.Add(employee);

    return Results.Json(employee);
});

app.MapPut("/api/empl", (Employee employeeData) =>
{
    Employee? employee = employees.FirstOrDefault(e => e.Id == employeeData.Id);
    if(employee is not null)
    {
        employee.Name = employeeData.Name;
        employee.Age = employeeData.Age;

        return Results.Json(employee);
    }
    else
        return Results.NotFound();
});

app.MapDelete("/api/empl/{id}", (int id) =>
{
    Employee? employee = employees.FirstOrDefault(e => e.Id == id);
    if(employee is not null)
    {
        employees.Remove(employee);
        return Results.Json(employee);
    }
    else
        return Results.NotFound();
});

app.MapPost("/form", async (HttpContext context) =>
{
    var form = context.Request.Form;
    string id = form["id"];
    string name = form["name"];
    string age = form["age"];

    await context.Response.WriteAsync($"Id: {id}, Name: {name}, Age: {age}");
});

app.MapPost("/upload", async (HttpContext context) =>
{
    var uploadDir = $"{Directory.GetCurrentDirectory()}/upload";
    Directory.CreateDirectory(uploadDir);

    string fileName = Guid.NewGuid().ToString();

    using(FileStream fileStream = new($"{uploadDir}/{fileName}.jpg", FileMode.Create))
    {
        await context.Request.Body.CopyToAsync(fileStream);
    }

    await context.Response.WriteAsync("File upload");
});

app.MapPost("/bindata", async (HttpContext context) =>
{
    using(StreamReader reader = new(context.Request.Body))
    {
        string message = await reader.ReadToEndAsync();
        Console.WriteLine(message);
        await context.Response.WriteAsync(message);
    }
});

app.MapPost("/bigform", async (HttpContext context) =>
{
    IFormFileCollection files = context.Request.Form.Files;
    var uploadDir = $"{Directory.GetCurrentDirectory()}/upload";
    Directory.CreateDirectory(uploadDir);

    foreach (var file in files)
    {
        string uploadPath = $"{uploadDir}/{file.FileName}";
        using (FileStream stream = new(uploadPath, FileMode.Create))
        {
            await file.CopyToAsync(stream);
        }
    }
    await context.Response.WriteAsync("Files is upload");

});


app.Run();
