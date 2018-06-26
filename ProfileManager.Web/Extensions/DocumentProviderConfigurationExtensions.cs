using Microsoft.Extensions.DependencyInjection;
using ProfileManager.AppService;
using ProfileManager.Entities;
using ProfileManager.Web.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Microsoft.Extensions.DependencyInjection
{
    public static class DocumentProviderConfigurationExtensions
    {
        public static void AddDocumentProvider(this IServiceCollection services, DocumentProviderOptions options)
        {

        }
    }

    public static class EmployeeDocumentExtensions
    {
        public static void AddEmployeeDocumentDatabase(this IServiceCollection services, DocumentProviderOptions options)
        {
            services.AddSingleton<IDocumentProvider<Employee>>((x) =>
            {
                return new CosmosDocumentProvider<Employee>(options.Endpoint, options.Key, options.Database, options.Collection);
            });

            services.AddSingleton<IEmployeeRepository, DocumentEmployeeRepository>();
        }
    }
}
