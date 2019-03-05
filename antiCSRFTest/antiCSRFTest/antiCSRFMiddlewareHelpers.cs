using Microsoft.AspNetCore.Http;

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
                //return flag on validation status.
                if (/*DataAdapterCall(Query), pass cookieVal, checks session data*/)
                {
                    //Then their cookie exists in session data group...
                    return true;
                }
                return false;   
            }
            /*Data Adapter Call, check against pre-session, then session cookies if no result for pre-session*/
            if (/*DataAdapterCall(Query), pass cookieVal, checks pre-session data*/)
            {

                //Then their cookie exists in pre-session data group...
                return true;
            }
            if (/*DataAdapterCall(Query), pass cookieVal, checks session data*/)
            {
                //Then their cookie exists in session data group...
                return true;
            }
            return false;
        }

        //This function:
        //  a. Creates a new Anti_CSRF Token
        //  b. Updates DB (presession) with that new token
        //  c. Returns null upon failure.
        public static HttpContext CreateUpdateAppendCookie(HttpContext httpContext)
        {
            
            //Create
            string newCookie = GenerateAntiCSRFToken();

            //Update
            if (/*DataAdapterCall(Update)*/)
            {
                //Append
                httpContext.Response.Cookies.Append(
                "anti_CSRF",
                newCookie,
                new CookieOptions()
                {
                    Secure = true,
                    HttpOnly = true,
                });

                return httpContext;
            }
            else
            {
                return null;
            }
        }

        private static string GenerateAntiCSRFToken()
        {
            //OWASP Anti-CSRF Cheatsheet says: "Alternative generation algorithms include the use of 256-bit BASE64 encoded hashes."
            //Test string. Would normally be generated here, using randomness, SHA-256 and base64 encoding before returned.
            return @"ZjAwZGExNjQ2NGZhNjkzZDhhOWQ2MzRlNjgzOTJiMjNjYjE0YmQ1MTQzYmQ1NzQ3M2EyMjgwYzJhNDg4MzAxZQ==";
        }
    }
}
