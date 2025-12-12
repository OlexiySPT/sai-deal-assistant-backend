using Sai.DealAssistant.Domain.Entities.ReadOnly;
using System;
using System.Collections.ObjectModel;

namespace Sai.DealAssistant.Domain.Entities;

public class User : BaseEntity
{
    public string Username { get; set; } = string.Empty;
    public string PasswordHash { get; set; } = string.Empty;
    public string Role { get; set; } = string.Empty;
}
