using System;
using System.Collections.Generic;
using System.Text;
using DiplomaDefense.Core;
using DiplomaDefense.Core.Exceptions;

namespace DiplomaDefense.App
{
    class Program
    {
        static DiplomaService service;
        static Student currentStudent = null;

        static void Main(string[] args)
        {
            try
            {
                Console.OutputEncoding = Encoding.UTF8;
            }
            catch
            {
                // на деяких терміналах не виходить, нічого страшного
            }

            service = new DiplomaService();
            Console.WriteLine("=== " + Messages.Get("AppTitle") + " ===");
            Console.WriteLine(Messages.Get("Welcome") + "!\n");

            while (true)
            {
                if (currentStudent == null)
                    ShowGuestMenu();
                else
                    ShowUserMenu();

                Console.Write(Messages.Get("AskChoice"));
                string choice = ReadLine();
                Console.WriteLine();

                if (currentStudent == null)
                {
                    if (choice == "1") Login();
                    else if (choice == "2") ShowStudents();
                    else if (choice == "3") AddStudent();
                    else if (choice == "4") ShowSupervisors();
                    else if (choice == "5") ShowReviewers();
                    else if (choice == "0") { Console.WriteLine(Messages.Get("Goodbye")); return; }
                    else Console.WriteLine(Messages.Get("UnknownCommand"));
                }
                else
                {
                    if (choice == "1") CreateProject();
                    else if (choice == "2") ShowMyProject();
                    else if (choice == "3") AddStage();
                    else if (choice == "4") AddUrgentStage();
                    else if (choice == "5") SetStage();
                    else if (choice == "6") AssignReviewer();
                    else if (choice == "7") GoDefense();
                    else if (choice == "8") ShowReport();
                    else if (choice == "9") Logout();
                    else if (choice == "0") { Console.WriteLine(Messages.Get("Goodbye")); return; }
                    else Console.WriteLine(Messages.Get("UnknownCommand"));
                }
            }
        }

        // ===== безпечне читання =====

        static string ReadLine()
        {
            string s = Console.ReadLine();
            // якщо вводу більше нема (кінець файлу) - виходимо щоб не зациклитись
            if (s == null) return "0";
            return s;
        }

        // читаємо число, якщо ввели щось не те - беремо 0
        static int ReadInt()
        {
            string s = ReadLine();
            int n;
            if (int.TryParse(s, out n)) return n;
            return 0;
        }

        // ===== меню =====

        static void ShowGuestMenu()
        {
            Console.WriteLine("--------------------------------");
            Console.WriteLine(Messages.Get("GuestLogin"));
            Console.WriteLine(Messages.Get("GuestListStudents"));
            Console.WriteLine(Messages.Get("GuestAddStudent"));
            Console.WriteLine(Messages.Get("GuestSupervisors"));
            Console.WriteLine(Messages.Get("GuestReviewers"));
            Console.WriteLine(Messages.Get("GuestExit"));
        }

        static void ShowUserMenu()
        {
            Console.WriteLine("--------------------------------");
            Console.WriteLine(Messages.Get("Hello") + ", " + currentStudent.FirstName + "!");
            Console.WriteLine(Messages.Get("UserCreateProject"));
            Console.WriteLine(Messages.Get("UserShowProject"));
            Console.WriteLine(Messages.Get("UserAddStage"));
            Console.WriteLine(Messages.Get("UserAddUrgentStage"));
            Console.WriteLine(Messages.Get("UserSetStage"));
            Console.WriteLine(Messages.Get("UserAssignReviewer"));
            Console.WriteLine(Messages.Get("UserGoDefense"));
            Console.WriteLine(Messages.Get("UserReport"));
            Console.WriteLine(Messages.Get("UserLogout"));
            Console.WriteLine(Messages.Get("UserExit"));
        }

        // ===== вхід =====

        static void Login()
        {
            Console.Write(Messages.Get("AskId"));
            string id = ReadLine();
            if (id == "0") return;
            try
            {
                currentStudent = service.FindStudentById(id);
                Console.WriteLine(Messages.Get("StudentLogged") + ": " + currentStudent.FullName);
            }
            catch (StudentNotFoundException ex)
            {
                Console.WriteLine(ex.Message);
            }
        }

        static void Logout()
        {
            currentStudent = null;
        }

        // ===== студенти =====

        static void AddStudent()
        {
            try
            {
                Console.Write(Messages.Get("AskId"));
                string id = ReadLine();
                if (id == "0") return;
                Console.Write(Messages.Get("AskFirstName"));
                string first = ReadLine();
                Console.Write(Messages.Get("AskLastName"));
                string last = ReadLine();
                Console.Write(Messages.Get("AskGroup"));
                string group = ReadLine();
                Console.Write(Messages.Get("AskRecordBook"));
                string book = ReadLine();

                var student = new Student(id, first, last, group, book);
                service.RegisterStudent(student);
                Console.WriteLine(Messages.Get("Saved") + ": " + student);
            }
            catch (DuplicateStudentException ex)
            {
                Console.WriteLine(ex.Message);
            }
            catch (ArgumentException ex)
            {
                Console.WriteLine(ex.Message);
            }
        }

