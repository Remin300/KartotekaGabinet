using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Windows.Forms;

namespace KartotekaGabinet
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }
        // Dane dla bazy danych Burian2015
        string tableName = "kartoteka_nazwiska";
        string tableName2 = "WIZYTY";
        string connectionString = "Server = 127.0.0.1; Database=C:\\Users\\user\\Desktop\\trening\\KARTOTEKA GABINET - Aplikacja okienkowa\\KartotekaGabinet 2.2 GL\\Kartoteka.fdb;User=SYSDBA;Password=masterkey;";


        private void Form1_Load(object sender, EventArgs e)
        {
            FirebirdDBHandler dbHandler = new FirebirdDBHandler(connectionString);
            DataTable searchResults = dbHandler.ShowTable(tableName, "NAZWISKO_IMIE");

            dataGridView1.DataSource = searchResults;
            dataGridView1.Sort(dataGridView1.Columns[1], ListSortDirection.Ascending);

            // Ustawienie szerokości kolumn
            dataGridView1.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.AllCells; // Dopasowanie szerokości do zawartości komórek
            dataGridView2.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.AllCells;

            dataGridView1.Columns[0].Visible = false;

        }

        private void buttonSearchPatient_Click(object sender, EventArgs e)
        {
            string searchPhrase = textBoxSearch.Text;
            FirebirdDBHandler dbHandler = new FirebirdDBHandler(connectionString);
            if (searchPhrase == string.Empty)
            {
                MessageBox.Show("Prosze wpisać NAZWISKO oraz IMIĘ");
            }
            else
            {
                DataTable searchResults = dbHandler.SearchPhrase(tableName, "NAZWISKO_IMIE", searchPhrase.ToUpper());

                dataGridView1.DataSource = searchResults;
                dataGridView2.DataSource = null;
                label1.Text = string.Empty;
            }

        }

        private void buttonAddPatient_Click(object sender, EventArgs e)
        {

            // Tworzenie nowego okna do wprowadzania danych
            using (Form2 inputNewDataForm = new Form2())
            {
                // Wyświetlenie okna i sprawdzenie, czy użytkownik kliknął przycisk OK
                if (inputNewDataForm.ShowDialog() == DialogResult.OK)
                {
                    FirebirdDBHandler dbHandler = new FirebirdDBHandler(connectionString);
                    List<string> newDataList = new List<string> {inputNewDataForm.InputName , inputNewDataForm.InputPhone,
                                                                    inputNewDataForm.InputPesel, inputNewDataForm.InputAdress,inputNewDataForm.InputNip,
                                                                       inputNewDataForm.InputBirthDate, inputNewDataForm.InputComments};

                    List<string> columnList = new List<string> { "nazwisko_imie", "telefon", "pesel", "adres", "nip", "data_urodzenia", "uwagi" };


                    dbHandler.AddRowToDatabaseAndDataGridView2(dataGridView1, tableName, newDataList, columnList);
                    dataGridView1.Refresh();

                    MessageBox.Show($"Dodano pacjenta!");

                    DataTable searchResults = dbHandler.SearchPhrase(tableName, "NAZWISKO_IMIE", inputNewDataForm.InputName.ToUpper());

                    dataGridView1.DataSource = searchResults;

                    dataGridView1.ClearSelection(); // Wyczyszczenie bieżącego zaznaczenia
                    if (dataGridView1.Rows.Count > 0) // Sprawdzenie, czy istnieją wiersze w DataGridView
                    {
                        dataGridView1.Rows[0].Selected = true; // Zaznaczenie pierwszego wiersza
                    }
                    //wyciągnięcie z głównej kartoteki ID_PACJENTA aby wyświtlić wszystko z tym primary key w tabeli Wizyty
                    string firstColumnValue = dataGridView1.SelectedRows[0].Cells[0].Value.ToString();

                    DataTable searchResults2 = dbHandler.ShowDataByPrimaryKey(tableName2, "id_pacjenta", firstColumnValue);

                    dataGridView2.DataSource = searchResults2;

                    dataGridView2.Columns[0].Visible = false;
                    dataGridView2.Columns[1].Visible = false;

                    // ustawienie nazwiska nad dataGridView2
                    string nameValue = dataGridView1.SelectedRows[0].Cells[1].Value.ToString();
                    label1.Visible = true;
                    label1.Text = nameValue;

                }

            }

        }

        private void buttonDelateRow_Click(object sender, EventArgs e)
        {
            FirebirdDBHandler dbHandler = new FirebirdDBHandler(connectionString);

            // ID_PACJENTA to pierwsza kolumna w bazie danych. Jest potrzebna do metody DeleteSeletctedRow.
            dbHandler.DeleteSelectedRowFromDatabase(dataGridView1, tableName, "ID_PACJENTA");
            dataGridView2.DataSource = null;
            label1.Text = string.Empty;
        }

        private void buttonAddVisit_Click(object sender, EventArgs e)
        {
            if (dataGridView1.SelectedRows.Count == 0)
            {
                MessageBox.Show("ZAZNACZ PACJENTA!");
                return;
            }
            try
            {

                //wyciągnięcie z głównej kartoteki ID_PACJENTA
                string firstColumnValue = dataGridView1.SelectedRows[0].Cells[0].Value.ToString();
                string name = dataGridView1.SelectedRows[0].Cells[1].Value.ToString();

                // Tworzenie nowego okna do wprowadzania danych
                using (Form3 inputNewDataForm = new Form3(name))
                {
                    // Wyświetlenie okna i sprawdzenie, czy użytkownik kliknął przycisk OK
                    if (inputNewDataForm.ShowDialog() == DialogResult.OK)
                    {
                        FirebirdDBHandler dbHandler = new FirebirdDBHandler(connectionString);

                        List<string> newDataList = new List<string> {firstColumnValue ,inputNewDataForm.InputDate , inputNewDataForm.InputSurface,
                                                                    inputNewDataForm.InputComment, inputNewDataForm.InputMaterial };

                        List<string> columnList = new List<string> { "ID_PACJENTA", "DATA_WIZYTY", "ZAB_NR", "OPIS", "MATERIAL" };

                        dbHandler.AddRowToDatabaseAndDataGridView2(dataGridView2, tableName2, newDataList, columnList);

                        DataTable searchResults = dbHandler.ShowDataByPrimaryKey(tableName2, "id_pacjenta", firstColumnValue);

                        dataGridView2.DataSource = searchResults;

                        dataGridView2.DataSource = searchResults;
                        dataGridView2.Columns[0].Visible = false;
                        dataGridView2.Columns[1].Visible = false;

                    }

                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void buttonDelateVisit_Click(object sender, EventArgs e)
        {
            string tableName2 = "WIZYTY";
            FirebirdDBHandler dbHandler = new FirebirdDBHandler(connectionString);

            // ID_PACJENTA to pierwsza kolumna w bazie danych. Jest potrzebna do metody DeleteSeletctedRow.
            dbHandler.DeleteSelectedRowFromDatabase(dataGridView2, tableName2, "ID_WIZYTY");
        }

        private void buttonRefresh_Click(object sender, EventArgs e)
        {
            FirebirdDBHandler dbHandler = new FirebirdDBHandler(connectionString);
            DataTable searchResults = dbHandler.ShowTable(tableName, "NAZWISKO_IMIE");

            dataGridView1.DataSource = searchResults;
            dataGridView2.DataSource = null;
            label1.Text = string.Empty;

        }

        private void buttonUpdate_Click(object sender, EventArgs e)
        {
            if (dataGridView1.SelectedRows.Count == 0)
            {
                MessageBox.Show("ZAZNACZ PACJENTA!");
                return;
            }
            // Tworzenie nowego okna do wprowadzania danych

            string firstColumnValue = dataGridView1.SelectedRows[0].Cells[0].Value.ToString();

            string fullName = dataGridView1.SelectedRows[0].Cells[1].Value.ToString();
            // nazwisko i imię dziele na dwie części
            string[] parts = fullName.Split(' ');
            string firstName = parts[0];
            string name = parts[1];

            string telefon = dataGridView1.SelectedRows[0].Cells[2].Value.ToString();
            string pesel = dataGridView1.SelectedRows[0].Cells[3].Value.ToString();
            string adres = dataGridView1.SelectedRows[0].Cells[4].Value.ToString();
            string nip = dataGridView1.SelectedRows[0].Cells[5].Value.ToString();
            string bdate = dataGridView1.SelectedRows[0].Cells[6].Value.ToString();
            string uwagi = dataGridView1.SelectedRows[0].Cells[7].Value.ToString();
            List<string> DataList = new List<string> { firstName, name, telefon, pesel, adres, nip, bdate, uwagi };



            using (Form2 inputNewDataForm = new Form2(DataList))
            {
                // Wyświetlenie okna i sprawdzenie, czy użytkownik kliknął przycisk OK
                if (inputNewDataForm.ShowDialog() == DialogResult.OK)
                {
                    FirebirdDBHandler dbHandler = new FirebirdDBHandler(connectionString);
                    List<string> newDataList = new List<string> {inputNewDataForm.InputName , inputNewDataForm.InputPhone,
                                                                    inputNewDataForm.InputPesel, inputNewDataForm.InputAdress,inputNewDataForm.InputNip,
                                                                       inputNewDataForm.InputBirthDate, inputNewDataForm.InputComments};

                    List<string> columnList = new List<string> { "nazwisko_imie", "telefon", "pesel", "adres", "nip", "data_urodzenia", "uwagi" };


                    dbHandler.UpdateData(tableName, "ID_PACJENTA", firstColumnValue, newDataList, columnList);

                    DataTable searchResults = dbHandler.SearchPhrase(tableName, "NAZWISKO_IMIE", inputNewDataForm.InputName.ToUpper());

                    dataGridView1.DataSource = searchResults;

                }

            }
        }

        private void buttonVisitUpdate_Click(object sender, EventArgs e)
        {
            if (dataGridView2.SelectedRows.Count == 0)
            {
                MessageBox.Show("ZAZNACZ WIZYTĘ!");
                return;
            }
            // uzyskanie z tabeli danych potrzebnych do uzupełnienia okna Form3 

            string firstColumnValue = dataGridView2.SelectedRows[0].Cells[0].Value.ToString();
            string patientID = dataGridView2.SelectedRows[0].Cells[1].Value.ToString();

            string visitDate = dataGridView2.SelectedRows[0].Cells[2].Value.ToString();
            string surface = dataGridView2.SelectedRows[0].Cells[3].Value.ToString();
            string material = dataGridView2.SelectedRows[0].Cells[4].Value.ToString();
            string Comments = dataGridView2.SelectedRows[0].Cells[5].Value.ToString();
            string name = dataGridView1.SelectedRows[0].Cells[1].Value.ToString();

            List<string> DataList = new List<string> { visitDate, surface, material, Comments };


            // urzycie Form3 z konstruktorem przyjmującym listę jako argument
            using (Form3 inputNewDataForm = new Form3(DataList, name))
            {
                // Wyświetlenie okna i sprawdzenie, czy użytkownik kliknął guzik
                if (inputNewDataForm.ShowDialog() == DialogResult.OK)
                {
                    FirebirdDBHandler dbHandler = new FirebirdDBHandler(connectionString);

                    List<string> newDataList = new List<string> {inputNewDataForm.InputDate, inputNewDataForm.InputSurface,
                                                                 inputNewDataForm.InputComment, inputNewDataForm.InputMaterial};

                    List<string> columnList = new List<string> { "DATA_WIZYTY", "ZAB_NR", "OPIS", "MATERIAL" };


                    dbHandler.UpdateData(tableName2, "ID_WIZYTY", firstColumnValue, newDataList, columnList);

                    // pokazanie w kontrolce dataGridView2 zakutualizowanych wizyt dla konkretnego ID pacjenta
                    DataTable searchResults = dbHandler.ShowDataByPrimaryKey(tableName2, "id_pacjenta", patientID);

                    dataGridView2.DataSource = searchResults;

                    dataGridView2.DataSource = searchResults;


                }

            }
        }

        private void dataGridView1_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            //zaznacz cały wiersz, na którym została wykonana akcja kliknięcia
            if (e.RowIndex >= 0)
            {
                dataGridView1.Rows[e.RowIndex].Selected = true;
            }
            try
            {
                //wyciągnięcie z głównej kartoteki ID_PACJENTA aby wyświtlić wszystko z tym primary key w tabeli Wizyty
                string firstColumnValue = dataGridView1.SelectedRows[0].Cells[0].Value.ToString();



                FirebirdDBHandler dbHandler = new FirebirdDBHandler(connectionString);

                DataTable searchResults = dbHandler.ShowDataByPrimaryKey(tableName2, "id_pacjenta", firstColumnValue);

                dataGridView2.DataSource = searchResults;

                dataGridView2.Columns[0].Visible = false;
                dataGridView2.Columns[1].Visible = false;

                // ustawienie nazwiska nad dataGridView2
                string nameValue = dataGridView1.SelectedRows[0].Cells[1].Value.ToString();
                label1.Visible = true;
                label1.Text = nameValue;
            }
            catch { }
        }

        private void dataGridView2_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            // Zaznacz cały wiersz, na którym została wykonana akcja kliknięcia
            if (e.RowIndex >= 0)
            {
                dataGridView2.Rows[e.RowIndex].Selected = true;
            }
        }
    }
}
