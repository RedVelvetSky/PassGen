# Security Toolbox

Security Toolbox is a .NET application that provides a suite of tools for enhancing your digital security. It includes features such as password generation, file encryption/decryption, file hashing, digital signatures, and steganography.

## Features

1. **Password Generation:** Generate secure passwords based on user-defined criteria.
2. **File Encryption (AES):** Encrypt files using the AES algorithm.
3. **File Decryption (AES):** Decrypt AES encrypted files.
4. **File Encryption (AES+RSA):** Encrypt files using both AES and RSA algorithms.
5. **File Decryption (AES+RSA):** Decrypt files encrypted with AES+RSA approach.
6. **File Hashing:** Generate hash values of files using various algorithms (MD5, SHA-1, SHA-2).
7. **Digital Signatures:** Create and verify digital signatures.
8. **Steganography:** Hide data within image and audio files.
9. **Exit:** Quit the application.

## Installation

1. Clone the repository: git clone https://github.com/RedVelvetSky/PassGen.git
2. Change to the project directory: cd PassGen
3. Open the solution file (`SecurityToolbox.sln`) with Visual Studio.
4. Build and run the project.

## Usage

### Password Generation

1. Choose "Generate Password" from the main menu.
2. Define the password length (minimum 8 characters).
3. Choose to include lowercase, uppercase letters, numbers, and special characters.
4. Generate a password and copy it to the clipboard for 10 seconds, if desired.

### File Encryption (AES)

1. Select "Encrypt file (AES)" from the main menu.
2. Provide the path to the file to encrypt.
3. Choose to generate or manually enter an encryption password.
4. Encrypt the file with the ".aes" extension.

### File Decryption (AES)

1. Choose "Decrypt file (AES)" from the main menu.
2. Enter the path to the encrypted file.
3. Provide the decryption password.
4. Decrypt the file with the original extension preceded by ".decrypted".

### File Encryption (AES+RSA)

1. Select "Encrypt file (AES+RSA)" from the main menu.
2. Provide the path to the file to encrypt.
3. Provide the RSA public key to use for encryption.
4. Encrypt the file with the ".rsa_aes" extension.

### File Decryption (AES+RSA)

1. Choose "Decrypt file (AES+RSA)" from the main menu.
2. Enter the path to the encrypted file.
3. Provide the RSA private key to use for decryption.
4. Decrypt the file with the original extension preceded by ".decrypted".

### File Hashing

1. Select "Hashing file" from the main menu.
2. Choose the hashing algorithm (MD5, SHA-1, SHA-2).
3. Enter the path to the file to hash.
4. Compute and display the file hash.

### Digital Signatures

1. Select "Digital Signatures" from the main menu.
2. Choose to create or verify a digital signature.
3. For creating a signature, provide the path to the file and the RSA private key.
4. For verifying a signature, provide the path to the file, the signature, and the RSA public key.

### Steganography

1. Select "Steganography" from the main menu.
2. Choose to hide or retrieve data from an image or audio.
3. For hiding data, provide the path to the image/audio file and text to be hidden.
4. For retrieving data, provide the path to the image/audio file you want to extract text from.

### Exiting the Application

1. Select "Exit" from the main menu.


## Dependencies

- Spectre.Console
- TextCopy

## Contributors

- Space Dimension (crustals@tutanota.com)

## License

This project is licensed under the MIT License - see the [LICENSE](LICENSE) file for details.

## Acknowledgements

- Pavel Jezek's lectures
- Martin Mares lectures
- Roman Oleynikov lectures
- Oksana Melnikova lectures
- Jiri Frantal classes
- https://stackoverflow.com/
- https://chat.openai.com/
- https://www.google.com/

## Contributing

Pull requests are welcome. Please open an issue first for major changes.

Update tests as appropriate.

## Contact

For questions or issues, contact crustals@tutanota.com.


