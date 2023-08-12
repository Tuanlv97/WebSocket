using System.Reflection;

namespace WebSocketService.Middlewares
{
    public static class SocketManagerExtensions
    {
        public static IServiceCollection AddWebSocketManager(this IServiceCollection services)
        {
            services.AddTransient<SocketConnectionManager>();

            foreach (var type in Assembly.GetEntryAssembly().ExportedTypes)
            {
                if (type.GetTypeInfo().BaseType == typeof(SocketManagerHandler))
                {
                    services.AddSingleton(type);
                }
            }

            return services;
        }

        public static IApplicationBuilder MapWebSocketManager(this IApplicationBuilder app,
                                                              PathString path,
                                                              SocketManagerHandler handler)
        {
            return app.Map(path, (_app) => _app.UseMiddleware<SocketManagerMiddleware>(handler));
        }
    }
}
