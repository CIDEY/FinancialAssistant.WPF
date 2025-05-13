using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using FinancialAssistant.Models;
using FinancialAssistant.Services;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows;
using FinancialAssistant.CustomView;

namespace FinancialAssistant.ViewModels
{
    public partial class GoalsViewModel : ObservableObject
    {
        private readonly DBService _dbService;
        private readonly long _userId;

        [ObservableProperty]
        private ObservableCollection<Goal> _goals = new();

        [ObservableProperty]
        private Goal? _selectedGoal;

        public GoalsViewModel(long userId)
        {
            _userId = userId;
            _dbService = new DBService();
            LoadDataCommand.Execute(null);
        }

        [RelayCommand]
        private async Task LoadData()
        {
            try
            {
                var goals = await _dbService.GetGoalsAsync(_userId);
                var summary = await _dbService.GetFinancialSummary(_userId, DateTime.Today);
                Goals = new ObservableCollection<Goal>(goals);

                for (int i = 0; i < Goals.Count; i++)
                    Goals[i].CurrentProgress = (double)summary.TotalBalance;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка загрузки целей: {ex.Message}");
            }
        }

        public async Task UpdateData()
        {
            try
            {
                var goals = await _dbService.GetGoalsAsync(_userId);
                var summary = await _dbService.GetFinancialSummary(_userId, DateTime.Today);
                Goals = new ObservableCollection<Goal>(goals);

                for (int i = 0; i < Goals.Count; i++)
                    Goals[i].CurrentProgress = (double)summary.TotalBalance;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка загрузки целей: {ex.Message}");
            }
        }

        [RelayCommand]
        private void OpenAddGoalPopup()
        {
            var newGoal = new Goal
            {
                UserId = _userId,
                Deadline = DateTime.Today.AddMonths(1)
            };

            var dialog = new AddGoalDialog();
            dialog.DataContext = new AddGoalDialogViewModel(this, _userId);
            

            GoalPopupContent = dialog;
            IsGoalPopupOpen = true;
            //SelectedGoal = newGoal;
            //IsGoalPopupOpen = true;
            //GoalPopupContent = dialog;
        }

        [RelayCommand]
        private async Task DeleteGoal(Goal goal)
        {
            if (MessageBox.Show("Удалить эту цель?", "Подтверждение",
                MessageBoxButton.YesNo) != MessageBoxResult.Yes) return;

            try
            {
                await _dbService.DeleteGoalAsync(goal.Id);
                Goals.Remove(goal);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка удаления: {ex.Message}");
            }
        }

        [ObservableProperty]
        private bool _isGoalPopupOpen;

        [ObservableProperty]
        private UserControl? _goalPopupContent;
    }
}
