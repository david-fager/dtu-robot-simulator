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
        public ActionResult<string> GetRaw()
        {
            throw new NotImplementedException();
        }

        [HttpGet("/bundled")]
        public ActionResult<string> GetBundled()
        {
            return "bundled";
        }

        [HttpPut("/edit")]
        public void Edit()
        {
            
        }
    }
}