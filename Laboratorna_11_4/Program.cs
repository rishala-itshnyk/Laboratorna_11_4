using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;

[StructLayout(LayoutKind.Explicit)]
public struct Student
{
    [FieldOffset(0)] public string LastName;
    [FieldOffset(32)] public int Exam1;
    [FieldOffset(36)] public int Exam2;
    [FieldOffset(40)] public int Exam3;
}

public class StudentWrapper
{
    public Student Data { get; set; }
}

enum SortOption
{
    AverageGrade = 1,
    LastName,
    SubjectGrade
}

public class Program
{
    static void Main()
    {
        Console.WriteLine("Введіть ім'я файлу:");
        string fileName = Console.ReadLine();
        List<StudentWrapper> students = new List<StudentWrapper>();

        while (true)
        {
            Console.WriteLine("\nМеню:");
            Console.WriteLine("1. Зчитати дані з файлу");
            Console.WriteLine("2. Записати дані у файл");
            Console.WriteLine("3. Додати/редагувати інформацію");
            Console.WriteLine("4. Вилучити інформацію");
            Console.WriteLine("5. Сортувати список");
            Console.WriteLine("6. Вихід");

            int choice = GetMenuChoice();

            switch (choice)
            {
                case 1:
                    students = Read(fileName);
                    Print(students);
                    break;
                case 2:
                    Write(fileName, students);
                    break;
                case 3:
                    students = AddOrUpdateStudent(students);
                    Print(students);
                    break;
                case 4:
                    students = Remove(students);
                    Print(students);
                    break;
                case 5:
                    students = Sort(students);
                    Print(students);
                    break;
                case 6:
                    return;
                default:
                    Console.WriteLine("Неправильний вибір. Спробуйте ще раз.");
                    break;
            }
        }
    }

    static int GetMenuChoice()
    {
        Console.Write("Введіть номер опції: ");
        int choice;
        while (!int.TryParse(Console.ReadLine(), out choice))
        {
            Console.WriteLine("Неправильний ввід. Будь ласка, введіть ціле число.");
            Console.Write("Введіть номер опції: ");
        }

        return choice;
    }

    public static List<StudentWrapper> Sort(List<StudentWrapper> students)
    {
        Console.WriteLine("\nСортування списку:");
        Console.WriteLine("1. За середнім балом");
        Console.WriteLine("2. За прізвищами (в алфавітному порядку)");
        Console.WriteLine("3. За оцінками із заданого предмету");

        int sortChoice = GetMenuChoice();

        SortOption sortOption;
        if (Enum.TryParse<SortOption>(sortChoice.ToString(), out sortOption))
        {
            switch (sortOption)
            {
                case SortOption.AverageGrade:
                    return students.OrderByDescending(s => (s.Data.Exam1 + s.Data.Exam2 + s.Data.Exam3) / 3.0).ToList();
                case SortOption.LastName:
                    return students.OrderBy(s => s.Data.LastName).ToList();
                case SortOption.SubjectGrade:
                    Console.Write("Введіть номер предмету для сортування (1, 2 або 3): ");
                    int subjectChoice = GetSubjectChoice();
                    return students.OrderByDescending(s => GetSubjectGrade(s.Data, subjectChoice)).ToList();
            }
        }

        return students;
    }

    public static List<StudentWrapper> Read(string fileName)
    {
        List<StudentWrapper> students = new List<StudentWrapper>();

        if (File.Exists(fileName))
        {
            try
            {
                using (BinaryReader reader = new BinaryReader(File.Open(fileName, FileMode.Open)))
                {
                    while (reader.BaseStream.Position != reader.BaseStream.Length)
                    {
                        Student student = new Student();
                        student.LastName = reader.ReadString();
                        student.Exam1 = reader.ReadInt32();
                        student.Exam2 = reader.ReadInt32();
                        student.Exam3 = reader.ReadInt32();
                        students.Add(new StudentWrapper { Data = student });
                    }
                }

                Console.WriteLine($"Дані зчитано з файлу '{fileName}'.");
                return students;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Помилка при зчитуванні даних: {ex.Message}");
            }
        }
        else
        {
            Console.WriteLine($"Файл '{fileName}' не знайдено.");
        }

        return students;
    }

