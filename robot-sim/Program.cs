var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();

var app = builder.Build();

app.UseAuthorization();

app.MapControllers();

robot_sim.Simulator.StartTicking();

app.Run("http://localhost:8000");
