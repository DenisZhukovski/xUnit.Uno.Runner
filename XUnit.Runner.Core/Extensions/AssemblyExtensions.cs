using System.Reflection;
using Xunit;
using Xunit.Abstractions;

namespace XUnit.Runners.Core;

public static class AssemblyExtensions
{
    public static string CrossPlatformLocation(this Assembly assembly)
    {
#if WINDOWS
	    var location = global::Windows.ApplicationModel.Package.Current.InstalledLocation.Path;
	    var nameWithoutExt = assm.GetName().Name;
	    var assemblyFileName = Path.Combine(location, $"{nameWithoutExt}.dll");
	    if (!File.Exists(assemblyFileName))
	    	throw new InvalidProgramException("Something is wrong");
#elif ANDROID
	    // this is required to exist, but is not used
	    var assemblyFileName = assembly.GetName().Name + ".dll";
	    assemblyFileName = Path.Combine(
            Android.App.Application.Context.CacheDir?.AbsolutePath ?? throw new NullReferenceException(
                "AbsolutePath cannot be calculated for CacheDir on Android platform because CacheDir is null for Android.App.Application.Context"
            ),
            assemblyFileName
        );
        if (!File.Exists(assemblyFileName))
        {
            File.Create(assemblyFileName).Close();
        }
#else
        var assemblyFileName = assembly.Location;
#endif
        return assemblyFileName;
    }
    
    public static TestAssemblyConfiguration TestConfiguration(this Assembly assembly)
    {
        var stream = TestConfigurationStream(assembly.CrossPlatformLocation());
        if (stream != null)
        {
            using (stream)
            {
                return ConfigReader.Load(stream);
            }
        }

        return new TestAssemblyConfiguration();
    }
    
    private static Stream? TestConfigurationStream(string assemblyName)
    {
#if __ANDROID__
			var assets = Android.App.Application.Context.Assets;
			var allAssets = assets.List(string.Empty);

			if (allAssets.Contains($"{assemblyName}.xunit.runner.json"))
				return assets.Open($"{assemblyName}.xunit.runner.json");

			if (allAssets.Contains("xunit.runner.json"))
				return assets.Open("xunit.runner.json");
#else

        // See if there's a directory with the assm name. this might be the case for appx
        if (Directory.Exists(assemblyName))
        {
            if (File.Exists(Path.Combine(assemblyName, $"{assemblyName}.xunit.runner.json")))
            {
                return File.OpenRead(Path.Combine(assemblyName, $"{assemblyName}.xunit.runner.json"));
            }

            if (File.Exists(Path.Combine(assemblyName, "xunit.runner.json")))
            {
                return File.OpenRead(Path.Combine(assemblyName, "xunit.runner.json"));
            }
        }

        // Fallback to working dir

        // look for a file called assemblyName.xunit.runner.json first 
        if (File.Exists($"{assemblyName}.xunit.runner.json"))
        {
            return File.OpenRead($"{assemblyName}.xunit.runner.json");
        }

        if (File.Exists("xunit.runner.json"))
        {
            return File.OpenRead("xunit.runner.json");
        }
#endif
        return null;
    }
    
    public static Task<List<ITestCase>> TestCases(this Assembly assembly, CancellationToken token)
    {
        return Task.Factory.StartNew(
            () =>
            {
                using var discovery = new TestDiscoverySink(() => token.IsCancellationRequested);
                using var framework = new XunitFrontController(
                    AppDomainSupport.Denied,
                    assembly.CrossPlatformLocation(),
                    null,
                    false
                );
                framework.Find(
                    false,
                    discovery,
                    TestFrameworkOptions.ForDiscovery(assembly.TestConfiguration())
                );
                discovery.Finished.WaitOne();
                return discovery.TestCases;
            },
            token,
            TaskCreationOptions.LongRunning,
            TaskScheduler.Default
        );
    }
}
