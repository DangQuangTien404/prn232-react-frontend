using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using RentalPropertyManagement.BLL.DTOs;
using RentalPropertyManagement.DAL.Entities;
using RentalPropertyManagement.DAL.Enums;

namespace RentalPropertyManagement.BLL.Interfaces
{
    public interface IChatService
    {
        Task<IEnumerable<UserDto>> GetContactListAsync(int currentUserId, UserRole currentUserRole);
        Task<IEnumerable<MessageDto>> GetChatHistoryAsync(int currentUserId, int otherUserId);
        Task SendMessageAsync(int senderId, int receiverId, string content);
    }
}
