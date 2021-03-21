using Microsoft.AspNetCore.Authorization;

namespace SimpleLogin.LoginFunctionality.Requirement {
public class VikingRequirement : IAuthorizationRequirement {
    public readonly string Viking = "Viking";

    public VikingRequirement()
    {
    }
}
}