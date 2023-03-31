using Microsoft.AspNetCore.Mvc;

namespace MediaCore.Controllers
{
    public class BaseController : ControllerBase
    {


        //forbidden response
        protected IActionResult Forbidden()
        {
            return StatusCode(403);
        }

        // InternalServerError response, gets an exception as parameter
        protected ActionResult InternalServerError(Exception ex)
        {
            return StatusCode(StatusCodes.Status500InternalServerError, ex);
        }



    }
}
