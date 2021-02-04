using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.IO;
using System.Windows.Forms;
using iTextSharp.text;
using iTextSharp.text.pdf;

namespace CourseWork
{
    public partial class NavigationForm : Form
    {
        public static void ExportToPdf(DataGridView dgv, string fileName)
        {
            BaseFont baseFont = 
                BaseFont.CreateFont("C:\\Windows\\Fonts\\arial.ttf", BaseFont.IDENTITY_H, BaseFont.NOT_EMBEDDED);
            PdfPTable table = new PdfPTable(dgv.Columns.Count);
            table.DefaultCell.Padding = 3;
            table.WidthPercentage = 100;
            table.HorizontalAlignment = Element.ALIGN_JUSTIFIED;
            table.DefaultCell.BorderWidth = 1;

            iTextSharp.text.Font font = new Font(baseFont, 10, iTextSharp.text.Font.NORMAL);
            //Заголовки столбцов
            foreach (DataGridViewColumn column in dgv.Columns)
            {
                PdfPCell cell = new PdfPCell(new Phrase(column.HeaderText, font));
                cell.BackgroundColor = new iTextSharp.text.BaseColor(240, 240, 240);
                table.AddCell(cell);
            }

            //Строки с данными
            foreach (DataGridViewRow row in dgv.Rows) 
            {
                foreach (DataGridViewCell cell in row.Cells)
                {
                    table.AddCell(new Phrase(cell.Value.ToString(), font));
                }
            }

            //Сохрание файла
            SaveFileDialog saveFile = new SaveFileDialog();
            saveFile.FileName = fileName;
            saveFile.Filter = "PDF файл | *.pdf";
            saveFile.DefaultExt = ".pdf";
            if (saveFile.ShowDialog() == DialogResult.OK)
            {
                using (FileStream stream = new FileStream(saveFile.FileName, FileMode.Create))
                {
                    Document pdfDocument = new Document(PageSize.A4, 10f, 10f, 10f, 0f);

                    PdfWriter.GetInstance(pdfDocument, stream);
                    pdfDocument.Open();
                    pdfDocument.Add(table);
                    pdfDocument.Close();
                    stream.Close();
                }
            }
        }

        public NavigationForm()
        {
            InitializeComponent();
        }

        public static DataTable table;
        private SqlDataAdapter adapter;

        public void LoadData()
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

                table = new DataTable();
                adapter = new SqlDataAdapter(command);
                adapter.Fill(table);
                dataGridView1.DataSource = table;

