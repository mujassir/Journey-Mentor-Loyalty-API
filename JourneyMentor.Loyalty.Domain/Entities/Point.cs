using System;

namespace JourneyMentor.Loyalty.Domain.Entities
{
    public class Point : Base
    {
        public Guid UserId { get; set; }
        public int Points { get; set; }
    }
}
