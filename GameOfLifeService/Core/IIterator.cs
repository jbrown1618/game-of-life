namespace GameOfLifeService.Core
{
    public interface IIterator
    {
        /// <summary>
        /// Iterate computes the state that results by applying the rules of
        /// the Game of Life to an initial state.
        /// </summary>
        /// <param name="state">The initial state</param>
        /// <returns>The next state</returns>
        public GameOfLifeState Iterate(GameOfLifeState state);
    }
}