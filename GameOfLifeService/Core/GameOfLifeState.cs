using System.Collections.Generic;

namespace GameOfLifeService.Core
{
    public class GameOfLifeState
    {
        public ushort Width { get; }
        public ushort Height { get; }
        public ISet<(ushort Row, ushort Col)> LiveCells { get; }

        public GameOfLifeState(ushort width, ushort height)
        {
            this.Width = width;
            this.Height = height;
            this.LiveCells = new HashSet<(ushort Row, ushort Col)>();
        }

        public GameOfLifeState(ushort width, ushort height, ISet<(ushort Row, ushort Col)> liveCells)
        {
            this.Width = width;
            this.Height = height;
            this.LiveCells = liveCells;
        }

        public override bool Equals(object obj)
        {
            if (obj == null || GetType() != obj.GetType()) return false;
            GameOfLifeState other = (GameOfLifeState)obj;

            if (other.Width != this.Width) return false;
            if (other.Height != this.Height) return false;
            if (other.LiveCells.Count != this.LiveCells.Count) return false;

            foreach ((ushort Row, ushort Col) coords in this.LiveCells)
            {
                if (!other.LiveCells.Contains(coords)) return false;
            }

            return true;
        }

        public override int GetHashCode() => base.GetHashCode();
    }
}