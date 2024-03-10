using Dotnet.Commands;
using Microsoft.Extensions.Hosting;
using Uno.Extensions;
using XUnit.Runners.Core;

namespace Xunit.Uno.Runner;

public class App : Application
{
    protected Window? MainWindow { get; private set; }
    protected IHost? Host { get; private set; }

    protected override async void OnLaunched(LaunchActivatedEventArgs args)
    {
#if __IOS__ || __ANDROID__
    FeatureConfiguration.Style.ConfigureNativeFrameNavigation();
#endif
        var builder = this.CreateBuilder(args)
            .Configure(host => host
#if DEBUG
                .UseEnvironment(Environments.Development)
#endif
                .ConfigureServices(RegisterServices)
                .UseNavigation(
                    RegisterRoutes,
                    configure: cfg => cfg with { AddressBarUpdateEnabled = true }
                )
            ).UseToolkitNavigation();
        MainWindow = builder.Window;
#if DEBUG
        MainWindow.EnableHotReload();
#endif
        Host = await MainWindow.InitializeNavigationAsync(
            () => Task.FromResult(builder.Build()),
            initialNavigate: async (sp, nav) =>
            {
                await nav.NavigateViewModelAsync<MainViewModel>(this);
            }
        );
    }

    private static void RegisterServices(HostBuilderContext context, IServiceCollection services)
    {
        services.AddSingleton(new Commands().Validated());
        services.AddSingleton<ITestScanner>(new AssemblyTestsScanner(assemblies: typeof(UnitTests).Assembly));
    }
    
    private static void RegisterRoutes(IViewRegistry views, IRouteRegistry routes)
    {
        views.Register(
            new ViewMap<MainPage, MainViewModel>(),
            new ViewMap<CreditsPage>()
        );
    }
}
