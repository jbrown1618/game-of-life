using Xunit;
using System.Collections.Generic;
using GameOfLifeService.Core;

namespace GameOfLifeService.Tests.Core
{
    public class ValidatorTest
    {
        private IValidator validator = new Validator();


        [Fact]
        public void Validate_ValidState_ReturnsEmptySet()
        {
            var state = new GameOfLifeState(5, 5);
            Assert.Empty(validator.Validate(state));

            state = new GameOfLifeState(5, 5, new HashSet<(ushort Row, ushort Col)> { (1, 1), (1, 2) });
            Assert.Empty(validator.Validate(state));
        }

        [Fact]
        public void Validate_OutOfBounds_ReturnsErrorsForEachDimension()
        {
            var state = new GameOfLifeState(5, 5, new HashSet<(ushort Row, ushort Col)> { (1, 5) });
            Assert.Equal(1, validator.Validate(state).Count);

            state = new GameOfLifeState(5, 5, new HashSet<(ushort Row, ushort Col)> { (5, 1) });
            Assert.Equal(1, validator.Validate(state).Count);

            state = new GameOfLifeState(5, 5, new HashSet<(ushort Row, ushort Col)> { (5, 5) });
            Assert.Equal(2, validator.Validate(state).Count);
        }

        [Fact]
        public void Validate_OutOfBounds_ReturnsErrorsForEachOccurrence()
        {
            var state = new GameOfLifeState(5, 5, new HashSet<(ushort Row, ushort Col)> {
                (1, 1),
                (5, 1), // 1 error
                (1, 5), // 1 error
                (5, 5), // 2 errors
                (0, 0)
            });
            Assert.Equal(4, validator.Validate(state).Count);
        }

        [Fact]
        public void Validate_NullLiveCells_ReturnsOneError()
        {
            var state = new GameOfLifeState(5, 5, null);
            Assert.Equal(1, validator.Validate(state).Count);
        }
    }
}
