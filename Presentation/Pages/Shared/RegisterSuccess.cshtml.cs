using Microsoft.AspNetCore.Mvc.RazorPages;

namespace StudentConferenceApp.Web.Pages
{
    public class RegisterSuccessModel : PageModel
    {
        public string Name { get; set; } = "Participant";

        public void OnGet(int id)
        {
            // You can load the name later if needed
        }
    }
}