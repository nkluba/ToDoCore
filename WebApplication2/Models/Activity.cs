namespace WebApplication2.Models
{
    public class Activity
    {
        public int Id { get; set; }
        public string Description { get; set; }
        public bool IsImportant { get; set; }
        public bool IsDone { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}
