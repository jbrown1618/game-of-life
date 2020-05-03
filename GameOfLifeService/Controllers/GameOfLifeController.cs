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
        private IIterator _iterator;

        public GameOfLifeController(ILogger<GameOfLifeController> logger, IIterator iterator)
        {
            _logger = logger;
            _iterator = iterator;
        }

        [HttpGet]
        public IActionResult Get(int width, int height)
        {
            if (width <= 0 || height <= 0)
            {
                return StatusCode(500, "Width and Height must be > 0");
            }
            if (height > ushort.MaxValue || width > ushort.MaxValue)
            {
                return StatusCode(500, $"Width and Height must be < {ushort.MaxValue}");
            }
            GameOfLifeStateDTO dto = GameOfLifeStateDTO.ToDTO(new GameOfLifeState((ushort)width, (ushort)height));
            return StatusCode(200, dto);
        }

        [HttpPost("iterate")]
        public IActionResult Iterate(GameOfLifeStateDTO dto)
        {
            GameOfLifeState previous = GameOfLifeStateDTO.FromDTO(dto);
            try
            {
                GameOfLifeState next = _iterator.Iterate(previous);
                GameOfLifeStateDTO nextDto = GameOfLifeStateDTO.ToDTO(next);
                return StatusCode(200, nextDto);
            }
            catch (System.ArgumentException e)
            {
                return StatusCode(500, e.Message);
            }
        }
    }
}
