using System;
using System.Data.SqlClient;
using System.Windows.Forms;

namespace CourseWork
{
    public partial class Department : Form
    {
        public Department(string fullName)
        {
            InitializeComponent();
            FullName = fullName;
        }
        public string FullName { get; private set; }

        private void button1_Click(object sender, EventArgs e) => Close();

        private async void Department_Load(object sender, EventArgs e)
        {
            string query =
                "select top(1) " +
                    "Department.DepartmentTitile as 'Наименование отделения', " +
                    "Department.OfficeNumber as 'Номер кабинета', " +
                    "Department.HeadOfDepartment as 'Начальник отделения', " +
                    "Department.PhoneNumber as 'Телефон', " +
                    "Department.NumberOfEmployees as 'количество работников'  " +
                "from " +
                    "Department, Emploee " +
                "where " +
                    "Department.DepartmentNumber = " +
                        "(select " +
                            "Emploee.DepartmentNumber " +
                        "from " +
                            "Emploee " +
                        "where" +
                            $"[Emploee].[Surname] + ' ' + [Emploee].[Name] + ' '+ [Emploee].[MidleName] = '{FullName}')";

            using (SqlConnection connection =  new SqlConnection(PersonalCard.ConnectionString))
            {
                await connection.OpenAsync();
                SqlCommand command = new SqlCommand(query, connection);
                SqlDataReader reader = await command.ExecuteReaderAsync();

                while (await reader.ReadAsync())
                {
                    lblDepartnenttTtle.Text = reader["Наименование отделения"].ToString();
                    lblHeadOfDepartment.Text = reader["Начальник отделения"].ToString();
                    lblNumberOfEmployes.Text = reader["количество работников"].ToString();
                    lblOfficeNumber.Text = reader["Номер кабинета"].ToString();
                    var phone = reader["Телефон"].ToString().ToCharArray();
                    var newPhone = new char[] { phone[0], phone[1], phone[2], '-', phone[3], phone[4], '-', phone[5], phone[6] };
                    var outPhone = new string(newPhone);
                    lblWorkNumber.Text = outPhone;
                }
            }
        }
    }
}