                dataGridView1.AutoResizeColumns();
                dataGridView1.ReadOnly = true;
                dataGridView1.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            }
        }
        private void NavigationForm_Load(object sender, EventArgs e)
        {
            LoadData();
            comboBox1.SelectedIndex = 2;
            
        }
        private  void btnMoreInfo_Click(object sender, EventArgs e)
        {
            PersonalCard personalCard = new PersonalCard(GetPersonnelNumber());
            personalCard.ShowDialog();
        }
        private void button4_Click(object sender, EventArgs e) => Close();
        private int GetPersonnelNumber()
        {
            var val = dataGridView1[0, dataGridView1.CurrentCell.RowIndex].Value;
            string query =
                "select top(1)" +
                    "Emploee.PersonnelNumber as 'PS'" +
                "from Emploee " +
                "where [Emploee].[Surname] + ' ' + Emploee.[Name] + ' ' + Emploee.MidleName " +
                $"= '{val}'";
            using (SqlConnection connection = new SqlConnection(PersonalCard.ConnectionString))
            {
                connection.Open();
                SqlCommand command = new SqlCommand(query, connection);
                SqlDataReader reader = command.ExecuteReader();
                while (reader.Read())
                {
                    return Convert.ToInt32(reader["PS"]);
                }
            }
            return 0;
        }
        private void button3_Click(object sender, EventArgs e)
        {
            string cmd = "";
            if (ContinesPronotions() == true)
            {
                cmd = $"delete from Calculated where PersonnelNumber={GetPersonnelNumber()}; " +
                      $"delete from Promotions where PersonnelNumber = {GetPersonnelNumber()}; " +
                      $"delete from Emploee where PersonnelNumber = {GetPersonnelNumber()}; " +
                      $"delete from ContactInformation where PersonnelNumber={GetPersonnelNumber()};";
            }
            else
            {
                cmd = $"delete from Calculated where PersonnelNumber = {GetPersonnelNumber()}; " +
                      $"delete from Emploee where PersonnelNumber = {GetPersonnelNumber()}; " +
                      $"delete from ContactInformation where PersonnelNumber={GetPersonnelNumber()};";
            }
            using (SqlConnection connection = new SqlConnection(PersonalCard.ConnectionString))
            {
                connection.Open();
                SqlCommand command = new SqlCommand(cmd, connection);
                if (DialogResult.OK == MessageBox.Show("Вы уверны что хотите удалить запись?",
                    "Подтверждение", MessageBoxButtons.OKCancel, MessageBoxIcon.Question))
                {
                    command.ExecuteNonQuery();
                    AddForm form = new AddForm();
                    form.RefreshDataSet();
                }
                else
                {
                    return;
                }
            }

        }
        private bool ContinesPronotions()
        {
            var persNum = GetPersonnelNumber();
            string cmd = "SELECT * FROM [Promotions] WHERE [Promotions].PersonnelNumber = " + persNum;

            using (SqlConnection connection = new SqlConnection(PersonalCard.ConnectionString))
            {
                connection.Open();
                SqlCommand command = new SqlCommand(cmd, connection);
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
        private void button1_Click(object sender, EventArgs e)
        {
            AddForm form = new AddForm();
            form.ShowDialog();
        }
        private async void textBox1_TextChanged(object sender, EventArgs e)
        {
            if (textBox1.Text == "")
            {
                LoadData();
            }
            else
            {
                string searh = '%' + textBox1.Text + '%';
                string query =
                    "SELECT DISTINCT " +
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
                         "(Positions.PositionId = Emploee.PositionId) " +
                            "and " +
                        $"(([Emploee].[Surname] like '{searh}')" +
                            $"or " +
                        $"([Emploee].[Name] like '{searh}') " +
                            $"or " +
                        $"([Emploee].[MidleName] like '{searh}')) " +
                    "ORDER BY ФИО";

                using (SqlConnection connection = new SqlConnection(PersonalCard.ConnectionString))
                {
                    await connection.OpenAsync();

                    SqlCommand command = new SqlCommand(query, connection);

                    table = new DataTable();
                    adapter = new SqlDataAdapter(command);
                    table.Clear();
                    adapter.Fill(table);
                    dataGridView1.DataSource = table;
                }
            }


        }
        private void button2_Click(object sender, EventArgs e)
        {
            ChengeRecords chengeRecords = new ChengeRecords(GetPersonnelNumber());
            chengeRecords.Show();
        }

        public static DataTable table2;
        private SqlDataAdapter adapter2;

        private async void tabDepartment_Enter(object sender, EventArgs e)
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
                adapter2 = new SqlDataAdapter(command);
                table2 = new DataTable();
                adapter2.Fill(table2);
                dataGridView2.DataSource = table2;
                dataGridView2.AutoResizeColumns();
                dataGridView2.ReadOnly = true;
                dataGridView2.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            }
        }
        private void button9_Click(object sender, EventArgs e) => Close();
        private void button6_Click_1(object sender, EventArgs e)
        {
            ChengeDepartment chengeDepartment = new ChengeDepartment(ChengeDepartment.FormTarget.Add, vs);
            chengeDepartment.ShowDialog();
        }

        public static List<string> vs = new List<string>();

        private void button7_Click(object sender, EventArgs e)
        {
            var row = dataGridView2.CurrentCell.RowIndex;

            ChengeDepartment chengeDepartment = new ChengeDepartment(ChengeDepartment.FormTarget.Chenge, vs);
            vs.Add(dataGridView2.Rows[row].Cells[0].Value.ToString());
            vs.Add(dataGridView2.Rows[row].Cells[1].Value.ToString());
            vs.Add(dataGridView2.Rows[row].Cells[2].Value.ToString());
            vs.Add(dataGridView2.Rows[row].Cells[3].Value.ToString());
            vs.Add(dataGridView2.Rows[row].Cells[4].Value.ToString());
            vs.Add(dataGridView2.Rows[row].Cells[5].Value.ToString());
            ChengeDepartment.Values = vs;
            chengeDepartment.ShowDialog();
        }
        private void button8_Click(object sender, EventArgs e)
        {
            var row = dataGridView2.CurrentCell.RowIndex;
            string cmd =
                "DELETE FROM [Department] where [DepartmentNumber] = " + dataGridView2.Rows[row].Cells[0].Value.ToString();

            if (DialogResult.Yes == MessageBox.Show("Вы действительно хотите удалить запись?",
                "Сообщение", MessageBoxButtons.YesNo, MessageBoxIcon.Question))
            {
                using (SqlConnection connection = new SqlConnection(PersonalCard.ConnectionString))
                {
                    connection.Open();
                    SqlCommand command = new SqlCommand(cmd, connection);
                    try
                    {
                        command.ExecuteNonQuery();
                    }
                    catch (Exception)
                    {
                        MessageBox.Show("Запись не может быть удалена.", "Ошибка",
                            MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }

                ChengeDepartment department = new ChengeDepartment(ChengeDepartment.FormTarget.Add, vs);
                department.RefreshDataSet();
            }
        }
        private void tabControl1_Enter(object sender, EventArgs e)
        {

        }

        public static DataTable table3;

        private void tabPage1_Enter(object sender, EventArgs e)
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
                table3 = new DataTable();
                adapter.Fill(table3);
                dataGridView3.DataSource = table3;
            }

            dataGridView3.AutoResizeColumns();
            dataGridView3.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.AllCells;
            dataGridView3.ReadOnly = true;
            dataGridView3.SelectionMode = DataGridViewSelectionMode.FullRowSelect;

        }
        private void tabPage1_Click(object sender, EventArgs e)
        {

        }
        private void button10_Click(object sender, EventArgs e) => Close();
        private async void button11_Click(object sender, EventArgs e)
        {
            var id = dataGridView3.Rows[dataGridView3.CurrentCell.RowIndex].Cells[0].Value.ToString();
            string cmd = "DELETE FROM [dbo].[Positions] WHERE PositionId = " + id;

            if (DialogResult.Yes == MessageBox.Show("Вы действительно хотите удалить запись",
                "Сообщение", MessageBoxButtons.YesNo, MessageBoxIcon.Question))
            {

            }
            using (SqlConnection connection = new SqlConnection(PersonalCard.ConnectionString))
            {
                await connection.OpenAsync();
                SqlCommand command = new SqlCommand(cmd, connection);
                try
                {
                    await command.ExecuteNonQueryAsync();
                    MessageBox.Show("Запись успешно удалена.", "Сообщения",
                        MessageBoxButtons.OK, MessageBoxIcon.Information);
                    ChengeRecordsInPositions chenge = new ChengeRecordsInPositions(ChengeRecordsInPositions.FormtTarget.Add, values);
                    chenge.RefresfDataset();
                }
                catch (Exception)
                {
                    MessageBox.Show("Не удалось удалить запись.", "Сообщейние",
                        MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }
            }
        }
        private void button13_Click(object sender, EventArgs e)
        {
            ChengeRecordsInPositions positions = new ChengeRecordsInPositions(ChengeRecordsInPositions.FormtTarget.Add, values);
            positions.ShowDialog();
        }

        private List<string> values = new List<string>();

        private void button12_Click(object sender, EventArgs e)
        {

            values.Add(dataGridView3.Rows[dataGridView3.CurrentCell.RowIndex].Cells[0].Value.ToString());
            values.Add(dataGridView3.Rows[dataGridView3.CurrentCell.RowIndex].Cells[1].Value.ToString());
            values.Add(dataGridView3.Rows[dataGridView3.CurrentCell.RowIndex].Cells[2].Value.ToString());

            ChengeRecordsInPositions positions = new ChengeRecordsInPositions(ChengeRecordsInPositions.FormtTarget.Chenge, values);
            positions.ShowDialog();
        }
        private void button5_Click(object sender, EventArgs e)
        {
            ExportToPdf(dataGridView1, "Сотрудники организации");
        }

        public static DataTable table4;

        private async void tabPage2_Enter(object sender, EventArgs e)
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
                table4 = new DataTable();
                SqlDataAdapter adapter = new SqlDataAdapter(comm);

                adapter.Fill(table4);
                dataGridView4.DataSource = table4;

                dataGridView4.AutoResizeColumns();
                dataGridView4.ReadOnly = true;
                dataGridView4.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            }
        }
        private void button18_Click(object sender, EventArgs e)
        {
            var payments = new ChengeAdditionalPayments(new List<string>(), ChengeAdditionalPayments.FormTarget.Add);
            payments.ShowDialog();
        }
        private void button17_Click(object sender, EventArgs e)
        {
            List<string> values2 = new List<string>();
            var row = dataGridView4.CurrentCell.RowIndex;
            values2.Add(dataGridView4.Rows[row].Cells[0].Value.ToString());
            values2.Add(dataGridView4.Rows[row].Cells[1].Value.ToString());
            values2.Add(dataGridView4.Rows[row].Cells[2].Value.ToString());

            var payments = new ChengeAdditionalPayments(values2, ChengeAdditionalPayments.FormTarget.Chenge);
            payments.ShowDialog();
        }
        private void button15_Click(object sender, EventArgs e) => Close();
        private async void button16_Click(object sender, EventArgs e)
        {
            var row = dataGridView4.CurrentCell.RowIndex;
            string cmd = "DELETE FROM [dbo].[AdditionPayments] WHERE AddPaymentsId = "
                + dataGridView4.Rows[row].Cells[0].Value.ToString();

            if (DialogResult.Yes == MessageBox.Show("Вы действительно хотите удалить эту запись?"
                , "Подтверждение", MessageBoxButtons.YesNo))
            {
                using (SqlConnection connection = new SqlConnection(PersonalCard.ConnectionString))
                {
                    await connection.OpenAsync();
                    SqlCommand command = new SqlCommand(cmd, connection);
                    try
                    {
                        await command.ExecuteNonQueryAsync();
                    }
                    catch (Exception)
                    {
                        MessageBox.Show("Не удалось удалить запись. Данный вид поплат уже присвоен одному из рабоников", "Сообщение", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        return;
                    }
                }
            }
            
        }

        private void button14_Click(object sender, EventArgs e)
        {
            ReportForm reportForm = new ReportForm();
            
            switch (comboBox1.SelectedIndex)
            {
                case 0:
                    //Принятие на работу
                    XtraReport1 report1 = new XtraReport1();
                    reportForm.documentViewer1.DocumentSource = report1;
                    reportForm.ShowDialog();
                    break;

                case 1:
                    //Доход сотрудников
                    XtraReport2 report2 = new XtraReport2();
                    reportForm.documentViewer1.DocumentSource = report2;
                    reportForm.ShowDialog();
                    break;
                
                case 2:
                    //Общая информация
                    XtraReport3 report3 = new XtraReport3();
                    reportForm.documentViewer1.DocumentSource = report3;
                    reportForm.ShowDialog();
                    break;

                default:
                    goto case 2;
            }
        }

        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {

        }
    }
}