namespace AppointmentManager.Models
{
    public class Appointment
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public DateTime Date { get; set; }
        public string User { get; set; }
        public bool IsApproved { get; set; } = false;
        public bool IsCanceled { get; set; } = false;
    }
}
