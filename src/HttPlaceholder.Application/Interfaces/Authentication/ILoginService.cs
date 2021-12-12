namespace HttPlaceholder.Application.Interfaces.Authentication;

public interface ILoginService
{
    void SetLoginCookie(string username, string password);

    bool CheckLoginCookie();
}