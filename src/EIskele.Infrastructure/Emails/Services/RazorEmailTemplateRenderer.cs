using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using RazorLight;

namespace EIskele.Infrastructure.Emails.Services;

public class RazorEmailTemplateRenderer : IEmailTemplateRenderer
{
    private readonly IRazorLightEngine _engine;

    public RazorEmailTemplateRenderer()
    {
        var templatesPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Emails", "Templates");
        
        _engine = new RazorLightEngineBuilder()
            .UseFileSystemProject(templatesPath)
            .UseMemoryCachingProvider()
            .Build();
    }

    public async Task<string> RenderAsync<TModel>(string templateKey, TModel model, CancellationToken cancellationToken = default)
    {
        var templateFile = $"{templateKey}.cshtml";
        
        try
        {
            return await _engine.CompileRenderAsync(templateFile, model);
        }
        catch (Exception ex)
        {
            var inner = ex.InnerException?.Message ?? string.Empty;
            throw new Exception($"E-posta şablonu render edilemedi: {templateKey}. Hata: {ex.Message} | İç Hata: {inner}");
        }
    }
}
