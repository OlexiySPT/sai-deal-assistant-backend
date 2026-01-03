using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sai.DealAssistant.Domain.Entities;

public class DealTag : BaseEntity
{
    public string Tag { get; set; } = string.Empty;
    public int DealId { get; set; }
    public Deal Deal { get; set; } = default!;
}
