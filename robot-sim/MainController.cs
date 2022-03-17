using Microsoft.AspNetCore.Mvc;

namespace robot_sim.Controllers
{
    [ApiController]
    [Route("")]
    public class MainController : ControllerBase
    {
        private readonly ILogger<MainController> _logger;

        public MainController(ILogger<MainController> logger)
        {
            _logger = logger;
        }

        [HttpGet]
        public ActionResult Get() { return PhysicalFile(Path.Combine(Directory.GetCurrentDirectory(), "index.html"), "text/html"); }



        [HttpGet("/raw")]
        public IActionResult GetRaw()
        {
            throw new NotImplementedException();
        }

        [HttpGet("/complete")]
        public IActionResult GetComplete()
        {
            var data = new GetTemplate()
            {
                ticks = Simulator.ticks,
                tickSpeed = Simulator.tickSpeed,
                robots = Simulator.robots,
                pickers = Simulator.pickerLocations,
                resetFlag = Simulator.resetFlag,
                robotCap = Simulator.robotMax,
                faultChance = Simulator.faultChance,
            };

            return Ok(data);
        }

        [HttpPut("/edit")]
        public ActionResult Edit(PutTemplate template)
        {
            System.Diagnostics.Debug.WriteLine("Received: {" + template.field + ", " + template.value + "}");

            if (template.field == "reset") Simulator.resetFlag = true;
            if (template.field == "robot") Simulator.addRobot();

            if (template.field == "speed")
                if (int.TryParse(template.value, out int value))
                {
                    if (value < 100) Simulator.tickSpeed = 100;
                    if (value > 60000) Simulator.tickSpeed = 60000;
                    if (value >= 100 && value <= 60000) Simulator.tickSpeed = value;
                }

            if (template.field == "max")
                if (int.TryParse(template.value, out int value))
                {
                    if (value < 1) Simulator.robotMax = 1;
                    if (value > 1000) Simulator.robotMax = 1000;
                    if (value >= 1 && value <= 1000) Simulator.robotMax = value;
                }

            if (template.field == "fault")
                if (double.TryParse(template.value.Replace(".", ","), out double value))
                {
                    if (value < 0.0) Simulator.faultChance = 0.0;
                    if (value > 100.0) Simulator.faultChance = 100.0;
                    if (value >= 0.0 && value <= 100.0) Simulator.faultChance = value;
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
    }

    public class PutTemplate
    {
        public string field { get; set; }
        public string value { get; set; }
    }
}