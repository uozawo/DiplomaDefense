using System;

namespace DiplomaDefense.Core
{
    // терміновий етап. це звичайний етап але з причиною чому він терміновий
    // тут показуємо успадкування: беремо все від Stage і додаємо своє
    public class UrgentStage : Stage
    {
        public string Reason { get; set; }   // чому терміново

        public UrgentStage(string name, DateTime deadline, string reason)
            : base(name, deadline)
        {
            Reason = reason;
        }

        public UrgentStage(string name, DateTime deadline, StageStatus status, string reason)
            : base(name, deadline, status)
        {
            Reason = reason;
        }

        public UrgentStage() : base()
        {
            Reason = "не вказано";
        }

        public UrgentStage(UrgentStage other) : base(other)
        {
            Reason = other.Reason;
        }

        // тут "U" щоб потім зрозуміти що це терміновий етап
        // і не загубити причину коли читаємо з файлу
        public override string ToFileString()
        {
            return "U~" + Name + "~" + Deadline.ToString("yyyy-MM-dd") + "~" + Status + "~" + Reason;
        }

        public override string ToString()
        {
            return base.ToString() + " | терміново: " + Reason;
        }
    }
}
