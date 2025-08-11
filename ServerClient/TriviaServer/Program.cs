var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var mongoConn = builder.Configuration.GetValue<string>("Mongo:ConnectionString");

if (string.IsNullOrWhiteSpace(mongoConn))
    throw new Exception("Mongo:ConnectionString is missing from configuration");

// register as singleton
builder.Services.AddSingleton(new MongoScoreboard(mongoConn));

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
