namespace HttPlaceholder.Configuration
{
    public class SettingsModel
    {
        public AuthenticationSettingsModel Authentication { get; set; }

        public WebSettingsModel Web { get; set; }

        public StorageSettingsModel Storage { get; set; }

        // ReSharper disable once UnusedAutoPropertyAccessor.Global
        public GuiSettingsModel Gui { get; set; }
    }
}
