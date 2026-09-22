using System.Text;

namespace PMSHub.Middlewares;

public sealed class ChamCongRawBodyMiddleware(RequestDelegate next)
{
    public const string RawPayloadItemKey = "ChamCong.RawPayload";

    public async Task InvokeAsync(HttpContext context)
    {
        if (HttpMethods.IsPost(context.Request.Method) &&
            context.Request.Path.Equals("/api/chamcong/post", StringComparison.OrdinalIgnoreCase))
        {
            context.Request.EnableBuffering();

            using var reader = new StreamReader(
                context.Request.Body,
                Encoding.UTF8,
                detectEncodingFromByteOrderMarks: true,
                leaveOpen: true);

            context.Items[RawPayloadItemKey] = await reader.ReadToEndAsync(context.RequestAborted);
            context.Request.Body.Position = 0;
        }

        await next(context);
    }
}
