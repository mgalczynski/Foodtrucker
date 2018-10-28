using System.Runtime.CompilerServices;

#if DEBUG
[assembly: InternalsVisibleTo("Persistency.Test")]
[assembly: InternalsVisibleTo("WebApplication.Test")]
[assembly: InternalsVisibleTo("DynamicProxyGenAssembly2")]
#endif