using FluentAssertions;
using Moq;
using AutoFixture;
using ayagroup_SMS.Application.EntityServices;
using ayagroup_SMS.Core.Entities;
using ayagroup_SMS.Core.Interfaces.Infrastructure;
using ayagroup_SMS.Core.DTOs.Requestes;
using ayagroup_SMS.Core.DTOs.Responses;
using Microsoft.Extensions.Logging;
using Xunit;
using System;
using Microsoft.AspNetCore.Http;
using System.Linq;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using System.Threading;

namespace ayagroup_SMS.Application.Tests.Services
{
    public class TasksServiceTests
    {
        private readonly Mock<IUnitOfWork> _mockUnitOfWork;
        private readonly Mock<IHttpContextAccessor> _mockHttpContextAccessor;
        private readonly Mock<ILogger<TasksService>> _mockLogger;
        private readonly Fixture _fixture;
        private readonly TasksService _service;
        private readonly Guid _testUserId = Guid.Parse("EC4A244B-3F8C-494C-B8B8-3EF2E9B2BD15");
        private readonly Guid _testTaskId = Guid.Parse("EC4A244B-3F8C-494C-B8B8-3EF2E9B2BD15");

        public TasksServiceTests()
        {
            _mockUnitOfWork = new Mock<IUnitOfWork>();
            _mockHttpContextAccessor = new Mock<IHttpContextAccessor>();
            _mockLogger = new Mock<ILogger<TasksService>>();
            _fixture = new Fixture();
            _fixture.Behaviors.Add(new OmitOnRecursionBehavior());

            _service = new TasksService(
                _mockUnitOfWork.Object,
                _mockLogger.Object,
                _mockHttpContextAccessor.Object
            );
        }

        [Fact]
        public async Task CreateAsync_ValidRequest_ReturnsSuccess()
        {
            // Arrange
            var validCategoryId = Guid.Parse("BFFD2523-8149-4099-8A49-A6C16102650B");

            var dto = new TaskCreateDto
            {
                Title = "Valid Task Title",
                Description = "Test description",
                DueDate = DateTime.UtcNow.AddDays(1),
                CategoryId = validCategoryId,
                Priority = Priority.Medium // Valid enum value
            };

            // Mock category exists
            _mockUnitOfWork.Setup(u => u.Categories.ExistsAsync(validCategoryId))
                           .ReturnsAsync(true);

            // Mock task operations
            _mockUnitOfWork.Setup(u => u.Tasks.AddAsync(It.IsAny<Tasks>()))
                .Returns(Task.CompletedTask);

            _mockUnitOfWork.Setup(u => u.SaveChangesAsync())
                .ReturnsAsync(1);

            // Act
            var result = await _service.CreateAsync(dto, _testUserId);

            // Assert
            result.Success.Should().BeTrue();
            result.Message.Should().Be("Task created successfully");
        }

    }
}