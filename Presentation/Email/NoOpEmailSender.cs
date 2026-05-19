using Microsoft.AspNetCore.Identity;
using StudentConferenceApp.DAL.Entities;

namespace Presentation.Email
{
    // Add this class at the bottom of Program.cs or in a new file
    using Microsoft.AspNetCore.Identity;

    // 1. Define this class at the bottom of Program.cs
    public class MyEmailSender : IEmailSender<ApplicationUser>
    {
        public Task SendConfirmationLinkAsync(ApplicationUser user, string email, string confirmationLink) => Task.CompletedTask;
        public Task SendPasswordResetLinkAsync(ApplicationUser user, string email, string resetLink) => Task.CompletedTask;
        public Task SendPasswordResetCodeAsync(ApplicationUser user, string email, string resetCode) => Task.CompletedTask;
    }

    // 2. Register it in your builder.Services section
   


  
    

}
