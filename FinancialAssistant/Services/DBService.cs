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
using FinancialAssistant.Classes;
using Newtonsoft.Json;
using System.Net.Http;

namespace FinancialAssistant.Services
{
    public class DBService : IDisposable
    {
        private readonly FinancialAssistantContext _context;
        private const string ApiUrl = "https://www.cbr-xml-daily.ru/daily_json.js";

        public DBService()
        {
            _context = new FinancialAssistantContext();
        }

        public Currency GetCurrencyByCode(string currencyCode)
        {
            return _context.Currencies.FirstOrDefault(c => c.Code == currencyCode);
        }

        public async Task UpdateCurrencyRatesAsync()
        {
            try
            {
                using (var httpClient = new HttpClient())
                {
                    var response = await httpClient.GetStringAsync(ApiUrl);
                    var currencyData = JsonConvert.DeserializeObject<CurrencyApiResponse>(response);

                    // Обновляем базу данных
                    foreach (var currency in currencyData.Valute)
                    {
                        var currencyModel = new Currency
                        {
                            Code = currency.Key,
                            Symbol = currency.Value.CharCode, // Здесь можно использовать CharCode или другое поле для символа
                            Rate = currency.Value.Value / currency.Value.Nominal
                        };

                        // Сохраняем или обновляем курс в базе данных
                        var existingCurrency = await _context.Currencies
                            .FirstOrDefaultAsync(c => c.Code == currencyModel.Code); // Используем код валюты для поиска

                        if (existingCurrency != null)
                        {
                            existingCurrency.Rate = currencyModel.Rate;
                        }
                        else
                        {
                            _context.Currencies.Add(currencyModel);
                        }
                    }

                    // Устанавливаем курс рубля равным 1
                    var rubleCurrency = await _context.Currencies
                        .FirstOrDefaultAsync(c => c.Code == "RUB"); // Используем код валюты для поиска
                    if (rubleCurrency != null)
                    {
                        rubleCurrency.Rate = 1;
                    }

                    await _context.SaveChangesAsync();
                }
            }
            catch (Exception ex)
            {
                // Логируем ошибку и используем последние данные из базы данных
                Console.WriteLine($"Error fetching currency rates: {ex.Message}");
            }
        }


        public async Task<List<Currency>> GetCurrenciesAsync()
        {
            try
            {
                // Используем Entity Framework Core для асинхронного получения всех валют из таблицы Currencies
                return await _context.Currencies.ToListAsync();
            }
            catch (Exception ex)
            {
                // Обработайте ошибку (логирование, выброс исключения и т.д.)
                System.Diagnostics.Debug.WriteLine($"Ошибка при получении валют: {ex.Message}");
                return new List<Currency>(); // Верните пустой список или обработайте ошибку по-другому
            }
        }

