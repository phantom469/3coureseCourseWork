using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Forms;

namespace CourseWork
{
    static class Program
    {
        /// <summary>
        /// Главная точка входа для приложения.
        /// </summary>
        [STAThread]
        static void Main()
        {
            CultureInfo culture = new CultureInfo("ru-RU", true);
            culture.NumberFormat.CurrencySymbol = "";
            System.Threading.Thread.CurrentThread.CurrentCulture = culture;
            DevExpress.Utils.FormatInfo.AlwaysUseThreadFormat = true;

            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new NavigationForm());
        }
    }
}
