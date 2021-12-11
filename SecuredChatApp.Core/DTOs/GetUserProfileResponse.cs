using System.ComponentModel.DataAnnotations;

namespace SecuredChatApp.Core.DTOs
{
    public class GetUserProfileResponse
    {
        public string Nickname { get; set; }
        public string Email { get; set; }
        public string AvatarUrl { get; set; }

        public GetUserProfileResponse(string nickname, string email, string avatarUrl)
        {
            Nickname = nickname;
            Email = email;
            AvatarUrl = avatarUrl;
        }
    }
}
