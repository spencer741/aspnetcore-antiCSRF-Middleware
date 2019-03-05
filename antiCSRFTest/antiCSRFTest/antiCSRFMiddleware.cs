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

    /* After this middleware has executed, there is further cookie mitigation needed later on in the pipeline.
     * After the user is authenticated, their anti_CSRF token needs to be upgraded from pre-session to session.
     * If you don't want to upgrade, just create another one and put that in the session data relating to the user.
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
            string cookieVal = null;
            bool isFormSubmit = httpContext.Request.HasFormContentType;

            if (containsCookie)
            {
                //Get cookie val here.
                cookieVal = httpContext.Request.Cookies["anti_CSRF"];
            }
           
            bool isValidated = false; //To implement DRY when returning.

            if (Method.Equals("POST") && containsCookie && AntiCSRFMiddlewareHelpers.isCookieValidated(cookieVal, isRequestingSecuredResource))  //***resource handling occurs in isCookieValidated Call here!!!***
            {
                if (isFormSubmit) //double submit cookie pattern.
                {
                    if (httpContext.Request.Form.TryGetValue("anti_CSRF", out StringValues anti_CSRF_Token))
                    {
                        if (anti_CSRF_Token == cookieVal)
                        {
                            //Validated
                            isValidated = true;
                        } 
                    } 
                }
                else
                {
                    //Validated. Not a form submit.
                    isValidated = true;
                }

                //validation check ; this pattern was taken on for more readable code
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
            else if (Method.Equals("GET"))
            {
                if (containsCookie && AntiCSRFMiddlewareHelpers.isCookieValidated(cookieVal, isRequestingSecuredResource)) //***resource handling occurs in isCookieValidated Call here!!!***
                {
                    //They are authenticated. Return here.
                    return _next(httpContext);
                    
                }
                else //if (!isRequestingSecuredResource) //Meaning it was not validated upon requesting a public resource, we generate a token because this could be a first time;
                {
                    //Generate pre-session cookie, add to pre-session table in db, and update the response with a cookie.
                    HttpContext newContext = AntiCSRFMiddlewareHelpers.CreateUpdateAppendCookie(httpContext);
                    if (newContext != null)
                    {
                        //Successful. They can now continue accessing public resource.
                        httpContext = newContext;
                        return _next(httpContext);
                    }
                    //Failed
                    return Task.CompletedTask;
                }     
            }
            else
            {
                //Other methods? Just decline until supported by app.
                //Incorrect Response type... likely hack
                httpContext.Response.StatusCode = 405; //Method not allowed.
                return Task.CompletedTask;
            }
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
