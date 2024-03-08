using System;
using System.Collections.Generic;
using System.Data.SQLite;
using System.Windows;

namespace lab2
{
    public partial class MainWindow : Window
    {
        private const string DatabasePath = "student.db";
        private const string ConnectionString = "Data Source=" + DatabasePath + ";Version=3;";

        public MainWindow()
        {
            InitializeComponent();
            CreateDatabase();

            // Загрузка данных из БД при открытии приложения
            LoadData();
        }

        private void CreateDatabase()
        {
            SQLiteConnection.CreateFile(DatabasePath);

            using (SQLiteConnection connection = new SQLiteConnection(ConnectionString))
            {
                connection.Open();

                // Создание таблицы студентов
                string createStudentsTableQuery =
                    "CREATE TABLE IF NOT EXISTS Students (Number INTEGER PRIMARY KEY, FullName TEXT, BirthDate TEXT);";
                using (SQLiteCommand command = new SQLiteCommand(createStudentsTableQuery, connection))
                {
                    command.ExecuteNonQuery();
                }

                // Создание таблицы оценок
                string createGradesTableQuery =
                    "CREATE TABLE IF NOT EXISTS Grades (Number INTEGER PRIMARY KEY, PhysicsGrade INTEGER, MathGrade INTEGER);";
                using (SQLiteCommand command = new SQLiteCommand(createGradesTableQuery, connection))
                {
                    command.ExecuteNonQuery();
                }
            }
        }

        private void AddButton_Click(object sender, RoutedEventArgs e)
        {
            int number = int.Parse(txtNumber.Text);
            string fullName = txtFullName.Text;
            int physicsGrade = int.Parse(txtPhysicsGrade.Text);
            int mathGrade = int.Parse(txtMathGrade.Text);
            string birthDate = datePicker.SelectedDate?.ToString("yyyy-MM-dd");

            using (SQLiteConnection connection = new SQLiteConnection(ConnectionString))
            {
                connection.Open();

                // Добавление записи в таблицу студентов
                string insertStudentQuery = $"INSERT INTO Students (Number, FullName, BirthDate) VALUES ({number}, '{fullName}', '{birthDate}');";
                using (SQLiteCommand command = new SQLiteCommand(insertStudentQuery, connection))
                {
                    command.ExecuteNonQuery();
                }

                // Добавление записи в таблицу оценок
                string insertGradesQuery = $"INSERT INTO Grades (Number, PhysicsGrade, MathGrade) VALUES ({number}, {physicsGrade}, {mathGrade});";
                using (SQLiteCommand command = new SQLiteCommand(insertGradesQuery, connection))
                {
                    command.ExecuteNonQuery();
                }
            }

            // Очистка полей после добавления
            txtNumber.Clear();
            txtFullName.Clear();
            txtPhysicsGrade.Clear();
            txtMathGrade.Clear();
            datePicker.SelectedDate = null;

            // Загрузка данных из БД после добавления
            LoadData();
        }

        private void EditButton_Click(object sender, RoutedEventArgs e)
        {
            if (dataGrid.SelectedItem is Student selectedStudent)
            {
                int number = selectedStudent.Number;
                string fullName = txtFullName.Text;
                int physicsGrade = int.Parse(txtPhysicsGrade.Text);
                int mathGrade = int.Parse(txtMathGrade.Text);
                string birthDate = datePicker.SelectedDate?.ToString("yyyy-MM-dd");

                using (SQLiteConnection connection = new SQLiteConnection(ConnectionString))
                {
                    connection.Open();

                    // Обновление записи в таблице студентов
                    string updateStudentQuery = $"UPDATE Students SET FullName = '{fullName}', BirthDate = '{birthDate}' WHERE Number = {number};";
                    using (SQLiteCommand command = new SQLiteCommand(updateStudentQuery, connection))
                    {
                        command.ExecuteNonQuery();
                    }

                    // Обновление записи в таблице оценок
                    string updateGradesQuery = $"UPDATE Grades SET PhysicsGrade = {physicsGrade}, MathGrade = {mathGrade} WHERE Number = {number};";
                    using (SQLiteCommand command = new SQLiteCommand(updateGradesQuery, connection))
                    {
                        command.ExecuteNonQuery();
                    }
                }

                // Очистка полей после редактирования
                txtFullName.Clear();
                txtPhysicsGrade.Clear();
                txtMathGrade.Clear();
                datePicker.SelectedDate = null;

                // Загрузка данных из БД после редактирования
                LoadData();
            }
        }

        private void DeleteButton_Click(object sender, RoutedEventArgs e)
        {
            if (dataGrid.SelectedItem is Student selectedStudent)
            {
                int number = selectedStudent.Number;

                using (SQLiteConnection connection = new SQLiteConnection(ConnectionString))
                {
                    connection.Open();

                    // Удаление записи из таблицы студентов
                    string deleteStudentQuery = $"DELETE FROM Students WHERE Number = {number};";
                    using (SQLiteCommand command = new SQLiteCommand(deleteStudentQuery, connection))
                    {
                        command.ExecuteNonQuery();
                    }

                    // Удаление записи из таблицы оценок
                    string deleteGradesQuery = $"DELETE FROM Grades WHERE Number = {number};";
                    using (SQLiteCommand command = new SQLiteCommand(deleteGradesQuery, connection))
                    {
                        command.ExecuteNonQuery();
                    }
                }

                // Загрузка данных из БД после удаления
                LoadData();
            }
        }

        private void LoadData()
        {
            using (SQLiteConnection connection = new SQLiteConnection(ConnectionString))
            {
                connection.Open();

                // Запрос данных из таблицы студентов и оценок
                string query = "SELECT Students.Number, FullName, PhysicsGrade, MathGrade, BirthDate " +
                               "FROM Students JOIN Grades ON Students.Number = Grades.Number;";
                using (SQLiteCommand command = new SQLiteCommand(query, connection))
                {
                    using (SQLiteDataReader reader = command.ExecuteReader())
                    {
                        List<Student> students = new List<Student>();
                        while (reader.Read())
                        {
                            int number = reader.GetInt32(0);
                            string fullName = reader.GetString(1);
                            int physicsGrade = reader.GetInt32(2);
                            int mathGrade = reader.GetInt32(3);
                            string birthDate = reader.GetString(4);
                            students.Add(new Student(number, fullName, physicsGrade, mathGrade, birthDate));
                        }

                        dataGrid.ItemsSource = students;
                    }
                }
            }
        }
    }

    public class Student
    {
        public int Number { get; set; }
        public string FullName { get; set; }
        public int PhysicsGrade { get; set; }
        public int MathGrade { get; set; }
        public string BirthDate { get; set; }

        public Student(int number, string fullName, int physicsGrade, int mathGrade, string birthDate)
        {
            Number = number;
            FullName = fullName;
            PhysicsGrade = physicsGrade;
            MathGrade = mathGrade;
            BirthDate = birthDate;
        }
    }
}
