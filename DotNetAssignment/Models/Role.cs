namespace DotNetAssignment.Models
{
    public class Role
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        
        // One role can have many users
        public ICollection<User> Users { get; set; } = new List<User>();
    }
}