using System.Net.Http;

namespace ConsoleApp1.task3
{
    public class ApiSimulator
    {
        public string GetUserData(int userId)
        {
            var _httpClient = new HttpClient();
            // Simulate an API call
            Task<string> dataTask = _httpClient.GetStringAsync($"https://api.example.com/users/{userId}");
            return dataTask.Result;
        }
    }
}
