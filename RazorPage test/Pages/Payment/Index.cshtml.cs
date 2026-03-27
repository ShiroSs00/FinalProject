using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace RazorPage_test.Pages.Payment;

[Authorize]
public class IndexModel : PageModel
{
    public void OnGet()
    {
    }
}
