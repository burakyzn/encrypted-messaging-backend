using System;

namespace SecuredChatApp.Core.Interfaces.Entities
{
    public interface IEntity<T>
    {
        T Id { get; set; }
        bool IsActive { get; set; }
        DateTime Created { get; set; }
        string Creator { get; set; }
    }
}