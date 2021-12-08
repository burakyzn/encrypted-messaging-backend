using System.ComponentModel.DataAnnotations;

namespace SecuredChatApp.Core.DTOs
{
    public class GetUserProfileResponse
    {
        public string Nickname { get; set; }
        public string Email { get; set; }

        public GetUserProfileResponse(string nickname, string email)
        {
            Nickname = nickname;
            Email = email;
        }
    }
}
