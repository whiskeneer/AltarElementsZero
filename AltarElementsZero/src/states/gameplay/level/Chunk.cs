namespace AltarElementsZero.src.states.gameplay.level
{
    struct Chunk
    {
        public byte Top {  get; set; }
        public byte Bottom { get; set; }
        public byte Left { get; set; }
        public byte Right { get; set; }

        // BackgroundIndex = 0 => invisible chunk (keep configuration of previously populated chunk)
        

        public byte BackgroundIndex { get; set; }
        public byte Reserved1 { get; set; }
        public byte Reserved2 { get; set; }
        public byte Reserved3 { get; set; }


    }
}
