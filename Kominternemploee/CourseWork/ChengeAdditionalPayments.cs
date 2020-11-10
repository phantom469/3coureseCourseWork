using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace CourseWork
{
    public partial class ChengeAdditionalPayments : Form
    {
        public ChengeAdditionalPayments(List<string> values, FormTarget target)
        {
            InitializeComponent();
            Target = target;
            Values = values;

            if (target == FormTarget.Add)
            {
                this.Text = "Добавить запись";
                button1.Text = "Добавить";
                textBox1.ReadOnly = false;
            }

            if (target == FormTarget.Chenge)
            {
                this.Text = "Редактировать запись";
                button1.Text = "Изменить";
                textBox1.Text = Values[0];
                textBox2.Text = Values[1];
                textBox3.Text = Values[2];
                textBox1.ReadOnly = true;
            }
        }
        public enum FormTarget { Add, Chenge }
        public FormTarget Target { get; private set; }
        public List<string> Values { get; set; }
        private async void RefreshDataset()
        {
            string cmd =
               "SELECT " +
                   "[AddPaymentsId] as 'Код доплтат', " +
                   "[Title] as 'Наименование', " +
                   "[Cost] as 'Сумма выплат' " +
               "FROM " +
                   "[dbo].[AdditionPayments]";
            using (SqlConnection connection = new SqlConnection(PersonalCard.ConnectionString))
            {
                await connection.OpenAsync();
                SqlCommand comm = new SqlCommand(cmd, connection);
                NavigationForm.table4.Clear();
                SqlDataAdapter adapter = new SqlDataAdapter(comm);
                adapter.Fill(NavigationForm.table4);
                NavigationForm form = new NavigationForm();
                form.Refresh();
            }
        }
        private void button2_Click(object sender, EventArgs e) => Close();

        private async void button1_Click(object sender, EventArgs e)
        {
            string cmd = "";

            if (Target == FormTarget.Add)
            {
                cmd =
                    "INSERT INTO " +
                        "[dbo].[AdditionPayments] " +
                        "([AddPaymentsId], [Title], [Cost]) " +
                    "VALUES " +
                        $"({textBox1.Text}, '{textBox2.Text}', {textBox3.Text.Replace(',', '.')})";
            }

            if (Target == FormTarget.Chenge)
            {
                cmd =
                    "UPDATE " +
                        "[dbo].[AdditionPayments] " +
                    "SET " +
                        $"[Title] = '{textBox2.Text}', " +
                        $"[Cost] = {textBox3.Text.Replace(',', '.')} " +
                    "WHERE " +
                        $"[AddPaymentsId] = {textBox1.Text}"; 
            }

            using (SqlConnection connection = new SqlConnection(PersonalCard.ConnectionString))
            {
                await connection.OpenAsync();
                SqlCommand command = new SqlCommand(cmd, connection);
                try
                {
                    await command.ExecuteNonQueryAsync();
                    MessageBox.Show("Записи успешно обновленны", "Сообщение", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    RefreshDataset();
                    this.Close();
                }
                catch (Exception)
                {
                    MessageBox.Show("Ну удалось обновить данные проверьте введённую информацию",
                        "Сообщение", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    textBox2.Text = String.Empty;
                    textBox3.Text = String.Empty;
                    return;
                }
            }
        }
    }
}
