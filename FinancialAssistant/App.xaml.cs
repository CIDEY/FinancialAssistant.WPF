﻿using FinancialAssistant.Models;
using System.Configuration;
using System.Data;
using System.Windows;

namespace FinancialAssistant
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        public static FinancialAssistantContext DB = new FinancialAssistantContext();
    }

}
