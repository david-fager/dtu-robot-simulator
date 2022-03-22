var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();

var app = builder.Build();

app.UseAuthorization();

app.MapControllers();

new Thread(robot_sim.SimulationManager.StartTicking).Start();

app.Run("http://localhost:8000");
