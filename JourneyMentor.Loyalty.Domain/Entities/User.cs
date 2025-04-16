namespace JourneyMentor.Loyalty.Domain.Entities
{
    public class User
    {
        public Guid Id { get; set; }
        public required string Username { get; set; }
        public required string Name { get; set; }
        public string? Email { get; set; }
    }
}
