using FinancialAssistant.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FinancialAssistant.Services
{
    public class DBService
    {
        private readonly FinancialAssistantContext _context;

        public DBService()
        {
            _context = App.DB;
        }

        public async Task<long?> GetRoleIdByName(string roleName)
        {
            var role = await _context.Roles.FirstOrDefaultAsync(r => r.Name == roleName);
            return role?.Id; // Возвращаем ID роли или null, если роль не найдена
        }

        public async Task CreateUser(string login, string email, string password)
        {
            var user = new User
            {
                Login = login,
                Email = email,
                Registrationdate = DateTime.Now,
                Passwordhash = password // Предполагается, что у вас есть метод для хеширования пароля
            };

            // Получаем ID роли
            var roleId = await GetRoleIdByName("user");
            if (roleId.HasValue)
            {
                // Получаем существующую роль из контекста
                var role = await _context.Roles.FindAsync(roleId.Value);
                if (role != null)
                {
                    user.Roles.Add(role); // Добавляем существующую роль к пользователю
                }
            }

            _context.Users.Add(user);

            await _context.SaveChangesAsync();
        }

        public async Task<bool> IsLoginTaken(string login)
        {
            return await _context.Users.AnyAsync(u => u.Login == login);
        }

        public async Task<bool> IsEmailTaken(string email)
        {
            return await _context.Users.AnyAsync(u => u.Email == email);
        }

        public async Task<bool> AuthValidation(string login, string password)
        {
            // Получаем пользователя по логину
            var user = await _context.Users
                .FirstOrDefaultAsync(u => u.Login == login);

            if (user == null)
                return false; // Пользователь не найден

            if (user.Passwordhash != password)
                return false;

            // Проверяем пароль
            return true;
        }

    }
}
