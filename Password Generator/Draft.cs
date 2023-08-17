//using System;
//using System.Diagnostics;
//using System.Security.Cryptography;
//using System.Text;
//using Spectre.Console;
//using TextCopy;

//public class PasswordGenerator
//{
//    private static readonly string LowercaseChars = "abcdefghijklmnopqrstuvwxyz";
//    private static readonly string UppercaseChars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ";
//    private static readonly string NumberChars = "0123456789";
//    private static readonly string SpecialChars = "!@#$%^&*()-_=+=|;:,.<>?";

//    public static string GeneratePassword(bool useLowercase, bool useUppercase, bool useNumbers, bool useSpecial, int length)
//    {
//        if (length <= 0 || (!useLowercase && !useUppercase && !useNumbers && !useSpecial))
//        {
//            throw new ArgumentException("Invalid password settings.");
//        }

//        var charPool = new StringBuilder();
//        if (useLowercase)
//            charPool.Append(LowercaseChars);
//        if (useUppercase)
//            charPool.Append(UppercaseChars);
//        if (useNumbers)
//            charPool.Append(NumberChars);
//        if (useSpecial)
//            charPool.Append(SpecialChars);

//        var rng = new Random();
//        return new string(Enumerable.Repeat(0, length).Select(_ => charPool[rng.Next(charPool.Length)]).ToArray());
//    }
//}

//public class Program
//{
//    static int selectedOption = 0;
//    static string[] options = { "Generate Password", "Settings", "Exit" };

//    static void Main(string[] args)
//    {
//        Console.WriteLine(@"
        
//         ██▓███   ▄▄▄        ██████   ██████   ▄████ ▓█████  ███▄    █ 
//        ▓██░  ██▒▒████▄    ▒██    ▒ ▒██    ▒  ██▒ ▀█▒▓█   ▀  ██ ▀█   █ 
//        ▓██░ ██▓▒▒██  ▀█▄  ░ ▓██▄   ░ ▓██▄   ▒██░▄▄▄░▒███   ▓██  ▀█ ██▒
//        ▒██▄█▓▒ ▒░██▄▄▄▄██   ▒   ██▒  ▒   ██▒░▓█  ██▓▒▓█  ▄ ▓██▒  ▐▌██▒
//        ▒██▒ ░  ░ ▓█   ▓██▒▒██████▒▒▒██████▒▒░▒▓███▀▒░▒████▒▒██░   ▓██░
//        ▒▓▒░ ░  ░ ▒▒   ▓▒█░▒ ▒▓▒ ▒ ░▒ ▒▓▒ ▒ ░ ░▒   ▒ ░░ ▒░ ░░ ▒░   ▒ ▒ 
//        ░▒ ░       ▒   ▒▒ ░░ ░▒  ░ ░░ ░▒  ░ ░  ░   ░  ░ ░  ░░ ░░   ░ ▒░                                 
//            Generate Your Secure Password using Secure Crypto PRNG
//                       (by Vladyslav Furda for C# course)
//        ");

//        //while (true)
//        //{
//        //    RenderMenu();
//        //    var key = Console.ReadKey(true);

//        //    switch (key.Key)
//        //    {
//        //        case ConsoleKey.UpArrow:
//        //            selectedOption = Math.Max(0, selectedOption - 1);
//        //            break;

//        //        case ConsoleKey.DownArrow:
//        //            selectedOption = Math.Min(options.Length - 1, selectedOption + 1);
//        //            break;

//        //        case ConsoleKey.Enter:
//        //            HandleOption();
//        //            break;

//        //        default:
//        //            // Do nothing on other keys
//        //            break;
//        //    }
//        //}

//        while (true)
//        {
//            //Console.Clear();

//            var options = new[]
//            {
//                "Option 1: Do something",
//                "Option 2: Do something else",
//                "Option 3: Exit"
//            };

//            var index = AnsiConsole.Prompt(
//                new SelectionPrompt<string>()
//                    .Title("Please select an option:")
//                    .PageSize(10)
//                    .AddChoices(options));

//            switch (index)
//            {
//                case "0":
//                    // Do something
//                    AnsiConsole.MarkupLine("[green]You selected Option 1![/]");
//                    break;
//                case "1":
//                    // Do something else
//                    AnsiConsole.MarkupLine("[green]You selected Option 2![/]");
//                    break;
//                case "2":
//                    // Exit
//                    AnsiConsole.MarkupLine("[red]Exiting...[/]");
//                    return;
//                default:
//                    AnsiConsole.MarkupLine("[yellow]Invalid option[/]");
//                    break;
//            }

//            AnsiConsole.MarkupLine("\nPress any key to continue...");
//            Console.ReadKey();
//        }

//    }

//    static void RenderMenu()
//    {
//        // Set cursor position to where dynamic content starts.
//        int startRow = 13;
//        Console.SetCursorPosition(0, startRow);

//        for (int i = 0; i < options.Length; i++)
//        {
//            if (i == selectedOption)
//            {
//                AnsiConsole.Markup($"[green]> {options[i]}[/]");
//            }
//            else
//            {
//                AnsiConsole.Write(options[i]);
//            }

//            // Clear the rest of the line
//            ClearLineFromCursorToEnd();

