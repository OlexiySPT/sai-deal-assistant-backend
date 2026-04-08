namespace Sai.DealAssistant.Infrastructure.Repositories;

internal static class SeedSqlResource
{
    private static readonly System.Reflection.Assembly _assembly = typeof(SeedSqlResource).Assembly;
    private static readonly string _namespace = typeof(SeedSqlResource).Namespace!;

    /// <summary>
    /// Reads an embedded SQL file from the Repositories/Sql folder.
    /// </summary>
    /// <param name="fileName">File name without path, e.g. "MultiplyFirms.sql"</param>
    internal static string Load(string fileName)
    {
        var resourceName = $"{_namespace}.Sql.Seed.{fileName}";
        using var stream = _assembly.GetManifestResourceStream(resourceName)
            ?? throw new InvalidOperationException($"Embedded SQL resource '{resourceName}' not found.");
        using var reader = new StreamReader(stream);
        return reader.ReadToEnd();
    }
}
