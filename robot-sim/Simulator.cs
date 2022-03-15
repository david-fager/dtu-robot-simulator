namespace robot_sim
{
    public static class Simulator
    {
        private static Random random = new Random();
        private static System.Timers.Timer timer;
        private static int ticks;
        private static List<Robot> robots;
        private static List<Position> pickerLocations = new List<Position>
        {
            new Position(1, 10), new Position(1, 20), new Position(1, 30), new Position(1, 40),
            new Position(1, 50), new Position(1, 60), new Position(1, 70), new Position(1, 80),
        };

        private static int robotCap = 1;
        private static int faultChance = 10;

        public static void StartTicking()
        {
            // starts and resets too
            if (timer == null)
            {
                ticks = 0;
                robots = new List<Robot>();

                timer = new System.Timers.Timer();
                timer.Interval = 1000;
                timer.Elapsed += tick;
                timer.Start();
            }
        }

        private static void tick(object sender, EventArgs e)
        {
            System.Diagnostics.Debug.WriteLine("tick " + ticks++);
            
            if (robots.Count < robotCap)
            {
                addRobots();
            }

            foreach(Robot robot in robots)
            {

            }
        }

        private static void addRobots()
        {
            for (int i = robots.Count; i < robotCap; i++)
            {
                var rStartX = random.Next(0, 99);
                var rStartY = random.Next(15, 99);

                var expectedPath = new List<Position>();


                robots.Add(new Robot(ticks, new Position { x = rStartX, y = rStartY }));

            }
        }

        private static void moveRobot()
        {
            // move the robot closer to
        }
    }
}
