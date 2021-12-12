using HttPlaceholder.Application.Configuration;
using Microsoft.Extensions.Options;

namespace HttPlaceholder.TestUtilities.Options;

public static class MockSettingsFactory
{
    public static IOptions<SettingsModel> GetSettings()
    {
        var settings = new SettingsModel
        {
            Authentication = new AuthenticationSettingsModel(),
            Storage = new StorageSettingsModel(),
            Web = new WebSettingsModel(),
            Gui = new GuiSettingsModel(),
            Stub = new StubSettingsModel()
        };
        return Microsoft.Extensions.Options.Options.Create(settings);
    }
}