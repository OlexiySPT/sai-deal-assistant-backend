namespace Sai.DealAssistant.Domain.Http;

using System.Threading.Tasks;

public interface IExternalPageReader
{
    Task<string> ReadPageRaw(string url);
}
