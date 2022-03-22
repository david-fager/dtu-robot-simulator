using System.Diagnostics;

namespace robot_sim
{
    public static class SimulationManager
    {
        private static readonly int gridHeight = 50;
        private static readonly int gridWidth = 100;

        private static Random random;
        private static int idCount;
        public static int ticks;
        public static List<Robot> robots;
        public static List<Position> pickerLocations;

        // these values can be set through the UI
        public static bool resetFlag;
        public static int tickSpeed; // ms
        public static int robotMax;
        public static int requestedRobots;

        public static double briefFaultChance = 5.0; // %
        public static double permamentFaultChance = 1.0; // %

        public static void StartTicking()
        {
            // on-start defaults
            Stopwatch sw = new Stopwatch();
            random = new Random();
            resetFlag = true;
            tickSpeed = 1000;
            robotMax = 1;

            // ticking loop
            while (true)
            {
                sw.Reset();
                sw.Start();
                Tick();
                sw.Stop();

                // sleep for remainding time (tickspeed minus how long the tick took)
                if (sw.ElapsedMilliseconds < tickSpeed) Thread.Sleep(Convert.ToInt32(tickSpeed - sw.ElapsedMilliseconds));
            }
        }

        // changeable defaults
        private static void Reset()
        {
            idCount = 0;
            ticks = 0;
            robots = new List<Robot>();
            pickerLocations = new List<Position>();
            for (int i = 9; i < 99; i = i + 10) pickerLocations.Add(new Position(i, 0));
            resetFlag = false;
        }

        private static void Tick()
        {
            if (resetFlag) Reset();
            ticks++;

            if (robots.Count < robotMax) for (int i = robots.Count; i < robotMax; i++) AddRobot();

            robots.RemoveAll(r => r.statusFlag > 2);

            foreach (Robot robot in robots)
            {
                if (robot.statusFlag == 0) robot.statusFlag = 1;

                // the robot made it to the picker, flag 3 = success
                if (robot.currentPosition.x == robot.picker.x && robot.currentPosition.y == robot.picker.y) robot.statusFlag = 4;

                // flag 1 = warning, robot needs path recalculation
                // if (robot.statusFlag == 2) robot.route = PlanRoute(robot.position, robot.picker);

                if (robot.statusFlag < 3)
                {
                    if (random.NextDouble() * 100.0 <= permamentFaultChance) robot.personality = (Personality)random.Next(1, 12);
                    robot.Move(random, briefFaultChance);
                    robot.State(random, briefFaultChance);
                }
            }

            PushEndpoints.PushToEndpoints(ticks, robots);

            for (int i = 0; i < requestedRobots; i++) AddRobot();
            requestedRobots = 0;
        }

        private static void AddRobot()
        {
            var startPosition = new Position(random.Next(gridWidth), random.Next(10, gridHeight));

            // picks a random picker for the robot to go towards
            var pickerLocation = pickerLocations[random.Next(pickerLocations.Count)];

            var plannedRoute = PlanRoute(startPosition, pickerLocation);

            var repairReasons = new List<string> { "unkown", "motor", "battery" }; // TODO: add more?
            var repairReason = repairReasons[random.Next(repairReasons.Count)];

            robots.Add(new Robot(idCount++, startPosition, plannedRoute, pickerLocation, repairReason));
        }

        // calculate the expected route from start position to picker
        private static LinkedList<Position> PlanRoute(Position fromPosition, Position toPosition)
        {
            var expectedPath = new LinkedList<Position>();
            var currentX = fromPosition.x;
            var currentY = fromPosition.y;

            var movementMode = 0;
            var planningStep = 0;
            while (!(currentX == toPosition.x && currentY == toPosition.y))
            {
                planningStep++;
                if (planningStep % 5 == 0 || planningStep % 9 == 0) movementMode = random.Next(0, 3);
                if (movementMode == 0) // goes whichever direction is the shortest, typically zigzagging
                {
                    if (Math.Abs(toPosition.x - currentX) > Math.Abs(toPosition.y - currentY)) expectedPath.AddLast(toPosition.x - currentX > 0 ? new Position(++currentX, currentY) : new Position(--currentX, currentY));
                    else expectedPath.AddLast(toPosition.y - currentY > 0 ? new Position(currentX, ++currentY) : new Position(currentX, --currentY));
                }
                if (movementMode == 1) // prefer going in x direction
                {
                    if (toPosition.x - currentX != 0) expectedPath.AddLast(toPosition.x - currentX > 0 ? new Position(++currentX, currentY) : new Position(--currentX, currentY));
                    else expectedPath.AddLast(toPosition.y - currentY > 0 ? new Position(currentX, ++currentY) : new Position(currentX, --currentY));
                }
                if (movementMode == 2) // prefer going in y direction
                {
                    if (toPosition.y - currentY != 0) expectedPath.AddLast(toPosition.y - currentY > 0 ? new Position(currentX, ++currentY) : new Position(currentX, --currentY));
                    else expectedPath.AddLast(toPosition.x - currentX > 0 ? new Position(++currentX, currentY) : new Position(--currentX, currentY));
                }
            }
            return expectedPath;
        }
    }
}
