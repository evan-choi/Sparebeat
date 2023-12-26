﻿using CefSharp;

namespace Sparebeat.Handler;

internal class AppSchemeHandlerFactory : ISchemeHandlerFactory
{
    public IResourceHandler Create(IBrowser browser, IFrame frame, string schemeName, IRequest request)
    {
        return new AppResourceHandler();
    }
}
