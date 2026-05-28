using System;
using System.Collections.Generic;

namespace DiplomaDefense.Core
{
    // результат захисту
    public enum DefenseResult
    {
        Pending,   // ще не захищався
        Passed,    // захистив
        Failed     // не захистив
    }

    // дипломний проект студента. реалізує IReportable бо вміє робити звіт
    public class DiplomaProject : IReportable
    {
        public string Id { get; private set; }
        public string Topic { get; private set; }
        public string StudentId { get; private set; }
        public string SupervisorId { get; set; }
        public string ReviewerId { get; set; }
        public DateTime CreatedDate { get; private set; }
        public DefenseResult Result { get; set; }
        public int Grade { get; set; }   // 0 значить ще не оцінений

        private List<Stage> stages;      // етапи цього проекту

        public List<Stage> Stages
        {
            get { return new List<Stage>(stages); }
        }

        // новий проект
        public DiplomaProject(string id, string topic, string studentId, string supervisorId)
        {
            Id = id;
            Topic = topic;
            StudentId = studentId;
            SupervisorId = supervisorId;
            ReviewerId = "";
            CreatedDate = DateTime.Now;
            Result = DefenseResult.Pending;
            Grade = 0;
            stages = new List<Stage>();
        }

        // проект з файлу (вже з датою і результатом)
        public DiplomaProject(string id, string topic, string studentId, string supervisorId,
                              string reviewerId, DateTime createdDate, DefenseResult result, int grade)
        {
            Id = id;
            Topic = topic;
            StudentId = studentId;
            SupervisorId = supervisorId;
            ReviewerId = reviewerId;
            CreatedDate = createdDate;
            Result = result;
            Grade = grade;
            stages = new List<Stage>();
        }

        // додати етап до проекту
        public void AddStage(Stage stage)
        {
            stages.Add(stage);
        }

        // чи всі етапи прийняті керівником
        public bool AllStagesApproved()
        {
            if (stages.Count == 0)
                return false;
            foreach (var s in stages)
            {
                if (!s.IsApproved())
                    return false;
            }
            return true;
        }

        // скільки відсотків етапів готово
        public int GetProgress()
        {
            if (stages.Count == 0)
                return 0;
            int done = 0;
            foreach (var s in stages)
            {
                if (s.IsApproved())
                    done++;
            }
            return done * 100 / stages.Count;
        }

        // чи можна йти на захист: всі етапи готові і є рецензент
        public bool IsReadyForDefense()
        {
            return AllStagesApproved() && !string.IsNullOrWhiteSpace(ReviewerId);
        }

        // звіт про проект (це метод з інтерфейсу IReportable)
        public string GetReport()
        {
            string text = "проект " + Id + " — \"" + Topic + "\"\n";
            text += "готовність: " + GetProgress() + "%\n";
            text += "етапів: " + stages.Count + "\n";
            foreach (var s in stages)
            {
                text += "  - " + s + "\n";
            }
            text += "результат захисту: " + Result;
            if (Grade > 0)
                text += " (оцінка " + Grade + ")";
            return text;
        }

        public override string ToString()
        {
            return "проект " + Id + " | " + Topic + " | готовність " + GetProgress() + "%";
        }

        // порівнюємо два проекти по id
        public static bool operator ==(DiplomaProject a, DiplomaProject b)
        {
            if (ReferenceEquals(a, b)) return true;
            if (a is null || b is null) return false;
            return a.Id == b.Id;
        }

        public static bool operator !=(DiplomaProject a, DiplomaProject b)
        {
            return !(a == b);
        }

        public override bool Equals(object obj)
        {
            return obj is DiplomaProject other && this == other;
        }

        public override int GetHashCode()
        {
            return Id.GetHashCode();
        }
    }
}
