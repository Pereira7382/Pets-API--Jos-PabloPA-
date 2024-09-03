using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using api.Data;
using api.Dtos.User;
using api.Mappers;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Mvc;


namespace api.Controllers
{
    [Route("api/user")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly ApplicationDBContext _context;
        
        public UserController(ApplicationDBContext context)
        {
            _context = context;
        }

        [HttpGet]

        public async Task<IActionResult> GetAll(){
            var users = await _context.Users.Include(user => user.pets).ToListAsync();
            var usersDto = users.Select(users => users.ToDto());
            return Ok(usersDto);
        }

        [HttpGet("{id}")]

         public async Task<IActionResult> getById([FromRoute] int id){
            var user =  await _context.Users.Include(user => user.pets).FirstOrDefaultAsync(u => u.Id == id);

            if(user== null){
                return NotFound();
            }
            return Ok(user.ToDto());
        }

                [HttpPost]
        public async Task<IActionResult> Create([FromBody] CreateUserRequestDto userDto){
            var userModel = userDto.ToUserFromCreateDto();
            await _context.Users.AddAsync(userModel);
            await _context.SaveChangesAsync();
            return CreatedAtAction(nameof(getById), new { id = userModel.Id}, userModel.ToDto());
        }

        [HttpPut]
        [Route("{id}")]
        public async Task<IActionResult> Update([FromRoute] int id, [FromBody] UpdateUserRequestDto userDto){
            var userModel = await _context.Users.FirstOrDefaultAsync(user => user.Id == id);
            if (userModel == null){
                return NotFound();
            }
            userModel.Age = userDto.Age;
            userModel.FirstName = userDto.FirstName;
            userModel.LastName = userDto.LastName;

             await _context.SaveChangesAsync();

            return Ok(userModel.ToDto());
        }

        [HttpDelete]
        [Route("{id}")]
        public async Task<IActionResult> Delete([FromRoute] int id){
            var userModel = await _context.Users.FirstOrDefaultAsync(user => user.Id == id);
            if (userModel == null){
                return NotFound();
            }
            _context.Users.Remove(userModel);

              await _context.SaveChangesAsync();

            return NoContent();
        }


        [HttpPost("create-with-pets")]
        public async Task<IActionResult> CreateUserAndPets([FromBody] CreateUserRequestDto createUserRequestDto)
        {
          
            var userModel = createUserRequestDto.ToUserFromCreateDto();

          
            foreach (var petDto in createUserRequestDto.Pets)
            {
                var petModel = petDto.ToPetFromCreateDto();
                petModel.UserId = userModel.Id;  
                userModel.pets.Add(petModel);
            }

            await _context.Users.AddAsync(userModel);
            await _context.Pets.AddRangeAsync(userModel.pets); 
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(getById), new { id = userModel.Id }, userModel.ToDto());
        }


    }
}