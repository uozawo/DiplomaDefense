using System;

namespace DiplomaDefense.Core
{
    // статус етапу
    public enum StageStatus
    {
        NotStarted,   // не почали
        InProgress,   // у роботі
        Submitted,    // здали на перевірку
        Approved,     // керівник прийняв
        Rejected      // повернув на переробку
    }

    // один етап роботи над дипломом
    // наприклад: вибір теми, написання розділу, передзахист
    public class Stage
    {
        public string Name { get; private set; }
        public StageStatus Status { get; set; }
        public DateTime Deadline { get; set; }

        public Stage(string name, DateTime deadline)
        {
            Name = name;
            Deadline = deadline;
            Status = StageStatus.NotStarted;
        }

        public Stage(string name, DateTime deadline, StageStatus status)
        {
            Name = name;
            Deadline = deadline;
            Status = status;
        }

        public Stage()
        {
            Name = "етап";
            Deadline = DateTime.Now.AddDays(7);
            Status = StageStatus.NotStarted;
        }

        // конструктор копій
        public Stage(Stage other)
        {
            Name = other.Name;
            Deadline = other.Deadline;
            Status = other.Status;
        }

        // рядок у файл. "S" значить звичайний етап. '~' розділяє поля
        public virtual string ToFileString()
        {
            return "S~" + Name + "~" + Deadline.ToString("yyyy-MM-dd") + "~" + Status;
        }

        public override string ToString()
        {
            return Name + " — " + Status + " (до " + Deadline.ToString("dd.MM.yyyy") + ")";
        }
    }
}
