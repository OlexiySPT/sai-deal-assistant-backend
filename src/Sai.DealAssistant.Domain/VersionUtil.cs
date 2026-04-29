using System.Text.RegularExpressions;

namespace Sai.DealAssistant.Domain;

public class VersionUtil
{
    public static bool IsValidVersion(string version)
    {
        return Regex.IsMatch(version, @"^[0-9]+(\.[0-9]+)*$");
    }
    public static int CompareVersion(string a, string b)
    {
        var pa = a.Split('.');
        var pb = b.Split('.');
        var len = Math.Max(pa.Length, pb.Length);
        for (int i = 0; i < len; i++)
        {
            var na = i < pa.Length ? int.Parse(pa[i]) : 0;
            var nb = i < pb.Length ? int.Parse(pb[i]) : 0;
            if (na != nb) return na.CompareTo(nb);
        }

        return 0;
    }
}