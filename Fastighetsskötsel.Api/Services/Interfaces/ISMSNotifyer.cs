namespace Fastighetsskötsel.Api.Services.Interfaces;

public interface ISMSNotifyer
{
    Task NotifyAsync(string message);
}