using System;
using System.Data;
using System.Data.SqlClient;
using System.Windows.Forms;

namespace CourseWork
{
    public partial class Promotions : Form
    {
        public int ActivePersonnelnumber { get; set; }

        public Promotions(int activePersonnelnumber, string fullname)
        {
            InitializeComponent();
            ActivePersonnelnumber = activePersonnelnumber;
            lblFullName.Text = fullname;
        }

        private async void Promotions_Load(object sender, EventArgs e)
        {
            dataGridView1.Rows.Clear();
            string query =
                "SELECT " +
                    "[Emploee].[Surname] + ' ' + [Emploee].[Name] + ' '+ [Emploee].[MidleName] as 'ФИО', " +
                    "[KindofPromotion].KindofPromotion AS 'Вид пообщрения', " +
                    "[Promotions].DocumentNumber AS 'Номер договора', " +
                    "[Promotions].[Date] AS 'Дата выдвчи', " +
                    "[Promotions].Cause AS 'Причина', " +
                    "[Promotions].AmountOfMoney AS 'Денежное вознаграждение' " +
                "FROM [Promotions] " +
                    "INNER JOIN [KindofPromotion] " +
                        "ON " +
                    "[KindofPromotion].IdKindofPromotion = [Promotions].[IdKindofPromotion] " +
                "INNER JOIN [Emploee] " +
                    "ON [Emploee].PersonnelNumber = [Promotions].PersonnelNumber " +
                "WHERE " +
                    "[Promotions].[PersonnelNumber] = " + ActivePersonnelnumber;


            using (SqlConnection connection = new SqlConnection(PersonalCard.ConnectionString))
            {
                await connection.OpenAsync();

                SqlCommand command = new SqlCommand(query, connection);
                SqlDataReader reader = await command.ExecuteReaderAsync();
                DataTable dataTable = new DataTable();
                
                dataTable.Load(reader);
                dataGridView1.DataSource = dataTable.DefaultView;
                dataGridView1.AutoResizeColumns();                
            }
                
        }

        private void button1_Click(object sender, EventArgs e)
        {
            Close();
        }
    }
}
