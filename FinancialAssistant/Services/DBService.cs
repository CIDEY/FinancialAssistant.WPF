using FinancialAssistant.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Principal;
using System.Text;
using BCrypt.Net;
using System.Threading.Tasks;
using System.Transactions;

namespace FinancialAssistant.Services
{
    public class DBService : IDisposable
    {
        private readonly FinancialAssistantContext _context;

        public DBService()
        {
            _context = App.DB;
        }

        #region User Management
        public async Task<bool> IsLoginTaken(string login)
        {
            return await _context.Users
                .AnyAsync(u => u.Login == login);
        }

        public async Task<bool> IsEmailTaken(string email)
        {
            return await _context.Users
                .AnyAsync(u => u.Email == email);
        }
        public async Task<long?> CreateUser(UserRegistrationDto dto)
        {
            using var transaction = await _context.Database.BeginTransactionAsync();
            try
            {
                if (await _context.Users.AnyAsync(u => u.Login == dto.Login))
                    throw new ArgumentException("Login already taken");

                if (await _context.Users.AnyAsync(u => u.Email == dto.Email))
                    throw new ArgumentException("Email already taken");

                var user = new User
                {
                    Login = dto.Login,
                    Email = dto.Email,
                    PasswordHash = BCrypt.Net.BCrypt.HashPassword(dto.Password, workFactor: 12),
                    RegistrationDate = DateTime.UtcNow,
                    Accounts = new List<Account>(),
                    UserRoles = new List<UserRole>() // Теперь свойство существует
                };

                // Добавление роли
                var role = await _context.Roles.FirstAsync(r => r.Name == "User");
                user.UserRoles.Add(new UserRole { Role = role });

                await _context.Users.AddAsync(user);
                await _context.SaveChangesAsync();
                await transaction.CommitAsync();

                return user.Id;
            }
            catch
            {
                await transaction.RollbackAsync();
                throw;
            }
        }

        public async Task<User?> Authenticate(string login, string password)
        {
            var user = await _context.Users
                .Include(u => u.UserRoles)
                .ThenInclude(ur => ur.Role)
                .FirstOrDefaultAsync(u => u.Login == login);

            if (user == null || !BCrypt.Net.BCrypt.Verify(password, user.PasswordHash))
                return null;

            return user;
        }
        #endregion

        #region Account Operations
        public async Task<Account> CreateAccount(long userId, AccountCreateDto dto)
        {
            var user = await _context.Users
                .Include(u => u.Accounts)
                .FirstOrDefaultAsync(u => u.Id == userId)
                ?? throw new ArgumentException("User not found");

            var currency = await _context.Currencies
                .FirstOrDefaultAsync(c => c.Code == dto.CurrencyCode)
                ?? throw new ArgumentException("Invalid currency");

            var account = new Account
            {
                Name = dto.Name,
                Balance = 0,
                Currency = currency,
                User = user,
                Type = dto.AccountType,
                CreatedAt = DateTime.UtcNow
            };

            user.Accounts.Add(account);
            await _context.SaveChangesAsync();
            return account;
        }

        public async Task<decimal> GetTotalBalance(long userId)
        {
            return await _context.Accounts
                .Where(a => a.UserId == userId)
                .Include(a => a.Currency) // Добавляем Include для Currency
                .SumAsync(a => a.Balance * a.Currency.Rate);
        }
        #endregion

        #region Transaction Processing
        public async Task ProcessTransaction(TransactionCreateDto dto)
        {
            using var transaction = await _context.Database.BeginTransactionAsync();
            try
            {
                var account = await _context.Accounts
                    .Include(a => a.Currency)
                    .FirstOrDefaultAsync(a => a.Id == dto.AccountId)
                    ?? throw new ArgumentException("Account not found");

                var transactionEntity = new Models.Transaction
                {
                    Amount = dto.Amount,
                    Type = dto.TransactionType, // Убрано приведение типов
                    Description = dto.Description,
                    Date = DateOnly.FromDateTime(DateTime.UtcNow),
                    AccountId = dto.AccountId,
                    CategoryId = dto.CategoryId
                };

                if (dto.TransactionType == TransactionType.Income)
                {
                    account.Balance += dto.Amount;
                }
                else
                {
                    account.Balance -= dto.Amount;
                }

                await _context.Transactions.AddAsync(transactionEntity);
                await _context.SaveChangesAsync();
                await transaction.CommitAsync();
            }
            catch
            {
                await transaction.RollbackAsync();
                throw;
            }
        }
        #endregion

        #region Analytics

        public async Task<FinancialSummary> GetFinancialSummary(long userId, DateTime month)
        {
            var startDate = DateOnly.FromDateTime(month.Date.AddDays(1 - month.Day));
            var endDate = DateOnly.FromDateTime(month.Date.AddMonths(1).AddDays(-1));

            var income = await _context.Transactions
                .Where(t => t.Account.UserId == userId &&
                           t.Type == TransactionType.Income &&
                           t.Date >= startDate &&
                           t.Date <= endDate)
                .SumAsync(t => t.Amount);

            var expenses = await _context.Transactions
                .Where(t => t.Account.UserId == userId &&
                           t.Type == TransactionType.Expense &&
                           t.Date >= startDate &&
                           t.Date <= endDate)
                .SumAsync(t => t.Amount);

            var totalBalance = await _context.Accounts
                .Where(a => a.UserId == userId)
                .SumAsync(a => a.Balance * a.Currency.Rate);

            return new FinancialSummary
            {
                TotalBalance = totalBalance,
                MonthlyIncome = income,
                MonthlyExpense = expenses
            };
        }

