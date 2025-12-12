using System;
using System.Collections.ObjectModel;

namespace Sai.DealAssistant.Domain.Entities.ReadOnly;
//Seeded data entity
public class EventType : BaseReadOnlyEntity

{
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
}
