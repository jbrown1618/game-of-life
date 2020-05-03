using System.Collections.Generic;

namespace GameOfLifeService.Core
{
    public static class Validator
    {
        public static ISet<string> Validate(GameOfLifeState state)
        {
            ISet<string> errors = new HashSet<string>();
            if (state.LiveCells == null)
            {
                errors.Add(MissingLiveCells());
                return errors;
            }

            foreach ((ushort row, ushort col) coords in state.LiveCells)
            {
                if (coords.row >= state.Height)
                {
                    errors.Add(RowOutOfRange(coords.row, coords.col, state.Height));
                }
                if (coords.col >= state.Width)
                {
                    errors.Add(ColOutOfRange(coords.row, coords.col, state.Width));
                }
            }
            return errors;
        }

        private static string MissingLiveCells()
        {
            return "Live cells missing";
        }

        private static string Duplicates()
        {
            return "Duplicates present in the list of live cells";
        }

        private static string RowOutOfRange(ushort row, ushort col, ushort height)
        {
            return $"Live cell coordinate ({row}, {col}) is out of range; the row index must be < {height}";
        }

        private static string ColOutOfRange(ushort row, ushort col, ushort width)
        {
            return $"Live cell coordinate ({row}, {col}) is out of range; the column index must be < {width}";
        }
    }
}