    public static void Write(string fileName, List<StudentWrapper> students)
    {
        try
        {
            using (BinaryWriter writer = new BinaryWriter(File.Open(fileName, FileMode.Create)))
            {
                foreach (var student in students)
                {
                    writer.Write(student.Data.LastName);
                    writer.Write(student.Data.Exam1);
                    writer.Write(student.Data.Exam2);
                    writer.Write(student.Data.Exam3);
                }
            }

            Console.WriteLine($"Дані записано у файл '{fileName}'.");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Помилка при записі даних: {ex.Message}");
        }
    }

    public static void Print(List<StudentWrapper> students)
    {
        if (students != null && students.Count > 0)
        {
            Console.WriteLine("\nСписок студентів:");

            foreach (var studentWrapper in students)
            {
                Student student = studentWrapper.Data;
                Console.WriteLine($"{student.LastName}, {student.Exam1}, {student.Exam2}, {student.Exam3}");
            }
        }
        else
        {
            Console.WriteLine("Список порожній.");
        }
    }


    public static List<StudentWrapper> AddOrUpdateStudent(List<StudentWrapper> students)
{
    Console.WriteLine("\nДодавання/редагування інформації про студента.");

    Console.Write("Введіть прізвище студента: ");
    string lastName = Console.ReadLine();

    if (students == null)
    {
        students = new List<StudentWrapper>();
    }

    int index = students.FindIndex(s => s.Data.LastName == lastName);

    if (index == -1)
    {
        Student newStudent = new Student();
        newStudent.LastName = lastName;
        students.Add(new StudentWrapper { Data = newStudent });
        index = students.Count - 1;
    }

    Student modifiedStudent = students[index].Data;

    Console.Write("Введіть оцінку з екзамену 1: ");
    modifiedStudent.Exam1 = GetValidGrade();

    Console.Write("Введіть оцінку з екзамену 2: ");
    modifiedStudent.Exam2 = GetValidGrade();

    Console.Write("Введіть оцінку з екзамену 3: ");
    modifiedStudent.Exam3 = GetValidGrade();

    students[index].Data = modifiedStudent;

    Console.WriteLine("Інформацію успішно змінено.");
    return students;
}
    static int GetValidGrade()
    {
        int grade;
        while (!int.TryParse(Console.ReadLine(), out grade) || grade < 0 || grade > 100)
        {
            Console.WriteLine("Неправильний ввід. Будь ласка, введіть ціле число від 0 до 100.");
            Console.Write("Введіть оцінку: ");
        }

        return grade;
    }

    public static List<StudentWrapper> Remove(List<StudentWrapper> students)
    {
        Console.Write("Введіть прізвище студента, якого ви хочете вилучити: ");
        string lastName = Console.ReadLine();

        int index = students.FindIndex(s => s.Data.LastName == lastName);

        if (index != -1)
        {
            students.RemoveAt(index);
            Console.WriteLine("Студента вилучено зі списку.");
        }
        else
        {
            Console.WriteLine("Студент з таким прізвищем не знайдений.");
        }

        return students;
    }

    static int GetSubjectChoice()
    {
        int choice;
        while (!int.TryParse(Console.ReadLine(), out choice) || choice < 1 || choice > 3)
        {
            Console.WriteLine("Неправильний ввід. Будь ласка, введіть 1, 2 або 3.");
            Console.Write("Введіть номер предмету: ");
        }

        return choice;
    }

    static int GetSubjectGrade(Student student, int subjectChoice)
    {
        switch (subjectChoice)
        {
            case 1:
                return student.Exam1;
            case 2:
                return student.Exam2;
            case 3:
                return student.Exam3;
            default:
                return 0;
        }
    }
}
