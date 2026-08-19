using Fastighetsskötsel.Api.Services.Interfaces;

namespace Fastighetsskötsel.Api.Services;

public class SMSNotifyer : ISMSNotifyer
{
    #region Dependencies
    private readonly IConfiguration _configuration;
    #endregion

    #region Constructor
    public SMSNotifyer(IConfiguration configuration)
    {
        _configuration = configuration;
    } 
    #endregion

    public async Task NotifyAsync(string message)
    {
        var apiKey = _configuration["SMSApiKey"];

        if (string.IsNullOrWhiteSpace(apiKey))
        {
            throw new InvalidOperationException("SMS API key could not be retrieved.");
        }

        // Fiktivt SMS-anrop
        await Task.CompletedTask;
    }
}