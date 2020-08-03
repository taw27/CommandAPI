using System;
using System.Collections.Generic;
using System.Linq;
using CommandAPI.Controllers;
using CommandAPI.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Xunit;

namespace CommandAPI.Tests
{
    public class CommandsControllerTests : IDisposable
    {
        DbContextOptionsBuilder<CommandContext> optionsBuilder;
        CommandContext dbContext;
        CommandsController controller;

        public CommandsControllerTests()
        {
            optionsBuilder = new DbContextOptionsBuilder<CommandContext>();
            optionsBuilder.UseInMemoryDatabase("UnitTestInMemDB");
            dbContext = new CommandContext(optionsBuilder.Options);

            controller = new CommandsController(dbContext);
        }

        public void Dispose()
        {

            optionsBuilder = null;
            foreach (var command in dbContext.CommandItems)
            {
                dbContext.CommandItems.Remove(command);
            }
            dbContext.SaveChanges();
            dbContext.Dispose();
            controller = null;
        }


        [Fact]
        public void GetCommandItemsReturnsZeroItemsWhenDBIsEmpty()
        {
            var result = controller.GetCommandItems();

            // Assert
            Assert.Empty(result.Value);

        }

        [Fact]
        public void GetCommandItemsReturnsOneItemWhenDBHasOneObject()
        {
            var command = new Command
            {
                HowTo = "Do Somethting",
                Platform = "Some Platform",
                CommandLine = "Some Command"
            };

            dbContext.CommandItems.Add(command);
            dbContext.SaveChanges();

            var result = controller.GetCommandItems();

            Assert.Single(result.Value);
        }

        [Fact]
        public void GetCommandItemsReturnNItemsWhenDBHasNObjects()
        {
            var command1 = new Command
            {
                HowTo = "Do Somethting",
                Platform = "Some Platform",
                CommandLine = "Some Command"
            };

            var command2 = new Command
            {
                HowTo = "Do Somethting2",
                Platform = "Some Platform2",
                CommandLine = "Some Command2"
            };
            dbContext.CommandItems.Add(command1);
            dbContext.CommandItems.Add(command2);
            dbContext.SaveChanges();

            var result = controller.GetCommandItems();

            Assert.Equal(2, result.Value.Count());
        }
        [Fact]
        public void GetCommandItemsReturnsTheCorrectType()
        {
            var result = controller.GetCommandItems();

            Assert.IsType<ActionResult<IEnumerable<Command>>>(result);
        }

        [Fact]
        public void GetCommandItemReturnsNullResultWhenInvalidID()
        {
            var result = controller.GetCommandItem(0);

            Assert.Null(result.Value);
        }

        [Fact]
        public void GetCommandItemReturns404NotFoundWhenInvalid()
        {
            var result = controller.GetCommandItem(0);

            Assert.IsType<NotFoundResult>(result.Result);

        }

        [Fact]
        public void GetCommandItemReturnsTheCorrectType()
        {
            var command = new Command
            {
                HowTo = "Do Somethting",
                Platform = "Some Platform",
                CommandLine = "Some Command"
            };
            dbContext.CommandItems.Add(command);
            dbContext.SaveChanges();

            var commandId = command.Id;
            var result = controller.GetCommandItem(commandId);

            Assert.IsType<ActionResult<Command>>(result);
        }

        [Fact]
        public void GetCommandItemReturnsTheCorrectResource()
        {
            var command = new Command
            {
                HowTo = "Do Somethting",
                Platform = "Some Platform",
                CommandLine = "Some Command"
            };
            dbContext.CommandItems.Add(command);
            dbContext.SaveChanges();

            var commandId = command.Id;
            var result = controller.GetCommandItem(commandId);

            Assert.Equal(commandId, result.Value.Id);
        }
    }
}