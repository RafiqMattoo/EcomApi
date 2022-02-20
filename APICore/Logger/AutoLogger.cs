using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Microsoft.IO;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace APIAPI
{
    public class RequestProfilerModel
    {
        public DateTimeOffset RequestTime { get; set; }
        public HttpContext Context { get; set; }
        public string Request { get; set; }
        public string Response { get; set; }
        public DateTimeOffset ResponseTime { get; set; }
    }
    public class RequestResponseLoggingMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<RequestResponseLoggingMiddleware> _logger;
        private readonly RecyclableMemoryStreamManager _recyclableMemoryStreamManager;
        private const int ReadChunkBufferLength = 4096;

        public RequestResponseLoggingMiddleware(RequestDelegate next, ILogger<RequestResponseLoggingMiddleware> logger)
        {
            _next = next;
            _logger = logger;
            _recyclableMemoryStreamManager = new RecyclableMemoryStreamManager();
        }

        public async Task Invoke(HttpContext context)
        {
            var requestProfilerModel = new RequestProfilerModel
            {
                RequestTime = new DateTimeOffset(),
                Context = context,
                Request = await FormatRequestAsync(context)
            };

            var originalBody = context.Response.Body;

            using (var newResponseBody = _recyclableMemoryStreamManager.GetStream())
            {
                context.Response.Body = newResponseBody;

                await _next(context);

                newResponseBody.Seek(0, SeekOrigin.Begin);
                await newResponseBody.CopyToAsync(originalBody);

                newResponseBody.Seek(0, SeekOrigin.Begin);
                requestProfilerModel.Response = await FormatResponseAsync(context, newResponseBody);
                requestProfilerModel.ResponseTime = new DateTimeOffset();

                _logger.LogInformation(requestProfilerModel.Request);
                _logger.LogInformation(requestProfilerModel.Response);

                StringBuilder sbRequestHeaders = new StringBuilder();
                string EMINumber = context.Request.Headers["EMINumber"];
                foreach (var item in context.Request.Headers)
                {
                    sbRequestHeaders.AppendLine(item.Key + ": " + item.Value.ToString());
                }

                StringBuilder sbResponseHeaders = new StringBuilder();
                foreach (var item in context.Response.Headers)
                {
                    sbResponseHeaders.AppendLine(item.Key + ": " + item.Value.ToString());
                }

                EMINumber = EMINumber == null || EMINumber == "" ? "general" : EMINumber;
                string filename = "Log_" + EMINumber + "__" + DateTime.Now.ToString("dd-MM-yyyy") + ".log";
                StringBuilder sbLog = new StringBuilder();
                sbLog.AppendLine("Status: " + context.Response.StatusCode + " - Route: " + context.Request.Method);
                sbLog.AppendLine("========================================================================================");
                sbLog.AppendLine("Request Headers:");
                sbLog.AppendLine(sbRequestHeaders.ToString());
                sbLog.AppendLine("========================================================================================");
                sbLog.AppendLine("Request Body:");
                sbLog.AppendLine(requestProfilerModel.Request);
                sbLog.AppendLine("========================================================================================");
                sbLog.AppendLine("Response Headers:");
                sbLog.AppendLine(sbResponseHeaders.ToString());
                sbLog.AppendLine("========================================================================================");

                sbLog.AppendLine("Response Body:");
                sbLog.AppendLine(requestProfilerModel.Response);
                sbLog.AppendLine("========================================================================================");


                var path = Directory.GetCurrentDirectory();

                string filepath = path + "\\Logs\\" + EMINumber + "\\" + filename;
                if (!Directory.CreateDirectory(path + "\\Logs\\" + EMINumber).Exists)
                {
                    Directory.CreateDirectory(path + "\\Logs\\" + EMINumber);
                    File.AppendAllText(filepath, sbLog.ToString());
                }
                else
                {
                    File.AppendAllText(filepath, sbLog.ToString());

                }
            }
        }

        private async Task<string> FormatResponseAsync(HttpContext context, Stream newResponseBody)
        {
            var request = context.Request;
            var response = context.Response;

            return $"Http Response Information: {Environment.NewLine}" +
                    $"Schema: {request.Scheme} {Environment.NewLine}" +
                    $"Host: {request.Host} {Environment.NewLine}" +
                    $"Path: {request.Path} {Environment.NewLine}" +
                    $"QueryString: {request.QueryString} {Environment.NewLine}" +
                    $"Headers: {Environment.NewLine}" + FormatHeaders(response.Headers) +
                    $"StatusCode: {response.StatusCode} {Environment.NewLine}" +
                    $"Response Body: {await ReadStreamInChunksAsync(newResponseBody)}";
        }

        private async Task<string> FormatRequestAsync(HttpContext context)
        {
            var request = context.Request;

            return $"Http Request Information: {Environment.NewLine}" +
                        $"Schema:{request.Scheme} {Environment.NewLine}" +
                        $"Host: {request.Host} {Environment.NewLine}" +
                        $"Path: {request.Path} {Environment.NewLine}" +
                        $"QueryString: {request.QueryString} {Environment.NewLine}" +
                        $"Headers: {Environment.NewLine}" + FormatHeaders(request.Headers) +
                        $"Request Body: {await GetRequestBodyAsync(request)}";
        }

        private string FormatHeaders(IHeaderDictionary headers)
        {
            var stringBuilder = new StringBuilder();

            foreach (var (key, value) in headers)
            {
                stringBuilder.AppendLine($"- {key}: {value}");
            }

            return stringBuilder.ToString();
        }

        public async Task<string> GetRequestBodyAsync(HttpRequest request)
        {
            request.EnableBuffering();
           // request.EnableRewind();
            using (var stream = _recyclableMemoryStreamManager.GetStream())
            {
                await request.Body.CopyToAsync(stream);
                request.Body.Seek(0, SeekOrigin.Begin);
                return await ReadStreamInChunksAsync(stream);
            }
        }

        private static async Task<string> ReadStreamInChunksAsync(Stream stream)
        {
            stream.Seek(0, SeekOrigin.Begin);
            string result;

            using (var stringWriter = new StringWriter())
            using (var streamReader = new StreamReader(stream))
            {
                var readChunk = new char[ReadChunkBufferLength];
                int readChunkLength;
                //do while: is useful for the last iteration in case readChunkLength < chunkLength
                do
                {
                    readChunkLength = await streamReader.ReadBlockAsync(readChunk, 0, ReadChunkBufferLength);
                    await stringWriter.WriteAsync(readChunk, 0, readChunkLength);
                } while (readChunkLength > 0);

                result = stringWriter.ToString();
            }

            return result;
        }
    }
    //public class AutoLogMiddleWare
    //{
    //    private readonly RequestDelegate _next;
    //    private readonly ILogger _logger;

    //    public AutoLogMiddleWare(RequestDelegate next)
    //    {
    //        _next = next;
    //    }

    //    public async Task Invoke(HttpContext context)
    //    {
    //        try
    //        {
    //            string route = context.Request.Path.Value;
    //            string httpStatus = "0";

    //            // Log Request
    //            var originalRequestBody = context.Request.Body;
    //            //originalRequestBody.Seek(0, SeekOrigin.Begin);
    //            string requestBody = await new StreamReader(originalRequestBody).ReadToEndAsync();
    //            //originalRequestBody.Seek(0, SeekOrigin.Begin);
    //            var req = context.Request;
    //            req.EnableBuffering();
    //            // Log Response
    //            string responseBody = string.Empty;
    //            using (var swapStream = new MemoryStream())
    //            {

    //                var originalResponseBody = context.Response.Body;
    //                context.Response.Body = swapStream;
    //                await _next(context);
    //                swapStream.Seek(0, SeekOrigin.Begin);
    //                responseBody = new StreamReader(swapStream).ReadToEnd();
    //                swapStream.Seek(0, SeekOrigin.Begin);
    //                await swapStream.CopyToAsync(originalResponseBody);
    //                context.Response.Body = originalResponseBody;
    //                httpStatus = context.Response.StatusCode.ToString();
    //            }

    //            // Clean route
    //            string cleanRoute = route;
    //            foreach (var c in Path.GetInvalidFileNameChars())
    //            {
    //                cleanRoute = cleanRoute.Replace(c, '-');
    //            }

    //            StringBuilder sbRequestHeaders = new StringBuilder();
    //            string EMINumber = context.Request.Headers["EMINumber"];
    //            foreach (var item in context.Request.Headers)
    //            {
    //                sbRequestHeaders.AppendLine(item.Key + ": " + item.Value.ToString());
    //            }

    //            StringBuilder sbResponseHeaders = new StringBuilder();
    //            foreach (var item in context.Response.Headers)
    //            {
    //                sbResponseHeaders.AppendLine(item.Key + ": " + item.Value.ToString());
    //            }

    //            EMINumber = EMINumber == null || EMINumber == "" ? "general" : EMINumber;
    //            string filename = "Log_" + EMINumber +"__"+ DateTime.Now.ToString("dd-MM-yyyy") + ".log";
    //            StringBuilder sbLog = new StringBuilder();
    //            sbLog.AppendLine("Status: " + httpStatus + " - Route: " + route);
    //            sbLog.AppendLine("========================================================================================");
    //            sbLog.AppendLine("Request Headers:");
    //            sbLog.AppendLine(sbRequestHeaders.ToString());
    //            sbLog.AppendLine("========================================================================================");
    //            sbLog.AppendLine("Request Body:");
    //            sbLog.AppendLine(requestBody);
    //            sbLog.AppendLine("========================================================================================");
    //            sbLog.AppendLine("Response Headers:");
    //            sbLog.AppendLine(sbResponseHeaders.ToString());
    //            sbLog.AppendLine("========================================================================================");

    //            sbLog.AppendLine("Response Body:");
    //            sbLog.AppendLine(responseBody);
    //            sbLog.AppendLine("========================================================================================");


    //            var path = Directory.GetCurrentDirectory();

    //            string filepath = path + "\\Logs\\" + EMINumber + "\\" + filename;
    //            if (!Directory.CreateDirectory(path + "\\Logs\\" + EMINumber).Exists)
    //            {
    //                Directory.CreateDirectory(path + "\\Logs\\" + EMINumber);
    //                File.AppendAllText(filepath, sbLog.ToString());
    //            }
    //            else
    //            {
    //                File.AppendAllText(filepath, sbLog.ToString());

    //            }

    //          // await _next.Invoke(context);

    //        }
    //        catch (Exception ex)
    //        {
    //            // It cannot cause errors no matter what
    //        }

    //            finally
    //        {

    //            }


    //    }
    //}

    //public class EnableRequestRewindMiddleware
    //{
    //    private readonly RequestDelegate _next;

    //    public EnableRequestRewindMiddleware(RequestDelegate next)
    //    {
    //        _next = next;
    //    }

    //    public async Task Invoke(HttpContext context)
    //    {
    //        //context.Request.EnableRewind();
    //        await _next(context);
    //    }
    //}

    //public static class EnableRequestRewindExtension
    //{
    //    public static IApplicationBuilder UseEnableRequestRewind(this IApplicationBuilder builder)
    //    {
    //        return builder.UseMiddleware<EnableRequestRewindMiddleware>();
    //    }
    //}
}

