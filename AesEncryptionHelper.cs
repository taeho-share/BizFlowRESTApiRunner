using System.Security.Cryptography;
using System.Text;

namespace BizFlowRESTapiRunner
{
    class AesEncryptionHelper
    {
        // Length of the Initialization Vector (IV) for AES-GCM
        private const int GcmIvLength = 12;
        // Length of the authentication tag for AES-GCM
        private const int GcmTagLength = 16;

        /// <summary>
        /// Encrypts the given payload using AES-GCM with the provided API key.
        /// </summary>
        /// <param name="payload">The plaintext payload to encrypt.</param>
        /// <param name="apiKey">The API key used as the encryption key.</param>
        /// <returns>The encrypted payload as a Base64-encoded string.</returns>
        public static string EncryptPayload(string payload, string apiKey)
        {
            // Convert the API key to a byte array
            byte[] keyBytes = Encoding.UTF8.GetBytes(apiKey);
            // Create a byte array to hold the IV
            byte[] iv = new byte[GcmIvLength];

            // Generate a secure random IV
            using (RandomNumberGenerator rng = RandomNumberGenerator.Create())
            {
                rng.GetBytes(iv);
            }

            // Create byte arrays to hold the encrypted data and the authentication tag
            byte[] encryptedData = new byte[payload.Length];
            byte[] tag = new byte[GcmTagLength];

            //Console.WriteLine("keyBytes: {0}", Convert.ToBase64String(keyBytes));
            //Console.WriteLine("iv: {0}", Convert.ToBase64String(iv));
            //Console.WriteLine("encryptedData: {0}", Convert.ToBase64String(encryptedData));
            //Console.WriteLine("tag: {0}", Convert.ToBase64String(tag));

            // Perform AES-GCM encryption
            using (AesGcm aesGcm = new AesGcm(keyBytes))
            {
                aesGcm.Encrypt(iv, Encoding.UTF8.GetBytes(payload), encryptedData, tag);
            }

            // Concatenate IV, ciphertext, and tag into a single byte array
            byte[] encryptedPayload = new byte[iv.Length + encryptedData.Length + tag.Length];
            Buffer.BlockCopy(iv, 0, encryptedPayload, 0, iv.Length);
            Buffer.BlockCopy(encryptedData, 0, encryptedPayload, iv.Length, encryptedData.Length);
            Buffer.BlockCopy(tag, 0, encryptedPayload, iv.Length + encryptedData.Length, tag.Length);

            // Convert the concatenated byte array to a Base64-encoded string and return it
            return Convert.ToBase64String(encryptedPayload);
        }
    }
}
