using HttPlaceholder.Application.Configuration;
using Microsoft.Extensions.Options;

namespace HttPlaceholder.TestUtilities.Options;

public static class MockSettingsFactory
{
    public static SettingsModel GetSettings() => new()
    {
        Authentication = new AuthenticationSettingsModel(),
        Storage = new StorageSettingsModel(),
        Web = new WebSettingsModel(),
        Gui = new GuiSettingsModel(),
        Stub = new StubSettingsModel()
    };

    public static IOptions<SettingsModel> GetOptions() => Microsoft.Extensions.Options.Options.Create(GetSettings());
}
