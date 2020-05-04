using Xunit;
using Moq;
using Microsoft.AspNetCore.Mvc;
using GameOfLifeService.Controllers;
using GameOfLifeService.Core;
using GameOfLifeService.DTO;

namespace GameOfLifeService.Tests.Controllers
{
    public class GameOfLifeControllerTest
    {
        private GameOfLifeController MakeController()
        {
            var mockIterator = new Mock<IIterator>();
            mockIterator
                .Setup(i => i.Iterate(It.IsAny<GameOfLifeState>()))
                .Returns(new GameOfLifeState(0, 0));
            return new GameOfLifeController(mockIterator.Object);
        }

        [Fact]
        public void Get_StateHasCorrectDimensions()
        {
            var controller = MakeController();
            var result = controller.Get(5, 6) as ObjectResult;

            Assert.NotNull(result);
            Assert.Equal(200, result.StatusCode);

            var state = result.Value as GameOfLifeStateDTO;

            Assert.NotNull(state);
            Assert.Equal(5, state.Width);
            Assert.Equal(6, state.Height);
            Assert.Empty(state.LiveCells);
        }

        [Fact]
        public void Get_NegativeDimension_ReturnsError()
        {
            var controller = MakeController();
            var result = controller.Get(-1, 6) as ObjectResult;

            Assert.NotNull(result);
            Assert.Equal(500, result.StatusCode);

            var error = result.Value as string;

            Assert.NotNull(error);
            Assert.Contains("nonnegative", error);

            result = controller.Get(5, -1) as ObjectResult;

            Assert.NotNull(result);
            Assert.Equal(500, result.StatusCode);

            error = result.Value as string;

            Assert.NotNull(error);
            Assert.Contains("nonnegative", error);
        }

        [Fact]
        public void Get_LargeDimension_ReturnsError()
        {
            var controller = MakeController();
            var result = controller.Get(ushort.MaxValue + 1, 1) as ObjectResult;

            Assert.NotNull(result);
            Assert.Equal(500, result.StatusCode);

            var error = result.Value as string;

            Assert.NotNull(error);
            Assert.Contains(ushort.MaxValue.ToString(), error);

            result = controller.Get(1, ushort.MaxValue + 1) as ObjectResult;

            Assert.NotNull(result);
            Assert.Equal(500, result.StatusCode);

            error = result.Value as string;

            Assert.NotNull(error);
            Assert.Contains(ushort.MaxValue.ToString(), error);
        }

        [Fact]
        public void Iterate_State_ReturnsState()
        {
            var controller = MakeController();
            var result = controller.Iterate(new GameOfLifeStateDTO()) as ObjectResult;

            Assert.NotNull(result);
            Assert.Equal(200, result.StatusCode);

            var state = result.Value as GameOfLifeStateDTO;

            Assert.NotNull(state);
        }

        [Fact]
        public void Iterate_Error_ReturnsError()
        {
            var mockIterator = new Mock<IIterator>();
            mockIterator
                .Setup(i => i.Iterate(It.IsAny<GameOfLifeState>()))
                .Throws(new System.ArgumentException("The error message"));
            var controller = new GameOfLifeController(mockIterator.Object);

            var result = controller.Iterate(new GameOfLifeStateDTO()) as ObjectResult;

            Assert.NotNull(result);
            Assert.Equal(500, result.StatusCode);

            var error = result.Value as string;

            Assert.NotNull(error);
            Assert.Contains("The error message", error);
        }
    }
}
