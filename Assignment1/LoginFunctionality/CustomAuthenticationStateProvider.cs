using System;
using System.Collections.Generic;
using System.Security.Claims;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.JSInterop;

public class CustomAuthenticationStateProvider : AuthenticationStateProvider {
    private List<User> users;
    private readonly IJSRuntime _jSRuntime;

    public CustomAuthenticationStateProvider(IJSRuntime jSRuntime) {
        _jSRuntime = jSRuntime;

    
        users = new List<User>();
        users.Add(new User {
            Username = "admin",
            Password = "fenris123",
            Roles = new[] {"admin"}
        });
    }
    
    public override async Task<AuthenticationState> GetAuthenticationStateAsync() {
        Console.WriteLine("Retrieving user info");
        

        var identity = new ClaimsIdentity();
        
        var serialisedData = await _jSRuntime.InvokeAsync<string>("sessionStorage.getItem", "currentUser");
        if (serialisedData != null) {
            User user = JsonSerializer.Deserialize<User>(serialisedData, options);

            if (user != null) {
                identity = SetupClaimsForUser(user);
            }
        }

        var claimsPrincipal = new ClaimsPrincipal(identity);
        return await Task.FromResult(new AuthenticationState(claimsPrincipal));
    }

    private ClaimsIdentity SetupClaimsForUser(User user) {
        ClaimsIdentity identity;
        List<Claim> claims = new List<Claim>();
        claims.Add(new Claim(ClaimTypes.Name, user.Username));
        claims.Add(new Claim(ClaimTypes.Role, user.Username));

        foreach (string role in user.Roles) {
            claims.Add(new Claim("Role", role));
        }

        identity = new ClaimsIdentity(claims, "apiauth_type");
        return identity;
    }
    
    public void ValidateLogin(string username, string password) {
        Console.WriteLine("Validating log in");

        if (string.IsNullOrEmpty(username)) throw new Exception("Enter username");

        if (string.IsNullOrEmpty(password)) throw new Exception("Enter password");

        try {
            var user = users.Find(u => u.Username.Equals(username) && u.Password.Equals(password));
            if (user != null) {
                Console.WriteLine("Found user");
            }

            var identity = new ClaimsIdentity();
            if (user != null) {
                identity = SetupClaimsForUser(user);
            }
            
            string serialisedData = JsonSerializer.Serialize(user, options);

            _jSRuntime.InvokeVoidAsync("sessionStorage.setItem", "currentUser", serialisedData);
            
            NotifyAuthenticationStateChanged(
                Task.FromResult(new AuthenticationState(new ClaimsPrincipal(identity))));
        } catch (Exception e) {
            Console.WriteLine(e);
            NotifyAuthenticationStateChanged(Task.FromResult(new AuthenticationState(new ClaimsPrincipal())));
        }
    }

    private JsonSerializerOptions options =
        new JsonSerializerOptions {
            DictionaryKeyPolicy = JsonNamingPolicy.CamelCase,
            IgnoreNullValues = true,
            IgnoreReadOnlyProperties = true,
            PropertyNameCaseInsensitive = true,
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
            ReadCommentHandling = JsonCommentHandling.Skip,
            WriteIndented = false
        };

    public void Logout() {
        Console.WriteLine("Logging out");

        var user = new ClaimsPrincipal(new ClaimsIdentity());
        
        NotifyAuthenticationStateChanged(Task.FromResult(new AuthenticationState(user)));
    }
}