namespace robot_sim
{
    public class Robot
    {
        private int robotID { get; set; }
        private Position position { get; set; }
        private List<Position> expectedPath { get; set; }

        public Robot(int id, Position pos, List<Position> expPath)
        {
            robotID = id;
            position = pos;
            expectedPath = expPath;
        }

        public void setExpectedPath(List<Position> expPath)
        {
            expectedPath = expPath;
        }
    }

    public class Position
    {
        private int x { get; set; }
        private int y { get; set; }
        public Position(int x, int y)
        {
            x = x;
            y = y;
        }
    }
}
