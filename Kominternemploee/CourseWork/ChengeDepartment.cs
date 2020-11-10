using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;
using System.Data.SqlClient;

namespace CourseWork
{
    public partial class ChengeDepartment : Form
    {
        public enum FormTarget
        {
            Chenge, 
            Add
        }
        public static List<string> Values { get; set; }
        public FormTarget FrmTarget { get; private set; }

        public ChengeDepartment(FormTarget formTarget, List<string> vs)
        {
            InitializeComponent();
            FrmTarget = formTarget;

            if (FrmTarget == FormTarget.Chenge)
            {
                tbDepartmentNumber.ReadOnly = true;
                button1.Text = "Изменить";
            }
            else
            {
                tbDepartmentNumber.ReadOnly = false;
                button1.Text = "Добавить";
            }

            Values = vs;
        }

        private int[] GetDepartmentNumber()
        {

            string cmd = "select Department.DepartmentNumber as 'DN' from Department";
            List<int> listInt = new List<int>();

            using (SqlConnection connection = new SqlConnection(PersonalCard.ConnectionString))
            {
                connection.Open();
                SqlCommand command = new SqlCommand(cmd, connection);
                SqlDataReader reader = command.ExecuteReader();
                int ordinalNumber = reader.GetOrdinal("DN");

                while (reader.Read())
                {
                    listInt.Add(reader.GetInt32(ordinalNumber));
                }

                return listInt.ToArray();
            }
        }

        public async void RefreshDataSet()
        {
            string cmd =
                "SELECT " +
                    "[DepartmentNumber] as 'Номер отделения', " +
                    "[DepartmentTitile] as 'Название', " +
                    "[OfficeNumber] as 'Кабинет', " +
                    "[PhoneNumber] as 'Контактный телейон', " +
                    "[HeadOfDepartment] as 'Начальник отделения'," +
                    "[NumberOfEmployees] as 'Количество сотрудников' " +
            "FROM [dbo].[Department]";

            using (SqlConnection connection = new SqlConnection(PersonalCard.ConnectionString))
            {
                await connection.OpenAsync();
                SqlCommand command = new SqlCommand(cmd, connection);
                var adapter = new SqlDataAdapter(command);
                NavigationForm.table2.Clear();
                adapter.Fill(NavigationForm.table2);
            }
        }

        private async void button1_Click(object sender, EventArgs e)
        {
            string cmd = string.Empty;
            if (FrmTarget == FormTarget.Add)
            {
                cmd =
                "INSERT INTO [dbo].[Department] " +
                "( " +
                    "[DepartmentNumber], " +
                    "[DepartmentTitile], " +
                    "[OfficeNumber], " +
                    "[PhoneNumber], " +
                    "[HeadOfDepartment]," +
                    "[NumberOfEmployees]) " +
                "VALUES" +
                    $"({tbDepartmentNumber.Text}, " +
                    $"'{tbDepartmentTitle.Text}', " +
                    $"{tbRoom.Text}, " +
                    $"{tbPhone.Text}, " +
                    $"'{tbHeadOfDepartment.Text}', " +
                    $"{tbEmpoeeCount.Text})";

                var depatrmentNumbers = GetDepartmentNumber();
                if (depatrmentNumbers.Contains(Convert.ToInt32(tbDepartmentNumber.Text)))
                {
                    MessageBox.Show("Ошибка! Введённый Номер отдления уже существует.", "Внимание ");
                }
            }

            if (FrmTarget == FormTarget.Chenge)
            {
                cmd =
                    "UPDATE [dbo].[Department]" +
                    "SET" +
                        $"[DepartmentTitile] = '{tbDepartmentTitle.Text}', " +
                        $"[OfficeNumber] = {tbRoom.Text}, " +
                        $"[PhoneNumber] = {tbPhone.Text}, " +
                        $"[HeadOfDepartment] = '{tbHeadOfDepartment.Text}', " +
                        $"[NumberOfEmployees] = {tbEmpoeeCount.Text} " +
                    $"WHERE " +
                        $"DepartmentNumber = {tbDepartmentNumber.Text}";
            }
            

            using (SqlConnection connection = new SqlConnection(PersonalCard.ConnectionString))
            {
                await connection.OpenAsync();
                SqlCommand command = new SqlCommand(cmd, connection);
                await command.ExecuteNonQueryAsync();

                MessageBox.Show("Записи успешно обновленны.", "Сообщение", 
                    MessageBoxButtons.OK, MessageBoxIcon.Information);
                
                Close();
            }
            NavigationForm form = new NavigationForm();
            RefreshDataSet();
            form.Refresh();
        }

        private void tbDepartmentNumber_Leave(object sender, EventArgs e)
        {
            if (FrmTarget == FormTarget.Add)
            {
                if (GetDepartmentNumber().Contains(Convert.ToInt32(tbDepartmentNumber.Text)))
                {
                    MessageBox.Show("Нверно указан номер отделения.", "Сообщение", 
                        MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    tbDepartmentNumber.Text = String.Empty;
                    tbDepartmentNumber.Focus();
                }
            }
        }

        private void tbPhone_Leave(object sender, EventArgs e)
        {
            int number = 0;
            if (!Int32.TryParse(tbDepartmentNumber.Text, out number))
            {
                MessageBox.Show("Неврно указан номер телефона.", "Соощение",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                tbPhone.Focus();
            }
        }

        private void tbEmpoeeCount_Leave(object sender, EventArgs e)
        {
            var phone = 0;
            if (!Int32.TryParse(tbEmpoeeCount.Text, out phone))
            {
                MessageBox.Show("Неврно указано количество сотрудникв.", "Соощение",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                tbEmpoeeCount.Focus();
            }
        }

        private void ChengeDepartment_Load(object sender, EventArgs e)
        {
            if (FrmTarget == FormTarget.Chenge)
            {

                tbDepartmentNumber.Text = Values[0];
                tbDepartmentTitle.Text = Values[1];
                tbRoom.Text = Values[2];
                tbPhone.Text = Values[3];
                tbHeadOfDepartment.Text = Values[4];
                tbEmpoeeCount.Text = Values[5];
            }
        }
    }
}
