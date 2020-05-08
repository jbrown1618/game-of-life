using System.Collections.Generic;
using GameOfLifeService.Core;

/// <summary>
/// A serializable format containing all the information in a <see cref="GameOfLifeState"/>.
/// </summary>
namespace GameOfLifeService.DTO
{
    public class GameOfLifeStateDTO
    {
        public ushort Width { get; set; }
        public ushort Height { get; set; }
        public CoordinateDTO[] LiveCells { get; set; }

        public GameOfLifeStateDTO() { }

        /// <summary>
        /// ToDTO constructs a serializable <see cref="GameOfLifeStateDTO"/> from
        /// a <see cref="GameOfLifeState"/>
        /// </summary>
        /// <param name="state">The GameOfLifeState to convert</param>
        /// <returns>A new DTO</returns>
        public static GameOfLifeStateDTO ToDTO(GameOfLifeState state)
        {
            if (state == null) return null;

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
                i++;
            }

            return dto;
        }

        /// <summary>
        /// FromDTO constructs a <see cref="GameOfLifeState"/> from
        /// a serializable <see cref="GameOfLifeStateDTO"/>
        /// </summary>
        /// <param name="dto">The GameOfLifeStateDTO to convert</param>
        /// <returns>A new state</returns>
        public static GameOfLifeState FromDTO(GameOfLifeStateDTO dto)
        {
            if (dto == null) return null;

            var liveCells = new HashSet<(ushort Row, ushort Col)>();
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