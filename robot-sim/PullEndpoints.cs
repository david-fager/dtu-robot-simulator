using Microsoft.AspNetCore.Mvc;

namespace robot_sim.Controllers
{
    [ApiController]
    [Route("")]
    public class PullEndpoints : ControllerBase
    {
        private readonly ILogger<PullEndpoints> _logger;

        public PullEndpoints(ILogger<PullEndpoints> logger)
        {
            _logger = logger;
        }

        // get/show the index.html file
        [HttpGet]
        public ActionResult Get() { return PhysicalFile(Path.Combine(Directory.GetCurrentDirectory(), "index.html"), "text/html"); }



        [HttpGet("/raw")]
        public IActionResult GetRaw()
        {
            return Ok(SimulationManager.robots.Select(robot => new {
                time = SimulationManager.ticks,
                robotID = robot.robotID,
                currentPosition = robot.currentPosition,
                expectedPosition = robot.expectedPosition,
                motorTemperature = robot.motorTemperature,
                batteryResistance = robot.batteryResistance,
                lastRepairReason = robot.lastRepairReason,
            }).ToList());
        }

        [HttpGet("/web")]
        public IActionResult GetComplete()
        {
            return Ok(new
            {
                ticks = SimulationManager.ticks,
                tickSpeed = SimulationManager.tickSpeed,
                robots = SimulationManager.robots,
                pickers = SimulationManager.pickerLocations,
                resetFlag = SimulationManager.resetFlag,
                robotCap = SimulationManager.robotMax,
            });
        }

        [HttpPut("/edit")]
        public ActionResult Edit(PutTemplate template)
        {
            System.Diagnostics.Debug.WriteLine("Received: {" + template.field + ", " + template.value + "}");

            if (template.field == "reset") SimulationManager.resetFlag = true;
            if (template.field == "robot") SimulationManager.requestedRobots++;

            if (template.field == "speed")
                if (int.TryParse(template.value, out int value))
                {
                    if (value < 100) SimulationManager.tickSpeed = 100;
                    if (value > 60000) SimulationManager.tickSpeed = 60000;
                    if (value >= 100 && value <= 60000) SimulationManager.tickSpeed = value;
                }

            if (template.field == "max")
                if (int.TryParse(template.value, out int value))
                {
                    if (value < 1) SimulationManager.robotMax = 1;
                    if (value > 1000) SimulationManager.robotMax = 1000;
                    if (value >= 1 && value <= 1000) SimulationManager.robotMax = value;
                }

            return Ok();
        }
    }

    public class PutTemplate
    {
        public string field { get; set; }
        public string value { get; set; }
    }
}