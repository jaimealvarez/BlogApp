using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BlogApp.Models;

namespace BlogApp.Data
{
    public class DbInitializer
    {
        public static void Initialize(EntityContext context)
        {
            context.Database.EnsureCreated();

            if (context.Users.Any())
            {
                return;
            }

            User user = new User() { Id = 1, UserName = "writer", Name = "Writer", EmailId = "writer@test.com", Password = "abc123", Role = "writer" };
            context.Users.Add(user);
            user = new User() { Id = 2, UserName = "editor", Name = "Editor", EmailId = "editor@test.com", Password = "abc123", Role = "editor" };
            context.Users.Add(user);
            context.SaveChanges();
        }
    }
}
