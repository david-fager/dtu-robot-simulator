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
                tick = SimulationManager.ticks,
                id = robot.robotID,
                pos = robot.currentPosition,
                expected = robot.expectedPosition,
                temp = (robot.motorTemperature * 9 / 5 + 32), // convert to fahrenheit
                resistance = robot.batteryResistance,
                personality = robot.personality.ToString(),
                repairReason = robot.lastRepairReason,
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
                robotCap = SimulationManager.robotMax,
                divChance = SimulationManager.briefFaultChance,
                perChance = SimulationManager.permamentFaultChance,
                motorRate = SimulationManager.motorChangeRate,
                resistanceRate = SimulationManager.batteryChangeRate,
                pickers = SimulationManager.pickerLocations,
                resetFlag = SimulationManager.resetFlag,
            });
        }

        [HttpPut("/edit")]
        public ActionResult Edit(PutTemplate template)
        {
            System.Diagnostics.Debug.WriteLine("Received: {" + template.field + ", " + template.value + "}");

            if (template.field == "res") SimulationManager.resetFlag = true;
            if (template.field == "add") SimulationManager.requestedRobots++;

            if (template.field == "tic")
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

            if (template.field == "div")
                if (double.TryParse(template.value.Replace(".", ","), out double value))
                {
                    if (value < 0.0) SimulationManager.briefFaultChance = 0.0;
                    if (value > 100.0) SimulationManager.briefFaultChance = 100.0;
                    if (value >= 0.0 && value <= 100.0) SimulationManager.briefFaultChance = value;
                }

            if (template.field == "per")
                if (double.TryParse(template.value.Replace(".", ","), out double value))
                {
                    if (value < 0.0) SimulationManager.permamentFaultChance = 0.0;
                    if (value > 100.0) SimulationManager.permamentFaultChance = 100.0;
                    if (value >= 0.0 && value <= 100.0) SimulationManager.permamentFaultChance = value;
                }

            if (template.field == "mot")
                if (double.TryParse(template.value.Replace(".", ","), out double value))
                {
                    if (value < 0.05) SimulationManager.motorChangeRate = 0.05;
                    if (value > 25.0) SimulationManager.motorChangeRate = 25.0;
                    if (value >= 0.05 && value <= 25.0) SimulationManager.motorChangeRate = value;
                }

            if (template.field == "bat")
                if (double.TryParse(template.value.Replace(".", ","), out double value))
                {
                    if (value < 0.0001) SimulationManager.batteryChangeRate = 0.0001;
                    if (value > 0.25) SimulationManager.batteryChangeRate = 0.25;
                    if (value >= 0.0001 && value <= 0.25) SimulationManager.batteryChangeRate = value;
                }

            if (template.field == "set")
                if (int.TryParse(template.value, out int value))
                {
                    if (value < 0) SimulationManager.setPersonality = 0;
                    if (value > 11) SimulationManager.setPersonality = 11;
                    if (value >= 0 && value <= 11) SimulationManager.setPersonality = value;
                }

            return Ok();
        }

        [HttpGet("/formats")]
        public IActionResult Formats()
        {
            return Ok(new
            {
                time = 1,
                robotID = 1,
                currentPosition = new Position(0, 1),
                expectedPosition = new Position(1, 0),
                motorTemperature = 0.1,
                batteryResistance = 0.1,
                lastRepairReason = "",
            });
        }
    }

    public class PutTemplate
    {
        public string field { get; set; }
        public string value { get; set; }
    }
}