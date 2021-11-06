namespace SecuredChatApp.Core.Entities
{
    public class FriendEntity : BaseEntity
    {
        public string User { get; set; }
        public string With { get; set; }
        public bool IsRequest { get; set; }
    }
}
