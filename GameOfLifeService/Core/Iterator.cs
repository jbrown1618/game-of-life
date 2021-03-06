using System;
using System.Collections.Generic;

namespace GameOfLifeService.Core
{
    /// <inheritdoc cref="IIterator"/>
    public class Iterator : IIterator
    {
        private IValidator _validator;

        public Iterator(IValidator validator)
        {
            this._validator = validator;
        }

        public GameOfLifeState Iterate(GameOfLifeState state)
        {
            ISet<string> errors = _validator.Validate(state);
            if (errors.Count > 0)
            {
                throw new System.ArgumentException("The starting state must be valid:\n" + String.Join("\n", errors));
            }

            // We only need to check cells that are live or adjacent to an existing live cell.
            // For large, sparse grids, this is a pretty big optimization.
            var potentialSurvivors = new HashSet<(ushort Row, ushort Col)>();
            foreach (var liveCoords in state.LiveCells)
            {
                potentialSurvivors.Add(liveCoords);
                foreach (var neighborCoords in GetNeighbors(liveCoords, state))
                {
                    potentialSurvivors.Add(neighborCoords);
                }
            }

            var survivingCells = new HashSet<(ushort Row, ushort Col)>();
            foreach (var coords in potentialSurvivors)
            {
                if (ShouldSurvive(coords, state))
                {
                    survivingCells.Add(coords);
                }
            }

            return new GameOfLifeState(state.Width, state.Height, survivingCells);
        }

        private bool ShouldSurvive((ushort Row, ushort Col) coords, GameOfLifeState state)
        {
            bool living = state.LiveCells.Contains(coords);
            uint numNeighbors = CountNeighbors(coords, state);
            return numNeighbors == 3 || (living && numNeighbors == 2);
        }

        private uint CountNeighbors((ushort Row, ushort Col) coords, GameOfLifeState state)
        {
            var neighborCount = 0u;
            foreach (var neighborCoords in GetNeighbors(coords, state))
            {
                if (state.LiveCells.Contains(neighborCoords))
                {
                    neighborCount++;
                }
            }
            return neighborCount;
        }

        private ISet<(ushort Row, ushort Col)> GetNeighbors((ushort Row, ushort Col) coords, GameOfLifeState state)
        {
            ushort i = coords.Row;
            ushort j = coords.Col;
            ISet<(ushort Row, ushort Col)> neighbors = new HashSet<(ushort Row, ushort Col)>();

            var offsets = new int[] { -1, 0, 1 };
            foreach (int rowOffset in offsets)
            {
                foreach (int colOffset in offsets)
                {
                    if (rowOffset == 0 && colOffset == 0) continue; // Skip self

                    neighbors.Add(NormalizeCoordinates((i + rowOffset, j + colOffset), state));
                }
            }
            return neighbors;
        }

        private (ushort Row, ushort Col) NormalizeCoordinates((int Row, int Col) coords, GameOfLifeState state)
        {
            return (
                (ushort)((coords.Row + state.Height) % state.Height),
                (ushort)((coords.Col + state.Width) % state.Width)
            );
        }
    }
}