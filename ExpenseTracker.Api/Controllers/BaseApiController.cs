using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace ExpenseTracker.Api.Controllers;

public abstract class BaseApiController : ControllerBase
{
    private int? _currentUserId;
    protected int CurrentUserId
    {
        get
        {
            _currentUserId ??= int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);
            return _currentUserId.Value;
        }
    }
}