//            // Move to the next line after each option
//            Console.WriteLine();
//        }

//        // Clear any old content that might still be visible below the current cursor position
//        int currentRow = startRow + options.Length;
//        for (int i = 0; i < 5; i++)
//        {
//            Console.SetCursorPosition(0, currentRow + i);
//            Console.Write(new string(' ', Console.WindowWidth - 5));  // Clearing from column 5
//        }
//    }

//    static void ClearLineFromCursorToEnd()
//    {
//        int spacesNeeded = Console.WindowWidth - Console.CursorLeft;
//        Console.Write(new string(' ', spacesNeeded));
//    }


//    static void HandleOption()
//    {
//        // Clear the screen for a focused view of the selected option's output
//        Console.Clear();

//        // Actions based on selected option
//        switch (selectedOption)
//        {
//            case 0:  // "Generate Password"
//                var length = AnsiConsole.Prompt(
//            new TextPrompt<int>("Enter password length: ")
//                .Validate(value =>
//                {
//                    return value >= 8 ? ValidationResult.Success() : ValidationResult.Error("[red]The password length must be at least 8[/]");
//                }));

//                var useLowercase = AnsiConsole.Confirm("Include lowercase letters?");
//                var useUppercase = AnsiConsole.Confirm("Include uppercase letters?");
//                var useNumbers = AnsiConsole.Confirm("Include numbers?");
//                var useSpecial = AnsiConsole.Confirm("Include special characters?");

//                //PasswordGenerator passwordGenerator = new PasswordGenerator();
//                //var password = "1234124";
//                //var password = GeneratePassword(false, false, true, true, length);
//                var password = PasswordGenerator.GeneratePassword(useLowercase, useUppercase, useNumbers, useSpecial, length);

//                Console.WriteLine();
//                AnsiConsole.MarkupLine($"[bold rapidblink]Generated Password:[/] [bold yellow]{password}[/]");
//                Console.WriteLine();

//                var Clipboard = AnsiConsole.Confirm("Do you want to copy password to clipboard for 10 seconds?");
//                if (Clipboard)
//                {
//                    ClipboardService.SetText(password);
//                    AnsiConsole.Progress()
//                    .Columns(new ProgressColumn[] { new SpinnerColumn(), new ProgressBarColumn(), new PercentageColumn(), new RemainingTimeColumn() })
//                    .Start(ctx =>
//                    {
//                        // Define a new task with an unknown total of items
//                        // In our case, we're just measuring time so we'll use 10 items for 10 seconds
//                        var task = ctx.AddTask("[green]Counting...[/]", new ProgressTaskSettings
//                        {
//                            MaxValue = 10
//                        });

//                        for (int i = 0; i < 10; i++)
//                        {
//                            if (task.IsFinished)
//                            {
//                                break;
//                            }

//                            // Sleep for one second to simulate work
//                            Thread.Sleep(1000);

//                            // Increment the task's current value
//                            task.Increment(1);
//                        }
//                    });

//                    AnsiConsole.MarkupLine($"[bold green]Progress bar finished. Clearing clipboard...[/]");

//                    ClipboardService.SetText(string.Empty); // Empty the clipboard

//                    AnsiConsole.MarkupLine($"[bold green]Clipboard emptied![/]");
//                }

//                break;

//            case 1:  // "Settings"
//                     // Handle settings logic here
//                AnsiConsole.MarkupLine($"[green]Settings are not yet implemented.[/]");
//                break;

//            case 2:  // "Exit"
//                Environment.Exit(0);
//                break;

//            default:
//                break;
//        }

//        // Pause to let the user see the result
//        AnsiConsole.MarkupLine("\n[bold]Press any key to return to the main menu...[/]");
//        Console.ReadKey();

//        // Clear and redraw the main menu
//        Console.Clear();
//        Console.WriteLine(@"
        
//         ██▓███   ▄▄▄        ██████   ██████   ▄████ ▓█████  ███▄    █ 
//        ▓██░  ██▒▒████▄    ▒██    ▒ ▒██    ▒  ██▒ ▀█▒▓█   ▀  ██ ▀█   █ 
//        ▓██░ ██▓▒▒██  ▀█▄  ░ ▓██▄   ░ ▓██▄   ▒██░▄▄▄░▒███   ▓██  ▀█ ██▒
//        ▒██▄█▓▒ ▒░██▄▄▄▄██   ▒   ██▒  ▒   ██▒░▓█  ██▓▒▓█  ▄ ▓██▒  ▐▌██▒
//        ▒██▒ ░  ░ ▓█   ▓██▒▒██████▒▒▒██████▒▒░▒▓███▀▒░▒████▒▒██░   ▓██░
//        ▒▓▒░ ░  ░ ▒▒   ▓▒█░▒ ▒▓▒ ▒ ░▒ ▒▓▒ ▒ ░ ░▒   ▒ ░░ ▒░ ░░ ▒░   ▒ ▒ 
//        ░▒ ░       ▒   ▒▒ ░░ ░▒  ░ ░░ ░▒  ░ ░  ░   ░  ░ ░  ░░ ░░   ░ ▒░                                 
//            Generate Your Secure Password using Secure Crypto PRNG
//                       (by Vladyslav Furda for C# course)
//        ");
//    }


//}
