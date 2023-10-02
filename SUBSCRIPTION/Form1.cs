using System.Data;
using Npgsql;

namespace SUBSCRIPTION
{
    public partial class Form1 : Form
    {        
        private NpgsqlConnection _connection;

        private NpgsqlCommandBuilder _command;

        private NpgsqlDataAdapter _dataAdapter;

        private DataSet _dataSet;

        private bool _newRowAdding = false;
        
        public Form1()
        {
            InitializeComponent();
        }

        private void toolStripButton1_Click(object sender, EventArgs e)
        {
            ReloadData();
        }

        private void LoadData()
        {
            try
            {
                _dataAdapter = new NpgsqlDataAdapter("SELECT *, 'Delete' AS Command FROM books", 
                                                      _connection);
                _command = new NpgsqlCommandBuilder(_dataAdapter);

                _command.GetDeleteCommand();
                _command.GetUpdateCommand();
                _command.GetInsertCommand();

                _dataSet = new DataSet();
                _dataAdapter.Fill(_dataSet, "books");
                dataGridView1.DataSource = _dataSet.Tables["books"];
                for (int i = 0; i < dataGridView1.Rows.Count; i++)
                {
                    DataGridViewLinkCell cell = new DataGridViewLinkCell();
                    dataGridView1[7, i] = cell;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Ошибка!", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void ReloadData()
        {
            try
            {
                _dataSet.Tables["books"].Clear();
                _dataAdapter.Fill(_dataSet, "books");
                dataGridView1.DataSource = _dataSet.Tables["books"];
                for (int i = 0; i < dataGridView1.Rows.Count; i++)
                {
                    DataGridViewLinkCell cell = new DataGridViewLinkCell();
                    dataGridView1[7, i] = cell;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Ошибка!", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            _connection = new NpgsqlConnection(
                                    "Host=localhost;" +
                                    "Username=postgres;" +
                                    "Password=sds61224;" +
                                    "Database=subscription"
                                    );
            _connection.Open();

            LoadData();
        }

        private void dataGridView1_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            try
            {
                if (e.ColumnIndex == 7)
                {
                    string task = dataGridView1.Rows[e.RowIndex].Cells[7].Value.ToString(); //9
                    if (task == "Delete")
                    {
                        if (MessageBox.Show("Удалить эту строку?", "Удаление", 
                            MessageBoxButtons.YesNo, MessageBoxIcon.Question)
                            == DialogResult.Yes)
                        {
                            int rowIndex = e.RowIndex;
                            dataGridView1.Rows.RemoveAt(rowIndex);
                            _dataSet.Tables["books"].Rows[rowIndex].Delete();
                            _dataAdapter.Update(_dataSet, "books");
                        }
                    }
                    else if (task == "Insert")
                    {
                        int rowIndex = dataGridView1.Rows.Count - 2;
                        DataRow row = _dataSet.Tables["books"].NewRow();

                        row[0] = dataGridView1.Rows[rowIndex].Cells[0].Value;
                        row[1] = dataGridView1.Rows[rowIndex].Cells[1].Value;
                        row[2] = dataGridView1.Rows[rowIndex].Cells[2].Value;
                        row[3] = dataGridView1.Rows[rowIndex].Cells[3].Value;
                        row[4] = dataGridView1.Rows[rowIndex].Cells[4].Value;
                        row[5] = dataGridView1.Rows[rowIndex].Cells[5].Value;
                        row[6] = dataGridView1.Rows[rowIndex].Cells[6].Value;

                        _dataSet.Tables["books"].Rows.Add(row);
                        _dataSet.Tables["books"].Rows.RemoveAt(_dataSet.Tables["books"].Rows.Count - 1);
                        dataGridView1.Rows.RemoveAt(dataGridView1.Rows.Count - 2);
                        dataGridView1.Rows[e.RowIndex].Cells[7].Value = "Delete";
                        _dataAdapter.Update(_dataSet, "books");
                        _newRowAdding = false;
                    }
                    else if (task == "Update")
                    {
                        int r = e.RowIndex;

                        _dataSet.Tables["books"].Rows[r][0] = dataGridView1.Rows[r].Cells[0].Value;
                        _dataSet.Tables["books"].Rows[r][1] = dataGridView1.Rows[r].Cells[1].Value;
                        _dataSet.Tables["books"].Rows[r][2] = dataGridView1.Rows[r].Cells[2].Value;
                        _dataSet.Tables["books"].Rows[r][3] = dataGridView1.Rows[r].Cells[3].Value;
                        _dataSet.Tables["books"].Rows[r][4] = dataGridView1.Rows[r].Cells[4].Value;
                        _dataSet.Tables["books"].Rows[r][5] = dataGridView1.Rows[r].Cells[5].Value;
                        _dataSet.Tables["books"].Rows[r][6] = dataGridView1.Rows[r].Cells[6].Value;

                        _dataAdapter.Update(_dataSet, "books");
                        dataGridView1.Rows[e.RowIndex].Cells[7].Value = "Delete";
                    }

                    ReloadData();
                }
            }
            catch
            {

            }
        }

        private void dataGridView1_UserAddedRow(object sender, DataGridViewRowEventArgs e)
        {
            try
            {
                if (_newRowAdding == false)
                {
                    _newRowAdding = true;
                    int lastRow = dataGridView1.Rows.Count - 2;
                    DataGridViewRow row = dataGridView1.Rows[lastRow];
                    DataGridViewLinkCell cell = new DataGridViewLinkCell();
                    dataGridView1[7, lastRow] = cell;
                    row.Cells["Command"].Value = "Insert";
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Ошибка!", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void dataGridView1_CellValueChanged(object sender, DataGridViewCellEventArgs e)
        {
            try
            {
                if (_newRowAdding == false)
                {
                    int rowIndex = dataGridView1.SelectedCells[0].RowIndex;
                    DataGridViewRow editingRow = dataGridView1.Rows[rowIndex];
                    DataGridViewLinkCell cell = new DataGridViewLinkCell();
                    dataGridView1[7, rowIndex] = cell;
                    editingRow.Cells["Command"].Value = "Update";
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Ошибка!", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            _connection.Close();
            _connection.Open();

            string bookId = textBox1.Text;
            
            NpgsqlCommand command = new NpgsqlCommand(
                                                  "SELECT books.bookid, heading.heading, books.author, books.name, books.information, books.isbn " +
                                                  "FROM books JOIN heading ON books.headingid = heading.id " +
                                                  $"WHERE bookid = '{bookId}';", 
                                                  _connection);
            NpgsqlDataReader reader = command.ExecuteReader();
            while (reader.Read())
            {
                string information = string.Empty;
                if (!reader.IsDBNull(4))
                {
                    information = reader.GetString(4);
                }
                
                string report = $"Номер издания: {reader.GetString(0)}\r\n" +
                                $"Рубрика: {reader.GetString(1)}\r\n" +
                                $"Автор(-ы): {reader.GetString(2)}\r\n" +
                                $"Название: {reader.GetString(3)}\r\n" +
                                $"Дополнительная информация: {information}\r\n" +
                                $"ISBN: {reader.GetString(5)}\r\n";
                textBox2.Text = report;
            }
        }
    }
}