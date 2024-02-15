using Microsoft.AspNetCore.Mvc;
using System.Text.Json;
using VooltTestBackend.Controllers;
using Xunit;

namespace VooltTestBackendTests
{
    public class BlocksControllerTests
    {
        [Fact]
        public void CreateBlock_NewBlock_Success()
        {
            // Arrange
            var controller = new BlocksController();
            var key = "testKey";
            var expectedFilePath = $"{BlocksController.BlocksDirectory}{key}.json";

            // Act
            var result = controller.CreateBlock(key) as OkResult;

            // Assert
            Assert.NotNull(result);
            Assert.True(File.Exists(expectedFilePath));
        }

        [Fact]
        public void CreateBlock_BlockAlreadyExists_Conflict()
        {
            // Arrange
            var controller = new BlocksController();
            var key = "testKey";
            var filePath = $"{BlocksController.BlocksDirectory}{key}.json";
            File.Create(filePath).Dispose(); // Create dummy file

            // Act
            var result = controller.CreateBlock(key) as ConflictObjectResult;

            // Assert
            Assert.NotNull(result);
            Assert.Equal(409, result.StatusCode);
        }

        [Fact]
        public void GetBlocks_BlockExists_ReturnsBlockData()
        {
            // Arrange
            var controller = new BlocksController();
            var key = "testKey";
            var filePath = $"{BlocksController.BlocksDirectory}{key}.json";
            var expectedBlockData = new Dictionary<string, object> { { "ID", "TestBlock" } };
            File.WriteAllText(filePath, JsonSerializer.Serialize(expectedBlockData));

            // Act
            var result = controller.GetBlocks(key) as OkObjectResult;

            // Assert
            Assert.NotNull(result);
            var blockData = Assert.IsType<Dictionary<string, object>>(result.Value);
            Assert.Equal(expectedBlockData, blockData);
        }

    }
}