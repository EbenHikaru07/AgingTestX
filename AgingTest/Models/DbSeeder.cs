using AgingTest.Data;
using Microsoft.AspNetCore.Identity;
using System.Collections.Generic;

namespace AgingTest.Models
{
    public class DbSeeder
    {
        public static void Seed(AppDbContext context)
        {
//            var users = new List<UserModel>
//{
//    new UserModel { badge="B1", nama_user="Admin", role="admin" },
//    new UserModel { badge="B2", nama_user="Operator", role="user" }
//};

//            foreach (var u in users)
//            {
//                u.password = hasher.HashPassword(u, "123");
//                u.status = true;
//                u.created_at = DateTime.Now;
//            }

//            context.tb_users.AddRange(users);
            if (!context.tb_users.Any())
            {
                var hasher = new PasswordHasher<UserModel>();

                var user = new UserModel
                {
                    user_badge = "IB0001",
                    username = "Eben",
                    user_role = "engineer",
                    user_status = true,
                    created_at = DateTime.Now
                };

                user.user_password = hasher.HashPassword(user, "Excelitas123");

                context.tb_users.Add(user);
                context.SaveChanges();
            }
        }
    }
}
