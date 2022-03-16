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
            var data = new DataTemplate()
            {
                ticks = Simulator.ticks,
                robots = Simulator.robots,
                pickers = Simulator.pickerLocations,
                resetFlag = Simulator.resetFlag,
                robotCap = Simulator.robotCap,
                faultChance = Simulator.faultChance,
            };

            return Ok(data);
        }

        [HttpPut("/edit")]
        public void Edit()
        {
            
        }
    }

    public class DataTemplate
    {
        public int ticks { get; set; }
        public List<Robot> robots { get; set; }
        public List<Position> pickers { get; set; }
        public bool resetFlag { get; set; }
        public int robotCap { get; set; }
        public double faultChance { get; set; }
    }

}