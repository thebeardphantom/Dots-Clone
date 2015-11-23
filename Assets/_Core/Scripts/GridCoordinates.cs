namespace DotsClone {
    public struct GridCoordinates {
        public static readonly GridCoordinates zero = new GridCoordinates();

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

        public override bool Equals(object obj) {
            if(GetType() != obj.GetType()) {
                return false;
            }

            var other = (GridCoordinates)obj;
            return other.column == column && other.row == row;
        }

        public static bool operator ==(GridCoordinates a, GridCoordinates b) {
            return a.column == b.column && a.row == b.row;
        }

        public static bool operator !=(GridCoordinates a, GridCoordinates b) {
            return a.column != b.column || a.row != b.row;
        }

        public override int GetHashCode() {
            return column.GetHashCode() + row.GetHashCode();
        }
    }
}
