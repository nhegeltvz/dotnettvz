using Data.Dto.CRUD.User;
using System.Net.Http.Json;

namespace Seeder
{
    public class DataSeeder
    {
        public async Task SeedData()
        {
            var handler = new HttpClientHandler { UseCookies = true };
            var client = new HttpClient(handler) { BaseAddress = new Uri("http://localhost:5097") };

            await SeedUsers(client);

            // login as admin after SeedUsers so registration auto-login doesn't overwrite the session
            var loginPage = await client.GetStringAsync("/Identity/Account/Login");
            var token = ExtractAntiforgeryToken(loginPage);

            await client.PostAsync("/Identity/Account/Login", new FormUrlEncodedContent(new[] {
                new KeyValuePair<string, string>("Input.Email", "admin@admin.com"),
                new KeyValuePair<string, string>("Input.Password", "Admin123!"),
                new KeyValuePair<string, string>("__RequestVerificationToken", token),
            }));

            await SeedPlayers(client);
            //await SeedPlayingFields();
            //await SeedParties();
            //await SeedMatches();

        }

        private async Task SeedMatches()
        {
            //throw new NotImplementedException();
        }

        private async Task SeedParties()
        {
            //throw new NotImplementedException();
        }

        private async Task SeedPlayingFields()
        {
            //throw new NotImplementedException();
        }

        private async Task SeedPlayers(HttpClient client)
        {
            var users = await client.GetFromJsonAsync<List<UserDto>>("/api/users");
        }
        private async Task SeedUsers(HttpClient client)
        {
            var users = new[]
            {
                ("dlivakovic", "dlivakovic@tvz.hr", "Dlivakovic123!"),
                ("jstanisic", "jstanisic@tvz.hr", "Jstanisic123!"),
                ("jsutalo", "jsutalo@tvz.hr", "Jsutalo123!"),
                ("jgvardiol", "jgvardiol@tvz.hr", "Jgvardiol123!"),
                ("lvuskovic", "lvuskovic@tvz.hr", "Lvuskovic123!"),
                ("lmodric", "lmodric@tvz.hr", "Lmodric123!"),
                ("mpasalic", "mpasalic@tvz.hr", "Mpasalic123!"),
                ("mbaturina", "mbaturina@tvz.hr", "Mbaturina123!"),
                ("psucic", "psucic@tvz.hr", "Psucic123!"),
                ("iperisic", "iperisic@tvz.hr", "Iperisic123!"),
                ("pmusa", "pmusa@tvz.hr", "Pmusa123!")
            };

            foreach(var (username, email, password) in users)
            {
                var pageHtml = await client.GetStringAsync("/Identity/Account/Register");
                var token = ExtractAntiforgeryToken(pageHtml);

               var response = await client.PostAsync("/Identity/Account/Register", new FormUrlEncodedContent(new[]
                {
                    new KeyValuePair<string, string>("Input.Username", username),
                    new KeyValuePair<string, string>("Input.Email", email),
                    new KeyValuePair<string, string>("Input.Password", password),
                    new KeyValuePair<string, string>("Input.ConfirmPassword", password),
                    new KeyValuePair<string, string>("__RequestVerificationToken", token),
                }));

                Console.WriteLine($"Registered user {username} with status code: {response.StatusCode}");
                var body = await response.Content.ReadAsStringAsync();
                var errors = System.Text.RegularExpressions.Regex.Matches(body, @"class=""text-danger[^""]*""[^>]*>(.*?)</span>");
                foreach (System.Text.RegularExpressions.Match error in errors)
                    Console.WriteLine($"  Error: {error.Groups[1].Value}");
            }
        }

        private string ExtractAntiforgeryToken(string html)
        {
            var match = System.Text.RegularExpressions.Regex.Match(
                html,
                @"<input[^>]*name=""__RequestVerificationToken""[^>]*value=""([^""]+)"""
            );

            if (!match.Success)
                throw new Exception("Antiforgery token not found in login page HTML.");

            return match.Groups[1].Value;
        }
    }

}
