namespace HttPlaceholder.Application.Configuration
{
    public class SettingsModel
    {
        public AuthenticationSettingsModel Authentication { get; set; }

        public WebSettingsModel Web { get; set; }

        public StorageSettingsModel Storage { get; set; }

        public GuiSettingsModel Gui { get; set; }

        public StubSettingsModel Stub { get; set; }
    }
}
