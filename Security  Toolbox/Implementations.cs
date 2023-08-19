using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Drawing;
using System.Drawing.Imaging;
using Spectre.Console;

namespace Implementation
{
    public class PasswordGenerator
    {
        private static readonly string LowercaseChars = "abcdefghijklmnopqrstuvwxyz";
        private static readonly string UppercaseChars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ";
        private static readonly string NumberChars = "0123456789";
        private static readonly string SpecialChars = "!@#$%^&*()-_=+=|;:,.<>?";

        public static string GeneratePassword(bool useLowercase, bool useUppercase, bool useNumbers, bool useSpecial, int length)
        {
            if (length <= 0 || (!useLowercase && !useUppercase && !useNumbers && !useSpecial))
            {
                throw new ArgumentException("Invalid password settings.");
            }

            string charPool = (useLowercase ? LowercaseChars : "") +
                              (useUppercase ? UppercaseChars : "") +
                              (useNumbers ? NumberChars : "") +
                              (useSpecial ? SpecialChars : "");

            var bytes = new byte[length];
            RandomNumberGenerator.Fill(bytes);
            return new string(bytes.Select(b => charPool[b % charPool.Length]).ToArray());
        }
    }

    public class EntropyCalculator
    {
        public static double CalculateStringEntropy(string input)
        {
            if (string.IsNullOrEmpty(input))
                return 0.0; // entropy is zero

            // counting the occurrence of each char
            Dictionary<char, int> charCounts = new Dictionary<char, int>();
            foreach (char c in input)
            {
                if (charCounts.ContainsKey(c))
                    charCounts[c]++;
                else
                    charCounts[c] = 1;
            }

            // probabilities and then the entropy computing
            double entropy = 0.0;
            int totalChars = input.Length;
            foreach (var count in charCounts.Values)
            {
                double probability = (double)count / totalChars;
                entropy += probability * Math.Log2(probability);
            }

            return -entropy;
        }
    }

    public class Encrypt
    {
        public static void EncryptFile(string inputFile, string outputFile, string password)
        {
            using (Aes aes = Aes.Create())
            {
                byte[] salt = new byte[16]; // 128 bits
                RandomNumberGenerator.Fill(salt);

                int iterations = 100000;
                Rfc2898DeriveBytes pdb = new Rfc2898DeriveBytes(password, salt, iterations, HashAlgorithmName.SHA256);
                aes.Key = pdb.GetBytes(32);
                aes.IV = pdb.GetBytes(16);

                using (FileStream fsOutput = new FileStream(outputFile, FileMode.Create))
                {
                    // writing the salt at the begining
                    fsOutput.Write(salt, 0, salt.Length);

                    using (FileStream fsInput = new FileStream(inputFile, FileMode.Open))
                    using (CryptoStream cs = new CryptoStream(fsOutput, aes.CreateEncryptor(), CryptoStreamMode.Write))
                    {
                        int data;
                        while ((data = fsInput.ReadByte()) != -1)
                        {
                            cs.WriteByte((byte)data);
                        }
                    }
                }
            }
        }
    }

    public class Decrypt
    {
        public static void DecryptFile(string inputFile, string outputFile, string password)
        {
            using (Aes aes = Aes.Create())
            {
                byte[] salt = new byte[16];

                using (FileStream fsInput = new FileStream(inputFile, FileMode.Open))
                {
                    fsInput.Read(salt, 0, salt.Length);

                    int iterations = 100000;
                    Rfc2898DeriveBytes pdb = new Rfc2898DeriveBytes(password, salt, iterations, HashAlgorithmName.SHA256);
                    aes.Key = pdb.GetBytes(32);
                    aes.IV = pdb.GetBytes(16);

                    using (CryptoStream cs = new CryptoStream(fsInput, aes.CreateDecryptor(), CryptoStreamMode.Read))
                    using (FileStream fsOutput = new FileStream(outputFile, FileMode.Create))
                    {
                        int data;
                        while ((data = cs.ReadByte()) != -1)
                        {
                            fsOutput.WriteByte((byte)data);
                        }
                    }

                }
            }
        }
    }

    public class Hashing
    {
        public static string HashingFileWithSHA256(string filePath)
        {
            using SHA256 sha256 = SHA256.Create();
            using FileStream stream = File.OpenRead(filePath);
            return BitConverter.ToString(sha256.ComputeHash(stream)).Replace("-", "").ToLower();
        }
        public static string HashFileWithSHA1(string filePath)
        {
            using SHA1 sha1 = SHA1.Create();
            using FileStream stream = File.OpenRead(filePath);
            return BitConverter.ToString(sha1.ComputeHash(stream)).Replace("-", "").ToLower();
        }

