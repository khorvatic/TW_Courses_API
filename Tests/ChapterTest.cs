using Application.DTO.Chapter;
using Application.Interfaces;
using Domain.Exceptions;
using Microsoft.AspNetCore.Mvc;
using Moq;
using Presentation.Controllers;
using System;
using System.Collections.Generic;
using System.Text;

namespace Tests
{
    public class ChapterTest
    {
        [Fact]
        public async Task GetChapterById_ReturnsOk_WhenChapterExists()
        {
            //Arrange
            var mockService = new Mock<IChapterService>();
            mockService.Setup(service => service.GetChapterByIdAsync(1))
                .ReturnsAsync(new ChapterDto { Id = 1, Name = "Test", Length = TimeSpan.FromMinutes(45) });

            // Act
            var controller = new ChapterController(mockService.Object);
            var result = await controller.GetChapterById(1);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result.Result);
            var chapter = Assert.IsType<ChapterDto>(okResult.Value);
            Assert.Equal("Test", chapter.Name);
        }

        [Fact]
        public async Task GetChapterById_ThrowsNotFound_WhenChapterDoesntExist()
        {
            // Arrange
            var mockService = new Mock<IChapterService>();
            mockService.Setup(service => service.GetChapterByIdAsync(999))
                .ThrowsAsync(new NotFoundException("Chapter not found"));

            // Act
            var controller = new ChapterController(mockService.Object);

            // Assert
            await Assert.ThrowsAsync<NotFoundException>(() => controller.GetChapterById(999));
        }

        [Fact]
        public async Task GetChapterByName_Should_ReturnOk_WhenChapterExists()
        {
            // Arrange
            var mockService = new Mock<IChapterService>();
            mockService.Setup(service => service.GetChapterByNameAsync("Test"))
                .ReturnsAsync(new ChapterDto { Id = 1, Name = "Test", Length = TimeSpan.FromMinutes(45) });

            // Act
            var controller = new ChapterController(mockService.Object);
            var result = await controller.GetChapterByName("Test");

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result.Result);
            var chapter = Assert.IsType<ChapterDto>(okResult.Value);
            Assert.Equal("Test", chapter.Name);
        }

        [Fact]
        public async Task CreateChapter_Should_ReturnCreatedAtAction_WhenChapterIsCreated()
        {
            // Arrange
            var mockService = new Mock<IChapterService>();
            var createChapterDto = new CreateChapterDto { 
                Name = "New Chapter",
                Length = TimeSpan.FromMinutes(30) 
            };
            var createdChapterDto = new ChapterDto
            {
                Id = 1,
                Name = "New Chapter",
                Length = TimeSpan.FromMinutes(30)
            };
            mockService.Setup(service => service.CreateChapterAsync(1, createChapterDto))
                .ReturnsAsync(createdChapterDto);

            // Act
            var controller = new ChapterController(mockService.Object);
            var result = await controller.CreateChapter(1, createChapterDto);

            // Assert
            var createdAtActionResult = Assert.IsType<CreatedAtActionResult>(result.Result);
            var chapter = Assert.IsType<ChapterDto>(createdAtActionResult.Value);
            Assert.Equal(nameof(ChapterController.GetChapterById), createdAtActionResult.ActionName);
            Assert.Equal(createdChapterDto.Id, createdAtActionResult.RouteValues["id"]);
        }

        [Fact]
        public async Task GetAllChapters_Should_ReturnOkWithList()
        {
            // Arrange
            var mockService = new Mock<IChapterService>();
            var chapters = new List<ChapterDto>
            {
                new ChapterDto { Id = 1, Name = "Chapter 1", Length = TimeSpan.FromMinutes(30), CourseId = 1 },
                new ChapterDto { Id = 2, Name = "Chapter 2", Length = TimeSpan.FromMinutes(45), CourseId = 1 }
            };
            mockService.Setup(service => service.GetAllChaptersAsync(1))
                .ReturnsAsync(chapters);

            // Act
            var controller = new ChapterController(mockService.Object);
            var result = await controller.GetAllChaptersForCourse(1);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result.Result);
            var returnedChapters = Assert.IsType<List<ChapterDto>>(okResult.Value);
            Assert.Equal(2, returnedChapters.Count);
        }

        [Fact]
        public async Task UpdateChapter_Should_ReturnOk_WhenChapterUpdated()
        {
            // Arrange
            var mockService = new Mock<IChapterService>();
            var updateChapterDto = new CreateChapterDto
            {
                Name = "Updated Chapter",
                Length = TimeSpan.FromMinutes(60)
            };
            var updatedChapterDto = new ChapterDto
            {
                Id = 1,
                Name = "Updated Chapter",
                Length = TimeSpan.FromMinutes(60)
            };
            mockService.Setup(service => service.UpdateChapterAsync(1, updateChapterDto))
                .ReturnsAsync(updatedChapterDto);

            // Act
            var controller = new ChapterController(mockService.Object);
            var result = await controller.UpdateChapter(1, updateChapterDto);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result.Result);
            var chapter = Assert.IsType<ChapterDto>(okResult.Value);
            Assert.Equal(chapter.Name, updatedChapterDto.Name);
            Assert.Equal(chapter.Length, updatedChapterDto.Length);
            mockService.Verify(service => service.UpdateChapterAsync(1, updateChapterDto), Times.Once);
        }

        [Fact]
        public async Task DeleteChapter_Should_ReturnOk_WhenChapterDeleted()
        {
            // Arrange
            var mockService = new Mock<IChapterService>();
            mockService.Setup(service => service.DeleteChapterAsync(1))
                .Returns(Task.CompletedTask);

            // Act
            var controller = new ChapterController(mockService.Object);
            var result = await controller.DeleteChapter(1);

            // Assert
            var okResult = Assert.IsType<OkResult>(result);
            mockService.Verify(service => service.DeleteChapterAsync(1), Times.Once);
        }
    }
}
