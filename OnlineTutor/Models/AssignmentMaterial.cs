namespace OnlineTutor.Models
{
    public class AssignmentMaterial
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public string FileUrl { get; set; }
        public MaterialType Type { get; set; }
        public int AssignmentId { get; set; }
        public virtual Assignment Assignment { get; set; }
    }

    public enum MaterialType
    {
        Document,
        Video,
        Link,
        Image
    }
}
