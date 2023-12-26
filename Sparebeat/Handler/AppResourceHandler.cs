using CefSharp;
using CefSharp.Callback;
using System;
using System.IO;
using System.Reflection;
using CefResourceHandler = CefSharp.ResourceHandler;

namespace Sparebeat.Handler;

internal class AppResourceHandler : IResourceHandler
{
    private static readonly Assembly _assembly;
    private static readonly string[] _resourceNames;

    private Stream _stream;
    private byte[] _buffer;
    private string _resourcePath;

    static AppResourceHandler()
    {
        _assembly = Assembly.GetExecutingAssembly();
        _resourceNames = _assembly.GetManifestResourceNames();
    }

    private static string GetResourcePath(string url)
    {
        var name = url.Split("://", 2)[1].TrimEnd('/').Replace('/', '.');
        return $"Sparebeat.Resources.App.{name}";
    }

    void IResourceHandler.Cancel()
    {
        _buffer = null;
        _stream = null;
    }

    void IDisposable.Dispose()
    {
        (this as IResourceHandler).Cancel();
    }

    void IResourceHandler.GetResponseHeaders(IResponse response, out long responseLength, out string redirectUrl)
    {
        responseLength = -1;
        redirectUrl = null;

        response.StatusCode = _stream != null ? 200 : 404;
        response.StatusText = "OK";

        if (response.StatusCode == 200)
            response.MimeType = CefResourceHandler.GetMimeType(Path.GetExtension(_resourcePath));

        if (_stream?.CanSeek == true)
            responseLength = _stream.Length;
    }

    bool IResourceHandler.Open(IRequest request, out bool handleRequest, ICallback callback)
    {
        _resourcePath = GetResourcePath(request.Url);

        handleRequest = Array.IndexOf(_resourceNames, _resourcePath) >= 0;

        if (handleRequest)
            _stream = _assembly.GetManifestResourceStream(_resourcePath);

        return handleRequest;
    }

    bool IResourceHandler.Read(Stream dataOut, out int bytesRead, IResourceReadCallback callback)
    {
        bytesRead = 0;

        callback.Dispose();

        if (_stream == null)
            return false;

        if (_buffer == null || _buffer.Length < dataOut.Length)
            _buffer = new byte[dataOut.Length];

        bytesRead = _stream.Read(_buffer, 0, _buffer.Length);

        if (bytesRead == 0)
            return false;

        dataOut.Write(_buffer, 0, bytesRead);

        return bytesRead > 0;
    }

    bool IResourceHandler.Skip(long bytesToSkip, out long bytesSkipped, IResourceSkipCallback callback)
    {
        if (_stream is not { CanSeek: true })
        {
            bytesSkipped = -2;
            return false;
        }

        bytesSkipped = bytesToSkip;
        _stream.Seek(bytesToSkip, SeekOrigin.Current);

        return false;
    }

    bool IResourceHandler.ProcessRequest(IRequest request, ICallback callback)
    {
        return true;

        throw new NotImplementedException("This method was deprecated and is no longer used.");
    }

    bool IResourceHandler.ReadResponse(Stream dataOut, out int bytesRead, ICallback callback)
    {
        throw new NotImplementedException("This method was deprecated and is no longer used.");
    }
}
