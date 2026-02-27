using System;

namespace Sai.DealAssistant.Domain.Exceptions
{
    public class TableNotExistsException : Exception
    {
        public TableNotExistsException(string tableName)
            : base($"Table '{tableName}' does not exist.") { }
    }
}