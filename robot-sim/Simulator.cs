using System.Diagnostics;

namespace robot_sim
{
    public static class Simulator
    {
        private static Stopwatch sw = new Stopwatch();
        private static Random random = new Random();
        private static int idCount;
        public static int ticks;
        public static List<Robot> robots;
        public static List<Position> pickerLocations;

        // these values can be set through the UI
        public static int tickSpeed = 1;
        public static bool resetFlag = true;
        public static int robotCap = 100;
        public static double faultChance = 10;

        public static void StartThread()
        {
            Thread thread = new Thread(StartTicking);
            thread.Start();
        }

        public static void StartTicking()
        {
            while (true)
            {
                if (resetFlag)
                {
                    idCount = 0;
                    ticks = 0;
                    robots = new List<Robot>();
                    pickerLocations = new List<Position>();
                    for (int i = 9; i < 99; i = i + 10) pickerLocations.Add(new Position(i, 0));
                    resetFlag = false;

                    // boundary test bots
                    //addRobot(new Position(0, 0));
                    //addRobot(new Position(99, 49));
                    //addRobot(new Position(0, 49));
                    //addRobot(new Position(99, 0));
                }
                sw.Reset();
                sw.Start();
                tick();
                sw.Stop();
                if (sw.ElapsedMilliseconds < ((long)tickSpeed * 1000l)) Thread.Sleep(Convert.ToInt32(((long)tickSpeed * 1000l) - sw.ElapsedMilliseconds));
            }
        }

        private static void tick()
        {
            Debug.WriteLine("tick " + ticks++); // For debugging

            if (robots.Count < robotCap) for (int i = robots.Count; i < robotCap; i++) addRobot();

            robots.RemoveAll(r => r.statusFlag == 2 || r.statusFlag == 3);

            foreach (Robot robot in robots)
            {
                // the robot made it to the picker, flag 3 = success
                if (robot.position.x == robot.pickerLocation.x && robot.position.y == robot.pickerLocation.y) robot.statusFlag = 3;

                // flag 1 = warning, robot needs path recalculation
                if (robot.statusFlag == 1) robot.expectedPath = calculatePath(robot.position, robot.pickerLocation);

                if (robot.statusFlag < 2) moveRobot(robot);

                //Debug.WriteLine(robot.ToString()); // For debugging
            }
        }

        private static void addRobot(Position specificPosition = null)
        {
            var position = specificPosition ?? new Position(random.Next(0, 99), random.Next(10, 49)); // start position

            var pickerLocation = pickerLocations[random.Next(0, pickerLocations.Count)]; // random picker location

            var expectedPath = calculatePath(position, pickerLocation); // calculate the expected route to picker

            robots.Add(new Robot(idCount++, position, expectedPath, pickerLocation));
        }

        private static Queue<Position> calculatePath(Position fromPosition, Position toPosition)
        {
            var expectedPath = new Queue<Position>();
            var currentX = fromPosition.x;
            var currentY = fromPosition.y;

            var mode = 0;
            var count = 0;
            var mod = 5;
            while (currentX != toPosition.x || currentY != toPosition.y)
            {
                if (count++ % mod == 0) mode = random.Next(0, 3);

                // direct movement
                if (mode == 0 || mode == 1)
                {
                    if (Math.Abs(toPosition.x - currentX) > Math.Abs(toPosition.y - currentY)) expectedPath.Enqueue(toPosition.x - currentX > 0 ? new Position(++currentX, currentY) : new Position(--currentX, currentY));
                    else expectedPath.Enqueue(toPosition.y - currentY > 0 ? new Position(currentX, ++currentY) : new Position(currentX, --currentY));
                }

                // prefer x
                if (mode == 2)
                {
                    if (toPosition.x - currentX != 0) expectedPath.Enqueue(toPosition.x - currentX > 0 ? new Position(++currentX, currentY) : new Position(--currentX, currentY));
                    else expectedPath.Enqueue(toPosition.y - currentY > 0 ? new Position(currentX, ++currentY) : new Position(currentX, --currentY));
                }

                // prefer y
                if (mode == 3)
                {
                    if (toPosition.y - currentY != 0) expectedPath.Enqueue(toPosition.y - currentY > 0 ? new Position(currentX, ++currentY) : new Position(currentX, --currentY));
                    else expectedPath.Enqueue(toPosition.x - currentX > 0 ? new Position(++currentX, currentY) : new Position(--currentX, currentY));
                }
            }

            return expectedPath;
        }

        private static void moveRobot(Robot robot)
        {
            // correct movement otherwise wrong movement
            if (random.NextDouble() * 100.0 > faultChance - 1.0)
            {
                robot.position = robot.expectedPath.Dequeue();
                robot.statusFlag = 0;
            }
            else
            {
                while (true)
                {
                    // wrong movement calculation continues untill one actually differs
                    var saveX = robot.position.x;
                    var saveY = robot.position.y;

                    var rMove = random.Next(0, 4); // 0 means no movement
                    if (rMove == 1) robot.position.x--;
                    else if (rMove == 2) robot.position.y++;
                    else if (rMove == 3) robot.position.x++;
                    else if (rMove == 4) robot.position.y--;

                    if (robot.position.x != robot.expectedPath.Peek().x || robot.position.y != robot.expectedPath.Peek().y) break;
                    else robot.position = new Position(saveX, saveY);
                }

                // if robot is status good, then set warning -- if warning set broken (flag 2)
                robot.statusFlag = robot.statusFlag == 0 ? 1 : 2;
            }
        }
    }
}
