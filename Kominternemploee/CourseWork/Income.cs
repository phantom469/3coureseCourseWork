using System;
using System.Data.SqlClient;
using System.Drawing;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Net;
using System.Text.RegularExpressions;


namespace CourseWork
{
    public partial class Income : Form
    {
        private string GetCurseUSD()
        {
            WebClient webClient = new WebClient();
            
            string res = webClient.DownloadString("https://bankibel.by/belarusbank/kursy-valut");
            string rate = Regex.Match(res, @"<td>([0-9]+\.[0-9]+)</td>").Groups[1].Value;
            return rate;
        }
        public int ActivePersonnelNumber { get; private set; }
        public string FullName { get; private set; }
        
        public Income(int activePersNum, string fullName)
        {
            
            InitializeComponent();
            ActivePersonnelNumber = activePersNum;
            FullName = fullName;
            
        }

        private void lblWage_ClientSizeChanged(object sender, EventArgs e) => lblPosition.Location = new Point(343 + lblWage.Width, 127);
        
        private async void GetAvgSalary()
        {
            string query =
                "select " +
                "(avg(AdditionPayments.Cost + [Positions].Wage + (Wage * Calculated.ProcentSeniorityBonus / 100))) as 'Средняя ЗП'" +
                " from Positions " +
                "inner  join Emploee on Positions.PositionId = Emploee.PositionId " +
                "inner join Calculated on Calculated.PersonnelNumber = Emploee.PersonnelNumber " +
                "inner join AdditionPayments on AdditionPayments.AddPaymentsId = Calculated.AddPaymentsId";
            using (SqlConnection connection = new SqlConnection(PersonalCard.ConnectionString))
            {
                await connection.OpenAsync();
                SqlCommand command = new SqlCommand(query, connection);
                SqlDataReader reader = await command.ExecuteReaderAsync();

                while (await reader.ReadAsync())
                {
                    lblAveragePayment.Text = reader["Средняя ЗП"].ToString().Substring(0, reader["Средняя ЗП"].ToString().Length - 4);
                }
            }
        }
        private (double avgSalary, double totalSalary) GetSalary()
        {
            var res = (0D, 0D);
            string query =
                "SELECT " +
                    
                    "(AdditionPayments.Cost + [Positions].Wage + (Wage * Calculated.ProcentSeniorityBonus / 100)) as 'Итого' " +
                    
                "from Positions " +
                    "inner join Emploee on Positions.PositionId = Emploee.PositionId " +
                    "inner join Calculated on Calculated.PersonnelNumber = Emploee.PersonnelNumber " +
                    "inner join AdditionPayments on AdditionPayments.AddPaymentsId = Calculated.AddPaymentsId " +
                "where " +
                    "Emploee.PersonnelNumber = " + ActivePersonnelNumber;

            using (SqlConnection connection = new SqlConnection(PersonalCard.ConnectionString))
            {
                connection.Open();
                SqlCommand command = new SqlCommand(query, connection);
                SqlDataReader reader = command.ExecuteReader();

                while (reader.Read())
                {
                    var total = reader["Итого"].ToString();
                    
                    res.Item2 = Double.Parse(total.Substring(0, total.Length - 4));
                }
            }

            query =
                "select " +
                "(avg(AdditionPayments.Cost + [Positions].Wage + (Wage * Calculated.ProcentSeniorityBonus / 100))) as 'Средняя ЗП'" +
                " from Positions " +
                "inner  join Emploee on Positions.PositionId = Emploee.PositionId " +
                "inner join Calculated on Calculated.PersonnelNumber = Emploee.PersonnelNumber " +
                "inner join AdditionPayments on AdditionPayments.AddPaymentsId = Calculated.AddPaymentsId";

            using (SqlConnection connection = new SqlConnection(PersonalCard.ConnectionString))
            {
                connection.Open();
                SqlCommand command = new SqlCommand(query, connection);
                SqlDataReader reader = command.ExecuteReader();

                while (reader.Read())
                {
                    var item = reader["Средняя ЗП"].ToString().Substring(0, reader["Средняя ЗП"].ToString().Length - 4);
                    
                    res.Item1 = Double.Parse(item);
                    
                }
            }
            return res;
        }
        private async void Income_Load(object sender, EventArgs e)
        {
            #region Основные данные
            lblFullName.Text = FullName;
            string query = 
                "SELECT " +
                    "[Positions].Wage as 'Оклад', " +
                    "Calculated.Prize as 'Премия', " +
                    "Calculated.ProcentSeniorityBonus as '% надбавки за стаж', " +
                    "(Wage * Calculated.ProcentSeniorityBonus / 100) as 'Сумма надбавки', " +
                    "AdditionPayments.Cost as 'Доплаты', " +
                    "(AdditionPayments.Cost + [Positions].Wage + (Wage * Calculated.ProcentSeniorityBonus / 100)) as 'Итого', " +
                    "Positions.PositionTitle as 'Должность' " +
                "from Positions " +
                    "inner join Emploee on Positions.PositionId = Emploee.PositionId " +
                    "inner join Calculated on Calculated.PersonnelNumber = Emploee.PersonnelNumber " +
                    "inner join AdditionPayments on AdditionPayments.AddPaymentsId = Calculated.AddPaymentsId " +
                "where " +
                    "Emploee.PersonnelNumber = " + ActivePersonnelNumber;

            using (SqlConnection connection = new SqlConnection(PersonalCard.ConnectionString))
            {
                await connection.OpenAsync();
                SqlCommand command = new SqlCommand(query, connection);
                SqlDataReader reader = await command.ExecuteReaderAsync();

                while (await reader.ReadAsync())
                {
                    lblWage.Text = reader["Оклад"].ToString() + "бел. руб.";
                    if (reader["Должность"].ToString().Length >30)
                    {
                        lblPosition.Text = '(' + reader["Должность"].ToString().Substring(0, reader["Должность"].ToString().Length - 10) + "..." + ')';
                    }
                    else
                    {
                        lblPosition.Text = '(' + reader["Должность"].ToString() + ')';
                    }
                    lblPrize.Text = reader["Премия"].ToString() + "бел. руб.";
                    lblAdditionalPayments.Text = reader["Доплаты"].ToString() + "бел. руб.";
                    var procent = reader["% надбавки за стаж"].ToString();
                    lblSeniorityBonusProcent.Text = procent.Substring(0, procent.Length - 3) + "%";
                    var cost = reader["Сумма надбавки"].ToString();
                    lblSeniorityBonusCost.Text = cost.Substring(0, cost.Length - 4) + "бел. руб.";
                    var total = reader["Итого"].ToString();
                    lblTotalBYN.Text = total.Substring(0, cost.Length - 3) + "бел. руб.";
                }
            }
            #endregion

            #region Средние сзначения
            GetAvgSalary();
            var salary = GetSalary();
            var difference = salary.avgSalary - salary.totalSalary;
            difference = Math.Round(difference, 2);
            if (difference < 0)
            {
                label14.Text = "Выше средней на:";
                lblMoreThenAverage.Visible = true;
                difference = Math.Abs(difference);
                
                var procent = difference / salary.totalSalary * 100;
                procent = Math.Round(procent, 2);
                lblMoreThenAverage.Text = difference.ToString();
                
                lblMoreThenAverageProcent.Text = '(' + procent.ToString() + '%' + ')';
                
            }
            else if (difference == 0)
            {
                label14.Text = "Значение соответствует средней";
                lblMoreThenAverage.Visible = false;
            }
            else
            {
                label14.Text = "Ниже средней на:";
                lblMoreThenAverage.Visible = true;
                difference = Math.Abs(difference);
                lblMoreThenAverage.Text = difference.ToString();

                
                lblMoreThenAverage.Text = difference.ToString();
  
                var procent = difference / salary.totalSalary * 100;
                procent = Math.Round(procent, 2);

                lblMoreThenAverageProcent.Text = '(' + procent.ToString() + '%' + ')';
            }
            #endregion
            try
            {
                GetCurseUSD();
                await Task.Run(() =>
                {
                    
                    var s = GetCurseUSD().Replace('.', ',');
                    var USDcurse = Double.Parse(s);
                    
                    var usdSalary = salary.totalSalary / USDcurse;
                    lbltotalUSD.Invoke((MethodInvoker)delegate { lbltotalUSD.Text = "USD:" + Math.Round(usdSalary, 2).ToString(); });
                    
                });
                
            }
            catch (Exception)
            {
                MessageBox.Show("Отсуттсвет интрернет соединени. Невозможно установить текущий курс. \n" +
                    "Расчёт будет вестить по курсу 2.41", "Внимание!", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                var usdSalary = salary.totalSalary / 2.41;
                lbltotalUSD.Text = "USD:" + Math.Round(usdSalary, 2).ToString();
            }

            
            
        }

        private void button1_Click(object sender, EventArgs e) => Close();

        private void lblMoreThenAverage_ClientSizeChanged(object sender, EventArgs e)
            => lblMoreThenAverageProcent.Location = new Point(470 + lblMoreThenAverage.Width, 324);
    }
}
