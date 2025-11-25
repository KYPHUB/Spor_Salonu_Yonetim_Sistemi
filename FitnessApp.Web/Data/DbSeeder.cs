using Microsoft.AspNetCore.Identity;

namespace FitnessApp.Web.Data;

public static class DbSeeder
{
    public static async Task SeedRolesAndAdminAsync(IServiceProvider service)
    {
        // Kullanıcı yöneticisi ve rol yöneticisi servislerini al
        var userManager = service.GetService<UserManager<AppUser>>();
        var roleManager = service.GetService<RoleManager<IdentityRole>>();

        // Rolleri tanımla
        await roleManager.CreateAsync(new IdentityRole("Admin"));
        await roleManager.CreateAsync(new IdentityRole("Member"));

        // Admin kullanıcısını oluştur
        var adminEmail = "ogrencinumarasi@sakarya.edu.tr";
        var adminUser = await userManager.FindByEmailAsync(adminEmail);

        if (adminUser == null)
        {
            adminUser = new AppUser
            {
                UserName = adminEmail,
                Email = adminEmail,
                FullName = "Admin User",
                EmailConfirmed = true,
                MembershipDate = DateTime.Now
            };

            // Şifre: sau
            // Identity varsayılan şifre kuralları (büyük/küçük harf, rakam, sembol) gerektirebilir.
            // Ancak ödevde "sau" istendiği için Program.cs'de şifre kurallarını gevşetmemiz gerekebilir.
            // Şimdilik oluşturmayı deneyelim, hata alırsak kuralları gevşetiriz.
            var result = await userManager.CreateAsync(adminUser, "sau"); 
            
            if (result.Succeeded)
            {
                await userManager.AddToRoleAsync(adminUser, "Admin");
            }
        }
    }
}