        public async Task AddAccount(Account account)
        {
            await _context.Accounts.AddAsync(account);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateAccount(Account account)
        {
            _context.Accounts.Update(account);
            await _context.SaveChangesAsync();

            var existing = await _context.Accounts
                .FirstOrDefaultAsync(a => a.Id == account.Id);

            if (existing != null)
            {
                _context.Entry(existing).CurrentValues.SetValues(account);
                await _context.SaveChangesAsync();
            }
        }

        public async Task DeleteAccount(long accountId)
        {
            var account = await _context.Accounts.FindAsync(accountId);
            if (account != null)
            {
                _context.Accounts.Remove(account);
                await _context.SaveChangesAsync();
            }
        }

        public async Task AddTransactionAsync(Models.Transaction transaction)
        {
            //using (var context = new FinancialAssistantContext())
            //{
            //    context.Transactions.Add(transaction);
            //    await context.SaveChangesAsync();
            //}

            using (var transactionScope = await _context.Database.BeginTransactionAsync())
            {
                try
                {
                    // Add the transaction to the database
                    await _context.Transactions.AddAsync(transaction);

                    // Retrieve the account associated with the transaction
                    var account = await _context.Accounts
                        .FirstOrDefaultAsync(a => a.Id == transaction.AccountId);

                    if (account != null)
                    {
                        // Update the account balance based on the transaction type
                        if (transaction.Type == TransactionType.Income)
                        {
                            account.Balance += transaction.Amount; // Increase balance for income
                        }
                        else if (transaction.Type == TransactionType.Expense)
                        {
                            account.Balance -= transaction.Amount; // Decrease balance for expense
                        }

                        // Save the updated account balance
                        _context.Accounts.Update(account);
                    }

                    // Save all changes to the database
                    await _context.SaveChangesAsync();
                    await transactionScope.CommitAsync();
                }
                catch
                {
                    await transactionScope.RollbackAsync();
                    throw; // Rethrow the exception to handle it further up the call stack
                }
            }
        }

        public async Task<List<Account>> GetAccountsAsync(long userId)
        {
            return await _context.Accounts
                .Include(a => a.Currency) // Загрузка связанных данных о валюте
                .AsNoTracking() // Важно для чтения без отслеживания
                .Where(a => a.UserId == userId)
                .ToListAsync();
        }

        public async Task<Account> GetAccountByIdAsync(long idAccount)
        {
            // Уберите using, так как контекст должен быть общим для всех операций
            return await _context.Accounts
                .FirstOrDefaultAsync(a => a.Id == idAccount);
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

        public async Task<List<MonthlyHistory>> GetYearlyHistory(long userId, DateTime startDate, DateTime endDate)
        {
            var startDateOnly = DateOnly.FromDateTime(startDate);
            var endDateOnly = DateOnly.FromDateTime(endDate);

            var transactions = await _context.Transactions
                .Where(t => t.Account.UserId == userId &&
                            t.Date >= startDateOnly && t.Date <= endDateOnly)
                .ToListAsync();

            // Отладочное сообщение
            Console.WriteLine($"Найдено транзакций: {transactions.Count}");

            return transactions
                .GroupBy(t => new { t.Date.Year, t.Date.Month })
                .Select(g => new MonthlyHistory
                {
                    Year = g.Key.Year,
                    Month = g.Key.Month,
                    Income = g.Where(t => t.Type == TransactionType.Income).Sum(t => t.Amount),
                    Expense = g.Where(t => t.Type == TransactionType.Expense).Sum(t => t.Amount)
                })
                .OrderBy(h => h.Year)
                .ThenBy(h => h.Month)
                .ToList();
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

        public async Task<List<Models.Transaction>> GetTransactionsByAccountIdAsync(long accountId)
        {
            try
            {
                return await _context.Transactions
                    .Where(t => t.AccountId == accountId)
                    .Include(t => t.Category)
                    .ToListAsync();
            }
            catch (Exception ex)
            {
                throw new Exception($"Ошибка получения транзакций: {ex.Message}");
            }
        }

        public async Task<List<Models.Transaction>> GetTransactionsByPeriod(long userId, DateTime startDate, DateTime endDate)
        {
            return await GetTransactionsByPeriod(
                userId,
                DateOnly.FromDateTime(startDate),
                DateOnly.FromDateTime(endDate));
        }

        public List<Models.Transaction> GetTransactionsByPeriod(long userId, long accountId)
        {
            return _context.Transactions
                .Where(t => t.Account.UserId == userId && t.AccountId == accountId)
                .ToList();
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

        public async Task<List<Goal>> GetGoalsAsync(long userId)
        {
            return await _context.Goals
                .Where(g => g.UserId == userId)
                .AsNoTracking()
                .ToListAsync();
        }

        public async Task DeleteGoalAsync(long goalId)
        {
            var goal = await _context.Goals.FindAsync(goalId);
            if (goal != null)
            {
                _context.Goals.Remove(goal);
                await _context.SaveChangesAsync();
            }
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
            _context?.Dispose();
        }
    }
}