        public static string HashFileWithMD5(string filePath)
        {
            using MD5 md5 = MD5.Create();
            using FileStream stream = File.OpenRead(filePath);
            return BitConverter.ToString(md5.ComputeHash(stream)).Replace("-", "").ToLower();
        }
    }

    public class ImageSteganography
    {

        public enum State
        {
            Hiding,
            Filling_With_Zeros
        };

        public static void embedText(string text, string inputImagePath)
        {

            string originalFilePath = inputImagePath;
            string directory = Path.GetDirectoryName(originalFilePath);
            string originalFileName = Path.GetFileName(originalFilePath);
            string newFileName = "hidden_" + originalFileName;
            string newFilePath = Path.Combine(directory, newFileName);

            Bitmap bmp = new Bitmap(inputImagePath);

            State state = State.Hiding;

            // index of the character that is being hidden
            int charIndex = 0;

            // value of the character converted to integer
            int charValue = 0;

            // index of the color element (R or G or B) that is currently being processed
            long pixelElementIndex = 0;

            // number of trailing zeros that have been added when finishing the process
            int zeros = 0;

            int R = 0, G = 0, B = 0;

            for (int i = 0; i < bmp.Height; i++)
            {
                for (int j = 0; j < bmp.Width; j++)
                { 
                    System.Drawing.Color pixel = bmp.GetPixel(j, i);

                    // clearing the least significant bit (LSB) from each pixel
                    R = pixel.R - pixel.R % 2;
                    G = pixel.G - pixel.G % 2;
                    B = pixel.B - pixel.B % 2;

                    // pass through RGB
                    for (int n = 0; n < 3; n++)
                    {
                        // if 8 bits has been processed
                        if (pixelElementIndex % 8 == 0)
                        { 
                            // finished when 8 zeros are added
                            if (state == State.Filling_With_Zeros && zeros == 8)
                            {
                                // apply the last pixel on the image
                                if ((pixelElementIndex - 1) % 3 < 2)
                                {
                                    bmp.SetPixel(j, i, System.Drawing.Color.FromArgb(R, G, B));
                                }
                                bmp.Save(newFilePath, ImageFormat.Png);
                            }

                            if (charIndex >= text.Length)
                            {
                                // zeros to mark the end of the text
                                state = State.Filling_With_Zeros;
                            }
                            else
                            {
                                charValue = text[charIndex++];
                            }
                        }

                        // check which pixel element has the turn to hide a bit in its LSB
                        switch (pixelElementIndex % 3)
                        {
                            case 0:
                                {
                                    if (state == State.Hiding)
                                    {
                                        R += charValue % 2;

                                        // removes the added rightmost bit of the character
                                        // such that next time we can reach the next one
                                        charValue /= 2;
                                    }
                                }
                                break;
                            case 1:
                                {
                                    if (state == State.Hiding)
                                    {
                                        G += charValue % 2;

                                        charValue /= 2;
                                    }
                                }
                                break;
                            case 2:
                                {
                                    if (state == State.Hiding)
                                    {
                                        B += charValue % 2;

                                        charValue /= 2;
                                    }

                                    bmp.SetPixel(j, i, System.Drawing.Color.FromArgb(R, G, B));
                                }
                                break;
                        }

                        pixelElementIndex++;

                        if (state == State.Filling_With_Zeros)
                        {
                            // incrementштп the value of zeros until it is 8
                            zeros++;
                        }
                    }
                }
            }
            bmp.Save(newFilePath, ImageFormat.Png);
        }

        public static string extractText(Bitmap bmp)
        {
            int colorUnitIndex = 0;
            int charValue = 0;

            // text that will be extracted
            string extractedText = String.Empty;

            for (int i = 0; i < bmp.Height; i++)
            {
                for (int j = 0; j < bmp.Width; j++)
                {
                    System.Drawing.Color pixel = bmp.GetPixel(j, i);

                    for (int n = 0; n < 3; n++)
                    {
                        switch (colorUnitIndex % 3)
                        {
                            case 0:
                                {
                                    // get the LSB from the pixel element (will be pixel.R % 2)
                                    // then add one bit to the right of the current character
                                    charValue = charValue * 2 + pixel.R % 2;
                                }
                                break;
                            case 1:
                                {
                                    charValue = charValue * 2 + pixel.G % 2;
                                }
                                break;
                            case 2:
                                {
                                    charValue = charValue * 2 + pixel.B % 2;
                                }
                                break;
                        }

                        colorUnitIndex++;

                        if (colorUnitIndex % 8 == 0)
                        {
                            charValue = reverseBits(charValue);

                            if (charValue == 0)
                            {
                                return extractedText;
                            }

                            char c = (char)charValue;

                            extractedText += c.ToString();
                        }
                    }
                }
            }
            return extractedText;
        }

