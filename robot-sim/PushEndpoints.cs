using Newtonsoft.Json;
using System.Net;

namespace robot_sim
{
    public static class PushEndpoints
    {
        public static string BaseURL = "http://localhost:8001";

        public static void PushToEndpoints(int tickSpeed, int time, List<Robot> robots)
        {
            using (var client = new HttpClient())
            {
                foreach (var robot in robots)
                {
                    //PushFull(client, time, robot);
                    //PushMovement(client, time, robot);
                    PushSensor(client, time, robot);
                }
            }
        }

        private static void PushFull(HttpClient client, int time, Robot robot)
        {

            CallPost(client, "/full", JsonConvert.SerializeObject(new
            {
                time = time,
                robotID = robot.robotID,
                currentPosition = robot.currentPosition,
                expectedPosition = robot.expectedPosition,
                motorTemperature = (robot.motorTemperature * 9 / 5 + 32), // convert to fahrenheit
                batteryResistance = robot.batteryResistance,
                //lastRepairReason = robot.lastRepairReason,
            }));
        }

        private static void PushMovement(HttpClient client, int time, Robot robot)
        {
            CallPost(client, "/movement", JsonConvert.SerializeObject(new
            {
                time = time,
                robotID = robot.robotID,
                currentPosition = robot.currentPosition,
                expectedPosition = robot.expectedPosition,
            }));
        }

        private static void PushSensor(HttpClient client, int time, Robot robot)
        {
            CallPost(client, "/sensor", JsonConvert.SerializeObject(new
            {
                time = time,
                robotID = robot.robotID,
                motorTemperature = (robot.motorTemperature * 9 / 5 + 32), // convert to fahrenheit
                batteryResistance = robot.batteryResistance,
                //lastRepairReason = robot.lastRepairReason,
            }));
        }

        private static void CallPost(HttpClient client, string endpoint, string jsonData)
        {
            try
            {
                var request = new HttpRequestMessage(HttpMethod.Post, BaseURL + endpoint);
                request.Content = new StringContent(jsonData, System.Text.Encoding.UTF8, "application/json");
                //client.Timeout = TimeSpan.FromMilliseconds(900);
                client.Send(request);
            }
            catch
            {
            }
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
    }
}
