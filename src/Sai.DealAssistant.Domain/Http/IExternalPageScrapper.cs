namespace Sai.DealAssistant.Domain.Http;

using System.Threading.Tasks;

public interface IExternalPageScrapper
{
    Task<string> ReadPageContent(string url);
}
