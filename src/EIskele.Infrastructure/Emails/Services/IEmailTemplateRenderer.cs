using System.Threading;
using System.Threading.Tasks;

namespace EIskele.Infrastructure.Emails.Services;

public interface IEmailTemplateRenderer
{
    Task<string> RenderAsync<TModel>(string templateKey, TModel model, CancellationToken cancellationToken = default);
}
