namespace Server
{
    public class Player
    {
        public Guid Id { get; set; } = Guid.NewGuid();
        public  int X { get; set; }
        public  int Y { get; set; }
        public float Rotation {  get; set; }
        public float Hp { get; set; }
    }
}
