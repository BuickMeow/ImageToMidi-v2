using Avalonia;
using ImageToMidi_v2.Models;
using ImageToMidi_v2.Services.Implementation;

namespace ImageToMidi_v2.Tests
{
    public class UnitTest1
    {
        [Fact]
        public void UpdateOffsetByPixelMovement_ShouldMaintain1To1PixelRatio()
        {
            // Arrange
            var calculator = new ZoomPanCalculator();
            var state = new ZoomPanState
            {
                ImageSize = new Size(1000, 800),
                ContainerSize = new Size(500, 400),
                Zoom = 2.0, // 高缩放级别
                Offset = new Point(0, 0)
            };

            // Act - 模拟鼠标移动100像素
            var pixelMovement = new Vector(100, 50);
            var newOffset = calculator.UpdateOffsetByPixelMovement(state, pixelMovement);

            // Assert - 验证偏移量的变化与像素移动成比例
            var displaySize = calculator.GetDisplaySize(state);
            var expectedOffsetX = 100.0 / displaySize.Width;
            var expectedOffsetY = 50.0 / displaySize.Height;

            Assert.Equal(expectedOffsetX, newOffset.X, precision: 6);
            Assert.Equal(expectedOffsetY, newOffset.Y, precision: 6);
        }

        [Fact]
        public void UpdateOffsetByPixelMovement_ShouldNotBeAffectedByZoomLevel()
        {
            // Arrange
            var calculator = new ZoomPanCalculator();
            var baseState = new ZoomPanState
            {
                ImageSize = new Size(1000, 800),
                ContainerSize = new Size(500, 400),
                Zoom = 1.0,
                Offset = new Point(0, 0)
            };

            var highZoomState = new ZoomPanState
            {
                ImageSize = new Size(1000, 800),
                ContainerSize = new Size(500, 400),
                Zoom = 5.0, // 高缩放级别
                Offset = new Point(0, 0)
            };

            // Act - 在不同缩放级别下应用相同的像素移动
            var pixelMovement = new Vector(100, 50);
            var baseOffset = calculator.UpdateOffsetByPixelMovement(baseState, pixelMovement);
            var highZoomOffset = calculator.UpdateOffsetByPixelMovement(highZoomState, pixelMovement);

            // Assert - 偏移量变化应该相同，不受缩放级别影响
            Assert.Equal(baseOffset.X, highZoomOffset.X, precision: 6);
            Assert.Equal(baseOffset.Y, highZoomOffset.Y, precision: 6);
        }
    }
}
