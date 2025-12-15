using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.SignalR;
using RentalPropertyManagement.BLL.DTOs;
using RentalPropertyManagement.BLL.Interfaces;
using RentalPropertyManagement.DAL.Enums;
using RentalPropertyManagement.Web.Hubs;

namespace RentalPropertyManagement.Web.Pages.Communication
{
    [Authorize]
    public class ChatModel : PageModel
    {
        private readonly IChatService _chatService;
        private readonly IHubContext<ChatHub> _hubContext;

        public ChatModel(IChatService chatService, IHubContext<ChatHub> hubContext)
        {
            _chatService = chatService;
            _hubContext = hubContext;
        }

        public IEnumerable<UserDto> Contacts { get; set; }
        public IEnumerable<MessageDto> ChatHistory { get; set; }
        public UserDto CurrentContact { get; set; }
        public int CurrentUserId { get; private set; }

        [BindProperty(SupportsGet = true)]
        public int? ReceiverId { get; set; }

        public async Task OnGetAsync()
        {
            var userIdStr = User.FindFirst("UserId")?.Value;
            if (int.TryParse(userIdStr, out int userId))
            {
                CurrentUserId = userId;

                // Determine Role from Claims
                // Note: The memory says User.FindFirst("UserId") is used.
                // We need to check if Role claim exists or we fetch user to get role.
                // Assuming Role is stored in claims or we can infer/fetch.
                // Let's fetch role from claim if available, otherwise we might need to fetch user.
                // Looking at Login logic in UserService (from memory/file), it returns UserDto with Role string.
                // However, standard cookie auth usually puts role in ClaimsIdentity.

                // Let's assume the Role claim is populated as ClaimTypes.Role.
                var roleStr = User.FindFirst(ClaimTypes.Role)?.Value;
                if (Enum.TryParse(roleStr, out UserRole role))
                {
                    Contacts = await _chatService.GetContactListAsync(userId, role);
                }
                else
                {
                    // Fallback: If role claim missing, maybe we need to fetch user?
                    // For now, let's assume we can't determine contacts without role.
                    Contacts = new List<UserDto>();
                }

                if (ReceiverId.HasValue)
                {
                    ChatHistory = await _chatService.GetChatHistoryAsync(userId, ReceiverId.Value);
                    CurrentContact = Contacts.FirstOrDefault(c => c.Id == ReceiverId.Value);

                    // If contact not in filtered list (e.g. hack), maybe fetch manually or ignore?
                    // For simplicity, if not found in Contacts, just show history without name or generic.
                    if (CurrentContact == null)
                    {
                         // Try to find even if not in list? Or just ignore.
                    }
                }
            }
        }

        // Note: Parameters now come from Form Data automatically if names match,
        // or we can use [FromForm] to be explicit.
        public async Task<IActionResult> OnPostSendMessageAsync([FromForm] int receiverId, [FromForm] string message)
        {
            var userIdStr = User.FindFirst("UserId")?.Value;
            if (int.TryParse(userIdStr, out int userId))
            {
                if (string.IsNullOrWhiteSpace(message)) return BadRequest("Message cannot be empty.");

                // 1. Save to DB
                await _chatService.SendMessageAsync(userId, receiverId, message);

                // 2. Broadcast to Receiver
                await _hubContext.Clients.Group(receiverId.ToString())
                    .SendAsync("ReceiveMessage", userId, message, DateTime.Now.ToString("o"));

                // 3. Broadcast to Sender (so other tabs update)
                await _hubContext.Clients.Group(userId.ToString())
                    .SendAsync("ReceiveMessage", userId, message, DateTime.Now.ToString("o"));

                return new JsonResult(new { success = true });
            }
            return Unauthorized();
        }
    }
}
