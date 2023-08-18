using System;
using System.Diagnostics;
using System.Security.Cryptography;
using System.Text;
using Spectre.Console;
using TextCopy;
using Implementation;
using System.Drawing;
using System.Security.Cryptography.X509Certificates;

class Program
{
    static void Main()
    {
        while (true)
        {
            AnsiConsole.Clear();
            ShowMainMenu();
        }
    }

    static void ShowMainMenu()
    {
        Console.ForegroundColor = ConsoleColor.Green;
        Console.WriteLine(@" 
        __________                           ________                 
        \______   \_____     ______  ______ /  _____/   ____    ____  
         |     ___/\__  \   /  ___/ /  ___//   \  ___ _/ __ \  /    \ 
         |    |     / __ \_ \___ \  \___ \ \    \_\  \\  ___/ |   |  \
         |____|    (____  //____  >/____  > \______  / \___  >|___|  /
                        \/      \/      \/         \/      \/      \/ 

                      Security Toolbox. No more, No Less.

       \---------------------------------------------------------------/
        ");

        Console.ResetColor();

        var MainMenuOption = AnsiConsole.Prompt(
            new SelectionPrompt<string>()
            .HighlightStyle(Spectre.Console.Color.Green)
                .PageSize(10)
                .AddChoices(new[]
                {
                    "[[0]] Generate Password",
                    "[[1]] Encrypt file (AES)",
                    "[[2]] Decrypt file (AES)",
                    "[[3]] Encrypt file (AES+RSA)",
                    "[[4]] Decrypt file (AES+RSA)",
                    "[[5]] Hashing file",
                    "[[6]] Digital Signature",
                    "[[7]] Steganography",
                    "[[99]] Exit"
                }));

        switch (MainMenuOption)
        {
            case "[[0]] Generate Password":
                GeneratePasswordMenu();
                break;
            case "[[1]] Encrypt file (AES)":
               EncryptFileAESMenu();
                break;
            case "[[2]] Decrypt file (AES)":
                DecryptFileAESMenu();
                break;
            case "[[3]] Encrypt file (AES+RSA)":
                EncryptFileMenu();
                break;
            case "[[4]] Decrypt file (AES+RSA)":
                DecryptFileMenu();
                break;
            case "[[5]] Hashing file":
                HashFileMenu();
                break;
            case "[[6]] Digital Signature":
                SignatureMenu();
                break;
            case "[[7]] Steganography":
                SteganographyMenu();
                break;
            case "[[99]] Exit":
                AnsiConsole.Clear();
                System.Environment.Exit(0);
                break;
        }
    }

    static void GeneratePasswordMenu()
    {
        //AnsiConsole.Clear();

        AnsiConsole.MarkupLine($"[rapidblink green] You're now in password generating mode [/]\n");

        var length = AnsiConsole.Prompt(
                    new TextPrompt<int>("[bold] 1. Enter password length: [/]")
                        .Validate(value =>
                        {
                            return value >= 8 ? ValidationResult.Success() : ValidationResult.Error("[red]The password length must be at least 8[/]");
                        }));

        Console.WriteLine();

        var useLowercase = AnsiConsole.Confirm("- Include lowercase letters?");
        var useUppercase = AnsiConsole.Confirm("- Include uppercase letters?");
        var useNumbers = AnsiConsole.Confirm("- Include numbers?");
        var useSpecial = AnsiConsole.Confirm("- Include special characters?");

        PasswordGenerator passwordGenerator = new PasswordGenerator();
        EntropyCalculator entropyCalculator = new EntropyCalculator();
        //var password = "1234124";
        //var password = GeneratePassword(false, false, true, true, length);
        var password = PasswordGenerator.GeneratePassword(useLowercase, useUppercase, useNumbers, useSpecial, length);

        Console.WriteLine();
        AnsiConsole.MarkupLine($"[bold]2. Generated Password:[/] [bold yellow]{password}[/]\n");
        // This will display the storage size in bits based on the assumption each character is encoded in 8 bits.
        AnsiConsole.MarkupLine($"- Storage size of generated password is {password.Length * 8} bits");
        double entropyPerCharacter = EntropyCalculator.CalculateStringEntropy(password); 
        AnsiConsole.MarkupLine($"- Entropy of generated password is {entropyPerCharacter * password.Length}");
        Console.WriteLine();

        var Clipboard = AnsiConsole.Confirm("[bold] 3. Do you want to copy password to clipboard for 10 seconds? [/]");
        if (Clipboard)
        {
            ClipboardService.SetText(password);
            AnsiConsole.Progress()
            .Columns(new ProgressColumn[] { new SpinnerColumn(), new ProgressBarColumn(), new PercentageColumn(), new RemainingTimeColumn() })
            .Start(ctx =>
            {

                var task = ctx.AddTask("[green]Counting...[/]", new ProgressTaskSettings
                {
                    MaxValue = 10
                });

                for (int i = 0; i < 10; i++)
                {
                    if (task.IsFinished)
                    {
                        break;
                    }

                    // Sleep for one second to simulate work
                    Thread.Sleep(1000);

                    // Increment the task's current value
                    task.Increment(1);
                }
            });

            AnsiConsole.MarkupLine($"[bold green]Progress bar finished. Clearing clipboard...[/]");

            ClipboardService.SetText(string.Empty); // Empty the clipboard

            AnsiConsole.MarkupLine($"[bold green]Clipboard emptied![/]");
        }

        Console.ReadKey();

        //AnsiConsole.Prompt(
        //    new SelectionPrompt<string>()
        //        .Title("Choose an action:")
        //        .PageSize(10)
        //        .AddChoices(new[]
        //        {
        //            "Return to main menu"
        //        }));
    }

    static void EncryptFileAESMenu()
    {
        //AnsiConsole.Clear();
        Encrypt encrypt = new Encrypt();

        AnsiConsole.MarkupLine($"[rapidblink green] You're now in encryption mode [/]\n");

        var filePath = AnsiConsole.Prompt(
                    new TextPrompt<string>("[bold] 1. Enter the path to the file you want to encrypt: [/]"));

        if (File.Exists(filePath))
        {
            string encryptedFilePath = filePath + ".aes";

            var passMode = AnsiConsole.Prompt(
           new SelectionPrompt<string>()
               .Title("Choose way to enter the password:")
               .PageSize(10)
               .HighlightStyle(Spectre.Console.Color.Green)
               .AddChoices(new[]
               {
                    "[[0]] Generate password",
                    "[[1]] Enter password"
               }));

            string password = "hardcodedtemppassword";

            switch (passMode)
            {
                case "[[0]] Generate password":
                    password = PasswordGenerator.GeneratePassword(true, true, true, true, 50);
                    AnsiConsole.MarkupLine($"[bold] 2. Generated password is: {password} [/]");

                    break;
                case "[[1]] Enter password":
                    password = AnsiConsole.Prompt(
                    new TextPrompt<string>("[bold] 2. Enter the password for encryption: [/]"));
                    break;
            }

            Encrypt.EncryptFile(filePath, encryptedFilePath, password);

            AnsiConsole.MarkupLine($"[bold] 3. File succesfully encrypted and saved to {encryptedFilePath} [/]\n");
        }
        else
        {
            Console.WriteLine();
            AnsiConsole.MarkupLine("[red]File not found![/]\n");
        }


        AnsiConsole.Prompt(
            new SelectionPrompt<string>()
                .Title("Choose an action:")
                .PageSize(10)
                .AddChoices(new[]
                {
                    "Return to main menu"
                }));
    }

    static void DecryptFileAESMenu()
    {
        //AnsiConsole.Clear();
        Decrypt decrypt = new Decrypt();

        AnsiConsole.MarkupLine($"[rapidblink red] You're now in decryption mode [/]\n");

        var filePath = AnsiConsole.Prompt(
                    new TextPrompt<string>("[bold] 1. Enter the path to the encrypted file you want to decrypt: [/]"));

        if (File.Exists(filePath))
        {
            string originalExtension = Path.GetExtension(Path.GetFileNameWithoutExtension(filePath));
            string baseName = Path.GetFileNameWithoutExtension(Path.GetFileNameWithoutExtension(filePath));
            string directoryPath = Path.GetDirectoryName(filePath);
            string decryptedFileName = $"{baseName}.decrypted{originalExtension}";
            string decryptedFilePath = Path.Combine(directoryPath, decryptedFileName);

            var password = AnsiConsole.Prompt(
                new TextPrompt<string>("[bold] 2. Enter decryption password: [/]"));

            Decrypt.DecryptFile(filePath, decryptedFilePath, password);

            AnsiConsole.MarkupLine($"[bold] 3. File succesfully decrypted! [/]\n");
        }
        else
        {
            AnsiConsole.MarkupLine("[red]File not found![/]\n");
        }


        AnsiConsole.Prompt(
            new SelectionPrompt<string>()
                .Title("Choose an action:")
                .PageSize(10)
                .AddChoices(new[]
                {
                    "Return to main menu"
                }));
    }

    static void HashFileMenu()
    {
        //AnsiConsole.Clear();
        Hashing hashing = new Hashing();

        AnsiConsole.MarkupLine($"[rapidblink green] You're now in hashing mode [/]\n");

        var hashMode = AnsiConsole.Prompt(
           new SelectionPrompt<string>()
               .Title("[bold] 1. Choose hashing alrogithm: [/]")
               .HighlightStyle(Spectre.Console.Color.Green)
               .PageSize(10)
               .AddChoices(new[]
               {
                   "[[1]] SHA-1",
                   "[[2]] SHA-2",
                   "[[3]] MD5",
               }));

        AnsiConsole.MarkupLine($"[bold] 1. Choose hashing alrogithm: {hashMode.Substring(5)}[/]");

        var filePath = AnsiConsole.Prompt(
                    new TextPrompt<string>("[bold] 2. Enter the path to the file you want to hash: [/]"));

        switch (hashMode)
        {
            case "[[3]] MD5":
                string md5Hash = Hashing.HashFileWithMD5(filePath);
                AnsiConsole.MarkupLine($"[bold] 3. File hashed succesfully:[/][bold green] {md5Hash} [/]\n");
                break;

            case "[[2]] SHA-2":
                string sha2hash = Hashing.HashingFileWithSHA256(filePath);
                AnsiConsole.MarkupLine($"[bold] 3. File hashed succesfully:[/][bold green] {sha2hash} [/]\n");
                break;

            case "[[1]] SHA-1":
                string sha1Hash = Hashing.HashFileWithSHA1(filePath);
                AnsiConsole.MarkupLine($"[bold] 3. File hashed succesfully:[/][bold green] {sha1Hash} [/]\n");
                break;
        }

        AnsiConsole.Prompt(
            new SelectionPrompt<string>()
                .Title("Choose an action:")
                .PageSize(10)
                .AddChoices(new[]
                {
                    "Return to main menu"
                }));
    }

    static void SteganographyMenu()
    {
        //AnsiConsole.Clear();
        ImageSteganography steganography = new ImageSteganography();
        AudioSteganography audioSteganography = new AudioSteganography();

        AnsiConsole.MarkupLine($"[rapidblink green] You're now in steganography mode [/]\n");

        var steganoMode = AnsiConsole.Prompt(
           new SelectionPrompt<string>()
               .Title("Choose variants:")
               .PageSize(10)
               .HighlightStyle(Spectre.Console.Color.Green)
               .AddChoices(new[]
               {
                    "[[0]] Hiding text in Image (LSB, .png only)",
                    "[[1]] Reading hidden text from Image (LSB, .png only)",
                    "[[2]] Hiding text in audio file (every 16th byte to hide the message, .wav tested)",
                    "[[3]] Extracting text from audio file (every 16th byte to hide the message, .wav tested)",
               }));

        switch (steganoMode)
        {
            case "[[0]] Hiding text in Image (LSB, .png only)":
                //Console.Clear();
                AnsiConsole.MarkupLine($"[rapidblink green] You're now in writing mode [/]\n");
                var filePath = AnsiConsole.Prompt(
                    new TextPrompt<string>("[bold] 1. Enter the path to the file you want to hide text into: [/]"));
                var text = AnsiConsole.Prompt(
                    new TextPrompt<string>("[bold] 2. Enter the text to be hidden: [/]"));

                
                ImageSteganography.embedText(text, filePath);

                //Steganography.HidingTextInImage(filePath, text);
                break;

            case "[[1]] Reading hidden text from Image (LSB, .png only)":
                //Console.Clear();
                AnsiConsole.MarkupLine($"[rapidblink red] You're now in reading mode [/]\n");
                filePath = AnsiConsole.Prompt(
                    new TextPrompt<string>("[bold] 1. Enter the path to the file you want to extract text: [/]"));

                Bitmap imageFrom = new Bitmap(filePath);

                //var hiddenText = Steganography.ExtractingTextInImage(filePath);
                var hiddenText = ImageSteganography.extractText(imageFrom); 

                Console.WriteLine($" 2. Hidden text was succesfully extracted: {hiddenText}\n");
                
                break;

            case "[[2]] Hiding text in audio file (every 16th byte to hide the message, .wav tested)":
                //Console.Clear();
                AnsiConsole.MarkupLine($"[rapidblink green] You're now in writing mode [/]\n");
                filePath = AnsiConsole.Prompt(
                    new TextPrompt<string>("[bold] 1. Enter the path to the file you want to hide text into: [/]"));
                text = AnsiConsole.Prompt(
                    new TextPrompt<string>("[bold] 2. Enter the text to be hidden: [/]"));
                AudioSteganography.HideMessageInAudio(filePath, text);
                break;

            case "[[3]] Extracting text from audio file (every 16th byte to hide the message, .wav tested)":
                //Console.Clear();
                AnsiConsole.MarkupLine($"[rapidblink red] You're now in reading mode [/]\n");

                filePath = AnsiConsole.Prompt(
                    new TextPrompt<string>("[bold] 1. Enter the path to the file you want to extract text: [/]"));
                string extractedMessage = AudioSteganography.ExtractMessageFromAudio(filePath);

                Console.WriteLine($" 2. Hidden text was succesfully extracted: {extractedMessage}\n");
                //throw new NotImplementedException();
                break;
        }

        AnsiConsole.Prompt(
            new SelectionPrompt<string>()
                .Title("Choose an action:")
                .PageSize(10)
                .AddChoices(new[]
                {
                    "Return to main menu"
                }));
    }

    static void SignatureMenu()
    {
        DigitalSignature digitalSignature = new DigitalSignature();

        var signMode = AnsiConsole.Prompt(
           new SelectionPrompt<string>()
               .Title("Choose variants:")
               .PageSize(10)
               .HighlightStyle(Spectre.Console.Color.Green)
               .AddChoices(new[]
               {
                    "[[0]] Signing document",
                    "[[1]] Verifying sign"
               }));

        switch (signMode)
        {
            case "[[0]] Signing document":

                //Console.Clear();
                AnsiConsole.MarkupLine($"[rapidblink green] You're now in signing mode [/]\n");

                var keyGenMode = AnsiConsole.Prompt(
           new SelectionPrompt<string>()
               .Title("Choose variants:")
               .PageSize(10)
               .HighlightStyle(Spectre.Console.Color.Green)
               .AddChoices(new[]
               {
                    "[[0]] Generate RSA keypair",
                    "[[1]] Upload private key"
               }));

                switch (keyGenMode)
                {
                    case "[[0]] Generate RSA keypair":
                        var keypairPath = AnsiConsole.Prompt(
                         new TextPrompt<string>("[bold] 1. Enter the path where you want to save the keypair file: [/]"));
                        DigitalSignature.GenerateKeys(keypairPath);

                        break;

                    case "[[1]] Upload private key":
                        var privateKeyPath = AnsiConsole.Prompt(
                        new TextPrompt<string>("[bold] 1. Enter the path to the privateKey.xml file: [/]"));
                        DigitalSignature.UploadKeys(privateKeyPath);

                        break;
                }


                var filePath = AnsiConsole.Prompt(
                           new TextPrompt<string>("[bold] 2. Enter the path to the file you want to sign: [/]"));

                DigitalSignature.SignDocument(filePath);

                break;

            case "[[1]] Verifying sign":

                AnsiConsole.MarkupLine($"[rapidblink red] You're now in verifying mode [/]\n");

                var docPath = AnsiConsole.Prompt(
                           new TextPrompt<string>("[bold] 1. Enter the path to the document: [/]"));

                var sigPath = AnsiConsole.Prompt(
                           new TextPrompt<string>("[bold] 2. Enter the path to the signature you want to verify: [/]"));

                var keyPath = AnsiConsole.Prompt(
                           new TextPrompt<string>("[bold] 3. Enter the path to your publicKey.xml: [/]"));

                DigitalSignature.VerifyDocumentSignature(docPath, sigPath, keyPath);

                break;
        }

        AnsiConsole.Prompt(
            new SelectionPrompt<string>()
                .Title("Choose an action:")
                .PageSize(10)
                .AddChoices(new[]
                {
                    "Return to main menu"
                }));

    }

    static void EncryptFileMenu()
    {
        AesRsaEncryption aesRsaEncryption = new AesRsaEncryption();
        RSAParameters publicKey;

        AnsiConsole.MarkupLine($"[rapidblink green] You're now in encryption mode [/]\n");

        var keypairPath = AnsiConsole.Prompt(
                          new TextPrompt<string>("[bold] 1. Enter the path where you want to save keypair: [/]"));

        using (RSA rsa = new RSACryptoServiceProvider(2048))
        {
            publicKey = rsa.ExportParameters(false);
            RSAParameters privateKey = rsa.ExportParameters(true);

            var publicKeyFullPath = Path.Combine(keypairPath, "publicKey.xml");
            var privateKeyFullPath = Path.Combine(keypairPath, "privateKey.xml");

            // Save the public key and private key to files (for demonstration purposes)
            File.WriteAllText(publicKeyFullPath, rsa.ToXmlString(false));
            File.WriteAllText(privateKeyFullPath, rsa.ToXmlString(true));
        }

        var filePath = AnsiConsole.Prompt(
                         new TextPrompt<string>("[bold] 2. Enter the path to the file to be encrypted: [/]"));

        string encryptedFilePath = filePath + ".bin";
        string encryptedKeyFilePath = encryptedFilePath + ".key";

        // Step 2: Encrypt the file using the EncryptFile method
        AesRsaEncryption.EncryptFile(filePath, encryptedFilePath, publicKey);

        AnsiConsole.MarkupLine($"[bold green] File succesfully encrypted and saved to: {encryptedFilePath} [/]\n");

        AnsiConsole.Prompt(
                    new SelectionPrompt<string>()
                        .Title("Choose an action:")
                        .PageSize(10)
                        .AddChoices(new[]
                        {
                    "Return to main menu"
                        }));
    }

    static void DecryptFileMenu()
    {
        AesRsaEncryption aesRsaEncryption = new AesRsaEncryption();
        RSAParameters privateKey;

        AnsiConsole.MarkupLine($"[rapidblink red] You're now in decryption mode [/]\n");

        var rsaKeypairPath = AnsiConsole.Prompt(
                         new TextPrompt<string>("[bold] 1. Enter the path to private assymetric key (.xml): [/]"));

        using (RSA rsa = new RSACryptoServiceProvider())
        {
            rsa.FromXmlString(File.ReadAllText(rsaKeypairPath));
            privateKey = rsa.ExportParameters(true);
        }

        var encryptedFilePath = AnsiConsole.Prompt(
                         new TextPrompt<string>("[bold] 2. Enter the path to encrypted file (.bin): [/]"));

        var encryptedKeyFilePath = AnsiConsole.Prompt(
                         new TextPrompt<string>("[bold] 3. Enter the path to encrypted assymetric key file (.key): [/]"));

        string originalExtension = Path.GetExtension(Path.GetFileNameWithoutExtension(encryptedFilePath));
        string baseName = Path.GetFileNameWithoutExtension(Path.GetFileNameWithoutExtension(encryptedFilePath));
        string directoryPath = Path.GetDirectoryName(encryptedFilePath);
        string decryptedFileName = $"{baseName}.decrypted{originalExtension}";
        string decryptedFilePath = Path.Combine(directoryPath, decryptedFileName);

        AesRsaEncryption.DecryptFile(encryptedFilePath, encryptedKeyFilePath, decryptedFilePath, privateKey);

        AnsiConsole.MarkupLine($"[bold green] File succesfully encrypted and saved to: {decryptedFilePath} [/]\n");
    }
}
