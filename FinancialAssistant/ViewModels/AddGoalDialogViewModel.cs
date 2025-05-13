using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using FinancialAssistant.Models;
using FinancialAssistant.Services;
using OfficeOpenXml.FormulaParsing.Excel.Functions.Database;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace FinancialAssistant.ViewModels
{
    public partial class AddGoalDialogViewModel : ObservableObject
    {
        private readonly DBService _dbService;

        public GoalsViewModel ViewModelPopup { get; private set; }

        [ObservableProperty]
        private string _goalName;

        [ObservableProperty]
        private string _goalValue;

        [ObservableProperty]
        private DateTime _deadLine = DateTime.Today.AddMonths(1);

        [ObservableProperty]
        private string _descriptionText;

        [ObservableProperty]
        private long _userId;

        public AddGoalDialogViewModel(GoalsViewModel viewModel, long userId)
        {
            ViewModelPopup = viewModel;
            _dbService = new();
            UserId = userId;
        }

        [RelayCommand]
        private void Cancel()
        {
            ViewModelPopup.IsGoalPopupOpen = false;
        }

        private bool ValidateInputs()
        {
            try
            {
                if (string.IsNullOrWhiteSpace(GoalName))
                {
                    MessageBox.Show("Ошибка", "Введите название цели");
                    return false;
                }

                if (Convert.ToDouble(GoalValue) <= 0)
                {
                    MessageBox.Show("Ошибка", "Сумма должна быть больше нуля");
                    return false;
                }

                if (DeadLine < DateTime.Today)
                {
                    MessageBox.Show("Ошибка", "Дата не может быть в прошлом");
                    return false;
                }
            }
            catch (Exception)
            {
                MessageBox.Show("Ошибка", "Что то пошло не так");
                return false;
            }
            

            return true;
        }

        [RelayCommand]
        private async Task Save()
        {
            if (ValidateInputs())
            {
                Goal goal = new()
                {
                    CreatedDate = DateTime.UtcNow,
                    Deadline = DateTime.SpecifyKind(DeadLine, DateTimeKind.Utc),
                    Name = GoalName,
                    Description = DescriptionText,
                    CurrentProgress = 0,
                    UserId = UserId,
                    TargetAmount = Convert.ToDouble(GoalValue)
                };

                await _dbService.AddGoalAsync(goal);

                await ViewModelPopup.UpdateData();
                ViewModelPopup.IsGoalPopupOpen = false;
            }
        }
    }
}
