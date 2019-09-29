# aspnetcore antiCSRF Middleware Boilerplate(ish)
This architecture handles a very specific threat vector and also does not encorporate session/user authentication, which is ultimately a separate vector that would be defended past this middleware in the request pipeline. For example, if the user was logged in and on the homepage of the app, but decided to refresh the page, a get request would be send to the current endpoint. An antiCSRF cookie would then be checked and evaluated if existant, passing the request to the next middleware (ideally session auth) where the session cookie would be checked and accepted appropriately.

Note that this is specifically tailored to prevent CSRF on a very basic layer. The attacker could still theoretically obtain a pre-session token if the attacker knew how pre-session tokens were obtained (which wouldn't take much work). This treads into a threat vector that is still not yet securable. If you would like to pursue research in investigating possible mitigations for this threat vector and others, contact me by opening an issue and we can exchange information.
![Check here for a really rough write up](
        https://github.com/spencer741/aspnetcore-antiCSRF-Middleware/blob/master/otherthreats.txt
      )

![alt text](
        https://github.com/spencer741/aspnetcore-antiCSRF-Middleware/blob/master/g534.png
      )
