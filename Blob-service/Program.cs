using Blob_service.Data;
using Blob_service.Hubs;
using Blob_service.Services;
using Blob_service.Services.Interfaces;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddDbContext<DeckDbContext>(option =>
{
    option.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection"));
});
builder.Services.AddSignalR();
builder.Services.AddControllers();
builder.Services.AddTransient<IDeckService, DeckService>();
builder.Services.AddTransient<IScoresService, ScoresService>();
builder.Services.AddTransient<IBidsService, BidsService>();
builder.Services.AddTransient<IGameDetailsService, GameDetailsService>();
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

app.UseCors(options => options.AllowAnyMethod().AllowAnyHeader().AllowCredentials().WithOrigins("http://localhost:3000").AllowCredentials());

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.MapHub<GameHub>("/GameHub");
app.MapHub<LobbyHub>("/LobbyHub");

app.Run();
