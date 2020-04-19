// ReSharper disable UnusedAutoPropertyAccessor.Global
namespace HttPlaceholder.Configuration
{
    public class WebSettingsModel
    {
        public int HttpsPort { get; set; }

        public int HttpPort { get; set; }

        public bool UseHttps { get; set; }

        public string PfxPath { get; set; }

        public string PfxPassword { get; set; }
    }
}