        public static int reverseBits(int n)
        {
            int result = 0;

            for (int i = 0; i < 8; i++)
            {
                result = result * 2 + n % 2;

                n /= 2;
            }
            return result;
        }
    }

    class AudioSteganography
    {

        const int HEADER_SIZE = 4; // 32 bits to store message length
        const int BYTE_INTERVAL = 16; // change every 16th byte

        public static void HideMessageInAudio(string inputAudioPath, string message)
        {
            byte[] messageBytes = Encoding.ASCII.GetBytes(message);
            byte[] audioBytes = File.ReadAllBytes(inputAudioPath);

            int audioIndex = 44; // start at the data part of the WAV file while skipping the header (thanks to Jezek's lectures)

            // hiding message length in first few bytes
            for (int i = 0; i < HEADER_SIZE; i++)
            {
                for (int j = 0; j < 8; j++)
                {
                    byte bit = (byte)((messageBytes.Length >> (i * 8 + j)) & 1);
                    audioBytes[audioIndex] = (byte)((audioBytes[audioIndex] & 0xFE) | bit); // masking lsb and writing bit
                    audioIndex += BYTE_INTERVAL;
                }
            }

            // hiding message itself
            for (int i = 0; i < messageBytes.Length; i++)
            {
                for (int j = 0; j < 8; j++)
                {
                    byte bit = (byte)((messageBytes[i] >> j) & 1);
                    audioBytes[audioIndex] = (byte)((audioBytes[audioIndex] & 0xFE) | bit);
                    audioIndex += BYTE_INTERVAL;
                }
            }

            string originalFilePath = inputAudioPath;
            string directory = Path.GetDirectoryName(originalFilePath);
            string originalFileName = Path.GetFileName(originalFilePath);
            string newFileName = "hidden_" + originalFileName;
            string newFilePath = Path.Combine(directory, newFileName);

            File.WriteAllBytes(newFilePath, audioBytes);
        }

        public static string ExtractMessageFromAudio(string inputAudioPath)
        {
            byte[] audioBytes = File.ReadAllBytes(inputAudioPath);

            int audioIndex = 44; // skipping the header again

            // extract message length (32 bits) from first few bytes
            int messageLength = 0;
            for (int i = 0; i < HEADER_SIZE; i++)
            {
                for (int j = 0; j < 8; j++)
                {
                    byte bit = (byte)(audioBytes[audioIndex] & 1);
                    messageLength |= (bit << (i * 8 + j));
                    audioIndex += BYTE_INTERVAL;
                }
            }

            byte[] messageBytes = new byte[messageLength];

            // Extract message
            for (int i = 0; i < messageLength; i++)
            {
                for (int j = 0; j < 8; j++)
                {
                    byte bit = (byte)(audioBytes[audioIndex] & 1);
                    messageBytes[i] |= (byte)(bit << j);
                    audioIndex += BYTE_INTERVAL;
                }
            }

            return Encoding.ASCII.GetString(messageBytes);
        }
    }

    public class DigitalSignature
    {
        static RSACryptoServiceProvider rsa = new RSACryptoServiceProvider(2048);
        static RSAParameters publicKey;
        static RSAParameters privateKey;

        public static void GenerateKeys(string publicKeyPath)
        {
            publicKey = rsa.ExportParameters(false);
            privateKey = rsa.ExportParameters(true);
            var publicKeyXml = rsa.ToXmlString(false);
            var privateKeyXml = rsa.ToXmlString(true);

            var publicKeyFullPath = Path.Combine(publicKeyPath, "publicKey.xml");
            var privateKeyFullPath = Path.Combine(publicKeyPath, "privateKey.xml");
            File.WriteAllText(publicKeyFullPath, publicKeyXml);
            File.WriteAllText(privateKeyFullPath, privateKeyXml);

            AnsiConsole.MarkupLine($"[bold green invert] RSA keys generated and keypair saved to publicKey.xml/privateKey.xml [/]\n");
        }

        public static void UploadKeys(string privateKeyPath)
        {
            var privateKeyXml = File.ReadAllText(privateKeyPath);
            rsa.FromXmlString(privateKeyXml);

            privateKey = rsa.ExportParameters(true);
        }

