using Sai.DealAssistant.Domain.Http;

using Microsoft.Playwright;

namespace Sai.DealAssistant.Infrastructure.Http;

public class ExternalPageScrapper : IExternalPageScrapper
{
    private readonly IBrowser _browser;

    public ExternalPageScrapper(IBrowser browser)
    {
        _browser = browser;
    }

    public async Task<string> ReadPageContent(string url)
    {
        var page = await _browser.NewPageAsync();
        await page.GotoAsync(url);
        var innerText = await page.InnerTextAsync("body");
        await page.CloseAsync();
        return innerText;
    }
}
