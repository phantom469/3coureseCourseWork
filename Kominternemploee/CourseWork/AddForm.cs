using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Windows.Forms;

namespace CourseWork
{
    public partial class AddForm : Form
    {
        public AddForm()
        {
            InitializeComponent();
            cbGender.SelectedIndex = 0;
            cbFamilyStatus.SelectedIndex = 0;
        }

        private async void AddForm_Load(object sender, EventArgs e)
        {
            // TODO: данная строка кода позволяет загрузить данные в таблицу "emploeesDataSet.KindofPromotion". При необходимости она может быть перемещена или удалена.
            this.kindofPromotionTableAdapter.Fill(this.emploeesDataSet.KindofPromotion);
            // TODO: данная строка кода позволяет загрузить данные в таблицу "emploeesDataSet.AdditionPayments". При необходимости она может быть перемещена или удалена.
            this.additionPaymentsTableAdapter.Fill(this.emploeesDataSet.AdditionPayments);
            this.positionsTableAdapter.Fill(this.emploeesDataSet.Positions);
            this.departmentTableAdapter.Fill(this.emploeesDataSet.Department);
            string cmd = "select top(1) Positions.Wage from Positions";

            using (SqlConnection connection = new SqlConnection(PersonalCard.ConnectionString))
            {
                await connection.OpenAsync();
                SqlCommand command = new SqlCommand(cmd, connection);
                var wage = await command.ExecuteScalarAsync();
                tbWage.Text = wage.ToString();
            }

        }

        private int[] GetPersnum() 
        {
            
            string cmd = "select [Emploee].PersonnelNumber as 'ТБ' from Emploee ";
            List<int> listInt = new List<int>();
            
            using (SqlConnection connection = new SqlConnection (PersonalCard.ConnectionString))
            {
                connection.Open();
                SqlCommand command = new SqlCommand(cmd, connection);
                SqlDataReader reader = command.ExecuteReader();
                int ordinalNumber = reader.GetOrdinal("ТБ");

                while (reader.Read())
                {
                    listInt.Add(reader.GetInt32(ordinalNumber));
                }

                return listInt.ToArray();
            }
        }

        private int[] CalcId()
        {

            string cmd = " select Calculated.CalculatedId as 'ID' from Calculated";
            List<int> listInt = new List<int>();

            using (SqlConnection connection = new SqlConnection(PersonalCard.ConnectionString))
            {
                connection.Open();
                SqlCommand command = new SqlCommand(cmd, connection);
                SqlDataReader reader = command.ExecuteReader();
                int ordinalNumber = reader.GetOrdinal("ID");

                while (reader.Read())
                {
                    listInt.Add(reader.GetInt32(ordinalNumber));
                }

                return listInt.ToArray();
            }
        }

        public void RefreshDataSet()
        {
            string query =
               "SELECT" +
                   "[Emploee].[Surname] + ' ' + [Emploee].[Name] + ' '+ [Emploee].[MidleName] as 'ФИО'," +
                   "[Emploee].[Gender]	as 'Пол'," +
                   "[Emploee].[Experience]	as 'Опыт работы'," +
                   "[Emploee].[EmploymentDate]	as 'Дата приёма на работу'," +

                   "[Positions].PositionTitle as 'Должность'" +
               "FROM" +
                   "[Emploee], [Department], [Positions]" +
                "WHERE" +
                   "(Department.DepartmentNumber = Emploee.DepartmentNumber)" +
                       "and" +
                    "(Positions.PositionId = Emploee.PositionId)" +
               "ORDER BY ФИО";

            using (SqlConnection connection = new SqlConnection(PersonalCard.ConnectionString))
            {
                connection.Open();

                SqlCommand command = new SqlCommand(query, connection);
                SqlDataAdapter adapter = new SqlDataAdapter(command);
                NavigationForm.table.Clear();
                adapter.Fill(NavigationForm.table); 
            }
        }
        private void button1_Click_1(object sender, EventArgs e) => Close();

