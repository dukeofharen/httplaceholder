namespace HttPlaceholder.Client.Configuration
{
    public class HttPlaceholderClientConfiguration
    {
        private string _rootUrl;

        public string RootUrl
        {
            get => _rootUrl;
            set
            {
                if (value.EndsWith("/"))
                {
                    _rootUrl = value;
                }

                _rootUrl = $"{value}/";
            }
        }

        public string Username { get; set; }

        public string Password { get; set; }
    }
}
