using System.Threading;
using System.Threading.Tasks;

namespace EIskele.Application.Common.Settings;

public interface ISystemSettingsProvider
{
    Task<string> GetSettingValueAsync(string key, string defaultValue = "", CancellationToken cancellationToken = default);
    Task<T> GetSettingValueAsync<T>(string key, T defaultValue, CancellationToken cancellationToken = default);
    Task<bool> IsFeatureEnabledAsync(string key, bool defaultValue = false, CancellationToken cancellationToken = default);
}
