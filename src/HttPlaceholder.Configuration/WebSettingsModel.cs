// ReSharper disable UnusedAutoPropertyAccessor.Global
namespace HttPlaceholder.Configuration
{
    public class WebSettingsModel
    {
        public string HttpsPort { get; set; }

        public string HttpPort { get; set; }

        public bool UseHttps { get; set; }

        public string PfxPath { get; set; }

        public string PfxPassword { get; set; }
    }
}
