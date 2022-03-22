using Newtonsoft.Json;
using System.Net;

namespace robot_sim
{
    public static class PushEndpoints
    {
        public static void PushToEndpoints(int time, List<Robot> robots)
        {
            using (var client = new HttpClient())
            {
                PushFull(client, time, robots);
                PushMovement(client, time, robots);
                PushSensor(client, time, robots);
            }
        }


        private static void PushFull(HttpClient client, int time, List<Robot> robots)
        {
            // TODO: pick data
            client.PostAsJsonAsync("http://localhost:8000/full", new { foo = "bar" });
        }

        private static void PushMovement(HttpClient client, int time, List<Robot> robots)
        {
            // TODO: pick data
            client.PostAsJsonAsync("http://localhost:8000/movement", new { foo = "bar" });
        }

        private static void PushSensor(HttpClient client, int time, List<Robot> robots)
        {
            // TODO: pick data
            client.PostAsJsonAsync("http://localhost:8000/sensor", new { foo = "bar" });
        }


        private class FullData
        {
            public int time { get; set; }
            public int robotID { get; set; }
            public Position currentPosition { get; set; }
            public Position expectedPosition { get; set; }
            public double motorTemperature { get; set; }
            public double batteryResistance { get; set; }
            public string lastRepairReason { get; set; }
        }

        private class MovementData
        {
            public int time { get; set; }
            public int robotID { get; set; }
            public Position currentPosition { get; set; }
            public Position expectedPosition { get; set; }
        }

        private class SensorData
        {
            public int time { get; set; }
            public int robotID { get; set; }
            public double motorTemperature { get; set; }
            public double batteryResistance { get; set; }
            public string lastRepairReason { get; set; }
        }
    }
}
