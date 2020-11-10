using System;
using System.Drawing;
using System.Windows.Forms;
using System.Data.SqlClient;
using System.IO;

namespace CourseWork
{
    public partial class ChengeRecords : Form
    {
        public int PersonnelNumber { get; private set; }
        public ChengeRecords(int persNum)
        {
            InitializeComponent();
            PersonnelNumber = persNum;
            cbGender.SelectedIndex = 0;
            cbFamilyStatus.SelectedIndex = 0;
        }
        private async void LoadData()
        {
            #region Загрузка данных
            string query;
            if (ContinePromotions())
            {
                query = 
                    "select " +
                        "Emploee.PersonnelNumber as 'ТН', " +
                        "Emploee.Surname as 'Sur', " +
                        "Emploee.[Name] as 'neme', " +
                        "Emploee.MidleName as 'MidleName', " +
                        "Emploee.Experience as 'Exp', " +
                        "Emploee.EmploymentDate as 'EmpDate', " +
                        "Emploee.DateOfBirth as 'Birth', " +
                        "Department.DepartmentNumber as 'Dep', " +
                        "Positions.PositionId as 'Positions', " +
                        "Calculated.Prize as 'Prize', " +
                        "Calculated.ProcentSeniorityBonus as 'Procent', " +
                        "AdditionPayments.AddPaymentsId as 'AddPay', " +
                        "KindofPromotion.IdKindofPromotion as 'Promotion', " +
                        "Promotions.[Date] as 'DocDate', " +
                        "Promotions.[DocumentNumber] as 'DocNumber', " +
                        "Promotions.Cause as 'Cause', " +
                        "Promotions.AmountOfMoney as 'Amount', " +
                        "ContactInformation.AddressOfResidence as 'Adress', " +
                        "ContactInformation.Phone as 'Phone', " +
                        "ContactInformation.Email as 'Email', " +
                        "Emploee.Photo as 'Фото' " +
                    "from [Emploee] " +
                        "inner join Department " +
                            "on Department.DepartmentNumber = Emploee.DepartmentNumber " +
                        "inner join Positions " +
                            "on Positions.PositionId = Emploee.PositionId " +
                        "inner join Calculated " +
                            "on Calculated.PersonnelNumber = Emploee.PersonnelNumber " +
                        "inner join Promotions " +
                            "on Promotions.PersonnelNumber = Emploee.PersonnelNumber " +
                        "inner join ContactInformation " +
                            "on ContactInformation.PersonnelNumber = Emploee.PersonnelNumber " +
                        "inner join AdditionPayments " +
                            "on AdditionPayments.AddPaymentsId = Calculated.AddPaymentsId " +
                        "inner join KindofPromotion " +
                            "on KindofPromotion.IdKindofPromotion = Promotions.IdKindofPromotion " +
                    "where " +
                        "Emploee.PersonnelNumber = " + PersonnelNumber;
            }
            else
            {
                query =
                    "select " +
                        "Emploee.PersonnelNumber as 'ТН', " +
                        "Emploee.Surname as 'Sur', " +
                        "Emploee.[Name] as 'neme', " +
                        "Emploee.MidleName as 'MidleName', " +
                        "Emploee.Experience as 'Exp', " +
                        "Emploee.EmploymentDate as 'EmpDate', " +
                        "Emploee.DateOfBirth as 'Birth', " +
                        "Department.DepartmentNumber as 'Dep', " +
                        "Positions.PositionId as 'Positions', " +
                        "Calculated.Prize as 'Prize', " +
                        "Calculated.ProcentSeniorityBonus as 'Procent', " +
                        "AdditionPayments.AddPaymentsId as 'AddPay', " +
                        "ContactInformation.AddressOfResidence as 'Adress', " +
                        "ContactInformation.Phone as 'Phone', " +
                        "ContactInformation.Email as 'Email', " +
                        "Emploee.Photo as 'Фото' " +
                "from [Emploee] " +
                    "inner join Department " +
                        "on Department.DepartmentNumber = Emploee.DepartmentNumber " +
                    "inner join Positions " +
                        "on Positions.PositionId = Emploee.PositionId " +
                    "inner join Calculated " +
                        "on Calculated.PersonnelNumber = Emploee.PersonnelNumber " +
                    "inner join ContactInformation " +
                        "on ContactInformation.PersonnelNumber = Emploee.PersonnelNumber " +
                    "inner join AdditionPayments " +
                        "on AdditionPayments.AddPaymentsId = Calculated.AddPaymentsId " +
                    "where " +
                        "Emploee.PersonnelNumber = " + PersonnelNumber; ;
            }
            SqlDataReader reader;
            using (SqlConnection connection = new SqlConnection(PersonalCard.ConnectionString))
            {
                await connection.OpenAsync();
                SqlCommand command = new SqlCommand(query, connection);
                reader = await command.ExecuteReaderAsync();

                while (await reader.ReadAsync())
                {
                    tbPersoneelNumber.Text = reader["ТН"].ToString();
                    tbSurname.Text = reader["Sur"].ToString();
                    tbNaame.Text = reader["neme"].ToString();
                    tbmidleName.Text = reader["MidleName"].ToString();
                    tbExpiriance.Text = reader["Exp"].ToString();
                    tbEmplooeDate.Value = Convert.ToDateTime(reader["EmpDate"]);
                    tbDateOfBidth.Value = Convert.ToDateTime(reader["Birth"]);
                    cbDepartment.SelectedValue = reader["Dep"];
                    cbPosition.SelectedValue = reader["Positions"];
                    tbPrize.Text = reader["Prize"].ToString();
                    tbProcentAdditional.Text = reader["Procent"].ToString();
                    cbadditionalPayments.SelectedValue = reader["AddPay"];
                    
                    if (ContinePromotions())
                    {
                        cbKindOfPromotion.SelectedValue = reader["Promotion"];
                        tbDocumentDate.Value = Convert.ToDateTime(reader["DocDate"]);
                        tbDocumentNumber.Text = reader["DocNumber"].ToString();
                        tbCouse.Text = reader["Cause"].ToString();
                        tbMoney.Text = reader["Amount"].ToString();
                    }

                    tbAdress.Text = reader["Adress"].ToString();
                    tbPhone.Text = reader["Phone"].ToString();
                    tbEmail.Text = reader["Email"].ToString();

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

        private void ChengeRecords_Load(object sender, EventArgs e)
        {
            this.additionPaymentsTableAdapter.Fill(this.emploeesDataSet.AdditionPayments);
            this.kindofPromotionTableAdapter.Fill(this.emploeesDataSet.KindofPromotion);
            this.positionsTableAdapter.Fill(this.emploeesDataSet.Positions);
            this.departmentTableAdapter.Fill(this.emploeesDataSet.Department);
            
            LoadData();
        }

        private void button1_Click(object sender, EventArgs e) => Close();
        private bool ContinePromotions()
        {
            string query = "SELECT * FROM [Promotions] WHERE [Promotions].PersonnelNumber = " + PersonnelNumber;
            using (SqlConnection connection = new SqlConnection(PersonalCard.ConnectionString))
            {
                 connection.Open();

                SqlCommand command = new SqlCommand(query, connection);
                SqlDataReader reader = command.ExecuteReader();

                if (reader.HasRows)
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
        }

        private void cbGender_SelectedIndexChanged(object sender, EventArgs e)
        {
        }

        private void cbPosition_SelectedIndexChanged(object sender, EventArgs e)
        {
            try
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
            catch (Exception)
            {

            }
            
        }

        private async void UpdateEmploee()
        {
            
            string cmd =
                "UPDATE [dbo].[Emploee] " +
                "SET " +
                    $"[Surname] = '{tbSurname.Text}', " +
                    $"[Name] = '{tbNaame.Text}', " +
                    $"[MidleName] = '{tbmidleName.Text}', " +
                    $"[Gender] = '{cbGender.Text}', " +
                    $"[FamilyStatus] = '{cbFamilyStatus.Text}', " +
                    $"[Experience] = {tbExpiriance.Text}, " +
                    $"[EmploymentDate] = CONVERT(DATE, '{tbEmplooeDate.Value.ToShortDateString()}', 131), " +
                    $"[DateOfBirth] = CONVERT(DATE, '{tbDateOfBidth.Value.ToShortDateString()}', 131), " +
                    $"[DepartmentNumber] = {cbDepartment.SelectedValue}, " +
                    $"[PositionId] = {cbPosition.SelectedValue} " +
                $"WHERE " +
                    $"PersonnelNumber =  {tbPersoneelNumber.Text}";
            
            using (SqlConnection connection = new SqlConnection(PersonalCard.ConnectionString))
            {
                await connection.OpenAsync();
                SqlCommand command = new SqlCommand(cmd, connection);
                await command.ExecuteNonQueryAsync();
            }
        }

        private async void UpdateCalculated()
        {
            string cmd =
                "UPDATE " +
                    "[dbo].[Calculated] " +
                "SET " +
                    $"[Prize] = {tbPrize.Text.Replace(',', '.')}, " +
                    $"[ProcentSeniorityBonus] = {tbProcentAdditional.Text.Replace(',', '.')}, " +
                    $"[AddPaymentsId] = {cbadditionalPayments.SelectedValue} " +
                "WHERE " +
                    $"PersonnelNumber = {tbPersoneelNumber.Text}";

            using (SqlConnection connection = new SqlConnection(PersonalCard.ConnectionString))
            {
                await connection.OpenAsync();
                SqlCommand command = new SqlCommand(cmd, connection);
                await command.ExecuteNonQueryAsync();
            }
        }
        private async void UpdateImage()
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
        private async void UpdateContactInformation()
        {
            string cmd =
                "UPDATE " +
                    "[dbo].[ContactInformation] " +
                "SET " +
                    $"[AddressOfResidence] = '{tbAdress.Text}', " +
                    $"[Phone] = {tbPhone.Text.Replace("-", "")}, " +
                    $"[Email] = '{tbEmail.Text}' " +
                "WHERE " +
                    $"[PersonnelNumber] = {tbPersoneelNumber.Text}";

            using (SqlConnection connection= new SqlConnection(PersonalCard.ConnectionString))
            {
                await connection.OpenAsync();
                SqlCommand command = new SqlCommand(cmd, connection);
                await command.ExecuteNonQueryAsync();
            }
        }
        private async void UpdatePromotions()
        {
            string cmd =
                "UPDATE " +
                    "[dbo].[Promotions] " +
                "SET" +
                    $"[IdKindofPromotion] = {cbKindOfPromotion.SelectedValue}, " +
                    $"[Cause] = '{tbCouse.Text}', " +
                    $"[Date] = CONVERT(DATE, '{tbDocumentDate.Value.ToShortDateString()}', 131), " +
                    $"[DocumentNumber] = {tbDocumentNumber.Text}, " +
                    $"[AmountOfMoney] = {tbMoney.Text.Replace(',', '.')} " +
                "WHERE " +
                    $"[PersonnelNumber] = {tbPersoneelNumber.Text}";

            using (SqlConnection connection = new SqlConnection(PersonalCard.ConnectionString))
            {
                await connection.OpenAsync();
                SqlCommand command = new SqlCommand(cmd, connection);
                await command.ExecuteNonQueryAsync();
            }
        }
        private void btnAdd_Click(object sender, EventArgs e)
        {
            try
            {
                UpdateEmploee();
                UpdateCalculated();
                if (!String.IsNullOrWhiteSpace(textBox1.Text))
                    UpdateImage();
                UpdateContactInformation();
                if (!String.IsNullOrWhiteSpace(tbCouse.Text))
                    UpdatePromotions();
                

                AddForm form = new AddForm();
                MessageBox.Show("Запись успешно обновлена", "Сообщение", MessageBoxButtons.OK, MessageBoxIcon.Information);
                form.RefreshDataSet();
                Close();
            }
            catch (Exception)
            {
                MessageBox.Show("Ошибка! Не удальсь обновить данные", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
        }

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
        
    }
}