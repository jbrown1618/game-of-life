using System.Collections.Generic;
using GameOfLifeService.Core;

namespace GameOfLifeService.DTO
{
    public class GameOfLifeStateDTO
    {
        public ushort Width { get; set; }
        public ushort Height { get; set; }
        public CoordinateDTO[] LiveCells { get; set; }

        public GameOfLifeStateDTO() { }

        public static GameOfLifeStateDTO ToDTO(GameOfLifeState state)
        {

            GameOfLifeStateDTO dto = new GameOfLifeStateDTO();
            dto.Width = state.Width;
            dto.Height = state.Height;

            dto.LiveCells = new CoordinateDTO[state.LiveCells.Count];
            int i = 0;
            foreach ((ushort Row, ushort Col) coords in state.LiveCells)
            {
                dto.LiveCells[i] = new CoordinateDTO();
                dto.LiveCells[i].Row = coords.Row;
                dto.LiveCells[i].Col = coords.Col;
            }

            return dto;
        }

        public static GameOfLifeState FromDTO(GameOfLifeStateDTO dto)
        {
            ISet<(ushort Row, ushort Col)> liveCells = new HashSet<(ushort Row, ushort Col)>();
            if (dto.LiveCells == null)
            {
                return new GameOfLifeState(dto.Width, dto.Height);
            }
            foreach (CoordinateDTO coords in dto.LiveCells)
            {
                liveCells.Add((coords.Row, coords.Col));
            }
            return new GameOfLifeState(dto.Width, dto.Height, liveCells);
        }
    }

    public class CoordinateDTO
    {
        public ushort Row { get; set; }
        public ushort Col { get; set; }

        public CoordinateDTO() { }
    }
}