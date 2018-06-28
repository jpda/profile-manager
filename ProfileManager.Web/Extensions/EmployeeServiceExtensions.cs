using Microsoft.Extensions.DependencyInjection;
using ProfileManager.AppService;
using ProfileManager.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;

namespace Microsoft.Extensions.DependencyInjection
{
    public static class EmployeeServiceExtensions
    {
        public static void AddCosmosEmployeeDocumentDatabase(this IServiceCollection services, DocumentProviderOptions options)
        {
            //services.AddSingleton<IDocumentProvider<Employee>>((x) =>
            //{
            //    return new CosmosDocumentProvider<Employee>(options.Endpoint, options.Key, options.Database, options.Collection);
            //});

            //services.AddSingleton<IEmployeeRepository, DocumentEmployeeRepository>();
        }

        public static void AddAzureFaceInfoProvider(this IServiceCollection services, FaceInfoProviderOptions options)
        {
            //services.AddSingleton<IFaceInfoProvider>((x) =>
            //{
            //    return new AzureFaceInfoProvider(options.Endpoint, options.Key);
            //});
        }
    }
}
