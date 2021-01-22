using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Web.Http;
using System.Web.Http.Cors;
using System.Web.Http.Routing;

namespace WEDEPX_API
{
    public static class WebApiConfig
    {
        public static void Register(HttpConfiguration config)
        {
            // Web API configuration and services

            // Web API routes
            config.EnableCors();
            config.Formatters.JsonFormatter.SupportedMediaTypes.Add(new MediaTypeHeaderValue("application/json"));
            config.Formatters.JsonFormatter.SupportedMediaTypes.Add(new MediaTypeHeaderValue("multipart/form-data"));


            config.MapHttpAttributeRoutes();

            config.Routes.MapHttpRoute(
                name: "DefaultApi",
                routeTemplate: "api/{controller}/{id}",
                defaults: new { id = RouteParameter.Optional }
            );
            config.Routes.MapHttpRoute(
             name: "Gete",
             routeTemplate: "Get",
             defaults: new { controller = "Values", action = "GetValues" }
            );
            config.Routes.MapHttpRoute(
             name: "Insert",
             routeTemplate: "Save",
             defaults: new { controller = "Values", action = "Save" }
            );
            config.Routes.MapHttpRoute(
            name: "Update",
            routeTemplate: "Update",
            defaults: new { controller = "Values", action = "Update" }
           );
            config.Routes.MapHttpRoute(
             name: "Del",
             routeTemplate: "Del/{EMP_CODE}",
             defaults: new { controller = "Values", action = "Delete" },
             constraints: new { httpMethod = new HttpMethodConstraint(HttpMethod.Get) }

            );
            config.Routes.MapHttpRoute(
             name: "Gen",
             routeTemplate: "Gen/{EMP_CODE}",
             defaults: new { controller = "Values", action = "Generate" },
             constraints: new { httpMethod = new HttpMethodConstraint(HttpMethod.Get) }

            );

        }
    }
}
