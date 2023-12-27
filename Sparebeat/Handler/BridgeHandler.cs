using CefSharp;
using CefSharp.Handler;

namespace Sparebeat.Handler;

public class BridgeHandler : ResourceRequestHandler
{
    protected override IResourceHandler GetResourceHandler(IWebBrowser chromiumWebBrowser, IBrowser browser, IFrame frame, IRequest request)
    {
        var resourceStream = AppResources.GetStream("app://bridge.html");
        return ResourceHandler.FromStream(resourceStream, autoDisposeStream: true);
    }
}
