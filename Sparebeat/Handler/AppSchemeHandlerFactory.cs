using System.IO;
using System.Net;
using CefSharp;

namespace Sparebeat.Handler;

public sealed class AppSchemeHandlerFactory : ISchemeHandlerFactory
{
    public IResourceHandler Create(IBrowser browser, IFrame frame, string schemeName, IRequest request)
    {
        if (AppResources.TryResolvePath(request.Url, out var resourcePath))
        {
            var mimeType = ResourceHandler.GetMimeType(Path.GetExtension(resourcePath));
            var resourceStream = AppResources.GetStream(resourcePath);

            return ResourceHandler.FromStream(resourceStream, mimeType, true);
        }

        return ResourceHandler.ForErrorMessage("Not found", HttpStatusCode.NotFound);
    }
}
