using System;
using System.Collections.Generic;

namespace GameOfLifeService.Core
{
    public static class Iterator
    {
        public static GameOfLifeState Iterate(GameOfLifeState state)
        {
            ISet<string> errors = Validator.Validate(state);
            if (errors.Count > 0)
            {
                throw new System.ArgumentException("The starting state must be valid:\n" + String.Join("\n", errors));
            }

            ISet<(ushort Row, ushort Col)> survivingCells = new HashSet<(ushort Row, ushort Col)>();

            // This is nowhere near the fastest algorithm, but whatever.
            for (ushort i = 0; i < state.Height; i++)
            {
                for (ushort j = 0; j < state.Width; j++)
                {
                    if (ShouldSurvive((i, j), state))
                    {
                        survivingCells.Add((i, j));
                    }
                }
            }

            return new GameOfLifeState(state.Width, state.Height, survivingCells);
        }

        private static bool ShouldSurvive((ushort Row, ushort Col) coords, GameOfLifeState state)
        {
            bool living = state.LiveCells.Contains(coords);
            uint numNeighbors = CountNeighbors(coords, state);
            return numNeighbors == 3 || (living && numNeighbors == 2);
        }

        private static uint CountNeighbors((ushort Row, ushort Col) coords, GameOfLifeState state)
        {
            ushort i = coords.Row;
            ushort j = coords.Col;
            uint neighborCount = 0;

            int[] offsets = new int[] { -1, 0, 1 };
            foreach (int rowOffset in offsets)
            {
                foreach (int colOffset in offsets)
                {
                    if (rowOffset == 0 && colOffset == 0) continue; // Skip self

                    (ushort Row, ushort Col) neighborCoords = NormalizeCoordinates((i + rowOffset, j + colOffset), state);
                    if (state.LiveCells.Contains(neighborCoords))
                    {
                        neighborCount++;
                    }
                }
            }
            return neighborCount;
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