        public static void SignDocument(string documentPath)
        {
            if (privateKey.D == null)
            {
                AnsiConsole.MarkupLine($"[bold red] Please generate RSA keys first. [/]\n");
                return;
            }

            if (!File.Exists(documentPath))
            {
                AnsiConsole.MarkupLine($"[bold red] File does not exist. [/]\n");
                return;
            }

            var data = File.ReadAllBytes(documentPath);
            var signature = SignData(data, privateKey);
            var signaturePath = documentPath + ".sig";
            File.WriteAllBytes(signaturePath, signature);

            AnsiConsole.MarkupLine($"[bold] 2. Document signed. Signature saved to {signaturePath} [/]\n");
        }

        public static void VerifyDocumentSignature(string documentPath, string signaturePath, string publicKeyPath)
        {

            if (!File.Exists(publicKeyPath))
            {
                Console.WriteLine("Public key file does not exist.");
                return;
            }

            var publicKeyXml = File.ReadAllText(publicKeyPath);
            rsa.FromXmlString(publicKeyXml);

            //var signaturePath = documentPath + ".sig";
            if (!File.Exists(signaturePath))
            {
                Console.WriteLine("Signature file does not exist.");
                return;
            }

            var data = File.ReadAllBytes(documentPath);
            var signature = File.ReadAllBytes(signaturePath);
            var isVerified = VerifyData(data, signature);

            AnsiConsole.MarkupLine($"[bold green] Signature Verified: {isVerified} [/]\n");
        }

        public static byte[] SignData(byte[] data, RSAParameters privateKey)
        {
            rsa.ImportParameters(privateKey);
            var signedData = rsa.SignData(data, HashAlgorithmName.SHA256, RSASignaturePadding.Pkcs1);
            return signedData;
        }

        static bool VerifyData(byte[] data, byte[] signature)
        {
            return rsa.VerifyData(data, signature, HashAlgorithmName.SHA256, RSASignaturePadding.Pkcs1);
        }
    }

    public class AesRsaEncryption
    {

        public static void EncryptFile(string inputFilePath, string outputFilePath, RSAParameters publicKey)
        {
            // generating a random symmetric key
            using (Aes aes = Aes.Create())
            {
                aes.GenerateKey();
                aes.GenerateIV();

                // encrypting the file data with the symmetric key
                using (FileStream inputFileStream = new FileStream(inputFilePath, FileMode.Open, FileAccess.Read))
                using (FileStream outputFileStream = new FileStream(outputFilePath, FileMode.Create, FileAccess.Write))
                using (CryptoStream cryptoStream = new CryptoStream(outputFileStream, aes.CreateEncryptor(), CryptoStreamMode.Write))
                {
                    byte[] buffer = new byte[1024];
                    int bytesRead;
                    while ((bytesRead = inputFileStream.Read(buffer, 0, buffer.Length)) > 0)
                    {
                        cryptoStream.Write(buffer, 0, bytesRead);
                    }
                }

                // encrypting the symmetric key with the recipient's RSA public key
                using (RSA rsa = new RSACryptoServiceProvider())
                {
                    rsa.ImportParameters(publicKey);
                    byte[] encryptedKey = rsa.Encrypt(aes.Key, RSAEncryptionPadding.Pkcs1);

                    // encrypted symmetric key and the IV together
                    File.WriteAllBytes(outputFilePath + ".key", encryptedKey.Concat(aes.IV).ToArray());
                }
            }
        }

        public static void DecryptFile(string inputFilePath, string keyFilePath, string outputFilePath, RSAParameters privateKey)
        {
            byte[] encryptedData = File.ReadAllBytes(keyFilePath);
            byte[] encryptedKey = encryptedData.Take(encryptedData.Length - 16).ToArray();
            byte[] iv = encryptedData.Skip(encryptedData.Length - 16).ToArray();

            // RSA private key to decrypt the symmetric key
            using (RSA rsa = new RSACryptoServiceProvider())
            {
                rsa.ImportParameters(privateKey);
                byte[] decryptedKey = rsa.Decrypt(encryptedKey, RSAEncryptionPadding.Pkcs1);

                // using decrypted symmetric key to decrypt the file data
                using (Aes aes = Aes.Create())
                {
                    aes.Key = decryptedKey;
                    aes.IV = iv;

                    using (FileStream inputFileStream = new FileStream(inputFilePath, FileMode.Open, FileAccess.Read))
                    using (FileStream outputFileStream = new FileStream(outputFilePath, FileMode.Create, FileAccess.Write))
                    using (CryptoStream cryptoStream = new CryptoStream(outputFileStream, aes.CreateDecryptor(), CryptoStreamMode.Write))
                    {
                        byte[] buffer = new byte[1024];
                        int bytesRead;
                        while ((bytesRead = inputFileStream.Read(buffer, 0, buffer.Length)) > 0)
                        {
                            cryptoStream.Write(buffer, 0, bytesRead);
                        }
                    }
                }
            }
        }
    }
    
}