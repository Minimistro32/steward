using System.Net.Mail;
using System.Text;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Steward.Server.Data;
using Steward.Server.Data.Entities;

namespace Steward.Server.Setup;

public static class TerminalSetup
{
    public static async Task<int> RunAsync(StewardDbContext db)
    {
        if (await IsConfiguredAsync(db))
        {
            Console.Error.WriteLine("Setup has already been completed or an admin already exists. No accounts were changed.");
            return 1;
        }

        if (Console.IsInputRedirected || Console.IsOutputRedirected)
        {
            Console.Error.WriteLine("Setup requires an interactive terminal. In a container, allocate a TTY with -it.");
            return 1;
        }

        Console.WriteLine("Welcome to Steward. Create your first admin account.");
        Console.WriteLine("Press Ctrl+C to cancel. Your PIN will not be displayed.");
        try
        {
            var name = ReadValue("Name: ", value => value.Length > 0, "Enter a name.");
            var email = ReadValue("Email: ", value =>
                MailAddress.TryCreate(value, out var address) && address.Address == value,
                "Enter a valid email address, such as name@example.com.");
            string pin;
            while (true)
            {
                pin = ReadSecret("PIN (at least 4 digits): ");
                if (pin.Length < 4 || pin.Length > 128 || pin.Any(character => character is < '0' or > '9'))
                {
                    Console.WriteLine("Enter at least four digits (maximum 128).");
                    continue;
                }
                if (pin == ReadSecret("Confirm PIN: ")) break;
                Console.WriteLine("PINs did not match. Try again.");
            }

            // SQLite's serializable transaction locks writes before rechecking,
            // preventing concurrent setup processes from creating two admins.
            await using var transaction = await db.Database.BeginTransactionAsync();
            if (await IsConfiguredAsync(db))
            {
                Console.Error.WriteLine("Another setup has completed. No accounts were changed.");
                return 1;
            }
            var user = new UserEntity { Name = name, Email = email, Type = UserType.Admin };
            user.PinHash = new PasswordHasher<UserEntity>().HashPassword(user, pin);
            db.Users.Add(user);
            db.SetupStates.Add(new SetupStateEntity { Id = 1, CompletedAt = DateTimeOffset.UtcNow });
            await db.SaveChangesAsync();
            await transaction.CommitAsync();
            Console.WriteLine("Admin account created. Start Steward with: dotnet run --project src/Steward.Server");
            return 0;
        }
        catch (OperationCanceledException)
        {
            Console.WriteLine("Setup cancelled. No account was created.");
            return 1;
        }
    }

    private static async Task<bool> IsConfiguredAsync(StewardDbContext db) =>
        await db.SetupStates.AnyAsync() || await db.Users.AnyAsync(user => user.Type == UserType.Admin);

    private static string ReadValue(string prompt, Func<string, bool> isValid, string error)
    {
        while (true)
        {
            Console.Write(prompt);
            var value = Console.ReadLine()?.Trim() ?? throw new OperationCanceledException();
            if (isValid(value)) return value;
            Console.WriteLine(error);
        }
    }

    private static string ReadSecret(string prompt)
    {
        Console.Write(prompt);
        var value = new StringBuilder();
        var previous = Console.TreatControlCAsInput;
        try
        {
            Console.TreatControlCAsInput = true;
            while (true)
            {
                var key = Console.ReadKey(intercept: true);
                if (key.Key == ConsoleKey.C && key.Modifiers.HasFlag(ConsoleModifiers.Control))
                    throw new OperationCanceledException();
                if (key.Key == ConsoleKey.Enter) return value.ToString();
                if (key.Key == ConsoleKey.Backspace)
                {
                    if (value.Length > 0) value.Length--;
                }
                else if (!char.IsControl(key.KeyChar) && value.Length < 128)
                    value.Append(key.KeyChar);
            }
        }
        finally
        {
            Console.TreatControlCAsInput = previous;
            Console.WriteLine();
        }
    }
}
