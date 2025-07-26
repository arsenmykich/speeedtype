using Microsoft.AspNetCore.Mvc;
using speedtype.DAL.IRepositories;
using speedtype.DAL.Entities;
using speedtype.DAL.IConfiguration;
using Swashbuckle.AspNetCore.Annotations;
using Microsoft.AspNetCore.Mvc.ModelBinding;

namespace speedtype.API.Controllers;

[ApiController]
[Route("api/[controller]")]
[Produces("application/json")]
public class UsersController : ControllerBase
{
    private readonly IUnitOfWork _unitOfWork;

    public UsersController(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    [HttpGet]
    [SwaggerOperation(Summary = "Get all users")]
    [SwaggerResponse(200, "Success", typeof(IEnumerable<User>))]
    [SwaggerResponse(400, "Bad Request", typeof(ModelStateDictionary))]
    [SwaggerResponse(500, "Internal Server Error", typeof(string))]
    public async Task<IActionResult> GetUsers()
    {
        var users = await _unitOfWork.Users.GetAllAsync();
        return Ok(users);
    }

    [HttpGet("{id}")]
    [SwaggerOperation(Summary = "Get a user by id")]
    [SwaggerResponse(200, "Success", typeof(User))]
    [SwaggerResponse(400, "Bad Request", typeof(ModelStateDictionary))]
    [SwaggerResponse(404, "Not Found", typeof(string))]
    [SwaggerResponse(500, "Internal Server Error", typeof(string))]
    public async Task<IActionResult> GetUser(int id)
    {
        var user = await _unitOfWork.Users.GetByIdAsync(id);
        if (user == null)
        {
            return NotFound();
        }
        return Ok(user);
    }

    [HttpPost]
    [SwaggerOperation(Summary = "Create a new user")]
    [SwaggerResponse(201, "Created", typeof(User))]
    [SwaggerResponse(400, "Bad Request", typeof(ModelStateDictionary))]
    [SwaggerResponse(500, "Internal Server Error", typeof(string))]
    public async Task<IActionResult> CreateUser(User user)
    {
        if(ModelState.IsValid){
            //user.Id = Guid.NewGuid();
            await _unitOfWork.Users.AddAsync(user);
            await _unitOfWork.CompleteAsync();
            return CreatedAtAction("GetUser", new { id = user.Id }, user);
            
        }
        return new JsonResult(ModelState) { StatusCode = 500 };
    }

    [HttpPut("{id}")]
    [SwaggerOperation(Summary = "Update a user by id")]
    [SwaggerResponse(200, "Success", typeof(User))]
    [SwaggerResponse(400, "Bad Request", typeof(ModelStateDictionary))]
    [SwaggerResponse(404, "Not Found", typeof(string))]
    [SwaggerResponse(500, "Internal Server Error", typeof(string))]
    public async Task<IActionResult> UpdateUser(int id, User user)
    {
        await _unitOfWork.Users.UpdateAsync(id, user);
        await _unitOfWork.CompleteAsync();
        return Ok(user);
    }

    [HttpDelete("{id}")]
    [SwaggerOperation(Summary = "Delete a user by id")]
    [SwaggerResponse(200, "Success", typeof(string))]
    [SwaggerResponse(400, "Bad Request", typeof(ModelStateDictionary))]
    [SwaggerResponse(404, "Not Found", typeof(string))]
    [SwaggerResponse(500, "Internal Server Error", typeof(string))]
    public async Task<IActionResult> DeleteUser(int id)
    {
        await _unitOfWork.Users.DeleteAsync(id);
        await _unitOfWork.CompleteAsync();
        return Ok();
    }
}
