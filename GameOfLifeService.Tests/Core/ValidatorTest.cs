using Xunit;
using System.Collections.Generic;
using GameOfLifeService.Core;

namespace GameOfLifeService.Tests.Core
{
    public class ValidatorTest
    {
        [Fact]
        public void Validate_ValidState_ReturnsEmptySet()
        {
            GameOfLifeState state = new GameOfLifeState(5, 5);
            Assert.Empty(Validator.Validate(state));

            state = new GameOfLifeState(5, 5, new HashSet<(ushort Row, ushort Col)> { (1, 1), (1, 2) });
            Assert.Empty(Validator.Validate(state));
        }

        [Fact]
        public void Validate_OutOfBounds_ReturnsErrorsForEachDimension()
        {
            GameOfLifeState state = new GameOfLifeState(5, 5, new HashSet<(ushort Row, ushort Col)> { (1, 5) });
            Assert.Equal(1, Validator.Validate(state).Count);

            state = new GameOfLifeState(5, 5, new HashSet<(ushort Row, ushort Col)> { (5, 1) });
            Assert.Equal(1, Validator.Validate(state).Count);

            state = new GameOfLifeState(5, 5, new HashSet<(ushort Row, ushort Col)> { (5, 5) });
            Assert.Equal(2, Validator.Validate(state).Count);
        }

        [Fact]
        public void Validate_OutOfBounds_ReturnsErrorsForEachOccurrence()
        {
            GameOfLifeState state = new GameOfLifeState(5, 5, new HashSet<(ushort Row, ushort Col)> {
                (1, 1),
                (5, 1), // 1 error
                (1, 5), // 1 error
                (5, 5), // 2 errors
                (0, 0)
            });
            Assert.Equal(4, Validator.Validate(state).Count);
        }

        [Fact]
        public void Validate_NullLiveCells_ReturnsOneError()
        {
            GameOfLifeState state = new GameOfLifeState(5, 5, null);
            Assert.Equal(1, Validator.Validate(state).Count);
        }
    }
}