        //Выбор Фото
        private void button2_Click(object sender, EventArgs e)
        {
            OpenFileDialog openFile = new OpenFileDialog();
            openFile.Filter = "Image Files(*.BMP;*.JPG;*.PNG)|*.BMP;*.JPG;*.GIF;*.PNG|" +
                              "All files (*.*)|*.*";
            if (openFile.ShowDialog() == DialogResult.OK)
            {
                pictureBox1.Image = Image.FromFile(openFile.FileName);
                
                FileInfo fileInfo = new FileInfo(openFile.FileName);
                
                textBox1.Text = fileInfo.FullName;
            }
        }

        private async void LoadImage()
        {
            string query = $@"UPDATE Emploee
                             SET Photo = 
                                (SELECT * FROM OPENROWSET(BULK N'{textBox1.Text}', SINGLE_BLOB) AS image)
                             WHERE PersonnelNumber = {tbPersoneelNumber.Text}";
            
            using (SqlConnection connection = new SqlConnection(PersonalCard.ConnectionString))
            {
                await connection.OpenAsync();
                SqlCommand command = new SqlCommand(query, connection);
                await command.ExecuteNonQueryAsync();
            }
        }

        private void btnAdd_Click(object sender, EventArgs e)
        {
            string query =
                "INSERT INTO " +
                    "[dbo].[ContactInformation] " +
                    "([PersonnelNumber], [AddressOfResidence], [Phone], [Email]) " +
                "VALUES " +
                    $"(@personnelnumber, @adress, @phone, @email) " +
                "INSERT INTO " +
                    "[dbo].[Emploee] " +
                     "([PersonnelNumber], [Surname], [Name], [MidleName], [Gender], [FamilyStatus], [Experience], [EmploymentDate], [DateOfBirth], [DepartmentNumber], [PositionId])" +
                "VALUES" +
                    $"({tbPersoneelNumber.Text}, @surname, @name, @midlename, " +
                    "@gender, @familustatus, @expiriance, @emplymentdate, @dateOfbidth, " +
                    "@departmentId, @positionId)";

            if (textBox1.Text == "")
            {
                MessageBox.Show("Фото работника не выбрано!", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                button2_Click(this, e);
            }
            else
            {
                LoadImage();
            }


            using (SqlConnection connection = new SqlConnection(PersonalCard.ConnectionString))
            {
                #region параметры контактной инормации
                connection.Open();
                SqlCommand command = new SqlCommand(query, connection);

                SqlParameter persNumParameter = new SqlParameter("@personnelnumber", tbPersoneelNumber.Text);
                command.Parameters.Add(persNumParameter);

                SqlParameter adressParametr = new SqlParameter("@adress", tbAdress.Text);
                command.Parameters.Add(adressParametr);

                SqlParameter phoneParametr = new SqlParameter("@phone", tbPhone.Text.Replace("-", ""));
                command.Parameters.Add(phoneParametr);

                SqlParameter emailParametr = new SqlParameter("@email", tbEmail.Text);
                command.Parameters.Add(emailParametr);
                #endregion
                #region Основные параметры
                SqlParameter surnameParametr = new SqlParameter("@surname", tbSurname.Text);
                command.Parameters.Add(surnameParametr);

                SqlParameter nameParametr = new SqlParameter("@name", tbNaame.Text);
                command.Parameters.Add(nameParametr);

                SqlParameter midleNameParam = new SqlParameter("@midlename", tbmidleName.Text);
                command.Parameters.Add(midleNameParam);

                SqlParameter genderParametr = new SqlParameter("@gender", cbGender.Text);
                command.Parameters.Add(genderParametr);

                SqlParameter familustatusParametr = new SqlParameter("@familustatus", cbFamilyStatus.Text);
                command.Parameters.Add(familustatusParametr);

                SqlParameter expirianceparametr = new SqlParameter("@expiriance", tbExpiriance.Text);
                command.Parameters.Add(expirianceparametr);

                command.Parameters.Add(new SqlParameter("@emplymentdate", tbEmplooeDate.Value));
                command.Parameters.Add(new SqlParameter("@dateOfbidth", tbDateOfBidth.Value));
                command.Parameters.Add(new SqlParameter("@departmentId", cbDepartment.SelectedValue));
                command.Parameters.Add(new SqlParameter("@positionId", cbPosition.SelectedValue));
                #endregion

                command.ExecuteNonQuery();

                RefreshDataSet();
                NavigationForm form = new NavigationForm();
                form.Refresh();
                this.Close();
                
            }
            LoadcalcInfo();
            if (!String.IsNullOrWhiteSpace(tbCouse.Text))
            {
                LoadPromotionInfo();
            }

        }

        private async void LoadcalcInfo()
        {
            string cmd =
                "INSERT INTO [dbo].[Calculated] " +
                    "([CalculatedId],[PersonnelNumber],[Prize],[ProcentSeniorityBonus],[AddPaymentsId]) " +
                "VALUES " +
                    $"({tbPersoneelNumber.Text}, " +
                    $"{tbPersoneelNumber.Text}, " +
                    $"{tbPrize.Text}, " +
                    $"{tbProcentAdditional.Text}, " +
                    $"{cbadditionalPayments.SelectedValue})";

            using (SqlConnection connection = new SqlConnection(PersonalCard.ConnectionString))
            {
                await connection.OpenAsync();
                SqlCommand command = new SqlCommand(cmd, connection);
                await command.ExecuteNonQueryAsync();
            }

        }
        private async void LoadPromotionInfo()
        {
            string cmd = 
                "INSERT INTO [dbo].[Promotions] " +
                "([PromotionId],[PersonnelNumber],[IdKindofPromotion], " +
                "[Cause],[Date],[DocumentNumber],[AmountOfMoney]) " +
                    "VALUES " +
                $"({tbPersoneelNumber.Text}, {tbPersoneelNumber.Text}, {cbKindOfPromotion.SelectedValue}, '{tbCouse.Text}', " +
                $"@documentNumber, {tbDocumentNumber.Text}, {tbMoney.Text})";
            using (SqlConnection connection = new SqlConnection(PersonalCard.ConnectionString))
            {
                await connection.OpenAsync();
                SqlCommand command = new SqlCommand(cmd, connection);

                command.Parameters.Add(new SqlParameter("@documentNumber", tbDocumentDate.Value));

                await command.ExecuteNonQueryAsync();
            }
        }

        //Проверка на правильность вввода табельного номера 
        private void tbPersoneelNumber_Leave(object sender, EventArgs e)
        {
            var persNms = GetPersnum();
            if (!String.IsNullOrWhiteSpace(tbPersoneelNumber.Text))
            {
                try
                {
                    if (persNms.Contains(Int32.Parse(tbPersoneelNumber.Text)))
                    {
                        MessageBox.Show("Сотрудник с таким персональным номером уже существет!",
                            "Ошибка ввода данных", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        tbPersoneelNumber.Focus();
                        tbPersoneelNumber.SelectAll();
                    }
                }
                catch (Exception)
                {
                    MessageBox.Show("Введдёны сиволы или некорректный Табелный номер. Пожалуйста введите корректные данные.",
                        "Ошибка ввода данных", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    tbPersoneelNumber.Focus();
                    tbPersoneelNumber.Text = "";
                }
                
            }
            
        }
        //Поверка правильно ли ввдён опыт работы
        private void tbExpiriance_Leave(object sender, EventArgs e)
        {
            try
            {
                Int32.Parse(tbExpiriance.Text);
                if (String.IsNullOrWhiteSpace(tbExpiriance.Text))
                {
                    MessageBox.Show("Введённые данные были неверные. Пожалуйста введеите данные, корректные для поля Опыт работы.", "Внимание", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                }
                
            }
            catch (Exception)
            {
                MessageBox.Show("Введённые данные были неверные. Пожалуйста введеите данные, корректные для поля Опыт работы.", "Внимание", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                tbExpiriance.Focus();
                tbExpiriance.SelectAll();
            }
        }

        private void panel3_Paint(object sender, PaintEventArgs e)
        {

        }

        private void panel4_Paint(object sender, PaintEventArgs e)
        {
            
        }

        private void cbPosition_SelectedIndexChanged(object sender, EventArgs e)
        {
            string cmd = "select Positions.Wage from Positions where PositionId = " + cbPosition.SelectedValue;
            using (SqlConnection connection = new SqlConnection(PersonalCard.ConnectionString))
            {
                connection.Open();
                SqlCommand command = new SqlCommand(cmd, connection);
                var wage = command.ExecuteScalar();
                tbWage.Text = wage.ToString();
            }
        }
    }
}
