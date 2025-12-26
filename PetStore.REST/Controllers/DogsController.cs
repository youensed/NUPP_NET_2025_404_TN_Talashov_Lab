using Microsoft.AspNetCore.Mvc;
using PetStore.REST.Models;
using PetStore.Common.Services;
using PetStore.Infrastructure.Models;

namespace PetStore.REST.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class DogsController : ControllerBase
    {
        private readonly ICrudServiceAsync<DogModel> _dogService;
        private readonly ILogger<DogsController> _logger;

        public DogsController(ICrudServiceAsync<DogModel> dogService, ILogger<DogsController> logger)
        {
            _dogService = dogService;
            _logger = logger;
        }

        // GET: api/dogs
        [HttpGet]
        public async Task<ActionResult<IEnumerable<DogDto>>> GetAllDogs([FromQuery] int? page, [FromQuery] int? amount)
        {
            try
            {
                IEnumerable<DogModel> dogs;

                if (page.HasValue && amount.HasValue)
                {
                    dogs = await _dogService.ReadAllAsync(page.Value, amount.Value);
                }
                else
                {
                    dogs = await _dogService.ReadAllAsync();
                }

                var dogDtos = dogs.Select(d => new DogDto
                {
                    Id = d.Id,
                    Name = d.Name,
                    Age = d.Age,
                    Breed = d.Breed,
                    IsTrained = d.IsTrained
                });

                return Ok(dogDtos);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving dogs");
                return StatusCode(500, "Internal server error");
            }
        }

        // GET: api/dogs/{id}
        [HttpGet("{id}")]
        public async Task<ActionResult<DogDto>> GetDogById(Guid id)
        {
            try
            {
                var dog = await _dogService.ReadAsync(id);

                if (dog == null)
                {
                    return NotFound($"Dog with ID {id} not found");
                }

                var dogDto = new DogDto
                {
                    Id = dog.Id,
                    Name = dog.Name,
                    Age = dog.Age,
                    Breed = dog.Breed,
                    IsTrained = dog.IsTrained
                };

                return Ok(dogDto);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving dog with ID {DogId}", id);
                return StatusCode(500, "Internal server error");
            }
        }

        // POST: api/dogs
        [HttpPost]
        public async Task<ActionResult<DogDto>> CreateDog([FromBody] DogCreateDto dogCreateDto)
        {
            try
            {
                var dogModel = new DogModel(
                    dogCreateDto.Name,
                    dogCreateDto.Age,
                    dogCreateDto.Breed,
                    dogCreateDto.IsTrained
                );

                var created = await _dogService.CreateAsync(dogModel);

                if (!created)
                {
                    return BadRequest("Failed to create dog");
                }

                var dogDto = new DogDto
                {
                    Id = dogModel.Id,
                    Name = dogModel.Name,
                    Age = dogModel.Age,
                    Breed = dogModel.Breed,
                    IsTrained = dogModel.IsTrained
                };

                return CreatedAtAction(nameof(GetDogById), new { id = dogDto.Id }, dogDto);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating dog");
                return StatusCode(500, "Internal server error");
            }
        }

        // PUT: api/dogs/{id}
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateDog(Guid id, [FromBody] DogUpdateDto dogUpdateDto)
        {
            try
            {
                if (id != dogUpdateDto.Id)
                {
                    return BadRequest("ID mismatch");
                }

                var existingDog = await _dogService.ReadAsync(id);

                if (existingDog == null)
                {
                    return NotFound($"Dog with ID {id} not found");
                }

                existingDog.Name = dogUpdateDto.Name;
                existingDog.Age = dogUpdateDto.Age;
                existingDog.Breed = dogUpdateDto.Breed;
                existingDog.IsTrained = dogUpdateDto.IsTrained;

                var updated = await _dogService.UpdateAsync(existingDog);

                if (!updated)
                {
                    return BadRequest("Failed to update dog");
                }

                return NoContent();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating dog with ID {DogId}", id);
                return StatusCode(500, "Internal server error");
            }
        }

        // DELETE: api/dogs/{id}
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteDog(Guid id)
        {
            try
            {
                var dog = await _dogService.ReadAsync(id);

                if (dog == null)
                {
                    return NotFound($"Dog with ID {id} not found");
                }

                var deleted = await _dogService.RemoveAsync(dog);

                if (!deleted)
                {
                    return BadRequest("Failed to delete dog");
                }

                return NoContent();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting dog with ID {DogId}", id);
                return StatusCode(500, "Internal server error");
            }
        }
    }
}

