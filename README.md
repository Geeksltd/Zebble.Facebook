[logo]: https://raw.githubusercontent.com/Geeksltd/Zebble.Facebook/master/Shared/Icon.png "Zebble.Facebook"


## Zebble.Facebook

![logo]

A Zebble plugin that allow you to Login with Facebook.


[![NuGet](https://img.shields.io/nuget/v/Zebble.Facebook.svg?label=NuGet)](https://www.nuget.org/packages/Zebble.Facebook/)

> With this plugin you can register with Facebook and allow user to logon with Facebook account in Zebble applications.

<br>


### Setup
* Available on NuGet: [https://www.nuget.org/packages/Zebble.Facebook/](https://www.nuget.org/packages/Zebble.Facebook/)
* Install in your platform client projects.
* Available for iOS, Android and UWP.
<br>


### Api Usage

#### Register with facebook
You should first go to https://developers.facebook.com/apps and register as a facebook developer.<br>
Create a new App for the application.<br>
Get the App ID assigned to you. for example: 124495023432023, and add to `config.xml`
```xml
<Facebook.App.Id>124495023432023</Facebook.App.Id>
```
In facebook Product Setup page, click "Get Started" for "Facebook Login". Ensure only the following are enabled:

- "Client OAuth Login"
- Embedded BRowser OAuth Login
- Register with Facebook button


In the app if you want to read the user's details from facebook to allow simplier registration, you can then read the data using:
```csharp
var user = await Facebook.Register(Facebook.Field.First_name, Facebook.Field.Last_name, Facebook.Field.Email, ...);
if (user != null)
{
      // Successful. Now you can use the data.
}
```
#### Log in with Facebook button
In the app if you want to allow logging in with facebook, then a secure approach is the following:

##### Button code:
```csharp
var accessToken = await Facebook.Login(Facebook.Field.Email);
if (accessToken.HsaValue())
{
      // Successful. Now use the API to generate a login token.
      var sessionToken = await HttpPost<string>("v1/login/facebook/" + accessToken);
      Device.IO.File("SessionToken.txt").WriteAllText(sessionToken);     
}
```
##### API Code:

 ```csharp
[HttpPost, Route("login/facebook/{accessToken}")]
public IHttpActionResult LoginWithFacebook(string accessToken)
{
      var json = ("https://" + "graph.facebook.com/me?fields=email&access_token=" + accessToken).AsUri().Download();
      var email = JsonConvert.DeserializeObject<JObject>(json)?.Property("email")?.Value?.ToString();

      if (email.IsEmpty()) return BadRequest("Invalid email.");
      var member = Database.Find<Member>(m => m.Email == email);

      if (member == null) return BadRequest($"The email '{email}' is not registered in our system. Please register.");

      var result = JwtAuthentication.CreateTicket(member);
      return Ok(result);
}
```

### Methods
| Method       | Return Type  | Parameters                          | Android | iOS | Windows |
| :----------- | :----------- | :-----------                        | :------ | :-- | :------ |
| Register         | Task<User&gt; | fields -> Field[] | x       | x   | x       |
| Login         | Task<string&gt;| fields -> Field[] | x       | x   | x       |
| Login         | Task<string&gt;| requestedPermissions -> string | x       | x   | x       |
| GetUserInfo         | Task<User&gt;| accessToken -> string, fields -> Field[] | x       | x   | x       |