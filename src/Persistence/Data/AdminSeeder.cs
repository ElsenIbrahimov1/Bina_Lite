using Application.Options;
using Domain.Constants;
using Domain.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Text;

namespace Persistence.Data;

public static class AdminSeeder
{
    public static async Task SeedAsync(UserManager<AppUser> userManager, IOptions<SeedOptions> seedOptions)
    {
        var opt = seedOptions.Value;

        if (string.IsNullOrWhiteSpace(opt.AdminEmail) ||
            string.IsNullOrWhiteSpace(opt.AdminPassword))
            return;

        var existing = await userManager.FindByEmailAsync(opt.AdminEmail);
        if (existing is not null)
            return;

        var admin = new AppUser
        {
            UserName = opt.AdminEmail,
            Email = opt.AdminEmail,
            FullName = opt.AdminFullName,
            EmailConfirmed = true
        };

        var result = await userManager.CreateAsync(admin, opt.AdminPassword);
        if (!result.Succeeded)
            return;

        await userManager.AddToRoleAsync(admin, RoleNames.Admin);
    }
}