using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sai.DealAssistant.Domain.Entities
{
    public class EventTag : BaseEntity
    {
        public int EventId { get; set; }
        public Event Event { get; set; } = default!;
        public string Tag { get; set; } = string.Empty;
    }
}
