using System;
using System.Data.SqlClient;
using System.Windows.Forms;

namespace CourseWork
{
    public partial class ContactInfo : Form
    {
        public int ActivePersonnelNumber { get; private set; }
        public ContactInfo(int personnelNumber)
        {
            InitializeComponent();
            ActivePersonnelNumber = personnelNumber;
        }

        private async void ContactInfo_Load(object sender, EventArgs e)
        {
            string query =
                "SELECT " +
                    "[Emploee].[Surname] + ' ' + [Emploee].[Name] + ' '+ [Emploee].[MidleName] as 'ФИО', " +
                    "[AddressOfResidence] as 'Адрес проживания', " +
                    "ContactInformation.Phone as 'Телефон', " +
                    "ContactInformation.Email as 'Почта' " +
                "FROM " +
                    "ContactInformation, " +
                    "Emploee " +
                "WHERE " +
                    "(ContactInformation.PersonnelNumber = Emploee.PersonnelNumber) " +
                        "and " +
                    $"(Emploee.PersonnelNumber = {ActivePersonnelNumber})";

            using (SqlConnection connection = new SqlConnection(PersonalCard.ConnectionString))
            {
                await connection.OpenAsync();

                SqlCommand command = new SqlCommand(query, connection);

                SqlDataReader reader = await command.ExecuteReaderAsync();

                while (await reader.ReadAsync())
                {
                    lblFullName.Text = reader["ФИО"].ToString();
                    lblAdress.Text   = reader["Адрес проживания"].ToString();
                    lblPhone.Text    = reader["Телефон"].ToString();
                    lblEmail.Text    = reader["Почта"].ToString();
                }
            }
        }

        private void btnClose_Click(object sender, EventArgs e)
        {
            Close();
        }
    }
}
