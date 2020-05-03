using Xunit;
using System.Collections.Generic;
using GameOfLifeService.Core;

namespace GameOfLifeService.Tests.Core
{
    public class IteratorTest
    {
        // TODO: Mock the validator
        private Iterator iterator = new Iterator(new Validator());

        [Fact]
        public void Iterate_InvalidState_Throws()
        {
            GameOfLifeState state = new GameOfLifeState(5, 5, null);
            Assert.Throws<System.ArgumentException>(() => iterator.Iterate(state));
        }

        [Fact]
        public void Iterate_TrivialState_ReturnsSame()
        {
            GameOfLifeState state = new GameOfLifeState(0, 0);
            GameOfLifeState next = iterator.Iterate(state);
            Assert.Equal(state, next);
        }

        [Fact]
        public void Iterate_DeadState_ReturnsSame()
        {
            GameOfLifeState state = new GameOfLifeState(10, 10);
            GameOfLifeState next = iterator.Iterate(state);
            Assert.Equal(state, next);
        }

        [Fact]
        public void Iterate_Block_ReturnsSame()
        {
            GameOfLifeState state = new GameOfLifeState(4, 4, new HashSet<(ushort Row, ushort Col)> {
                (1, 1), (1, 2), (2, 1), (2, 2)
            });
            GameOfLifeState next = iterator.Iterate(state);
            Assert.Equal(state, next);
        }

        [Fact]
        public void Iterate_BeeHive_ReturnsSame()
        {
            GameOfLifeState state = new GameOfLifeState(5, 6, new HashSet<(ushort Row, ushort Col)> {
                (1, 2), (1, 3), (2, 1), (2, 4), (3, 2), (3, 3)
            });
            GameOfLifeState next = iterator.Iterate(state);
            Assert.Equal(state, next);
        }

        [Fact]
        public void Iterate_Loaf_ReturnsSame()
        {
            GameOfLifeState state = new GameOfLifeState(6, 6, new HashSet<(ushort Row, ushort Col)> {
                (1, 2), (1, 3), (2, 1), (2, 4), (3, 2), (3, 4), (4, 3)
            });
            GameOfLifeState next = iterator.Iterate(state);
            Assert.Equal(state, next);
        }

        [Fact]
        public void Iterate_Tub_ReturnsSame()
        {
            GameOfLifeState state = new GameOfLifeState(5, 5, new HashSet<(ushort Row, ushort Col)> {
                (1, 2), (2, 1), (2, 3), (3, 2)
            });
            GameOfLifeState next = iterator.Iterate(state);
            Assert.Equal(state, next);
        }

        [Fact]
        public void Iterate_Blinker_OscillatesWithPeriod2()
        {
            GameOfLifeState state = new GameOfLifeState(5, 5, new HashSet<(ushort Row, ushort Col)> {
                (2, 1), (2, 2), (2, 3)
            });
            GameOfLifeState next = iterator.Iterate(state);
            Assert.NotEqual(state, next);

            next = iterator.Iterate(next);
            Assert.Equal(state, next);
        }

        [Fact]
        public void Iterate_Pentadecathalon_OscillatesWithPeriod15()
        {
            GameOfLifeState state = new GameOfLifeState(11, 18, new HashSet<(ushort Row, ushort Col)> {
                (4, 5),
                (5, 5),
                (6, 4), (6, 6),
                (7, 5),
                (8, 5),
                (9, 5),
                (10, 5),
                (11, 4), (11, 6),
                (12, 5),
                (13, 5)
            });

            GameOfLifeState next = iterator.Iterate(state);

            int p = 1;
            while (!state.Equals(next))
            {
                next = iterator.Iterate(next);
                p++;

            }
            Assert.Equal(15, p);
        }

        [Fact]
        public void Iterate_Glider_LoopsAroundToStart()
        {
            GameOfLifeState state = new GameOfLifeState(100, 100, new HashSet<(ushort Row, ushort Col)> {
                (1, 2), (2, 3), (3, 1), (3, 2), (3, 3)
            });
            GameOfLifeState next = iterator.Iterate(state);

            int p = 1;
            while (!state.Equals(next))
            {
                next = iterator.Iterate(next);
                p++;

            }
            Assert.Equal(400, p);
        }
    }
}
