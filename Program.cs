var builder = WebApplication.CreateBuilder(args);

// ---------------------------------------------------
// -- Build SQL SERVER Connection String
// ---------------------------------------------------
var conStrBuilder= new System.Data.SqlClient.SqlConnectionStringBuilder();
conStrBuilder.DataSource=builder.Configuration["dbconfig:servername"];
conStrBuilder.InitialCatalog= builder.Configuration["dbconfig:databasename"];
conStrBuilder.UserID=builder.Configuration["dbconfig:username"];
conStrBuilder.Password= builder.Configuration["dbconfig:password"];

LoadData.SetConnectionString(conStrBuilder.ConnectionString);

// ---------------------------------------------------

var  MyAllowSpecificOrigins = "_myAllowSpecificOrigins";
builder.Services.AddCors(options=>{
    options.AddPolicy(name: MyAllowSpecificOrigins, builder=>{
        builder.WithOrigins("https://localhost","http://localhost:3000")
        .AllowAnyHeader().AllowAnyMethod();
    });

});

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseStaticFiles(); // Added
app.UseRouting(); //Added
app.UseCors(MyAllowSpecificOrigins); //Added
app.UseAuthorization();

app.MapControllers();

app.Run();
