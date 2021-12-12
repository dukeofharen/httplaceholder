using System;

namespace HttPlaceholder.Client.Configuration;

public class HttPlaceholderClientConfiguration
{
    private string _rootUrl;

    public string RootUrl
    {
        get => _rootUrl;
        set
        {
            if (!string.IsNullOrWhiteSpace(value))
            {
                _rootUrl = value.EndsWith("/") ? value : $"{value}/";
            }
        }
    }

    public string Username { get; set; }

    public string Password { get; set; }

    public void Validate()
    {
        if (string.IsNullOrWhiteSpace(RootUrl))
        {
            throw new ArgumentException(
                $"No value set for {nameof(RootUrl)} in HttPlaceholder configuration.");
        }
    }
}