using Newtonsoft.Json;
using System.Net;

namespace robot_sim
{
    public static class PushEndpoints
    {
        public static string BaseURL = "http://localhost:8001";

        public static void PushToEndpoints(int tickSpeed, int time, List<Robot> robots)
        {
            //if (CheckConnection(tickSpeed))
            //{
                using (var client = new HttpClient())
                {
                    PushFull(client, time, robots);
                    PushMovement(client, time, robots);
                    PushSensor(client, time, robots);
                }
            //}
        }

        private static bool CheckConnection(int tickSpeed)
        {
            if (tickSpeed < 1000) return true; // the program will crash if endpoint isn't reachable
            try
            {
                HttpWebRequest request = (HttpWebRequest)WebRequest.Create(BaseURL);
                request.Method = "HEAD";
                request.Timeout = 900;
                HttpWebResponse response = (HttpWebResponse)request.GetResponse();
                response.Close();
                return (response.StatusCode == HttpStatusCode.OK);
            }
            catch
            {
                return false;
            }
        }

        private static void PushFull(HttpClient client, int time, List<Robot> robots)
        {
            foreach (var robot in robots)
            {
                var data = new
                {
                    time = time,
                    robotID = robot.robotID,
                    currentPosition = robot.currentPosition,
                    expectedPosition = robot.expectedPosition,
                    motorTemperature = (robot.motorTemperature * 9 / 5 + 32), // convert to fahrenheit
                    batteryResistance = robot.batteryResistance,
                    //lastRepairReason = robot.lastRepairReason,
                };

                var request = new HttpRequestMessage(HttpMethod.Post, "http://localhost:8001/full");
                request.Content = new StringContent(JsonConvert.SerializeObject(data), System.Text.Encoding.UTF8, "application/json");
                client.Send(request);
            }
        }

        private static void PushMovement(HttpClient client, int time, List<Robot> robots)
        {
            foreach (var robot in robots)
            {
                var data = new
                {
                    time = time,
                    robotID = robot.robotID,
                    currentPosition = robot.currentPosition,
                    expectedPosition = robot.expectedPosition,
                };

                var request = new HttpRequestMessage(HttpMethod.Post, BaseURL + "/movement");
                request.Content = new StringContent(JsonConvert.SerializeObject(data), System.Text.Encoding.UTF8, "application/json");
                client.Send(request);
            }
        }

        private static void PushSensor(HttpClient client, int time, List<Robot> robots)
        {
            foreach (var robot in robots)
            {
                var data = new
                {
                    time = time,
                    robotID = robot.robotID,
                    motorTemperature = (robot.motorTemperature * 9 / 5 + 32), // convert to fahrenheit
                    batteryResistance = robot.batteryResistance,
                    //lastRepairReason = robot.lastRepairReason,
                };

                var request = new HttpRequestMessage(HttpMethod.Post, BaseURL + "/sensor");
                request.Content = new StringContent(JsonConvert.SerializeObject(data), System.Text.Encoding.UTF8, "application/json");
                client.Send(request);
            }
        }
    }
}
