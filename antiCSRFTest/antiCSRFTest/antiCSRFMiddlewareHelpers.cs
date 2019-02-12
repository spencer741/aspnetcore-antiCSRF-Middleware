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

                /*Data Adapter Call, check against session cookies*/
                //return flag on validation status.
            }

            /*Data Adapter Call, check against pre-session, then session cookies if no result for pre-session*/
            //return flag on validation status.
        }

        //This function:
        //  a. Creates a new Anti_CSRF Token
        //  b. Updates DB with that new token
        public static HttpContext CreateUpdateAppendCookie(HttpContext httpContext)
        {
            //https://stackoverflow.com/questions/12116511/how-to-delete-cookie-from-net
            //Reference accepted answer and comments. Might as well just expire the cookie when it is expired and leave the key-value, because
            //it will be replaced in mitigation anyways.
            
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
            //OWASP Anti-CSRF Cheatsheet they say: "Alternative generation algorithms include the use of 256-bit BASE64 encoded hashes."
        }
    }
}
