using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using FirebirdSql.Data.FirebirdClient;

namespace KartotekaGabinet
{
    internal class FirebirdDBHandler
    {
        private string connectionString;

        public FirebirdDBHandler(string connectionString)
        {
            this.connectionString = connectionString;
        }

        /// <summary>
        /// Pokazuje bazę danych za pomocą "dataTable"
        /// </summary>
        /// <param name="tableName"></param>
        /// <returns>dataTable</returns>
        public DataTable ShowTable(string tableName, string columnName)
        {
            DataTable dataTable = new DataTable();
            string query = $"SELECT * FROM {tableName} ORDER BY {columnName} ASC";

            using (FbConnection connection = new FbConnection(connectionString))
            {
                try
                {
                    connection.Open();
                    using (FbCommand command = new FbCommand(query, connection))
                    {

                        using (FbDataAdapter adapter = new FbDataAdapter(command))
                        {
                            adapter.Fill(dataTable);
                        }
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Error: " + ex.Message);
                }
            }

            return dataTable;
        }

        /// <summary>
        /// Szuka wybranej frazy w bazie danych
        /// </summary>
        /// <param name="tableName"></param>
        /// <param name="columnName"></param>
        /// <param name="phrase"></param>
        /// <returns>dataTable</returns>
        public DataTable SearchPhrase(string tableName, string columnName, string phrase)
        {
            DataTable dataTable = new DataTable();
            string query = $"SELECT * FROM {tableName} WHERE {columnName} LIKE @phrase";


            using (FbConnection connection = new FbConnection(connectionString))
            {
                try
                {
                    connection.Open();
                    using (FbCommand command = new FbCommand(query, connection))
                    {
                        command.Parameters.Add("@phrase", FbDbType.VarChar).Value = $"{phrase}%";
                        using (FbDataAdapter adapter = new FbDataAdapter(command))
                        {
                            adapter.Fill(dataTable);
                        }
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Error: " + ex.Message);
                }
            }

            return dataTable;
        }


        /// <summary>
        /// Uaktualnia baze danych na podstawie zmian w kontrolce dataGridView
        /// </summary>
        /// <param name="dataGridView"></param>
        /// <param name="tableName"></param>
        public void UpdateDatabaseFromDataGridView(DataGridView dataGridView, string tableName)
        {
            string query = $"SELECT * FROM {tableName}";
            using (FbConnection connection = new FbConnection(connectionString))
            {
                try
                {
                    connection.Open();
                    using (FbDataAdapter adapter = new FbDataAdapter(query, connection))
                    {
                        FbCommandBuilder commandBuilder = new FbCommandBuilder(adapter);
                        adapter.UpdateCommand = commandBuilder.GetUpdateCommand();

                        DataTable dataTable = (DataTable)dataGridView.DataSource; // tabela edytowana w programie
                        adapter.Update(dataTable); //nadpisanie zmian dodanych w programie
                        MessageBox.Show("Uaktualniono Bazę danych!");
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Error: " + ex.Message);
                }
            }
        }


        /// <summary>
        /// Dodaje nowego pacjenta o wskazanych danych
        /// </summary>
        /// <param name="dataGridView"></param>
        /// <param name="tableName"></param>
        /// <param name="nameColumnValue"></param>
        /// <param name="otherColumnValues"></param>
        public void AddRowToDatabaseAndDataGridView2(DataGridView dataGridView, string tableName, List<string> newDataList, List<string> columnList)
        {
            // Tworzymy połączenie z bazą danych Firebird
            using (FbConnection connection = new FbConnection(connectionString))
            {
                try
                {
                    // Otwieramy połączenie
                    connection.Open();

                    // Tworzymy zapytanie SQL do wstawienia nowego wiersza
                    string columns = string.Join(",", columnList);
                    string values = string.Join(",", newDataList.Select((_, index) => $"@val{index}")); // Tworzymy listę parametrów w ilości równej indeksom listy

                    string query = $"INSERT INTO {tableName} ({columns}) VALUES ({values})";

                    // Tworzymy polecenie do wykonania zapytania SQL
                    using (FbCommand command = new FbCommand(query, connection))
                    {
                        // Dodajemy parametry do polecenia
                        for (int i = 0; i < newDataList.Count; i++)
                        {
                            command.Parameters.Add($"@val{i}", FbDbType.VarChar).Value = newDataList[i];
                        }

                        // Wykonujemy polecenie
                        int rowsAffected = command.ExecuteNonQuery();

                        // Jeżeli dodano wiersz, aktualizujemy DataTable powiązane z kontrolką DataGridView
                        if (rowsAffected > 0)
                        {
                            // Dodajemy wiersz do DataTable
                            DataTable dataTable = (DataTable)dataGridView.DataSource;
                            DataRow newRow = dataTable.NewRow();

                            // Ustawiamy wartości kolumn dla nowego wiersza
                            for (int i = 0; i < columnList.Count; i++)
                            {
                                newRow[columnList[i]] = newDataList[i];
                            }
                            dataTable.Rows.Add(newRow);

                            // Odświeżamy kontrolkę DataGridView
                            dataGridView.Refresh();
                        }
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Error: " + ex.Message);
                }
            }
        }


        /// <summary>
        /// Usuwa zaznaczoną w kontrolce "dataGridViev" pozycję
        /// </summary>
        /// <param name="dataGridView"></param>
        /// <param name="tableName"></param>
        /// <param name="primaryKeyColumnName"></param>
        public void DeleteSelectedRowFromDatabase(DataGridView dataGridView, string tableName, string primaryKeyColumnName)
        {
            // Sprawdź, czy został zaznaczony wiersz
            if (dataGridView.SelectedRows.Count == 0)
            {
                MessageBox.Show("Nie wybrano żadnej pozycji.");
                return;
            }

            // Pobierz wartość klucza głównego zaznaczonego wiersza
            object primaryKeyValue = dataGridView.SelectedRows[0].Cells[primaryKeyColumnName].Value;

            // Potwierdź usunięcie wiersza
            DialogResult result = MessageBox.Show("Czy na pewno chcesz usunąć Pozycję z listy?", "Potwierdzenie usunięcia", MessageBoxButtons.YesNo);
            if (result == DialogResult.Yes)
            {
                // Utwórz zapytanie SQL do usunięcia wiersza
                string query = $"DELETE FROM {tableName} WHERE {primaryKeyColumnName} = @primaryKeyValue";

                // Utwórz połączenie z bazą danych Firebird
                using (FbConnection connection = new FbConnection(connectionString))
                {
                    try
                    {
                        // Otwórz połączenie
                        connection.Open();

                        // Utwórz polecenie SQL
                        using (FbCommand command = new FbCommand(query, connection))
                        {
                            // Dodaj parametry do polecenia
                            command.Parameters.Add("@primaryKeyValue", FbDbType.VarChar).Value = primaryKeyValue;

                            // Wykonaj polecenie
                            int rowsAffected = command.ExecuteNonQuery();

                            // Usuń zaznaczony wiersz z DataTable powiązanego z kontrolką DataGridView
                            if (rowsAffected > 0)
                            {
                                DataTable dataTable = (DataTable)dataGridView.DataSource;
                                DataRow[] rowsToDelete = dataTable.Select($"{primaryKeyColumnName} = '{primaryKeyValue}'");
                                foreach (DataRow row in rowsToDelete)
                                {
                                    row.Delete();
                                }
                                dataTable.AcceptChanges();
                            }

                            MessageBox.Show(" Pozyja została usunięta");
                        }
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show("Błąd: " + ex.Message);
                    }
                }
            }
        }

        /// <summary>
        /// Aktualizuje tabele Firebird 
        /// </summary>
        /// <param name="tableName"></param>
        /// <param name="firstIdColumnName"></param>
        /// <param name="recordId"></param>
        /// <param name="newValues"></param>
        /// <param name="columns"></param>
        public void UpdateData(string tableName,string firstIdColumnName, string recordId, List<string> newValues, List<string> columns)
        {
            if (columns.Count != newValues.Count)
            {
                Console.WriteLine("Liczba kolumn i nowych wartości nie jest taka sama.");
                return;
            }

            string query = $"UPDATE {tableName} SET ";
            for (int i = 0; i < columns.Count; i++)
            {
                query += $"{columns[i]} = @Value{i}";
                if (i < columns.Count - 1)
                    query += ", ";
            }
            query += $" WHERE {firstIdColumnName} = @RecordId";

            using (FbConnection connection = new FbConnection(connectionString))
            {
                using (FbCommand command = new FbCommand(query, connection))
                {
                    for (int i = 0; i < newValues.Count; i++)
                    {
                        command.Parameters.AddWithValue($"@Value{i}",newValues[i]);
                    }
                    command.Parameters.AddWithValue("@RecordId", recordId);

                    try
                    {
                        connection.Open();
                        int rowsAffected = command.ExecuteNonQuery();
                        Console.WriteLine($"Zaktualizowano.");
                    }
                    catch (FbException ex)
                    {
                        Console.WriteLine("Błąd podczas aktualizacji danych: " + ex.Message);
                    }
                }
            }
        }
        /// <summary>
        /// Wyszukaj po kluczu Primary key
        /// </summary>
        /// <param name="tableName"></param>
        /// <param name="FkColumnName"></param>
        /// <param name="patientId"></param>
        /// <returns></returns>
        public DataTable ShowDataByPrimaryKey(string tableName, string FkColumnName, string patientId )
        {
            DataTable dataTable = new DataTable();
            string query = $"select * from {tableName} where {tableName}.{FkColumnName} = {patientId}";

            using (FbConnection connection = new FbConnection(connectionString))
            {
                try
                {
                    connection.Open();
                    using (FbCommand command = new FbCommand(query, connection))
                    {

                        using (FbDataAdapter adapter = new FbDataAdapter(command))
                        {
                            adapter.Fill(dataTable);
                        }
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Error: " + ex.Message);
                }
            }
            return dataTable;
        }
    }
}
