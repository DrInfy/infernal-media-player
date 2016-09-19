namespace SEdge.Maths
{
    public struct Average
    {
        private int numbers;
        public double Value { get; private set; }

        public void Add(float val)
        {
            this.Value = (this.Value * this.numbers + val) / (this.numbers + 1);
            this.numbers++;
        }
    }
}