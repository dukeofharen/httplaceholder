namespace HttPlaceholder.Application.Interfaces.Http;

public interface IClientDataResolver
{
    string GetClientIp();

    string GetHost();

    bool IsHttps();
}