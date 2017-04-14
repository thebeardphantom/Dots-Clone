namespace DotsClone
{
    /// <summary>
    /// Defines a dot's location in the dot grid
    /// </summary>
    public struct GridCoordinates
    {
        /// <summary>
        /// X Position in Grid
        /// </summary>
        public byte column;
        /// <summary>
        /// Y Position in Grid
        /// </summary>
        public byte row;

        public GridCoordinates(byte column, byte row)
        {
            this.column = column;
            this.row = row;
        }

        public override bool Equals(object obj)
        {
            if (GetType() != obj.GetType())
            {
                return false;
            }

            var other = (GridCoordinates)obj;
            return other.column == column && other.row == row;
        }

        public static bool operator ==(GridCoordinates left, GridCoordinates right)
        {
            return left.column == right.column && left.row == right.row;
        }

        public static bool operator !=(GridCoordinates left, GridCoordinates right)
        {
            return left.column != right.column || left.row != right.row;
        }

        public override int GetHashCode()
        {
            return column.GetHashCode() + row.GetHashCode();
        }
    }
}
