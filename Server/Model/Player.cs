namespace Server
{
    public class Player
    {
        // player
        public Guid Id { get; set; } = Guid.NewGuid();
        public  int X { get; set; } // toa do goc trai x =0
        public  int Y { get; set; }// toa do goc trai y=0
        public float Rotation {  get; set; }
        public float Hp { get; set; }

        //sprite
        public float Width { get; set; }// chieu dai cua sprite
        public float Height { get; set; }// chieu cao cua sprite

        // pivot xoay cua sprite
        public float PivotX { get; set; }// pivot 
        public float PivotY { get; set;}



    }
}
