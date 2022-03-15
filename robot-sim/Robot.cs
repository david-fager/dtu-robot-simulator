namespace robot_sim
{
    public class Robot
    {
        public int robotID { get; set; }
        public Position position { get; set; }
        public Queue<Position> expectedPath { get; set; }
        public Position pickerLocation { get; set; }
        public int statusFlag { get; set; }

        public Robot(int id, Position pos, Queue<Position> expPath, Position pickerLoc)
        {
            this.robotID = id;
            this.position = pos;
            this.expectedPath = expPath;
            this.pickerLocation = pickerLoc;
            this.statusFlag = 0;
        }

        public string ToString()
        {
            return "ID: " + robotID
                + " Pos: (" + position.x + ", " + position.y + ")"
                + " Moves: " + expectedPath.Count
                + " Picker: (" + pickerLocation.x + ", " + pickerLocation.y + ")"
                + " Status: " + statusFlag;
        }
    }

    public class Position
    {
        public int x { get; set; }
        public int y { get; set; }
        public Position(int x, int y)
        {
            this.x = x;
            this.y = y;
        }
    }
}
