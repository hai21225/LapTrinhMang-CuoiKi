namespace Client.Models
{
    public class Bullet
    {
        public Guid Id { get; set; }
        public Guid OwnerId { get; set; }
        //public float Speed { get; set; }
        public float RotationAngle { get; set; }
        public float X { get; set; }
        public float Y { get; set; }
        //public float Damage { get; set; }
    }
}