        public async Task<long?> AuthValidation(string login, string password)
        {
            try
            {
                var user = await _context.Users
                    .AsNoTracking()
                    .FirstOrDefaultAsync(u => u.Login == login);

                if (user == null) return null;

                return BCrypt.Net.BCrypt.Verify(password, user.PasswordHash)
                    ? user.Id
                    : null;
            }
            catch (Exception ex)
            {
                // Логирование ошибки
                Console.WriteLine($"Auth error: {ex.Message}");
                return null;
            }
        }

        public async Task<List<MonthlyHistory>> GetYearlyHistory(long userId)
        {
            var startDate = DateOnly.FromDateTime(DateTime.Today.AddYears(-1));

            return await _context.Transactions
                .Where(t => t.Account.UserId == userId &&
                           t.Date >= startDate)
                .GroupBy(t => new { t.Date.Year, t.Date.Month })
                .Select(g => new MonthlyHistory
                {
                    Year = g.Key.Year,
                    Month = g.Key.Month,
                    Income = g.Where(t => t.Type == TransactionType.Income)
                             .Sum(t => t.Amount),
                    Expense = g.Where(t => t.Type == TransactionType.Expense)
                              .Sum(t => t.Amount)
                })
                .OrderBy(h => h.Year)
                .ThenBy(h => h.Month)
                .ToListAsync();
        }

        // Для получения за месяц
        public async Task<List<Models.Transaction>> GetTransactionsForMonth(long userId, int year, int month)
        {
            var start = new DateOnly(year, month, 1);
            var end = start.AddMonths(1).AddDays(-1);
            return await GetTransactionsByPeriod(userId, start, end);
        }

        // Для получения за текущий месяц
        public async Task<List<Models.Transaction>> GetCurrentMonthTransactions(long userId)
        {
            var today = DateOnly.FromDateTime(DateTime.UtcNow);
            return await GetTransactionsForMonth(userId, today.Year, today.Month);
        }

        public async Task<List<Models.Transaction>> GetTransactionsByPeriod(long userId, DateTime startDate, DateTime endDate)
        {
            return await GetTransactionsByPeriod(
                userId,
                DateOnly.FromDateTime(startDate),
                DateOnly.FromDateTime(endDate));
        }


        public async Task<FinancialReport> GenerateMonthlyReport(long userId, int year, int month)
        {
            var startDate = new DateOnly(year, month, 1);
            var endDate = startDate.AddMonths(1).AddDays(-1);

            var transactions = await GetTransactionsByPeriod(userId, startDate, endDate);

            return new FinancialReport
            {
                Income = transactions
                    .Where(t => t.Type == TransactionType.Income) // Теперь типы совпадают
                    .Sum(t => t.Amount),
                Expenses = transactions
                    .Where(t => t.Type == TransactionType.Expense) // Типы совпадают
                    .Sum(t => t.Amount),
                Transactions = transactions
            };
        }

        // Базовый метод для получения транзакций
        public async Task<List<Models.Transaction>> GetTransactionsByPeriod(
            long userId,
            DateOnly startDate,
            DateOnly endDate)
        {
            return await _context.Transactions
                .Include(t => t.Account)
                .Include(t => t.Category)
                .Where(t =>
                    t.Account.UserId == userId &&
                    t.Date >= startDate &&
                    t.Date <= endDate)
                .OrderByDescending(t => t.Date)
                .AsNoTracking()
                .ToListAsync();
        }
        #endregion

        #region Helper Types

        public class FinancialSummary
        {
            public decimal TotalBalance { get; set; }
            public decimal MonthlyIncome { get; set; }
            public decimal MonthlyExpense { get; set; }
            public decimal NetProfit => MonthlyIncome - MonthlyExpense;
        }

        public class MonthlyHistory
        {
            public int Year { get; set; }
            public int Month { get; set; }
            public decimal Income { get; set; }
            public decimal Expense { get; set; }
        }

        public class UserRegistrationDto
        {
            public string Login { get; set; }
            public string Email { get; set; }
            public string Password { get; set; }
        }

        public class AccountCreateDto
        {
            public string Name { get; set; }
            public string CurrencyCode { get; set; }
            public string AccountType { get; set; }
        }

        public class TransactionCreateDto
        {
            public decimal Amount { get; set; }
            public TransactionType TransactionType { get; set; } // Используем enum напрямую
            public string Description { get; set; }
            public long AccountId { get; set; }
            public long? CategoryId { get; set; }
        }

        public class FinancialReport
        {
            public decimal Income { get; set; }
            public decimal Expenses { get; set; }
            public decimal NetProfit => Income - Expenses;
            public List<Models.Transaction> Transactions { get; set; }
        }
        #endregion

        public void Dispose()
        {
            _context.Dispose();
        }
    }
}
