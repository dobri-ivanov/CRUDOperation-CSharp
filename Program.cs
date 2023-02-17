using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data;
using System.Data.OleDb;

namespace CRUDOperations
{
    class Program
    {
        static void Main(string[] args)
        {
            //Unicode
            Console.OutputEncoding = Encoding.Unicode;
            Console.InputEncoding = Encoding.Unicode;

            //Title
            Console.WriteLine("CRUD OPERATION v 1.0.0");
            Console.WriteLine("----------------------------------------------");
            Console.WriteLine();

            //Connection with database
            string connectionString = $"Provider=Microsoft.Jet.OLEDB.4.0;Data Source=E:\\[Coding]\\Projects\\CRUDOperations\\SchoolEn.mdb";
            
            //Business logic
            while (true)
            {
                Console.WriteLine("Enter number 1-5");
                Console.WriteLine("1    - Insert new student");
                Console.WriteLine("2    - Read information about students and their parents");
                Console.WriteLine("2.1  - Read information about student by name");
                Console.WriteLine("3    - Modify student name");
                Console.WriteLine("4    - Delete student");
                Console.WriteLine("5    - Count of tests");
                Console.WriteLine("----------------------------------------------");
                Console.Write("Number: ");
                string number = Console.ReadLine();

                switch (number)
                {
                    case "1":
                        InsertNewStudent(connectionString);
                        break;
                    case "2":
                        ReadStudentsParents(connectionString);
                        break;
                    case "2.1":
                        ReadStudentsByName(connectionString);
                        break;
                    case "3":
                        UpdateStudentName(connectionString);
                        break;
                    case "4":
                        DeleteStudent(connectionString);
                        break;
                    case "5":
                        Console.WriteLine($"Count of tests: {CountTests(connectionString)}");
                        Console.WriteLine();
                        break;
                    default:
                        Console.WriteLine("Not valid operation!");
                        break;
                }
            }
        }

        private static int CountTests(string connectionString)
        {
            using (OleDbConnection connection = new OleDbConnection(connectionString))
            {
                connection.Open();

                string query =
                    "SELECT COUNT(*)" +
                    "FROM Test";
                OleDbCommand command = new OleDbCommand(query, connection);
                return int.Parse(command.ExecuteScalar().ToString());
            }
        }

        private static void UpdateStudentName(string connectionString)
        {
            Console.Write("Enter student's ID: ");
            int studentID = int.Parse(Console.ReadLine());
            Console.Write("Enter new name: ");
            string newName = Console.ReadLine();

            using (OleDbConnection connection = new OleDbConnection(connectionString))
            {
                connection.Open();
                string query =
                    "UPDATE Students " +
                   $"SET StudentName = '{newName}' " +
                   $"WHERE ID = {studentID}";
                OleDbCommand command = new OleDbCommand(query, connection);
                int rows = command.ExecuteNonQuery();

                if (rows > 0)
                {
                    Console.WriteLine("Student successfully updated!");
                }
                else
                {
                    Console.WriteLine("There isn't any student with that ID!");
                }
                Console.WriteLine();
            }
        }
        private static void DeleteStudent(string connectionString)
        {
            Console.Write("Student's ID: ");
            int studentID = int.Parse(Console.ReadLine());

            string query =
                "DELETE FROM " +
                "Students " +
                $"WHERE ID = {studentID}";
            using (OleDbConnection connection = new OleDbConnection(connectionString))
            {
                connection.Open();
                OleDbCommand command = new OleDbCommand(query, connection);
                try
                {
                    int rows = command.ExecuteNonQuery();
                    if (rows > 0)
                    {
                        Console.WriteLine($"Successfuly deleted student with ID - {studentID}");
                    }
                    else
                    {
                        Console.WriteLine("There isn't any student whit that ID!");
                    }
                }
                catch (Exception)
                {
                    Console.WriteLine("The student is related to other tables' information");
                    Console.WriteLine();
                    return;
                }

            }
            Console.WriteLine();
        }
        private static void InsertNewStudent(string connectionString)
        {
            int classID = -1;

            Console.Write("Enter name: ");
            string name = Console.ReadLine();
            Console.Write("Enter citizenID: ");
            string citizenID = Console.ReadLine();
            Console.Write("Enter class: ");
            string className = Console.ReadLine();

            string query =
                "SELECT ID " +
                "FROM Classes " +
                $"WHERE ClassName = '{className}'";
            using (OleDbConnection connection = new OleDbConnection(connectionString))
            {
                connection.Open();
                OleDbCommand command = new OleDbCommand(query, connection);
                using (OleDbDataReader reader = command.ExecuteReader())
                {
                    if (reader.Read())
                    {
                        classID = int.Parse(reader[0].ToString());
                    }
                    else
                    {
                        Console.WriteLine("There isn't any class with that name?");
                        Console.WriteLine();
                        return;
                    }
                }
            }

            query =
                "INSERT INTO Students(StudentName, ClassID, EGN) " +
                "VALUES " +
                $"('{name}', {classID}, '{citizenID}')";

            using (OleDbConnection connection = new OleDbConnection(connectionString))
            {
                connection.Open();
                OleDbCommand command = new OleDbCommand(query, connection);
                command.ExecuteNonQuery();
            }
            Console.WriteLine("Done!");
            Console.WriteLine();
        }
        private static void ReadStudentsParents(string connectionString)
        {
            string query =
                "SELECT s.StudentName, p.ParentName, p.Telephone " +
                "FROM Students AS s " +
                "LEFT OUTER JOIN Parents AS p ON s.ID = p.StudentID " +
                "ORDER BY s.StudentName";

            using (OleDbConnection connection = new OleDbConnection(connectionString))
            {
                connection.Open();

                OleDbCommand command = new OleDbCommand(query, connection);
                OleDbDataReader reader = command.ExecuteReader();

                Console.WriteLine("Student Name\t\t\tParent Name\t\t\tPhoneNumber");
                Console.WriteLine("----------------------------------------------------------------------------------");

                int counter = 0;
                while (reader.Read())
                {
                    counter++;
                    string studentName = reader[0].ToString();
                    string parentName = reader[1].ToString();
                    string phoneNumbe = reader[2].ToString();

                    Console.WriteLine($"{studentName,-32}{parentName,-32}{phoneNumbe}");
                }
                Console.WriteLine("----------------------------------------------------------------------------------");
                Console.WriteLine($"Count of students: {counter}");
                Console.WriteLine("Done!");
                Console.WriteLine();
                reader.Close();
            }

        }
        private static void ReadStudentsByName(string connectionString)
        {
            Console.Write("Enter student's name: ");
            string studentName = Console.ReadLine();

            using (OleDbConnection connection = new OleDbConnection(connectionString))
            {
                connection.Open();

                string query =
               "SELECT StudentName, EGN, Adress " +
               "FROM Students " +
               $"WHERE StudentName = '{studentName}' ";
                OleDbCommand command = new OleDbCommand(query, connection);
                using (OleDbDataReader reader = command.ExecuteReader())
                {
                    Console.WriteLine("Student Name\t\t\tCitizenID\t\t\tAdress");
                    while (reader.Read())
                    {
                        string parentName = reader[1].ToString();
                        string phoneNumbe = reader[2].ToString();

                        Console.WriteLine($"{studentName,-32}{parentName,-32}{phoneNumbe}");
                    }
                }
                Console.WriteLine("Done!");
            }

        }
    }
}
