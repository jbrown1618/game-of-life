using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using GameOfLifeService.Core;
using GameOfLifeService.DTO;

namespace GameOfLifeService.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class GameOfLifeController : ControllerBase
    {

        private readonly ILogger<GameOfLifeController> _logger;

        public GameOfLifeController(ILogger<GameOfLifeController> logger)
        {
            _logger = logger;
        }

        [HttpGet]
        public IActionResult Get(int width, int height)
        {
            if (width <= 0 || height <= 0)
            {
                return StatusCode(500, "Width and Height must be positive");
            }
            GameOfLifeStateDTO dto = GameOfLifeStateDTO.ToDTO(new GameOfLifeState(10, 10));
            return StatusCode(200, dto);
        }

        [HttpPost("iterate")]
        public IActionResult Iterate(GameOfLifeStateDTO dto)
        {
            GameOfLifeState previous = GameOfLifeStateDTO.FromDTO(dto);
            ISet<string> errors = Validator.Validate(previous);
            if (errors.Count > 0)
            {
                return StatusCode(500, errors);
            }
            GameOfLifeState next = Iterator.Iterate(previous);
            GameOfLifeStateDTO nextDto = GameOfLifeStateDTO.ToDTO(next);

            return StatusCode(200, nextDto);
        }
    }
}
