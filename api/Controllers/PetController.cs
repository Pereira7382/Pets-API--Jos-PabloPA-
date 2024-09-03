using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using api.Data;
using api.Dtos.User;
using api.Dtos.Pet;
using api.Mappers;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace api.Controllers
{
   
    [Route("api/pet")]
    [ApiController]
    public class PetController : ControllerBase
    {

        private readonly ApplicationDBContext _context;
        public PetController(ApplicationDBContext context)
        {
            _context = context;
        }
        [HttpGet]
        public async Task<IActionResult> GetAll(){
            var pets = await _context.Pets.ToListAsync();
            var petsDto = pets.Select(pets => pets.ToDto());
            return Ok(petsDto);
        }

        [HttpGet("{id}")]

        public async Task<IActionResult>getById([FromRoute] int id){
            var pets = await _context.Pets.FirstOrDefaultAsync(u => u.Id == id);

             if(pets== null){
                return NotFound();
            }
            return Ok(pets.ToDto());
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] CreatePetRequestDto petDto){
            var petModel = petDto.ToPetFromCreateDto();
            await _context.Pets.AddAsync(petModel);
            await _context.SaveChangesAsync();
            return CreatedAtAction(nameof(getById), new { id = petModel.Id}, petModel.ToDto());
        }


        [HttpPut]
        [Route("{id}")]
        public async Task<IActionResult> Update([FromRoute] int id, [FromBody] UpdatePetRequestDto petDto){
            var petModel = await _context.Pets.FirstOrDefaultAsync(pet => pet.Id == id);
            if (petModel == null){
                return NotFound();
            }
            petModel.Name = petDto.Name;
            petModel.Animal = petDto.Animal;
            
             await _context.SaveChangesAsync();

            return Ok(petModel.ToDto());
        }

        [HttpDelete]
        [Route("{id}")]
        public async Task<IActionResult> Delete([FromRoute] int id){
            var petModel = await _context.Pets.FirstOrDefaultAsync(pet => pet.Id == id);
            if (petModel == null){
                return NotFound();
            }
            _context.Pets.Remove(petModel);

              await _context.SaveChangesAsync();

            return NoContent();
        }

        [HttpPost("{userId}/assign-pet/{petId}")]
        public async Task<IActionResult> AssignPetToUser([FromRoute] int userId, [FromRoute] int petId)
        {
            var petModel = await _context.Pets.FirstOrDefaultAsync(pet => pet.Id == petId);
            var userModel = await _context.Users.FirstOrDefaultAsync(user => user.Id == userId);

            if (petModel == null)
            {
                return NotFound("Mascota no encontrada.");
            }               

            if (userModel == null)
            {
                return NotFound("Usuario no encontrado.");
            }

            petModel.UserId = userId;
            await _context.SaveChangesAsync();

            return Ok(petModel.ToDto());
        }

    }
}