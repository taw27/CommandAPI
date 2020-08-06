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


        [Fact]
        public void PostCommandItemObjectCountIncrementWhenValidObject()
        {
            var command = new Command
            {
                HowTo = "Do Somethting",
                Platform = "Some Platform",
                CommandLine = "Some Command"
            };
            var originalCount = dbContext.CommandItems.Count();
            var result = controller.PostCommandItem(command);

            Assert.Equal(originalCount + 1, dbContext.CommandItems.Count());
        }
        [Fact]
        public void PostCommandItemReturns201CreatedWhenValidObject()
        {
            var command = new Command
            {
                HowTo = "Do Somethting",
                Platform = "Some Platform",
                CommandLine = "Some Command"
            };

            var result = controller.PostCommandItem(command);

            Assert.IsType<CreatedAtActionResult>(result.Result);
        }

        [Fact]
        public void PutCommandItemAttributeUpdatedWhenValidObject()
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

            command.HowTo = "Updated";

            controller.PutCommandItem(commandId, command);
            var result = dbContext.CommandItems.Find(commandId);

            Assert.Equal(command.HowTo, result.HowTo);
        }

        [Fact]
        public void PutCommandItemReturns204WhenValidObject()
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

            command.HowTo = "Updated";

            var result = controller.PutCommandItem(commandId, command);

            Assert.IsType<NoContentResult>(result);
        }

        [Fact]
        public void PutCommandItemReturns400WhenInvalidObject()
        {
            var command = new Command
            {
                HowTo = "Do Somethting",
                Platform = "Some Platform",
                CommandLine = "Some Command"
            };
            dbContext.CommandItems.Add(command);
            dbContext.SaveChanges();

            var commandId = command.Id + 1;

            command.HowTo = "Updated";

            var result = controller.PutCommandItem(commandId, command);

            Assert.IsType<BadRequestResult>(result);
        }

        [Fact]
        public void PutCommandItemAttributeUnchangedWhenInvalidObject()
        {
            var command = new Command
            {
                HowTo = "Do Somethting",
                Platform = "Some Platform",
                CommandLine = "Some Command"
            };
            dbContext.CommandItems.Add(command);
            dbContext.SaveChanges();
            var command2 = new Command
            {
                Id = command.Id,
                HowTo = "UPDATED",
                Platform = "UPDATED",
                CommandLine = "UPDATED"
            };

            controller.PutCommandItem(command.Id + 1, command2);

            var result = dbContext.CommandItems.Find(command.Id);

            Assert.Equal(command.HowTo, result.HowTo);
        }

        [Fact]
        public void DeleteCommandItemObjectsDecrementWhenValidObjectID()
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
            var oldCount = dbContext.CommandItems.Count();

            controller.DeleteCommandItem(commandId);

            Assert.Equal(oldCount - 1, dbContext.CommandItems.Count());
        }

        [Fact]
        public void DeleteCommandItemReturns200OKWHenValidObjectID()
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

            var result = controller.DeleteCommandItem(commandId);

            Assert.Null(result.Result);

        }


        [Fact]
        public void DeleteCommandItemReturns404NotFoundWhenInvalidObjectID()
        {
            var result = controller.DeleteCommandItem(-1);
            Assert.IsType<NotFoundResult>(result.Result);
        }

        [Fact]
        public void DeleteCommandItemObjectCountNotDecrementedWhenInvalidObjectID()
        {
            var command = new Command
            {
                HowTo = "Do Somethting",
                Platform = "Some Platform",
                CommandLine = "Some Command"
            }; dbContext.CommandItems.Add(command); dbContext.SaveChanges();
            var commandId = command.Id;
            var objCount = dbContext.CommandItems.Count();

            var result = controller.DeleteCommandItem(commandId + 1);

            Assert.Equal(objCount, dbContext.CommandItems.Count());
        }


    }
}