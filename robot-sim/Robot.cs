namespace robot_sim
{
    public class Robot
    {
        public int robotID { get; set; }
        public Personality personality { get; set; }
        public Position position { get; set; }
        public Queue<Position> route { get; set; }
        public Position picker { get; set; }
        public double motorTemperature { get; set; }
        public double batteryResistance { get; set; }
        public int statusFlag { get; set; }

        public Robot(int robotID, Position position, Queue<Position> route, Position picker)
        {
            this.robotID = robotID;
            this.position = position;
            this.route = route;
            this.picker = picker;
            personality = Personality.Normal;
            motorTemperature = 0.0;
            batteryResistance = 0.0;
            statusFlag = 0;
        }

        public void Move(Random random, double briefFaultChance)
        {
            var personalities = new List<Personality> { Personality.Movement, Personality.MovementMotor, Personality.MovementMotorBattery, Personality.MovementBattery, Personality.FullStop, Personality.FullStopMotor, Personality.FullStopMotorBattery, Personality.FullStopBattery };
            if (random.NextDouble() * 100.0 <= briefFaultChance || personalities.Contains(personality))
            {
                var randomMove = random.Next(0, 2);
                if (route.Peek().x - position.x != 0 && randomMove == 0) position.y--;
                else if (route.Peek().x - position.x != 0 && randomMove == 1) position.y++;
                else if (route.Peek().y - position.y != 0 && randomMove == 0) position.x--;
                else if (route.Peek().y - position.y != 0 && randomMove == 1) position.x++;
                statusFlag = 2;
            }
            else
            {
                position = route.Dequeue();
            }
        }

        public void State(Random random, double briefFaultChance)
        {
            var personalities = new List<Personality> { Personality.MovementMotor, Personality.MovementMotorBattery, Personality.MovementBattery, Personality.Motor, Personality.MotorBattery, Personality.Battery, Personality.FullStopMotor, Personality.FullStopMotorBattery, Personality.FullStopBattery };
            if (random.NextDouble() * 100.0 <= briefFaultChance || personalities.Contains(personality))
            {

            }
            else
            {

            }
        }

        public string ToString()
        {
            return "ID: " + robotID
                + " Personality: " + personality
                + " Position: (" + position.x + ", " + position.y + ")"
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
