﻿using FinancialAssistant.Classes;
using FinancialAssistant.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace FinancialAssistant.WindowsStart
{
    /// <summary>
    /// Логика взаимодействия для StartWindow.xaml
    /// </summary>
    public partial class StartWindow : Window
    {
        public StartWindow()
        {
            InitializeComponent();
            DataContext = new MainViewModel(AppContextSession.CurrentUserId);
        }

        private void Exit_Btn(object sender, RoutedEventArgs e)
        {
            AppContextSession.CurrentUserId = -1;
            MainWindow mainWindow = new MainWindow();
            mainWindow.Show();
            this.Close();
        }
    }
}
