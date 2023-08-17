using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;
using System.Drawing.Imaging;
using NAudio.Wave;
using static System.Net.Mime.MediaTypeNames;
using System.Security.Cryptography.X509Certificates;
using Spectre.Console;

namespace Implementation
{
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
            RandomNumberGenerator.Fill(bytes);  // Use the recommended Fill method
            return new string(bytes.Select(b => charPool[b % charPool.Length]).ToArray());
        }
    }

    public class EntropyCalculator
    {
        public static double CalculateStringEntropy(string input)
        {
            if (string.IsNullOrEmpty(input))
                return 0.0;

            // Step 1: Count the occurrence of each character
            Dictionary<char, int> charCounts = new Dictionary<char, int>();
            foreach (char c in input)
            {
                if (charCounts.ContainsKey(c))
                    charCounts[c]++;
                else
                    charCounts[c] = 1;
            }

            // Step 2: Compute the probabilities and then the entropy
            double entropy = 0.0;
            int totalChars = input.Length;
            foreach (var count in charCounts.Values)
            {
                double probability = (double)count / totalChars;
                entropy += probability * Math.Log2(probability);
            }

            // Step 3: Return the negative of the computed sum
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
                    // Write the salt first
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
        //public static void HidingTextInImage(string inputImagePath, string text)
        //{
        //    //string inputImagePath = @"path\to\input\image.png";
        //    //string outputImagePath = @"path\to\output\image.png";

        //    string originalFilePath = inputImagePath;
        //    string directory = Path.GetDirectoryName(originalFilePath);
        //    string originalFileName = Path.GetFileName(originalFilePath);
        //    string newFileName = "hidden_" + originalFileName;
        //    string newFilePath = Path.Combine(directory, newFileName);

        //    Bitmap image = new Bitmap(inputImagePath);
        //    //string text = "Your secret message here";

        //    // Store the length of the message in the first pixel
        //    image.SetPixel(0, 0, Color.FromArgb(text.Length, 0, 0));

        //    int textIndex = 0;
        //    for (int i = 1; i < image.Width; i++)
        //    {
        //        for (int j = 0; j < image.Height; j++)
        //        {
        //            if (textIndex < text.Length)
        //            {
        //                Color pixel = image.GetPixel(i, j);
        //                char c = text[textIndex];
        //                byte r = (byte)((pixel.R & 0xFE) | (c & 1)); // Set the least significant bit
        //                image.SetPixel(i, j, Color.FromArgb(r, pixel.G, pixel.B));
        //                textIndex++;
        //            }
        //        }
        //    }

        //    image.Save(newFilePath);
        //}

        //public static string ExtractingTextInImage(string inputImagePath)
        //{
        //    Bitmap image = new Bitmap(inputImagePath);

        //    // Get the length of the message from the first pixel
        //    int messageLength = image.GetPixel(0, 0).R;

        //    StringBuilder sb = new StringBuilder();
        //    int textIndex = 0;
        //    byte currentChar = 0;
        //    int bitIndex = 0;
        //    for (int i = 1; i < image.Width; i++)
        //    {
        //        for (int j = 0; j < image.Height; j++)
        //        {
        //            if (textIndex < messageLength)
        //            {
        //                Color pixel = image.GetPixel(i, j);
        //                byte r = pixel.R;
        //                currentChar = (byte)(currentChar | ((r & 1) << (7 - bitIndex))); // Shift the bit to the correct position
        //                bitIndex++;
        //                if (bitIndex == 8) // If we have 8 bits
        //                {
        //                    sb.Append((char)currentChar);
        //                    currentChar = 0;
        //                    bitIndex = 0;
        //                    textIndex++;
        //                }
        //            }
        //        }
        //    }

        //    string hiddenText = sb.ToString();
        //    return hiddenText;
        //}

        //public static void HidingTextInImage(string inputImagePath, string text)
        //{
        //    string originalFilePath = inputImagePath;
        //    string directory = Path.GetDirectoryName(originalFilePath);
        //    string originalFileName = Path.GetFileName(originalFilePath);
        //    string newFileName = "hidden_" + originalFileName;
        //    string newFilePath = Path.Combine(directory, newFileName);

        //    Bitmap image = new Bitmap(inputImagePath);

        //    // Store the length of the message in the first pixel
        //    image.SetPixel(0, 0, Color.FromArgb(text.Length, 0, 0));

        //    int textIndex = 0;
        //    int bitIndex = 0;
        //    for (int i = 1; i < image.Width && textIndex < text.Length; i++)
        //    {
        //        for (int j = 0; j < image.Height && textIndex < text.Length; j++)
        //        {
        //            Color pixel = image.GetPixel(i, j);
        //            char c = text[textIndex];
        //            byte r = (byte)((pixel.R & 0xFE) | ((c >> bitIndex) & 1)); // Set the least significant bit
        //            image.SetPixel(i, j, Color.FromArgb(r, pixel.G, pixel.B));
        //            bitIndex++;
        //            if (bitIndex == 8) // If we have 8 bits
        //            {
        //                bitIndex = 0;
        //                textIndex++;
        //            }
        //        }
        //    }

        //    image.Save(newFilePath, ImageFormat.Png); // Save as PNG
        //}

        //public static string ExtractingTextInImage(string inputImagePath)
        //{
        //    Bitmap image = new Bitmap(inputImagePath);

        //    // Get the length of the message from the first pixel
        //    int messageLength = image.GetPixel(0, 0).R;

        //    StringBuilder sb = new StringBuilder();
        //    int textIndex = 0;
        //    byte currentChar = 0;
        //    int bitIndex = 0;
        //    for (int i = 1; i < image.Width && textIndex < messageLength; i++)
        //    {
        //        for (int j = 0; j < image.Height && textIndex < messageLength; j++)
        //        {
        //            Color pixel = image.GetPixel(i, j);
        //            byte r = pixel.R;
        //            currentChar = (byte)(currentChar | ((r & 1) << (7 - bitIndex))); // Shift the bit to the correct position
        //            bitIndex++;
        //            if (bitIndex == 8) // If we have 8 bits
        //            {
        //                sb.Append((char)currentChar);
        //                currentChar = 0;
        //                bitIndex = 0;
        //                textIndex++;
        //            }
        //        }
        //    }

        //    string hiddenText = sb.ToString();
        //    return hiddenText;
        //}

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

            // initially, we'll be hiding characters in the image
            State state = State.Hiding;

            // holds the index of the character that is being hidden
            int charIndex = 0;

            // holds the value of the character converted to integer
            int charValue = 0;

            // holds the index of the color element (R or G or B) that is currently being processed
            long pixelElementIndex = 0;

            // holds the number of trailing zeros that have been added when finishing the process
            int zeros = 0;

            // hold pixel elements
            int R = 0, G = 0, B = 0;

            // pass through the rows
            for (int i = 0; i < bmp.Height; i++)
            {
                // pass through each row
                for (int j = 0; j < bmp.Width; j++)
                {
                    // holds the pixel that is currently being processed
                    System.Drawing.Color pixel = bmp.GetPixel(j, i);

                    // now, clear the least significant bit (LSB) from each pixel element
                    R = pixel.R - pixel.R % 2;
                    G = pixel.G - pixel.G % 2;
                    B = pixel.B - pixel.B % 2;

                    // for each pixel, pass through its elements (RGB)
                    for (int n = 0; n < 3; n++)
                    {
                        // check if new 8 bits has been processed
                        if (pixelElementIndex % 8 == 0)
                        {
                            // check if the whole process has finished
                            // we can say that it's finished when 8 zeros are added
                            if (state == State.Filling_With_Zeros && zeros == 8)
                            {
                                // apply the last pixel on the image
                                // even if only a part of its elements have been affected
                                if ((pixelElementIndex - 1) % 3 < 2)
                                {
                                    bmp.SetPixel(j, i, System.Drawing.Color.FromArgb(R, G, B));
                                }

                                // return the bitmap with the text hidden in
                                bmp.Save(newFilePath, ImageFormat.Png);
                            }

                            // check if all characters has been hidden
                            if (charIndex >= text.Length)
                            {
                                // start adding zeros to mark the end of the text
                                state = State.Filling_With_Zeros;
                            }
                            else
                            {
                                // move to the next character and process again
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
                                        // the rightmost bit in the character will be (charValue % 2)
                                        // to put this value instead of the LSB of the pixel element
                                        // just add it to it
                                        // recall that the LSB of the pixel element had been cleared
                                        // before this operation
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
                            // increment the value of zeros until it is 8
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

            // holds the text that will be extracted from the image
            string extractedText = String.Empty;

            // pass through the rows
            for (int i = 0; i < bmp.Height; i++)
            {
                // pass through each row
                for (int j = 0; j < bmp.Width; j++)
                {
                    System.Drawing.Color pixel = bmp.GetPixel(j, i);

                    // for each pixel, pass through its elements (RGB)
                    for (int n = 0; n < 3; n++)
                    {
                        switch (colorUnitIndex % 3)
                        {
                            case 0:
                                {
                                    // get the LSB from the pixel element (will be pixel.R % 2)
                                    // then add one bit to the right of the current character
                                    // this can be done by (charValue = charValue * 2)
                                    // replace the added bit (which value is by default 0) with
                                    // the LSB of the pixel element, simply by addition
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

                        // if 8 bits has been added,
                        // then add the current character to the result text
                        if (colorUnitIndex % 8 == 0)
                        {
                            // reverse? of course, since each time the process occurs
                            // on the right (for simplicity)
                            charValue = reverseBits(charValue);

                            // can only be 0 if it is the stop character (the 8 zeros)
                            if (charValue == 0)
                            {
                                return extractedText;
                            }

                            // convert the character value from int to char
                            char c = (char)charValue;

                            // add the current character to the result text
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
        //public static void Embed(string inputFilePath, string outputFilePath, string message)
        //{
        //    var waveReader = new WaveFileReader(inputFilePath);
        //    var samples = new float[waveReader.Length / 4];
        //    var waveChannel = new WaveChannel32(waveReader);
        //    waveChannel.Read(samples, 0, (int)(waveReader.Length / 4));
        //    waveReader.Close();

        //    string binaryMessage = StringToBinary(message);
        //    int msgLength = Math.Min(binaryMessage.Length, samples.Length);
        //    for (int i = 0; i < msgLength; i++)
        //    {
        //        float hiddenData = (binaryMessage[i] - '0') * 0.01f;  // Spread the data
        //        samples[i] += hiddenData; // Add the hidden data to the original sample
        //    }

        //    var waveWriter = new WaveFileWriter(outputFilePath, waveChannel.WaveFormat);
        //    waveWriter.WriteSamples(samples, 0, samples.Length);
        //    waveWriter.Close();
        //}

        //public static string Extract(string inputFilePath, int msgLength)
        //{
        //    var waveReader = new WaveFileReader(inputFilePath);
        //    var samples = new float[waveReader.Length / 4];
        //    var waveChannel = new WaveChannel32(waveReader);
        //    waveChannel.Read(samples, 0, (int)(waveReader.Length / 4));
        //    waveReader.Close();

        //    char[] binaryMessage = new char[msgLength];
        //    for (int i = 0; i < msgLength; i++)
        //    {
        //        binaryMessage[i] = (char)('0' + (int)Math.Round(samples[i] * 100)); // Retrieve the data and convert it back to a character
        //    }

        //    return BinaryToString(new string(binaryMessage));
        //}

        //private static string StringToBinary(string message)
        //{
        //    StringBuilder binaryMessage = new StringBuilder();
        //    foreach (char c in message)
        //    {
        //        binaryMessage.Append(Convert.ToString(c, 2).PadLeft(8, '0'));
        //    }
        //    return binaryMessage.ToString();
        //}

        //private static string BinaryToString(string binaryMessage)
        //{
        //    StringBuilder text = new StringBuilder();
        //    for (int i = 0; i < binaryMessage.Length; i += 8)
        //    {
        //        string byteString = binaryMessage.Substring(i, 8);
        //        text.Append((char)Convert.ToInt32(byteString, 2));
        //    }
        //    return text.ToString();
        //}

        //static void Main()
        //{
        //    string inputFilePath = "path\\to\\your\\input.wav";
        //    string outputFilePath = "path\\to\\your\\output.wav";
        //    string message = "Hello, World!";  // Your hidden text message

        //    Embed(inputFilePath, outputFilePath, message);

        //    string extractedMessage = Extract(outputFilePath, message.Length * 8); // Each character is represented by 8 bits
        //    Console.WriteLine("Extracted Message: " + extractedMessage);
        //}

        //const int HEADER_SIZE = 4; // 32 bits to store message length

        //public static void HideMessageInAudio(string inputAudioPath, string message)
        //{
        //    byte[] messageBytes = Encoding.ASCII.GetBytes(message);
        //    byte[] audioBytes = System.IO.File.ReadAllBytes(inputAudioPath);

        //    int audioIndex = 44; // Skipping the header, start at the data part of the WAV file

        //    // Hide message length (32 bits) in first few bytes
        //    for (int i = 0; i < HEADER_SIZE; i++)
        //    {
        //        for (int j = 0; j < 8; j++)
        //        {
        //            byte bit = (byte)((messageBytes.Length >> (i * 8 + j)) & 1);
        //            audioBytes[audioIndex] = (byte)((audioBytes[audioIndex] & 0xFE) | bit);
        //            audioIndex++;
        //        }
        //    }

        //    // Hide message
        //    for (int i = 0; i < messageBytes.Length; i++)
        //    {
        //        for (int j = 0; j < 8; j++)
        //        {
        //            byte bit = (byte)((messageBytes[i] >> j) & 1);
        //            audioBytes[audioIndex] = (byte)((audioBytes[audioIndex] & 0xFE) | bit);
        //            audioIndex++;
        //        }
        //    }

        //    string originalFilePath = inputAudioPath;
        //    string directory = Path.GetDirectoryName(originalFilePath);
        //    string originalFileName = Path.GetFileName(originalFilePath);
        //    string newFileName = "hidden_" + originalFileName;
        //    string newFilePath = Path.Combine(directory, newFileName);

        //    System.IO.File.WriteAllBytes(newFilePath, audioBytes);
        //}

        //public static string ExtractMessageFromAudio(string inputAudioPath)
        //{
        //    byte[] audioBytes = System.IO.File.ReadAllBytes(inputAudioPath);

        //    int audioIndex = 44; // Skipping the header, start at the data part of the WAV file

        //    // Extract message length (32 bits) from first few bytes
        //    int messageLength = 0;
        //    for (int i = 0; i < HEADER_SIZE; i++)
        //    {
        //        for (int j = 0; j < 8; j++)
        //        {
        //            byte bit = (byte)(audioBytes[audioIndex] & 1);
        //            messageLength |= (bit << (i * 8 + j));
        //            audioIndex++;
        //        }
        //    }

        //    byte[] messageBytes = new byte[messageLength];

        //    // Extract message
        //    for (int i = 0; i < messageLength; i++)
        //    {
        //        for (int j = 0; j < 8; j++)
        //        {
        //            byte bit = (byte)(audioBytes[audioIndex] & 1);
        //            messageBytes[i] |= (byte)(bit << j);
        //            audioIndex++;
        //        }
        //    }

        //    return Encoding.ASCII.GetString(messageBytes);
        //}


        const int HEADER_SIZE = 4; // 32 bits to store message length
        const int BYTE_INTERVAL = 16; // Change every 16th byte

        public static void HideMessageInAudio(string inputAudioPath, string message)
        {
            byte[] messageBytes = Encoding.ASCII.GetBytes(message);
            byte[] audioBytes = File.ReadAllBytes(inputAudioPath);

            int audioIndex = 44; // Skipping the header, start at the data part of the WAV file

            // Hide message length (32 bits) in first few bytes
            for (int i = 0; i < HEADER_SIZE; i++)
            {
                for (int j = 0; j < 8; j++)
                {
                    byte bit = (byte)((messageBytes.Length >> (i * 8 + j)) & 1);
                    audioBytes[audioIndex] = (byte)((audioBytes[audioIndex] & 0xFE) | bit);
                    audioIndex += BYTE_INTERVAL;
                }
            }

            // Hide message
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

            int audioIndex = 44; // Skipping the header, start at the data part of the WAV file

            // Extract message length (32 bits) from first few bytes
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

            var publicKeyFullPath = Path.Combine(publicKeyPath, "publicKey.xml");
            File.WriteAllText(publicKeyFullPath, publicKeyXml);

            AnsiConsole.MarkupLine($"[bold green invert] RSA keys generated and public key saved to publicKey.xml [/]\n");
        }

        public static void SignDocument(string documentPath)
        {
            if (privateKey.D == null)
            {
                AnsiConsole.MarkupLine($"[bold red] Please generate RSA keys first. [/]\n");
                return;
            }

            //Console.WriteLine("Enter the path to the document to be signed:");
            //var documentPath = Console.ReadLine();

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

            //Console.WriteLine("Enter the path to the original document:");
            //var documentPath = Console.ReadLine();

            //if (!File.Exists(documentPath))
            //{
            //    Console.WriteLine("File does not exist.");
            //    return;
            //}

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

}