using FinancialAssistant.Classes;
using FinancialAssistant.Models;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace FinancialAssistant.Services
{
    public class CurrencyService
    {
        private const string ApiUrl = "https://www.cbr-xml-daily.ru/daily_json.js";
        private readonly FinancialAssistantContext _context;

        public CurrencyService(FinancialAssistantContext context)
        {
            _context = context;
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
                            Rate = currency.Value.Value
                        };

                        // Сохраняем или обновляем курс в базе данных
                        var existingCurrency = await _context.Currencies.FindAsync(currencyModel.Code);
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
                    var rubleCurrency = await _context.Currencies.FindAsync("RUB");
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
    }
}
