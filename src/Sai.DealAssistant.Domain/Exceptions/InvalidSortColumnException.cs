namespace Sai.DealAssistant.Domain.Exceptions;

public class InvalidSortColumnException : Exception
{
	public InvalidSortColumnException(string columnName, IEnumerable<string> keys)
		: base($"Invalid sort column: {columnName} . possible values: {string.Join(", ", keys)}")
	{
		ColumnName = columnName;
	}

	public string ColumnName { get; }
}
