using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Primitives;

namespace antiCSRFTest.Middleware
{
    public static class antiCSRFMiddlewareHelper
    {
        public static HttpContext antiCSRFMiddlewareHelper(HttpContext httpContext)
        {
            //The most common way to protect against CSRF attacks is by using a token that is returned on a GET request for a form,
            //and must be present for a POST request to complete. The token must be unique to the user (Cannot be shared), and can 
            //either be per “session” or per “request” based.

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
            
            //All POST and GET requests get caught by this middleware...
            //If it is a post request, a cookie is required, and in some circumstances, a matching cookie and hidden form field for either public or secured posts.
            //If it is a get request, a cookie is required for secured get. For public gets, it generates one for you if you don't have one.
            if (Method == "POST")
            {
                if (containsCookie && /*isCookieValidated(pass cookieval)*/)
                {
                    if (isFormSubmit)
                    {
                        if (httpContext.Request.Form.TryGetValue("anti_CSRF", out StringValues anti_CSRF_Token))
                        {
                            if (/*anti_CSRF_Token == cookieVal*/)
                            {

                            }
                            else
                            {
                                //XSRF
                            }
                        }
                        else
                        {
                            //XSRF
                        }
                    }
                    else
                    {
                        //Validated. Not a form submit.
                    }

                }
                else
                {
                    //XSRF
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
            else if (Method == "GET")
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
   
        }
       
        //This function:
        //  a. Creates a new Anti_CSRF Token
        //  b. Updates DB with that new token
        private static HttpContext CreateUpdateAppend(HttpContext httpContext)
        {
            //Creates new token, Updates DB, appends to the response, and returns.
            //Should be async task?
            string newCookie = GenerateAntiCSRFToken();

            httpContext.Response.Cookies.Append(
                "AntiCSRFToken",
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
            //Combination of a completely random number (128 bits) + 28 bits (based 64 encoding containing info that denotes whether it is pre-session or session.)

        }
    }
}
