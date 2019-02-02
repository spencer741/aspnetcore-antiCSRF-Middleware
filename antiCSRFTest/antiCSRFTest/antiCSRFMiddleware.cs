using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Primitives;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AntiCSRFTest.Middleware
{
    public class AntiCSRFMiddleware
    {
        private readonly RequestDelegate _next;

        public AntiCSRFMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public Task Invoke(HttpContext httpContext)
        {

            //The most common way to protect against CSRF attacks is by using a token that is returned on a GET request for a form,
            //and must be present for a POST request to complete. The token must be unique to the user (Cannot be shared), and can 
            //either be per “session” or per “request” based.

            //NOTE: we are not in the business of upgrading the status of pre-session cookies to session cookies.
            //pre-session and session cookies are created as such and destroyed as such.

            string Method = httpContext.Request.Method;

            bool isRequestingSecuredResource = httpContext.Request.Path.ToString().Contains("/Secured");

            bool containsCookie = (httpContext.Request.Cookies["anti_CSRF"].ToString() != null);
            if (containsCookie)
            {
                //Get cookie val here.
                string cookieVal = httpContext.Request.Cookies["anti_CSRF"];
            }

            bool isFormSubmit = httpContext.Request.HasFormContentType; //application/x-www-form-urlencoded

            //if (!string.IsNullOrEmpty(Method)) //I don't really know why I did the first part.


            //TODO: Need to check if secured resource or public resource is being requested to look for pre-session vs session cookie.
            //Every public request can be gotten using a pre-session or session cookie.
            //Every secured request can only be gotten with a session cookie.
            //^^This scope needs to be
            //All POST and GET requests get caught by this middleware...
            //If it is a post request, a cookie is required, and in some circumstances, a matching cookie and hidden form field for either public or secured posts.
            //If it is a get request, a cookie is required for secured get. For public gets, it generates one for you if you don't have one.
            bool isValidated = false;
            if (Method.Equals("POST") && containsCookie /*&& isCookieValidated(cookieval, isSecuredResource ***resource handling here!!!***) */ )
            {
                //if secured, only check cookie in session table
                //if not secured, check both pre-session and session. Reason for this is to allow public resources to be posted by private members. This is not mutally exclusive, while secured is.
                //still has to be in double submit cookie pattern for public post.
                
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
                //If execution makes it here, request has passed initial anti xsrf pass.

                /*//MIGHT NOT NEED THIS FOR POSTS...
                    //check to see if they are accessing a secured or public resource.
                    if (isRequestingSecuredResource) //Secured Post
                    {

                    }
                    else //Public Post
                    {
                        //
                    }
                */

                //doc for above:
                //We want to check to see if the cookie is a valid anti CSRF value

                //If this is a POST request, 
                //We get cookie val.
                //we need to then validate the cookie.
                //if the cookie checks out, we need to check if the content type is
                //application /x-www-form-urlencoded, indicating a form submit, which 
                //should have the same value as the cookie we just validated. This is
                //part of the double submit cookie spec.

                //If so, we want to append a new cookie to response due to this being per-request
                //csRF protection.
                //if not, csrf attack is likely. And no request should get past.
                //Make sure proper flags are set.


                //Gen Note: no string values !String.IsNullOrEmpty(Method) according to book

            }
            else if (Method.Equals("GET"))
            {
                //check to see if they are accessing a secured or public resource.
                //If so,  Otherwise, it is highly likely 
                //an attacker is attempting xsrf.
                if (isRequestingSecuredResource) //Secured get
                {
                    //Secure resource is being requested.
                    //So, Cookie has to be on request.
                    //if so, check the access control table for that cookie.
                    //if cookie checks out, they are a verified requester that originated from us and can continue.
                    //if not, under xsrf attack.
                }
                else //Public get
                {
                    //Check to see if a cookie was transmitted on request.
                    //if so, check the pre-session table for that cookie.
                    //if the cookie checks out, they are a verified requester.
                    //if the cookie doesn't check out, CRSF attack.
                    //if not, Generate new cookie
                    //append to pre-session table
                    //append cookie to request
                }
            }
            else
            {
                //Other methods? Just decline until supported?
            }

            //return Task.CompletedTask;

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