        static void ShowStudents()
        {
            var list = service.Students;
            if (list.Count == 0) { Console.WriteLine(Messages.Get("EmptyList")); return; }
            foreach (var s in list)
                Console.WriteLine("  " + s.GetDisplayInfo());
        }

        static void ShowSupervisors()
        {
            var list = service.Supervisors;
            if (list.Count == 0) { Console.WriteLine(Messages.Get("EmptyList")); return; }
            foreach (var s in list)
                Console.WriteLine("  " + s.GetDisplayInfo());
        }

        static void ShowReviewers()
        {
            var list = service.Reviewers;
            if (list.Count == 0) { Console.WriteLine(Messages.Get("EmptyList")); return; }
            foreach (var r in list)
                Console.WriteLine("  " + r.GetDisplayInfo());
        }

        // ===== проект =====

        static void CreateProject()
        {
            try
            {
                if (service.GetProjectByStudent(currentStudent.Id) != null)
                {
                    Console.WriteLine(Messages.Get("HasProjectAlready"));
                    return;
                }
                Console.Write(Messages.Get("AskTopic"));
                string topic = ReadLine();
                Console.Write(Messages.Get("AskSupervisorId"));
                string supId = ReadLine();

                var project = service.CreateProject(currentStudent, topic, supId);
                Console.WriteLine(Messages.Get("ProjectCreated") + ": " + project.Id);
            }
            catch (EmptyTopicException ex)
            {
                Console.WriteLine(ex.Message);
            }
        }

        // знаходимо проект поточного студента або кажемо що його нема
        static DiplomaProject MyProjectOrNull()
        {
            var project = service.GetProjectByStudent(currentStudent.Id);
            if (project == null)
                Console.WriteLine(Messages.Get("NoProject"));
            return project;
        }

        static void ShowMyProject()
        {
            var project = MyProjectOrNull();
            if (project == null) return;
            Console.WriteLine(project);
            int i = 1;
            foreach (var s in project.Stages)
            {
                Console.WriteLine("  " + i + ") " + s);
                i++;
            }
        }

        static void AddStage()
        {
            var project = MyProjectOrNull();
            if (project == null) return;
            Console.Write(Messages.Get("AskStageName"));
            string name = ReadLine();
            Console.Write(Messages.Get("AskDays"));
            int days = ReadInt();
            service.AddStage(project.Id, name, days);
            Console.WriteLine(Messages.Get("StageAdded"));
        }

        static void AddUrgentStage()
        {
            var project = MyProjectOrNull();
            if (project == null) return;
            Console.Write(Messages.Get("AskStageName"));
            string name = ReadLine();
            Console.Write(Messages.Get("AskDays"));
            int days = ReadInt();
            Console.Write(Messages.Get("AskReason"));
            string reason = ReadLine();
            service.AddUrgentStage(project.Id, name, days, reason);
            Console.WriteLine(Messages.Get("StageAdded"));
        }

        static void SetStage()
        {
            var project = MyProjectOrNull();
            if (project == null) return;
            try
            {
                Console.Write(Messages.Get("AskStageNumber"));
                int num = ReadInt();
                Console.Write(Messages.Get("AskStatus"));
                int st = ReadInt();
                if (st < 0 || st > 4)
                {
                    Console.WriteLine(Messages.Get("WrongNumber"));
                    return;
                }
                service.SetStageStatus(project.Id, num, (StageStatus)st);
                Console.WriteLine(Messages.Get("StageChanged"));
            }
            catch (ArgumentException ex)
            {
                Console.WriteLine(ex.Message);
            }
        }

        static void AssignReviewer()
        {
            var project = MyProjectOrNull();
            if (project == null) return;
            try
            {
                var r = service.AssignReviewer(project.Id);
                Console.WriteLine(Messages.Get("ReviewerAssigned") + ": " + r.FullName);
            }
            catch (ReviewerOverloadedException ex)
            {
                Console.WriteLine(ex.Message);
            }
        }

        static void GoDefense()
        {
            var project = MyProjectOrNull();
            if (project == null) return;
            try
            {
                Console.Write(Messages.Get("AskGrade"));
                int grade = ReadInt();
                bool passed = service.GoToDefense(project.Id, grade);
                if (passed)
                    Console.WriteLine(Messages.Get("DefensePassed"));
                else
                    Console.WriteLine(Messages.Get("DefenseFailed"));
            }
            catch (StageNotApprovedException)
            {
                Console.WriteLine(Messages.Get("NotReady"));
            }
        }

        static void ShowReport()
        {
            var project = MyProjectOrNull();
            if (project == null) return;
            Console.WriteLine(project.GetReport());
        }
    }
}
