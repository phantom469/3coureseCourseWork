using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Windows.Forms;

namespace CourseWork
{
    public partial class ChengeRecordsInPositions : Form
    {
        public enum FormtTarget
        {
            Add,
            Chenge
        }

        public void RefresfDataset()
        {
            string cmd =
                "SELECT " +
                    "[PositionId] as 'Код', " +
                    "[PositionTitle] as 'Должность', " +
                    "[Wage] as 'Оклад' " +
                "FROM " +
                    "[dbo].[Positions]";
            using (SqlConnection connection = new SqlConnection(PersonalCard.ConnectionString))
            {
                connection.Open();
                SqlCommand command = new SqlCommand(cmd, connection);
                SqlDataAdapter adapter = new SqlDataAdapter(command);
                NavigationForm.table3.Clear();
                adapter.Fill(NavigationForm.table3);
            }
        }

        public List<string> Values { get; set; }
        public FormtTarget Target { get; private set; }

        public ChengeRecordsInPositions(FormtTarget target, List<string> vs)
        {
            InitializeComponent();

            Target = target;
            Values = vs;
            if (target == FormtTarget.Add)
            {
                texbox3.ReadOnly = false;
                this.Text = "Добавить новую должность";
                button1.Text = "Добавить";
            }

            if (target == FormtTarget.Chenge)
            {
                this.Text = "Редактирование";
                texbox3.ReadOnly = true;
                button1.Text = "Изменить";
                texbox3.Text = Values[0];
                textBox1.Text = Values[1];
                textBox2.Text = Values[2];
            }
        }
        
        private void button2_Click(object sender, EventArgs e) => Close();

        private void button1_Click(object sender, EventArgs e)
        {
            string cmd = "";
            if (Target == FormtTarget.Add)
            {
                cmd =
                    "INSERT INTO [dbo].[Positions] " +
                        "([PositionId] , [PositionTitle], [Wage]) " +
                    "VALUES " +
                        $"({texbox3.Text}, '{textBox1.Text}', {textBox2.Text.Replace(',', '.')})";
            }

            if (Target == FormtTarget.Chenge)
            {
                cmd =
                    "UPDATE [dbo].[Positions] " +
                    "SET " +
                        $"[PositionTitle] = '{textBox1.Text}', " +
                        $"[Wage] = {textBox2.Text.Replace(',', '.')}" +
                    $"WHERE PositionId = {texbox3.Text}";
            }

            using (SqlConnection connection = new SqlConnection(PersonalCard.ConnectionString))
            {
                connection.Open();
                SqlCommand command = new SqlCommand(cmd, connection);
                command.ExecuteNonQuery();

                MessageBox.Show("Записи успешно обновленны", "Сообщение",
                    MessageBoxButtons.OK, MessageBoxIcon.Information);

                RefresfDataset();
                NavigationForm navigationForm = new NavigationForm();
                navigationForm.Refresh();
                Close();
            }
        }

        private void ChengeRecordsInPositions_Load(object sender, EventArgs e)
        {

        }
    }
}
