using System.Collections.Generic;

namespace GameOfLifeService.Core
{
    public interface IValidator
    {
        /// <summary>
        /// Validate inspects a state to ensure that it represents a valid
        /// Game of Life state. That is, the set of live cells is not null,
        /// and each pair of coordinates in the set represents a cell that
        /// is within the bounds set by the width and height.
        /// </summary>
        /// <param name="state">The state to validate</param>
        /// <returns>
        /// A set of strings, each of which represents an error in the state.
        /// Returns an empty set for a valid state.
        /// </returns>
        public ISet<string> Validate(GameOfLifeState state);
    }
}