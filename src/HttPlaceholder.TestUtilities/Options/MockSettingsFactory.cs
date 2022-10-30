using HttPlaceholder.Application.Configuration;
using Microsoft.Extensions.Options;
using Moq;

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

    public static IOptionsMonitor<SettingsModel> GetOptionsMonitor(SettingsModel settingsModel)
    {
        var mock = new Mock<IOptionsMonitor<SettingsModel>>();
        mock
            .Setup(m => m.CurrentValue)
            .Returns(settingsModel);
        return mock.Object;
    }

    public static IOptionsMonitor<SettingsModel> GetOptionsMonitor() => GetOptionsMonitor(GetSettings());
}
