using System.Collections.Generic;

namespace GameOfLifeService.Core
{
    public interface IValidator
    {
        public ISet<string> Validate(GameOfLifeState state);
    }
}