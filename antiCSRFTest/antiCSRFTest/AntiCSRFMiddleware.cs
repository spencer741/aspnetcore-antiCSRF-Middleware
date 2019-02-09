using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Primitives;
using System.Threading.Tasks;

namespace AntiCSRFTest.Middleware
{

    /* General:
     * 
     * All POST and GET requests get caught by this middleware...
     * 
     * Some information:
     * The most common way to protect against CSRF attacks is by using a token that
     * is returned on a GET request for a form, and must be present for a POST request to complete. 
     * The token must be unique to the user (Cannot be shared), and can either be per “session” or per “request” based.
     *
     * Note:
     * We are not in the business of upgrading the status of pre-session cookies to session cookies.
     * pre-session and session cookies are created as such and destroyed as such. 
     * This has the possibility to decrease our attack surface in what I call a hypothetical
     * "cookie upgrade bypass attack."
     * 
     * Note:
     * !String.IsNullOrEmpty(Method) according to John Sharp
    */

    /* Logic Overview:
     * 
     * If it is a post request, a cookie is required, and in some 
     * circumstances, a matching cookie and hidden form field for
     * either public or secured posts.
     * 
     * If it is a get request, a cookie is required for secured get.
     * For public gets, it generates one for you if you don't have one.
     * 
     * if secured, only check cookie against the session values db.
     * if not secured, check both pre-session and session in db.
     * Reason for this is to allow public resources to be posted
     * by private members. This is not mutally exclusive, while secured is.
     * still has to be in double submit cookie pattern for public post.
     * Note: 
     *  This is handled in the isCookieValidated call, noted in code below.
     * Note:
     *  Every public request can be gotten using a pre-session or session cookie.
     *  Every secured request can only be gotten with a session cookie.
    */

    public class AntiCSRFMiddleware
    {
        private readonly RequestDelegate _next;

        public AntiCSRFMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public Task Invoke(HttpContext httpContext)
        {
            
            string Method = httpContext.Request.Method;
            bool isRequestingSecuredResource = httpContext.Request.Path.ToString().Contains("/Secured");
            bool containsCookie = (httpContext.Request.Cookies["anti_CSRF"].ToString() != null);
            if (containsCookie)
            {
                //Get cookie val here.
                string cookieVal = httpContext.Request.Cookies["anti_CSRF"];
            }
            bool isFormSubmit = httpContext.Request.HasFormContentType;

            
            bool isValidated = false; //To implement DRY when returning.
            if (Method.Equals("POST") && containsCookie /*&& isCookieValidated(cookieval, isSecuredResource ***resource handling here!!!***) */ )
            {
                if (isFormSubmit) //double submit cookie pattern.
                {
                    if (httpContext.Request.Form.TryGetValue("anti_CSRF", out StringValues anti_CSRF_Token))
                    {
                        if (/*anti_CSRF_Token == cookieVal*/)
                        {
                            //Validated
                            isValidated = true; //executes next middleware
                        } 
                    } 
                }
                else
                {
                    //Validated. Not a form submit.
                    isValidated = true;
                }

                //validation check
                if (isValidated)
                {
                    return _next(httpContext);
                }
                else
                {
                    //CSRF Detected!
                    httpContext.Response.StatusCode = 401;
                    return Task.CompletedTask;
                }

                

            }
            else if (Method.Equals("GET") && containsCookie /*&& isCookieValidated(cookieval, isSecuredResource ***resource handling here!!!***) */ )
            {
                //GET Logic
            }
            else
            {
                //Other methods? Just decline until supported by app?
            }

            //return Task.CompletedTask; //NOPE! all return values handled within conditionals

        }

    }

    // Extension method used to add the middleware to the HTTP request pipeline.
    public static class AntiCSRFMiddlewareExtensions
    {
        public static IApplicationBuilder UseMiddleware(this IApplicationBuilder builder)
        {
            return builder.UseMiddleware<AntiCSRFMiddleware>();
        }
    }
}
