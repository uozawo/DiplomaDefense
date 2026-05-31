using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using DiplomaDefense.Core.Exceptions;

namespace DiplomaDefense.Core
{
    // головний клас. тримає всі списки і робить всю роботу
    public class DiplomaService
    {
        private List<Student> students;
        private List<Supervisor> supervisors;
        private List<Reviewer> reviewers;
        private List<DiplomaProject> projects;
        private int projectCounter;   // щоб робити id проектам: P001, P002 ...

        // віддаємо копії списків
        public List<Student> Students { get { return new List<Student>(students); } }
        public List<Supervisor> Supervisors { get { return new List<Supervisor>(supervisors); } }
        public List<Reviewer> Reviewers { get { return new List<Reviewer>(reviewers); } }
        public List<DiplomaProject> Projects { get { return new List<DiplomaProject>(projects); } }

        public DiplomaService()
        {
            students = new List<Student>();
            supervisors = new List<Supervisor>();
            reviewers = new List<Reviewer>();
            projects = new List<DiplomaProject>();
            projectCounter = 0;

            // читаємо все з файлів коли запускаємось
            LoadStudents();
            LoadSupervisors();
            LoadReviewers();
            LoadProjects();
            // після читання проектів рахуємо хто скільки рецензує
            RebuildReviewerLoad();
        }

        // прибираємо символи якими ми розділяємо поля у файлі
        // щоб користувач випадково не зламав файл
        private string Clean(string text)
        {
            if (text == null) return "";
            return text.Replace(";", " ").Replace("|", " ").Replace("~", " ").Trim();
        }

        // ===== читання з файлів =====

        private void LoadStudents()
        {
            if (!File.Exists(Constants.StudentsFile)) return;
            string[] lines = File.ReadAllLines(Constants.StudentsFile, Encoding.UTF8);
            foreach (string line in lines)
            {
                if (string.IsNullOrWhiteSpace(line)) continue;
                string[] p = line.Split(';');
                if (p.Length >= 5)
                    students.Add(new Student(p[0], p[1], p[2], p[3], p[4]));
            }
            Console.WriteLine("прочитано студентів: " + students.Count);
        }

        private void LoadSupervisors()
        {
            if (!File.Exists(Constants.SupervisorsFile)) return;
            string[] lines = File.ReadAllLines(Constants.SupervisorsFile, Encoding.UTF8);
            foreach (string line in lines)
            {
                if (string.IsNullOrWhiteSpace(line)) continue;
                string[] p = line.Split(';');
                if (p.Length >= 5)
                    supervisors.Add(new Supervisor(p[0], p[1], p[2], p[3], p[4]));
            }
            Console.WriteLine("прочитано керівників: " + supervisors.Count);
        }

        private void LoadReviewers()
        {
            if (!File.Exists(Constants.ReviewersFile)) return;
            string[] lines = File.ReadAllLines(Constants.ReviewersFile, Encoding.UTF8);
            foreach (string line in lines)
            {
                if (string.IsNullOrWhiteSpace(line)) continue;
                string[] p = line.Split(';');
                if (p.Length >= 4)
                    reviewers.Add(new Reviewer(p[0], p[1], p[2], p[3]));
            }
            Console.WriteLine("прочитано рецензентів: " + reviewers.Count);
        }

        private void LoadProjects()
        {
            if (!File.Exists(Constants.ProjectsFile)) return;
            string[] lines = File.ReadAllLines(Constants.ProjectsFile, Encoding.UTF8);
            foreach (string line in lines)
            {
                if (string.IsNullOrWhiteSpace(line)) continue;
                string[] p = line.Split(';');
                if (p.Length < 9) continue;

                string id = p[0];
                string topic = p[1];
                string studentId = p[2];
                string supervisorId = p[3];
                string reviewerId = p[4];
                DateTime date = DateTime.Parse(p[5]);
                DefenseResult result = (DefenseResult)Enum.Parse(typeof(DefenseResult), p[6]);
                int grade = int.Parse(p[7]);
                string stagesBlob = p[8];

                var project = new DiplomaProject(id, topic, studentId, supervisorId,
                                                 reviewerId, date, result, grade);

                // читаємо етапи. вони розділені |, поля етапу розділені ~
                if (!string.IsNullOrWhiteSpace(stagesBlob))
                {
                    string[] stageParts = stagesBlob.Split('|');
                    foreach (string sp in stageParts)
                    {
                        if (string.IsNullOrWhiteSpace(sp)) continue;
                        string[] f = sp.Split('~');
                        // f[0] = тип (S або U)
                        string name = f[1];
                        DateTime deadline = DateTime.Parse(f[2]);
                        StageStatus status = (StageStatus)Enum.Parse(typeof(StageStatus), f[3]);
                        if (f[0] == "U")
                        {
                            string reason = f.Length > 4 ? f[4] : "не вказано";
                            project.AddStage(new UrgentStage(name, deadline, status, reason));
                        }
                        else
                        {
                            project.AddStage(new Stage(name, deadline, status));
                        }
                    }
                }

                projects.Add(project);

                // оновлюємо лічильник по найбільшому номеру
                if (id.StartsWith("P") && int.TryParse(id.Substring(1), out int num))
                {
                    if (num > projectCounter) projectCounter = num;
                }
            }
            Console.WriteLine("прочитано проектів: " + projects.Count);
        }

