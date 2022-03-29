using Newtonsoft.Json;

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

            try
            {
                foreach (var robot in robots)
                {
                    var request = new HttpRequestMessage(HttpMethod.Post, "http://localhost:8001/full");
                    request.Content = new StringContent(JsonConvert.SerializeObject(robot), System.Text.Encoding.UTF8, "application/json");
                    client.Send(request);
                }
            }
            catch (Exception ex)
            {

            }
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
            try
            {
                foreach (var robot in robots)
                {
                    var request = new HttpRequestMessage(HttpMethod.Post, "http://localhost:8001/movement");
                    request.Content = new StringContent(JsonConvert.SerializeObject(robot), System.Text.Encoding.UTF8, "application/json");
                    client.Send(request);
                }
            }
            catch (Exception ex)
            {

            }
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

            try
            {
                foreach (var robot in robots)
                {
                    var request = new HttpRequestMessage(HttpMethod.Post, "http://localhost:8001/sensor");
                    request.Content = new StringContent(JsonConvert.SerializeObject(robot), System.Text.Encoding.UTF8, "application/json");
                    client.Send(request);
                }
            }
            catch (Exception ex)
            {

            }
        }
    }
}
