namespace Nikse.SubtitleEdit.Core.ContainerFormats.Ebml
{
    internal class Element
    {
        #region  Public Fields and Properties

        public ElementId Id
        {
            get { return this.id; }
        }

        public long DataPosition
        {
            get { return this.dataPosition; }
        }

        public long DataSize
        {
            get { return this.dataSize; }
        }

        public long EndPosition
        {
            get { return this.dataPosition + this.dataSize; }
        }

        #endregion

        #region Local Fields

        private readonly ElementId id;
        private readonly long dataPosition;
        private readonly long dataSize;

        #endregion

        #region Common

        public Element(ElementId id, long dataPosition, long dataSize)
        {
            this.id = id;
            this.dataPosition = dataPosition;
            this.dataSize = dataSize;
        }

        public override string ToString()
        {
            return string.Format(@"{0} ({1})", this.id, this.dataSize);
        }

        #endregion
    }
}