        // рахуємо скільки проектів у кожного рецензента (після читання файлу)
        private void RebuildReviewerLoad()
        {
            foreach (var pr in projects)
            {
                if (string.IsNullOrWhiteSpace(pr.ReviewerId)) continue;
                foreach (var r in reviewers)
                {
                    if (r.Id == pr.ReviewerId)
                        r.AssignProject(pr.Id);
                }
            }
        }

        // ===== запис у файл =====

        private void SaveProjects()
        {
            List<string> lines = new List<string>();
            foreach (var pr in projects)
            {
                // збираємо етапи в один рядок
                string stagesBlob = "";
                List<Stage> st = pr.Stages;
                for (int i = 0; i < st.Count; i++)
                {
                    if (i > 0) stagesBlob += "|";
                    stagesBlob += st[i].ToFileString();
                }

                string line = pr.Id + ";" + pr.Topic + ";" + pr.StudentId + ";" +
                              pr.SupervisorId + ";" + pr.ReviewerId + ";" +
                              pr.CreatedDate.ToString("yyyy-MM-dd") + ";" +
                              pr.Result + ";" + pr.Grade + ";" + stagesBlob;
                lines.Add(line);
            }
            File.WriteAllLines(Constants.ProjectsFile, lines, Encoding.UTF8);
        }

        // ===== пошук =====

        public Student FindStudentById(string id)
        {
            foreach (var s in students)
            {
                if (s.Id == id) return s;
            }
            throw new StudentNotFoundException(id);
        }

        public DiplomaProject FindProjectById(string id)
        {
            foreach (var p in projects)
            {
                if (p.Id == id) return p;
            }
            throw new ProjectNotFoundException(id);
        }

        // знайти проект студента (або null якщо нема)
        public DiplomaProject GetProjectByStudent(string studentId)
        {
            foreach (var p in projects)
            {
                if (p.StudentId == studentId) return p;
            }
            return null;
        }

        // ===== студенти =====

        public void RegisterStudent(Student student)
        {
            foreach (var s in students)
            {
                if (s.Id == student.Id)
                    throw new DuplicateStudentException(student.Id);
            }
            students.Add(student);
            File.AppendAllText(Constants.StudentsFile,
                student.ToFileString() + Environment.NewLine, Encoding.UTF8);
        }

        // ===== проекти =====

        // створити проект і одразу додати кілька стандартних етапів
        public DiplomaProject CreateProject(Student student, string topic, string supervisorId)
        {
            topic = Clean(topic);
            if (string.IsNullOrWhiteSpace(topic))
                throw new EmptyTopicException();

            projectCounter++;
            string id = "P" + projectCounter.ToString("D3");

            var project = new DiplomaProject(id, topic, student.Id, supervisorId);

            // стандартні етапи підготовки диплома
            project.AddStage(new Stage("вибір теми", DateTime.Now.AddDays(7)));
            project.AddStage(new Stage("огляд літератури", DateTime.Now.AddDays(21)));
            project.AddStage(new Stage("основний розділ", DateTime.Now.AddDays(45)));
            project.AddStage(new Stage("передзахист", DateTime.Now.AddDays(60)));

            projects.Add(project);
            student.ProjectId = id;
            SaveProjects();
            return project;
        }

        public void AddStage(string projectId, string name, int days)
        {
            var project = FindProjectById(projectId);
            project.AddStage(new Stage(Clean(name), DateTime.Now.AddDays(days)));
            SaveProjects();
        }

        public void AddUrgentStage(string projectId, string name, int days, string reason)
        {
            var project = FindProjectById(projectId);
            project.AddStage(new UrgentStage(Clean(name), DateTime.Now.AddDays(days), Clean(reason)));
            SaveProjects();
        }

        // змінити статус етапу за його номером (з 1)
        public void SetStageStatus(string projectId, int stageNumber, StageStatus status)
        {
            var project = FindProjectById(projectId);
            List<Stage> st = project.Stages;
            if (stageNumber < 1 || stageNumber > st.Count)
                throw new ArgumentException("такого етапу нема");
            // беремо справжній етап, не копію
            // (Stages віддає копію списку, але самі обʼєкти ті самі)
            st[stageNumber - 1].Status = status;
            SaveProjects();
        }

        // ===== рецензент =====

        // знайти вільного рецензента і дати йому проект
        public Reviewer AssignReviewer(string projectId)
        {
            var project = FindProjectById(projectId);
            foreach (var r in reviewers)
            {
                if (r.CanTakeMore())
                {
                    r.AssignProject(projectId);
                    project.ReviewerId = r.Id;
                    SaveProjects();
                    return r;
                }
            }
            throw new ReviewerOverloadedException();
        }

        // ===== захист =====

        // піти на захист. якщо не готовий - помилка. якщо готовий - ставимо оцінку
        public bool GoToDefense(string projectId, int grade)
        {
            var project = FindProjectById(projectId);
            if (!project.IsReadyForDefense())
                throw new StageNotApprovedException(projectId);

            project.Grade = grade;
            if (grade >= Constants.PassingGrade)
                project.Result = DefenseResult.Passed;
            else
                project.Result = DefenseResult.Failed;

            SaveProjects();
            return project.Result == DefenseResult.Passed;
        }
    }
}
