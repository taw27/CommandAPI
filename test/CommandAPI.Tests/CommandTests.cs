using System;
using CommandAPI.Models;
using Xunit;

namespace CommandAPI.Tests
{
    public class CommandTests : IDisposable
    {
        Command testCommand;

        public CommandTests()
        {
            // Arrange
            testCommand = new Command
            {
                HowTo = "Do something awsome",
                Platform = "some platform",
                CommandLine = "some command line"
            };

        }

        public void Dispose()
        {
            testCommand = null;
        }

        [Fact]
        public void CanChangeHowTo()
        {

            // Act
            testCommand.HowTo = "Execute Unit Tests";

            // Assert
            Assert.Equal("Execute Unit Tests", testCommand.HowTo);
        }


        [Fact]

        public void CanChangeCommandLine()
        {
            testCommand.CommandLine = "dotnet test";
            // Assert
            Assert.Equal("dotnet test", testCommand.CommandLine);
        }

        [Fact]
        public void CanChangePlatform()
        {
            testCommand.Platform = "xUnit";
            Assert.Equal("xUnit", testCommand.Platform);
        }
    }

}