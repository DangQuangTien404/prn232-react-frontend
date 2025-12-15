using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using RentalPropertyManagement.BLL.DTOs;
using RentalPropertyManagement.BLL.Interfaces;
using RentalPropertyManagement.DAL.Data;
using RentalPropertyManagement.DAL.Entities;
using RentalPropertyManagement.DAL.Enums;

namespace RentalPropertyManagement.BLL.Services
{
    public class ChatService : IChatService
    {
        private readonly RentalDbContext _context;

        public ChatService(RentalDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<UserDto>> GetContactListAsync(int currentUserId, UserRole currentUserRole)
        {
            IQueryable<User> query = _context.Users.Where(u => u.Id != currentUserId);

            if (currentUserRole == UserRole.Landlord) // Is Landlord
            {
                // Landlord sees Tenants
                query = query.Where(u => u.Role == UserRole.Tenant);
            }
            else if (currentUserRole == UserRole.Tenant) // Is Tenant
            {
                // Tenant sees Landlords
                query = query.Where(u => u.Role == UserRole.Landlord);
            }
            else
            {
                // Default fallback or for other roles: see nobody or everyone?
                // Per requirement "Tenant <-> Landlord", others might see nobody.
                return new List<UserDto>();
            }

            return await query.Select(u => new UserDto
            {
                Id = u.Id,
                FullName = u.FirstName + " " + u.LastName,
                Email = u.Email,
                Role = u.Role.ToString()
            }).ToListAsync();
        }

        public async Task<IEnumerable<MessageDto>> GetChatHistoryAsync(int currentUserId, int otherUserId)
        {
            var messages = await _context.Messages
                .Where(m => (m.SenderId == currentUserId && m.ReceiverId == otherUserId) ||
                            (m.SenderId == otherUserId && m.ReceiverId == currentUserId))
                .OrderBy(m => m.Timestamp)
                .Select(m => new MessageDto
                {
                    Id = m.Id,
                    SenderId = m.SenderId,
                    ReceiverId = m.ReceiverId,
                    Content = m.Content,
                    Timestamp = m.Timestamp,
                    IsRead = m.IsRead,
                    SenderName = m.Sender.FirstName + " " + m.Sender.LastName
                })
                .ToListAsync();

            return messages;
        }

        public async Task SendMessageAsync(int senderId, int receiverId, string content)
        {
            var message = new Message
            {
                SenderId = senderId,
                ReceiverId = receiverId,
                Content = content,
                Timestamp = DateTime.Now,
                IsRead = false
            };

            _context.Messages.Add(message);
            await _context.SaveChangesAsync();
        }
    }
}
