// See https://aka.ms/new-console-template for more information
// using System.Net.Http;
// using System.Text.Json;

// Console.WriteLine(args[0]);
// string url  = $"https://api.github.com/users/{args[0]}/events";
// var https = new HttpClient();
// https.DefaultRequestHeaders.UserAgent.ParseAdd("BananaMonkey");
// var response = await https.GetStringAsync(url); 

// var jsonFormat = JsonSerializer.Deserialize<JsonElement>(response);
// var jsons = jsonFormat[0];
// var payload = jsons.GetProperty("payload");


// // var jsonSerialization = JsonSerializer.Serialize(jsonFormat, new JsonSerializerOptions{WriteIndented = true});

// Console.WriteLine(payload);

//  using GitHubActivity.Presentation;

// TaskMenu taskMenu = new TaskMenu();
// await taskMenu.Start(args[0]);