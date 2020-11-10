using System;
using System.Data.SqlClient;
using System.Drawing;
using System.Windows.Forms;
using System.Configuration;
using System.IO;

namespace CourseWork
{
    public partial class PersonalCard : Form
    {
        public int PersonnelNumber { get; private set; }
        public PersonalCard(int persNum)
        {
            InitializeComponent();
            PersonnelNumber = persNum;
        }
        /// <summary>
        ///Возвращает строку подключения для БД
        /// </summary>
        public static string ConnectionString { get; } = ConfigurationManager.ConnectionStrings["EmploeesConnectionString"].ConnectionString;
        private async void LoadData(int personnelerNnmber)
        {
            #region Загрузка данных
            string query =
                "SELECT" +
                    "[Emploee].[PersonnelNumber] as 'Табельный номер'," +
                    "[Emploee].[Surname] + ' ' + [Emploee].[Name] + ' '+ [Emploee].[MidleName] as 'ФИО'," +
                    "[Emploee].[Gender]	as 'Пол'," +
                    "[Emploee].[DateOfBirth] as 'Дата рождения'," +
                    "[Emploee].[FamilyStatus] as 'Семейный статус'," +
                    "[Emploee].[Experience]	as 'Опыт работы'," +
                    "[Emploee].[EmploymentDate]	as 'Дата приёма на работу'," +
                    "[Emploee].[Photo] AS 'Фото', " +
                    "[Department].[DepartmentTitile] as 'Отделение'," +
                    "[Positions].PositionTitle as 'Должность'" +
                "FROM" +
                    "[Emploee], [Department], [Positions]" +
                 "WHERE" +
                    "(Department.DepartmentNumber = Emploee.DepartmentNumber)" +
                        "and" +
                     "(Positions.PositionId = Emploee.PositionId)" +
                        "and" +
                     "[Emploee].[PersonnelNumber] = " + personnelerNnmber;
            SqlDataReader reader;
            using (SqlConnection connection = new SqlConnection(ConnectionString))
            {
                await connection.OpenAsync();
                SqlCommand command = new SqlCommand(query, connection);
                reader = await command.ExecuteReaderAsync();

                while (await reader.ReadAsync())
                {
                    lblFullName.Text = reader["ФИО"].ToString();
                    lblPersonnelerNumber.Text = reader["Табельный номер"].ToString();
                    lblGender.Text = reader["Пол"].ToString();
                    string date = reader["Дата рождения"].ToString();
                    date = date.Substring(0, date.Length - 7);
                    lbDateOfBidth.Text = date;
                    lblFamilyStatus.Text = reader["Семейный статус"].ToString();
                    lblExpiriance.Text = reader["Опыт работы"].ToString() + " лет";
                    lblDepartment.Text = reader["Отделение"].ToString();
                    string dateOfEmp = reader["Дата приёма на работу"].ToString();
                    lblDateOfEmployment.Text = dateOfEmp.Substring(0, dateOfEmp.Length -7);
                    lblPositions.Text = reader["Должность"].ToString();

                    if (reader["Фото"] != DBNull.Value)
                    {
                        MemoryStream imagestream = new MemoryStream((byte[])reader["Фото"]);
                        Image image = Image.FromStream(imagestream);

                        pictureBox1.Image = image;
                    }

                }
            }
            #endregion
        }
        /// <summary>
        /// Кнопка "подробнее", которая открыват форму с просмотрои информации о
        /// </summary>
        Button btnAbout = new Button();
        private  void Form1_Load(object sender, EventArgs e)
        {
            
            LoadData(PersonnelNumber);
            btnAbout.Text = "Подробнеее";
            btnAbout.Enabled = true;
            int x = lblPositions.Location.X + lblPositions.Width;
            btnAbout.Location = new Point(x, 250);
            btnAbout.Click += BtnAbout_Click;
            this.Controls.Add(btnAbout);
        }
        private void BtnAbout_Click(object sender, EventArgs e)
        {
            Department department = new Department(lblFullName.Text);
            department.ShowDialog();

        }
        public int GetNextPersNum(int previosNumber)
        {
            int personnelerNumber = 0;
            string query =
                "SELECT top(1) " +
                    "[Emploee].PersonnelNumber " +
                "FROM " +
                    "Emploee " +
                $"WHERE PersonnelNumber > {previosNumber} " +
                    "ORDER BY PersonnelNumber";
            using (SqlConnection connection = new SqlConnection(ConnectionString))
            {
                connection.Open();
                SqlCommand command = new SqlCommand(query, connection);
                SqlDataReader reader = command.ExecuteReader();

                while (reader.Read())
                {
                    personnelerNumber = Convert.ToInt32(reader["PersonnelNumber"]);
                }
            }
            return personnelerNumber;
        }
        public int GetPreviosPersNum(int previosNumber)
        {
            int personnelerNumber = 0;
            string query =
                "SELECT top(1) " +
                    "[Emploee].PersonnelNumber " +
                "FROM " +
                    "Emploee " +
                $"WHERE PersonnelNumber < {previosNumber} " +
                    "ORDER BY PersonnelNumber DESC";
            using (SqlConnection connection = new SqlConnection(ConnectionString))
            {
                connection.Open();
                SqlCommand command = new SqlCommand(query, connection);
                SqlDataReader reader = command.ExecuteReader();

                while (reader.Read())
                {
                    personnelerNumber = Convert.ToInt32(reader["PersonnelNumber"]);
                }
            }
            return personnelerNumber;
        }
        private void btnMoveNext_Click_1(object sender, EventArgs e)
        => LoadData(GetNextPersNum(Convert.ToInt32(lblPersonnelerNumber.Text)));
        private void btnMoveBack_Click(object sender, EventArgs e)
        => LoadData(GetPreviosPersNum(Convert.ToInt32(lblPersonnelerNumber.Text)));
        private void btnAboutInfo_Click(object sender, EventArgs e)
        {
            ContactInfo contact = new ContactInfo(Int32.Parse(lblPersonnelerNumber.Text));
            contact.ShowDialog();
        }
        private async void btnPromotions_Click(object sender, EventArgs e)
        {
            string query = "SELECT * FROM [Promotions] WHERE [Promotions].PersonnelNumber = " + lblPersonnelerNumber.Text;
            using (SqlConnection connection = new SqlConnection(ConnectionString))
            {
                await connection.OpenAsync();

                SqlCommand command = new SqlCommand(query, connection);
                SqlDataReader reader = await command.ExecuteReaderAsync();

                if (reader.HasRows)
                {
                    Promotions promotions = new Promotions(Int32.Parse(lblPersonnelerNumber.Text)
                                                            , lblFullName.Text);
                    promotions.ShowDialog();
                }
                else
                {
                    MessageBox.Show($"Сотрудник {lblFullName.Text} пока не получал никаких поощрений", 
                        "Сообщение", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
            }
        }
        private void btnViewIncome_Click(object sender, EventArgs e)
        {
            Income income = new Income(Int32.Parse(lblPersonnelerNumber.Text), lblFullName.Text);
            income.ShowDialog();
        }
        private void lblDepartment_ClientSizeChanged(object sender, EventArgs e)
        {
            int x = 110 + lblDepartment.Width;
            btnAbout.Location = new Point(x, 250);
        }
    }
}
