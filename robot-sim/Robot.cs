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
        public int killSwitch { get; set; }

        public Robot(int robotID, Position currentPosition, LinkedList<Position> route, Position picker, double batteryResistance, string lastRepairReason)
        {
            this.robotID = robotID;
            this.currentPosition = expectedPosition = currentPosition;
            this.route = route;
            this.picker = picker;
            this.batteryResistance = batteryResistance;
            this.lastRepairReason = lastRepairReason;
            personality = Personality.Normal;
            motorTemperature = 20.0;
            statusFlag = 0;
            killSwitch = 10;
        }

        public void Move(Random random, double briefFaultChance)
        {
            expectedPosition = route.First(); // robot is moving, so expected is next route position
            var movementPersonalities = new List<Personality> { Personality.Movement, Personality.MovementMotor, Personality.MovementMotorBattery, Personality.MovementBattery };
            var fullStopPersonalities = new List<Personality> { Personality.FullStop, Personality.FullStopMotor, Personality.FullStopMotorBattery, Personality.FullStopBattery };
            var briefDiversion = random.NextDouble() * 100.0 <= briefFaultChance;
            var diversionType = briefDiversion ? random.Next(2) : -1;
            if (diversionType == 0 || movementPersonalities.Contains(personality))
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
            else if (diversionType == 1 || fullStopPersonalities.Contains(personality))
            {
                statusFlag = 2;
            }
            else
            {
                currentPosition = route.First();
                route.RemoveFirst();
                statusFlag = 1;
            }

            if (movementPersonalities.Contains(personality) || fullStopPersonalities.Contains(personality))
                if (--killSwitch < 1) statusFlag = 3;
        }

        public void State(Random random, double briefFaultChance, double motorRate, double batteryRate)
        {
            var motorPersonalities = new List<Personality> { Personality.Motor, Personality.MovementMotor, Personality.MovementMotorBattery, Personality.MotorBattery, Personality.FullStopMotor, Personality.FullStopMotorBattery };
            var batteryPersonalities = new List<Personality> { Personality.Battery, Personality.MovementBattery, Personality.MovementMotorBattery, Personality.MotorBattery, Personality.FullStopBattery, Personality.FullStopMotorBattery };
            var briefDiversion = random.NextDouble() * 100.0 <= briefFaultChance;
            if (briefDiversion || motorPersonalities.Contains(personality))
            {
                var lowerLimit = 80.0;
                if (personality == Personality.Motor || personality == Personality.MotorBattery) lowerLimit = 90.0;
                motorTemperature = MotorTempChange(random, motorTemperature, lowerLimit, 100.0, motorRate);
            }
            
            if (batteryPersonalities.Contains(personality))
            {
                var lowerLimit = 0.9;
                if (personality == Personality.Battery || personality == Personality.MotorBattery) lowerLimit = 1.0;
                if (batteryResistance >= lowerLimit / 2) batteryResistance += lowerLimit - batteryResistance; // jump quickly over limit
                else if (batteryResistance < lowerLimit / 2) batteryResistance += (lowerLimit - batteryResistance) / 2; // jump quickly over limit
                batteryResistance += random.NextDouble() * batteryRate;
            }

            if (personality == Personality.Normal || batteryPersonalities.Contains(personality))
                motorTemperature = MotorTempChange(random, motorTemperature, 70.0, 80.0, motorRate);

            if (personality == Personality.Normal)
            {
                batteryResistance += random.NextDouble() * batteryRate;
                if (batteryResistance >= 1) personality = Personality.Battery;
            }
        }

        private double MotorTempChange(Random random, double current, double minChange, double maxChange, double rate)
        {
            var suggestion = random.NextDouble() * rate;
            var temp = random.Next(2) == 0 ? current + suggestion : current - suggestion;
            if (temp <= minChange) current += suggestion + minChange / current;
            else if (temp >= maxChange) current -= suggestion;
            else current = temp;
            return current;
        }

        public string ToString()
        {
            return "ID: " + robotID
                + " Personality: " + personality
                + " Position: (" + currentPosition.x + ", " + currentPosition.y + ")"
                + " Route length: " + route.Count
                + " Picker: (" + picker.x + ", " + picker.y + ")"
                + " Motor temp (C): " + motorTemperature
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
