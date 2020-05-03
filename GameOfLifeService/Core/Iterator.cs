using System.Collections.Generic;

namespace GameOfLifeService.Core
{
    public static class Iterator
    {
        public static GameOfLifeState Iterate(GameOfLifeState previous)
        {
            if (Validator.Validate(previous).Count > 0)
            {
                throw new System.ArgumentException("The starting state must be valid");
            }

            ISet<(ushort Row, ushort Col)> livingCells = previous.LiveCells;
            ISet<(ushort Row, ushort Col)> survivingCells = new HashSet<(ushort Row, ushort Col)>();

            // This is nowhere near the fastest algorithm, but whatever.
            for (ushort i = 0; i < previous.Height; i++)
            {
                for (ushort j = 0; j < previous.Width; j++)
                {
                    bool hasTopLeft = livingCells.Contains(NormalizeCoordinates((i - 1, j - 1), previous));
                    bool hasTop = livingCells.Contains(NormalizeCoordinates((i - 1, j), previous));
                    bool hasTopRight = livingCells.Contains(NormalizeCoordinates((i - 1, j + 1), previous));
                    bool hasLeft = livingCells.Contains(NormalizeCoordinates((i, j - 1), previous));
                    bool living = livingCells.Contains((i, j));
                    bool hasRight = livingCells.Contains(NormalizeCoordinates((i, j + 1), previous));
                    bool hasBottomLeft = livingCells.Contains(NormalizeCoordinates((i + 1, j - 1), previous));
                    bool hasBottom = livingCells.Contains(NormalizeCoordinates((i + 1, j), previous));
                    bool hasBottomRight = livingCells.Contains(NormalizeCoordinates((i + 1, j + 1), previous));
                    int numNeighbors = (hasTopLeft ? 1 : 0)
                            + (hasTop ? 1 : 0)
                            + (hasTopRight ? 1 : 0)
                            + (hasLeft ? 1 : 0)
                            + (hasRight ? 1 : 0)
                            + (hasBottomLeft ? 1 : 0)
                            + (hasBottom ? 1 : 0)
                            + (hasBottomRight ? 1 : 0);

                    if (numNeighbors == 3 || (living && numNeighbors == 2))
                    {
                        survivingCells.Add((i, j));
                    }
                }
            }

            return new GameOfLifeState(previous.Width, previous.Height, survivingCells);
        }

        private static (ushort Row, ushort Col) NormalizeCoordinates((int Row, int Col) coords, GameOfLifeState state)
        {
            return (
                (ushort)((coords.Row + state.Height) % state.Height),
                (ushort)((coords.Col + state.Width) % state.Width)
            );
        }
    }
}