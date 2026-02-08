using System;

namespace Sai.DealAssistant.Domain.Exceptions
{
    public class ColumnNotExistsException : Exception
    {
        public ColumnNotExistsException(string columnName)
            : base($"Column '{columnName}' does not exist.") { }
    }
}