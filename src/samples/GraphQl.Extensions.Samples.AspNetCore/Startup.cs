using System.Text;
using GraphQl.Extensions.Formatters;
using GraphQl.Extensions.Samples.AspNetCore.DAO;
using GraphQl.Extensions.Samples.AspNetCore.Model;
using GraphQL;
using GraphQL.Types;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Formatters;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace GraphQl.Extensions.Samples.AspNetCore
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddMvc(options =>
                {
                    options.RespectBrowserAcceptHeader = true;
                    options.OutputFormatters.Add(new GraphQlCsvFormatter("products", ";", Encoding.UTF8));
                    options.OutputFormatters.Add(new GraphQlXlsxFormatter("products"));

                })
                .SetCompatibilityVersion(CompatibilityVersion.Version_2_2);

            services.AddSingleton<ProductType>();
            services.AddSingleton<ProductsQuery>();

            services.AddSingleton<IDocumentExecuter, DocumentExecuter>();
            services.AddScoped<IProductDao, ProductDao>();

            var serviceProvider = services.BuildServiceProvider();
            services.AddSingleton<ISchema>(
                new ProductsSchema(new FuncDependencyResolver(type => serviceProvider.GetService(type))));
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseMvc();
        }
    }
}
