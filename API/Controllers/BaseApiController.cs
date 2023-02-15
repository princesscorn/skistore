using Microsoft.AspNetCore.Mvc;

namespace API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]     // api is optional, but it's conventional to do it
    public class BaseApiController : ControllerBase
    {
        
    }
}