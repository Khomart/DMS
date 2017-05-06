using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
namespace ContosoUniversity
{
    public static class Routes
    {
        public static string[] ReturnRoutes(ControllerContext conCon)
        {
            string[] route = new string[3];
            route[0] = conCon.RouteData.Values["action"].ToString();
            route[1] = conCon.RouteData.Values["controller"].ToString();
            route[2] = conCon.RouteData.Values["area"].ToString();

            return route;
        }
    }
}
