using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SchoolProject.Api.Bases;
using SchoolProject.Core.Features.Instructors.Commands.Models;
using SchoolProject.Core.Features.Instructors.Queries.Models;
using SchoolProject.Data.AppMetaData;

namespace SchoolProject.Api.Controllers
{
    [ApiController]
    [Authorize(Roles = "Admin")]
    public class InstructorController : AppControllerBase
    {
        [HttpGet(Router.InstructorRouting.GetSalarySummation)]
        public async Task<IActionResult> GetSalarySummation()
        {
            var response = await Mediator.Send(new GetSummationSalaryOfInstructorQuery());
            return Ok(response);
        }

        [HttpPost(Router.InstructorRouting.AddInstructor)]
        public async Task<IActionResult> AddInstructor([FromForm] AddInstructorCommand command)
        {
            var response = await Mediator.Send(command);
            return Ok(response);
        }
    }
}
