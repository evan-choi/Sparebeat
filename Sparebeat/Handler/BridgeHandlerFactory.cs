using CefSharp;
using CefSharp.Handler;

namespace Sparebeat.Handler;

public sealed class BridgeHandlerFactory : IResourceRequestHandlerFactory
{
    public bool HasHandlers => true;

    public IResourceRequestHandler GetResourceRequestHandler(IWebBrowser chromiumWebBrowser, IBrowser browser, IFrame frame, IRequest request, bool isNavigation, bool isDownload, string requestInitiator, ref bool disableDefaultHandling)
    {
        return request.Url.StartsWith("https://sparebeat.com/app/")
            ? new BridgeHandler()
            : new ResourceRequestHandler();
    }
}
