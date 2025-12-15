using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using RentalPropertyManagement.BLL.Services;
using RentalPropertyManagement.DAL.Data;
using RentalPropertyManagement.DAL.Entities;
using RentalPropertyManagement.DAL.Enums;
using Xunit;

namespace RentalPropertyManagement.Tests
{
    public class ChatServiceTests
    {
        private RentalDbContext GetInMemoryDbContext()
        {
            var options = new DbContextOptionsBuilder<RentalDbContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options;

            return new RentalDbContext(options);
        }

        [Fact]
        public async Task GetContactListAsync_Landlord_ShouldReturnTenants()
        {
            // Arrange
            using var context = GetInMemoryDbContext();

            // Seed Users
            var landlord = new User { Id = 1, FirstName = "Land", LastName = "Lord", Email = "ll@test.com", Role = UserRole.Landlord, PasswordHash = "hash" };
            var tenant1 = new User { Id = 2, FirstName = "Ten", LastName = "Ant1", Email = "t1@test.com", Role = UserRole.Tenant, PasswordHash = "hash" };
            var tenant2 = new User { Id = 3, FirstName = "Ten", LastName = "Ant2", Email = "t2@test.com", Role = UserRole.Tenant, PasswordHash = "hash" };
            var otherLandlord = new User { Id = 4, FirstName = "Other", LastName = "Lord", Email = "ol@test.com", Role = UserRole.Landlord, PasswordHash = "hash" };

            context.Users.AddRange(landlord, tenant1, tenant2, otherLandlord);
            await context.SaveChangesAsync();

            var service = new ChatService(context);

            // Act
            var result = await service.GetContactListAsync(landlord.Id, UserRole.Landlord);

            // Assert
            Assert.Equal(2, result.Count()); // Should see 2 tenants
            Assert.Contains(result, u => u.Id == tenant1.Id);
            Assert.Contains(result, u => u.Id == tenant2.Id);
            Assert.DoesNotContain(result, u => u.Id == otherLandlord.Id);
        }

        [Fact]
        public async Task GetContactListAsync_Tenant_ShouldReturnLandlords()
        {
            // Arrange
            using var context = GetInMemoryDbContext();

            var tenant = new User { Id = 1, FirstName = "Ten", LastName = "Ant", Email = "t@test.com", Role = UserRole.Tenant, PasswordHash = "hash" };
            var landlord1 = new User { Id = 2, FirstName = "Land", LastName = "Lord1", Email = "l1@test.com", Role = UserRole.Landlord, PasswordHash = "hash" };
            var otherTenant = new User { Id = 3, FirstName = "Other", LastName = "Tenant", Email = "ot@test.com", Role = UserRole.Tenant, PasswordHash = "hash" };

            context.Users.AddRange(tenant, landlord1, otherTenant);
            await context.SaveChangesAsync();

            var service = new ChatService(context);

            // Act
            var result = await service.GetContactListAsync(tenant.Id, UserRole.Tenant);

            // Assert
            Assert.Single(result);
            Assert.Equal(landlord1.Id, result.First().Id);
        }

        [Fact]
        public async Task SendMessageAsync_ShouldSaveToDatabase()
        {
            // Arrange
            using var context = GetInMemoryDbContext();
            var service = new ChatService(context);

            int senderId = 1;
            int receiverId = 2;
            string content = "Hello World";

            // Act
            await service.SendMessageAsync(senderId, receiverId, content);

            // Assert
            var message = await context.Messages.FirstOrDefaultAsync();
            Assert.NotNull(message);
            Assert.Equal(senderId, message.SenderId);
            Assert.Equal(receiverId, message.ReceiverId);
            Assert.Equal(content, message.Content);
            Assert.False(message.IsRead);
            Assert.True(message.Timestamp > DateTime.MinValue);
        }

        [Fact]
        public async Task GetChatHistoryAsync_ShouldReturnMessagesBetweenUsers()
        {
            // Arrange
            using var context = GetInMemoryDbContext();

            var user1 = new User { Id = 1, FirstName = "User", LastName = "One", Role = UserRole.Landlord, Email = "u1", PasswordHash="p" };
            var user2 = new User { Id = 2, FirstName = "User", LastName = "Two", Role = UserRole.Tenant, Email = "u2", PasswordHash = "p" };
            var user3 = new User { Id = 3, FirstName = "User", LastName = "Three", Role = UserRole.Tenant, Email = "u3", PasswordHash = "p" };

            context.Users.AddRange(user1, user2, user3);

            context.Messages.AddRange(
                new Message { SenderId = 1, ReceiverId = 2, Content = "Hi 2", Timestamp = DateTime.Now.AddMinutes(-10), Sender = user1, Receiver = user2 },
                new Message { SenderId = 2, ReceiverId = 1, Content = "Hello 1", Timestamp = DateTime.Now.AddMinutes(-5), Sender = user2, Receiver = user1 },
                new Message { SenderId = 1, ReceiverId = 3, Content = "Hi 3", Timestamp = DateTime.Now.AddMinutes(-2), Sender = user1, Receiver = user3 } // Irrelevant message
            );
            await context.SaveChangesAsync();

            var service = new ChatService(context);

            // Act
            var history = await service.GetChatHistoryAsync(1, 2);

            // Assert
            Assert.Equal(2, history.Count());
            Assert.Equal("Hi 2", history.First().Content);
            Assert.Equal("Hello 1", history.Last().Content);
        }
    }
}
