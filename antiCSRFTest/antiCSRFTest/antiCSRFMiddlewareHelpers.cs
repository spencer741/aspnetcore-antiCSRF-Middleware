using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Primitives;

namespace AntiCSRFTest.Middleware
{
    public static class AntiCSRFMiddlewareHelpers
    {
        //This function:
        //  a. Checks whether cookie is validated based on the requester requesting a secured or public resource.
        //  b. If requesting secured, can only be validated against session values in db.
        //  c. If requesting public, can be validated against pre-session or session values in db.
        public static bool isCookieValidated(string cookieVal, bool isRequestingSecuredResource)
        {
            if (isRequestingSecuredResource)
            {
                //OLTP_Query, check against only session cookies
                //return flag on validation status.
            }
         
            //OLTP_Query, check against pre-session and session cookie
            //return flad on validation status.
        }
       
        //This function:
        //  a. Creates a new Anti_CSRF Token
        //  b. Updates DB with that new token
        public static HttpContext CreateUpdateAppendCookie(HttpContext httpContext)
        {
            //Creates new token, Updates DB, appends to the response, and returns.
            //TODO: make sure proper flags are set for cookie mitigation.
            //Do I have to delete old one? is it automatically appended to httpContext response?
            string newCookie = GenerateAntiCSRFToken();

            httpContext.Response.Cookies.Append(
                "AntiCSRFToken", //This needs to be anti_CSRF
                newCookie,
                new CookieOptions()
                {
                    Secure = true,
                    HttpOnly = true,

                }
            );
        

        }

        private static string GenerateAntiCSRFToken()
        {
            //Combination of a completely random number (128 bits + 28?)
       
        }
    }
}
