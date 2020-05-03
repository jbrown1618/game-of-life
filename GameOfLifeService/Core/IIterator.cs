namespace GameOfLifeService.Core
{
    public interface IIterator
    {
        public GameOfLifeState Iterate(GameOfLifeState state);
    }
}