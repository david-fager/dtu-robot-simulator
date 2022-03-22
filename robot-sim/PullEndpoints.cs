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

        [HttpGet]
        public ActionResult Get() { return PhysicalFile(Path.Combine(Directory.GetCurrentDirectory(), "index.html"), "text/html"); }



        [HttpGet("/raw")]
        public IActionResult GetRaw()
        {
            var list = new List<RawTemplate>();
            foreach (var robot in SimulationManager.robots) list.Add(new RawTemplate
            {
                time = SimulationManager.ticks,
                robotID = robot.robotID,
                currentPos = robot.position,
                //expectedPos = robot.expectedPath.Count > 0 ? robot.expectedPath.Peek() : null,
                temperature = robot.temperature,
            });
            return Ok(list);
        }

        [HttpGet("/complete")]
        public IActionResult GetComplete()
        {
            return Ok(new GetTemplate()
            {
                ticks = SimulationManager.ticks,
                tickSpeed = SimulationManager.tickSpeed,
                robots = SimulationManager.robots,
                pickers = SimulationManager.pickerLocations,
                resetFlag = SimulationManager.resetFlag,
                robotCap = SimulationManager.robotMax,
                faultChance = SimulationManager.faultChance,
                personalityMultiplier = SimulationManager.personalityMultiplier,
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

            if (template.field == "fault")
                if (double.TryParse(template.value.Replace(".", ","), out double value))
                {
                    if (value < 0.0) SimulationManager.faultChance = 0.0;
                    if (value > 100.0) SimulationManager.faultChance = 100.0;
                    if (value >= 0.0 && value <= 100.0) SimulationManager.faultChance = value;
                }

            if (template.field == "mult")
                if (int.TryParse(template.value, out int value))
                {
                    if (value < 1) SimulationManager.personalityMultiplier = 1;
                    if (value > 100) SimulationManager.personalityMultiplier = 100;
                    if (value >= 1 && value <= 100) SimulationManager.personalityMultiplier = value;
                }

            return Ok();
        }
    }

    public class GetTemplate
    {
        public int ticks { get; set; }
        public int tickSpeed { get; set; }
        public List<Robot> robots { get; set; }
        public List<Position> pickers { get; set; }
        public bool resetFlag { get; set; }
        public int robotCap { get; set; }
        public double faultChance { get; set; }
        public int personalityMultiplier { get; set; }
    }

    public class PutTemplate
    {
        public string field { get; set; }
        public string value { get; set; }
    }

    public class RawTemplate
    {
        public int time { get; set; }
        public int robotID { get; set; }
        public Position currentPos { get; set; }
        public Position expectedPos { get; set; }
        public double temperature { get; set; }
    }
}