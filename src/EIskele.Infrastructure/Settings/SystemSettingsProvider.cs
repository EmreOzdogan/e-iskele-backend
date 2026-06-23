using System;
using System.ComponentModel;
using System.Threading;
using System.Threading.Tasks;
using EIskele.Application.Common.Settings;
using EIskele.Persistence;
using Microsoft.EntityFrameworkCore;

namespace EIskele.Infrastructure.Settings;

public class SystemSettingsProvider : ISystemSettingsProvider
{
    private readonly EIskeleDbContext _dbContext;

    public SystemSettingsProvider(EIskeleDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<string> GetSettingValueAsync(string key, string defaultValue = "", CancellationToken cancellationToken = default)
    {
        var setting = await _dbContext.SystemSettings
            .AsNoTracking()
            .FirstOrDefaultAsync(s => s.Key == key, cancellationToken);

        return setting?.Value ?? defaultValue;
    }

    public async Task<T> GetSettingValueAsync<T>(string key, T defaultValue, CancellationToken cancellationToken = default)
    {
        var stringValue = await GetSettingValueAsync(key, string.Empty, cancellationToken);

        if (string.IsNullOrEmpty(stringValue))
            return defaultValue;

        try
        {
            var converter = TypeDescriptor.GetConverter(typeof(T));
            if (converter != null && converter.CanConvertFrom(typeof(string)))
            {
                var convertedValue = converter.ConvertFromString(stringValue);
                return convertedValue == null ? defaultValue : (T)convertedValue;
            }

            return (T)Convert.ChangeType(stringValue, typeof(T));
        }
        catch
        {
            return defaultValue;
        }
    }

    public async Task<bool> IsFeatureEnabledAsync(string key, bool defaultValue = false, CancellationToken cancellationToken = default)
    {
        var feature = await _dbContext.FeatureFlags
            .AsNoTracking()
            .FirstOrDefaultAsync(f => f.Key == key, cancellationToken);

        return feature?.IsEnabled ?? defaultValue;
    }
}
