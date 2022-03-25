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
            var data = robots.Select(robot => new
            {
                time = time,
                robotID = robot.robotID,
                currentPosition = robot.currentPosition,
                expectedPosition = robot.expectedPosition,
                motorTemperature = (robot.motorTemperature * 9 / 5 + 32), // convert to fahrenheit
                batteryResistance = robot.batteryResistance,
                lastRepairReason = robot.lastRepairReason,
            }).ToList();
            if (robots.Count == 1) client.PostAsJsonAsync("http://localhost:8000/full", data.FirstOrDefault());
            else client.PostAsJsonAsync("http://localhost:8000/full", data);
        }

        private static void PushMovement(HttpClient client, int time, List<Robot> robots)
        {
            var data = robots.Select(robot => new
            {
                time = time,
                robotID = robot.robotID,
                currentPosition = robot.currentPosition,
                expectedPosition = robot.expectedPosition,
            }).ToList();
            if (robots.Count == 1) client.PostAsJsonAsync("http://localhost:8000/movement", data.FirstOrDefault());
            else client.PostAsJsonAsync("http://localhost:8000/movement", data);
        }

        private static void PushSensor(HttpClient client, int time, List<Robot> robots)
        {
            var data = robots.Select(robot => new
            {
                time = time,
                robotID = robot.robotID,
                motorTemperature = (robot.motorTemperature * 9 / 5 + 32), // convert to fahrenheit
                batteryResistance = robot.batteryResistance,
                lastRepairReason = robot.lastRepairReason,
            }).ToList();
            if (robots.Count == 1) client.PostAsJsonAsync("http://localhost:8000/sensor", data.FirstOrDefault());
            else client.PostAsJsonAsync("http://localhost:8000/sensor", data);
        }
    }
}
