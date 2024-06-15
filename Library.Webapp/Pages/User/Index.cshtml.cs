using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using Library.Application.Infrastructure.Repositories;
using System.Collections.Generic;
using System.Linq;

namespace Library.Webapp.Pages.User
{
    public class IndexModel : PageModel
    {
        private readonly UserRepository _users;

        public IndexModel(UserRepository users)
        {
            _users = users;
        }
        
        public IEnumerable<Application.Model.User> Users => _users.Set
            .Include(u => u.Libraries)
            .OrderBy(u => u.UserType)
            .ThenBy(u => u.Username);
        
        public void OnGet() {}
    }
}
