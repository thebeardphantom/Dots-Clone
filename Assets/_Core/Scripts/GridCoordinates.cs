namespace DotsClone {
    public struct GridCoordinates {
        public static readonly GridCoordinates zero = new GridCoordinates();
        public static readonly GridCoordinates max = new GridCoordinates(byte.MaxValue, byte.MaxValue);

        /// <summary>
        /// X Position in Grid
        /// </summary>
        public byte column;
        /// <summary>
        /// Y Position in Grid
        /// </summary>
        public byte row;

        public GridCoordinates(byte column, byte row) {
            this.column = column;
            this.row = row;
        }
    }
}
