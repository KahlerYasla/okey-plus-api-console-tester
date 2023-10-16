using System;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.AspNetCore.SignalR.Client;
using Microsoft.Extensions.Options;
using Microsoft.Identity.Client;



Console.BackgroundColor = ConsoleColor.DarkGreen;
Console.ForegroundColor = ConsoleColor.Black;
Console.WriteLine("----- Okey Plus Client -----");
Console.ResetColor();

Console.WriteLine("====================================");

// Replace with the URL of your SignalR hub
var hubUrl = "https://localhost:7075/chatHub";

string jwtToken = "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJ1c2VySWQiOiI3OWRlM2VhMC1iOWVkLTQzMjgtODA5Zi1mMjQzMjlmNTExM2IiLCJuYmYiOjE2OTQ3MDMyMjYsImV4cCI6MTY5NDczMzIyNiwiaXNzIjoiSXNzdWVySW5mb3JtYXRpb24iLCJhdWQiOiJBdWRpZW5jZUluZm9ybWF0aW9uIn0.Ktf-gvbGjLtID48pnb-kIOEfWn5ztld4RWwIWxwqqow";

var hubConnection = new HubConnectionBuilder()
    .WithUrl(hubUrl, options =>
    {
        options.Headers["Authorization"] = "Bearer " + jwtToken;
    })
    .Build();

hubConnection.On<string, string>("ReceiveMessage", (user, message) =>
{
    Console.ForegroundColor = ConsoleColor.Black;
    Console.BackgroundColor = ConsoleColor.DarkGreen;
    Console.Write($"{user}:");

    Console.ResetColor();
    Console.ForegroundColor = ConsoleColor.DarkGreen;
    Console.WriteLine($" {message}");
    Console.ResetColor();

    Console.WriteLine("====================================");
    RenderThePlayersBoard(message);
    Console.WriteLine("====================================");


});

await hubConnection.StartAsync();

Console.Write("Enter your username: ");

var user = Console.ReadLine();
Console.WriteLine("====================================");

Console.ForegroundColor = ConsoleColor.Green;
Console.Write("How to use: Type a message (or 'exit' to quit) \n00 GetPieceFromLeft, 01 GetPieceFromMiddle, 02 ThrowPieceToRight, 03 OpenPair, 04 OpenSeries\n");
Console.ResetColor();

Console.WriteLine("====================================");

await hubConnection.InvokeAsync("JoinOrCreateGroup", user);

while (true)
{
    var message = Console.ReadLine() ?? "";

    if (message.ToLower() == "exit")
    {
        break;
    }

    Console.WriteLine("====================================");

    //     public async Task SendMessage(string userNameOfSender, string? userNameOfReceiver, string? groupId, string message)
    await hubConnection.InvokeAsync("SendMessage", user, null, null, message);
}

await hubConnection.StopAsync();

#region GUI ====================================

void RenderThePlayersBoard(string message)
{
    // Deserialize the JSON message to a C# object
    var gameData = JsonSerializer.Deserialize<GameData>(message);

    if (gameData != null)
    {
        Console.Write("Your Deck ~> ");
        foreach (var piece in gameData.PlayerPieces!)
        {
            Console.ForegroundColor = GetPieceColor(piece[0]);
            Console.Write($"{piece} ");
            Console.ResetColor();
        }
        Console.WriteLine();
    }
    else
    {
        Console.WriteLine("Failed to parse game data.");
    }
}

// Define a method to get the color for a piece based on its code
static ConsoleColor GetPieceColor(char code)
{
    return code switch
    {
        'r' => ConsoleColor.Red,
        'g' => ConsoleColor.Green,
        'b' => ConsoleColor.Blue,
        'o' => ConsoleColor.DarkYellow,
        _ => ConsoleColor.White,
    };
}


#endregion GUI ====================================