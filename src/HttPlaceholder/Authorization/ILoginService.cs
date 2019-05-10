namespace HttPlaceholder.Authorization
{
    public interface ILoginService
    {
        void SetLoginCookie(string username, string password);

        bool CheckLoginCookie();
    }
}
