namespace robot_sim
{
    public class Robot
    {
        public int robotID { get; set; }
        public Personality personality { get; set; }
        public Position currentPosition { get; set; }
        public Position expectedPosition { get; set; }
        public LinkedList<Position> route { get; set; }
        public Position picker { get; set; }
        public double motorTemperature { get; set; }
        public double batteryResistance { get; set; }
        public int statusFlag { get; set; }
        public string lastRepairReason { get; set; }

        public Robot(int robotID, Position currentPosition, LinkedList<Position> route, Position picker, string lastRepairReason)
        {
            this.robotID = robotID;
            this.currentPosition = expectedPosition = currentPosition;
            this.route = route;
            this.picker = picker;
            this.lastRepairReason = lastRepairReason;
            personality = Personality.Normal;
            motorTemperature = 0.0;
            batteryResistance = 0.0;
            statusFlag = 0;
        }

        public void Move(Random random, double briefFaultChance)
        {
            expectedPosition = route.First(); // robot is moving, so expected is next route position
            var personalities = new List<Personality> { Personality.Movement, Personality.MovementMotor, Personality.MovementMotorBattery, Personality.MovementBattery, Personality.FullStop, Personality.FullStopMotor, Personality.FullStopMotorBattery, Personality.FullStopBattery };
            var briefDiversion = random.NextDouble() * 100.0 <= briefFaultChance;
            if (briefDiversion || personalities.Contains(personality))
            {
                var currentX = currentPosition.x;
                var currentY = currentPosition.y;

                var randomMove = random.Next(0, 2);
                if (route.First().x - currentPosition.x != 0 && randomMove == 0) currentPosition.y--;
                else if (route.First().x - currentPosition.x != 0 && randomMove == 1) currentPosition.y++;
                else if (route.First().y - currentPosition.y != 0 && randomMove == 0) currentPosition.x--;
                else if (route.First().y - currentPosition.y != 0 && randomMove == 1) currentPosition.x++;
                statusFlag = 2;

                if (briefDiversion) route.AddFirst(new Position(currentX, currentY));
            }
            else
            {
                currentPosition = route.First();
                route.RemoveFirst();
                statusFlag = 1;
            }
        }

        public void State(Random random, double briefFaultChance)
        {
            var personalities = new List<Personality> { Personality.MovementMotor, Personality.MovementMotorBattery, Personality.MovementBattery, Personality.Motor, Personality.MotorBattery, Personality.Battery, Personality.FullStopMotor, Personality.FullStopMotorBattery, Personality.FullStopBattery };
            if (random.NextDouble() * 100.0 <= briefFaultChance || personalities.Contains(personality))
            {
                // TODO: implement
            }
            else
            {
                // TODO: implement
            }
        }

        public string ToString()
        {
            return "ID: " + robotID
                + " Personality: " + personality
                + " Position: (" + currentPosition.x + ", " + currentPosition.y + ")"
                + " Route length: " + route.Count
                + " Picker: (" + picker.x + ", " + picker.y + ")"
                + " Motor temp: " + motorTemperature
                + " Battery resistance: " + batteryResistance
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

    public enum Personality {
        Normal, // no problems
        Movement, // only movement problems
        MovementMotor, // movement and motor temp problems
        MovementMotorBattery, // movement, motor temp and battery problems
        MovementBattery, // movement and battery problems
        Motor, // only motor temp problems
        MotorBattery, // motor and battery problems
        Battery, // only battery problems
        FullStop, // movement stop
        FullStopMotor, // movement stop
        FullStopMotorBattery, // movement stop
        FullStopBattery, // movement stop
    }
}
