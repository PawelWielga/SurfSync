using SurfSync.Browser;
using System.Reflection;

namespace SurfSync.Browsers;

public static class BrowserResolver
{
    public static List<IBrowserService> GetBrowsersInstances()
    {
        var interfaceType = typeof(IBrowserService);
        var assembly = Assembly.GetExecutingAssembly();

        var instances = assembly.GetTypes()
            .Where(t => interfaceType.IsAssignableFrom(t) && !t.IsInterface && !t.IsAbstract)
            .Select(t => (IBrowserService)Activator.CreateInstance(t))
            .ToList();

        return instances;
    }
}