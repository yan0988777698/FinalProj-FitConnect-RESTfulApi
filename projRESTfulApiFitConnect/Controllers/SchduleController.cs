using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using projRESTfulApiFitConnect.Models;

namespace projRESTfulApiFitConnect.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class SchduleController : ControllerBase
    {
        private readonly GymContext _context;
        private readonly IWebHostEnvironment _env;

        public SchduleController(GymContext context, IWebHostEnvironment env)
        {
            _context = context;
            _env = env;
        }


    }
}
