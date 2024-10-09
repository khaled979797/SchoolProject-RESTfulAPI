using AutoMapper;
using FluentAssertions;
using Microsoft.Extensions.Localization;
using Moq;
using SchoolProject.Core.Features.Students.Commands.Handlers;
using SchoolProject.Core.Features.Students.Commands.Models;
using SchoolProject.Core.Mapping.Students;
using SchoolProject.Core.Resources;
using SchoolProject.Data.Entities;
using SchoolProject.Service.Abstracts;
using System.Net;

namespace SchoolProject.XUnitTest.CoreTests.Students.Commands
{
    public class StudentCommandHandlerTest
    {
        private readonly Mock<IStudentService> studentServiceMock;
        private readonly IMapper mapperMock;
        private readonly Mock<IStringLocalizer<SharedResources>> stringLocalizerServiceMock;
        private readonly StudentProfile studentProfile;
        public StudentCommandHandlerTest()
        {
            studentServiceMock = new();
            stringLocalizerServiceMock = new();
            studentProfile = new();
            var configuation = new MapperConfiguration(x => x.AddProfile(studentProfile));
            mapperMock = new Mapper(configuation);
        }

        [Fact]
        public async Task Handle_AddStudent_Should_Add_Data_And_StatusCode201()
        {
            //Arrange
            var handler = new StudentCommandHandler(studentServiceMock.Object, mapperMock, stringLocalizerServiceMock.Object);
            var addStudentCommand = new AddStudentCommand { NameAr = "محمد", NameEn = "Mohamed" };
            studentServiceMock.Setup(x => x.AddAsync(It.IsAny<Student>())).Returns(Task.FromResult("Success"));

            //Act
            var result = await handler.Handle(addStudentCommand, default);

            //Assert
            result.Succeeded.Should().BeTrue();
            result.StatusCode.Should().Be(HttpStatusCode.Created);
            studentServiceMock.Verify(x => x.AddAsync(It.IsAny<Student>()), Times.Once, "Not Called");
        }

        [Fact]
        public async Task Handle_AddStudent_Should_return_StatusCode400()
        {
            //Arrange
            var handler = new StudentCommandHandler(studentServiceMock.Object, mapperMock, stringLocalizerServiceMock.Object);
            var addStudentCommand = new AddStudentCommand { NameAr = "محمد", NameEn = "Mohamed" };
            studentServiceMock.Setup(x => x.AddAsync(It.IsAny<Student>())).Returns(Task.FromResult("BadRequest"));

            //Act
            var result = await handler.Handle(addStudentCommand, default);

            //Assert
            result.Succeeded.Should().BeFalse();
            result.StatusCode.Should().Be(HttpStatusCode.BadRequest);
            studentServiceMock.Verify(x => x.AddAsync(It.IsAny<Student>()), Times.Once, "Not Called");
        }

        [Fact]
        public async Task Handle_UpdateStudent_Should_Return_StatusCode404()
        {
            //Arrange
            var handler = new StudentCommandHandler(studentServiceMock.Object, mapperMock, stringLocalizerServiceMock.Object);
            var editStudentCommand = new EditStudentCommand { Id = 6, NameAr = "محمد", NameEn = "Mohamed" };
            Student? student = null;
            int xResult = 0;
            studentServiceMock.Setup(x => x.GetStudentByIdWithIncludeAsync(editStudentCommand.Id)).Returns(Task.FromResult(student)).Callback((int x) => xResult = x);
            //Act
            var result = await handler.Handle(editStudentCommand, default);

            //Assert
            result.Succeeded.Should().BeFalse();
            result.StatusCode.Should().Be(HttpStatusCode.NotFound);
            xResult.Should().Be(editStudentCommand.Id);
            studentServiceMock.Verify(x => x.GetStudentByIdWithIncludeAsync(It.IsAny<int>()), Times.Once, "Not Called");
        }
    }
}
