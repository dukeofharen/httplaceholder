namespace HttPlaceholder.Application.Configuration;

public class ConfigMetadataModel
{
    public string Key { get; set; }

    public string Description { get; set; }

    public string Example { get; set; }

    public string Path { get; set; }

    public bool? IsBoolValue { get; set; }
}