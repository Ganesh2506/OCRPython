using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Security.Cryptography;
using System.IO;
using Microsoft.SqlServer.Server;

    public static class GorillaExpenseCryptography
    {
        #region Encryption and Decryption using RijndaelManaged Algorithum (Algorithm that is not FIPS compliant)
        //http://blog.aggregatedintelligence.com/2007/10/fips-validated-cryptographic-algorithms.html
        // This constant is used to determine the keysize of the encryption algorithm in bits.
        // We divide this by 8 within the code below to get the equivalent number of bytes.
        private const int Keysize = 256;

        // This constant determines the number of iterations for the password bytes generation function.
        private const int DerivationIterations = 1000;

        [SqlFunction()]
        public static string Encrypt(string plainText, bool ForceFIPS = false)
        {
            try
            {
                if (ForceFIPS == false)
                {
                    #region EnableNONFIPS==YES
                    string passPhrase = Constants.ENCRYPTION_KEY;
                    // Salt and IV is randomly generated each time, but is preprended to encrypted cipher text
                    // so that the same Salt and IV values can be used when decrypting.  
                    var saltStringBytes = Generate256BitsOfRandomEntropy();
                    var ivStringBytes = Generate256BitsOfRandomEntropy();
                    var plainTextBytes = Encoding.UTF8.GetBytes(plainText);
                    using (var password = new Rfc2898DeriveBytes(passPhrase, saltStringBytes, DerivationIterations))
                    {
                        var keyBytes = password.GetBytes(Keysize / 8);
                        using (var symmetricKey = new RijndaelManaged())
                        {
                            symmetricKey.BlockSize = 256;
                            symmetricKey.Mode = CipherMode.CBC;
                            symmetricKey.Padding = PaddingMode.PKCS7;
                            using (var encryptor = symmetricKey.CreateEncryptor(keyBytes, ivStringBytes))
                            {
                                using (var memoryStream = new MemoryStream())
                                {
                                    using (var cryptoStream = new CryptoStream(memoryStream, encryptor, CryptoStreamMode.Write))
                                    {
                                        cryptoStream.Write(plainTextBytes, 0, plainTextBytes.Length);
                                        cryptoStream.FlushFinalBlock();
                                        // Create the final bytes as a concatenation of the random salt bytes, the random iv bytes and the cipher bytes.
                                        var cipherTextBytes = saltStringBytes;
                                        cipherTextBytes = cipherTextBytes.Concat(ivStringBytes).ToArray();
                                        cipherTextBytes = cipherTextBytes.Concat(memoryStream.ToArray()).ToArray();
                                        memoryStream.Close();
                                        cryptoStream.Close();
                                        return Convert.ToBase64String(cipherTextBytes);
                                    }
                                }
                            }
                        }
                    }
                    #endregion
                }
                else
                {
                    #region Default Encryption FIPS Enabled
                    return plainText.EncryptWithFIPS();
                 
                }
                
            }
            catch (Exception ex)
            {
                throw new Exception(ex.ToString());
            }

        }

       
          private static byte[] Generate256BitsOfRandomEntropy()
        {
            var randomBytes = new byte[32]; // 32 Bytes will give us 256 bits.
            using (var rngCsp = new RNGCryptoServiceProvider())
            {
                // Fill the array with cryptographically secure random bytes.
                rngCsp.GetBytes(randomBytes);
            }
            return randomBytes;
        }

         [SqlFunction()]
          public static string Decrypt(this string cipherText, bool ForceFIPS = false)
          {
              try
              {

                  if (ForceFIPS == false)
                  {
                      #region EnableNONFIPS==YES
                      string passPhrase = Constants.ENCRYPTION_KEY;
                      // Get the complete stream of bytes that represent:
                      // [32 bytes of Salt] + [32 bytes of IV] + [n bytes of CipherText]
                      var cipherTextBytesWithSaltAndIv = Convert.FromBase64String(cipherText);
                      // Get the saltbytes by extracting the first 32 bytes from the supplied cipherText bytes.
                      var saltStringBytes = cipherTextBytesWithSaltAndIv.Take(Keysize / 8).ToArray();
                      // Get the IV bytes by extracting the next 32 bytes from the supplied cipherText bytes.
                      var ivStringBytes = cipherTextBytesWithSaltAndIv.Skip(Keysize / 8).Take(Keysize / 8).ToArray();
                      // Get the actual cipher text bytes by removing the first 64 bytes from the cipherText string.
                      var cipherTextBytes = cipherTextBytesWithSaltAndIv.Skip((Keysize / 8) * 2).Take(cipherTextBytesWithSaltAndIv.Length - ((Keysize / 8) * 2)).ToArray();

                      using (var password = new Rfc2898DeriveBytes(passPhrase, saltStringBytes, DerivationIterations))
                      {
                          var keyBytes = password.GetBytes(Keysize / 8);
                          using (var symmetricKey = new RijndaelManaged())
                          {
                              symmetricKey.BlockSize = 256;
                              symmetricKey.Mode = CipherMode.CBC;
                              symmetricKey.Padding = PaddingMode.PKCS7;
                              using (var decryptor = symmetricKey.CreateDecryptor(keyBytes, ivStringBytes))
                              {
                                  using (var memoryStream = new MemoryStream(cipherTextBytes))
                                  {
                                      using (var cryptoStream = new CryptoStream(memoryStream, decryptor, CryptoStreamMode.Read))
                                      {
                                          var plainTextBytes = new byte[cipherTextBytes.Length];
                                          var decryptedByteCount = cryptoStream.Read(plainTextBytes, 0, plainTextBytes.Length);
                                          memoryStream.Close();
                                          cryptoStream.Close();
                                          return Encoding.UTF8.GetString(plainTextBytes, 0, decryptedByteCount);
                                      }
                                  }
                              }
                          }
                      }
                      #endregion
                  }
                   else
                {
                    #region Default Encryption FIPS Enabled
                    return cipherText.DecryptFIPS();
                    #endregion
                }
                 
              }
              catch (Exception ex)
              {
                  throw new Exception(ex.ToString());
              }
          }
        #endregion

         public static string EncryptWithFIPS(this string plainText)
         {
             try
             {
                 //DESCryptoServiceProvider takes only 8 bytes Key so we take first 8 characters from ENCRYPTION_KEY
                 string passPhrase = Constants.ENCRYPTION_KEY.Substring(0, 8);
                 byte[] bytes = ASCIIEncoding.ASCII.GetBytes(passPhrase);
                 if (String.IsNullOrEmpty(plainText))
                 {
                     throw new ArgumentNullException("The string which needs to be encrypted can not be null.");
                 }
                 DESCryptoServiceProvider cryptoProvider = new DESCryptoServiceProvider();
                 cryptoProvider.Key = bytes;
                 cryptoProvider.IV = bytes;
                 using (MemoryStream memoryStream = new MemoryStream())
                 {
                     ICryptoTransform desencrypt = cryptoProvider.CreateEncryptor();
                     CryptoStream cryptostream = new CryptoStream(memoryStream, desencrypt, CryptoStreamMode.Write);
                     StreamWriter writer = new StreamWriter(cryptostream);
                     writer.Write(plainText);
                     writer.Flush();
                     cryptostream.FlushFinalBlock();
                     writer.Flush();
                     return Convert.ToBase64String(memoryStream.GetBuffer(), 0, (int)memoryStream.Length);
                 }
             }
             catch (Exception ex)
             {
                 throw new Exception(ex.ToString());
             }
         }

         public static string DecryptFIPS(this string cipherText)
         {
             try
             {
                 string passPhrase = Constants.ENCRYPTION_KEY.Substring(0, 8);
                 byte[] bytes = ASCIIEncoding.ASCII.GetBytes(passPhrase);
                 if (String.IsNullOrEmpty(cipherText))
                 {
                     throw new ArgumentNullException("The string which needs to be decrypted can not be null.");
                 }
                 DESCryptoServiceProvider cryptoProvider = new DESCryptoServiceProvider();
                 using (MemoryStream memoryStream = new MemoryStream(Convert.FromBase64String(cipherText)))
                 {
                     CryptoStream cryptoStream = new CryptoStream(memoryStream, cryptoProvider.CreateDecryptor(bytes, bytes), CryptoStreamMode.Read);
                     StreamReader reader = new StreamReader(cryptoStream);
                     return reader.ReadToEnd();
                 }
             }
             catch (Exception ex)
             {
                 throw new Exception(ex.ToString());
             }
         }
        
       

      
    }

     public class Constants
    {
        //public const int ACCOUNT_FOUR_DIGIT = 4;
        //public const int ACCOUNT_PAD_TWELVE_DIGIT = 12;
        public const int ACCOUNT_EIGHT_DIGIT = 8;
        public const int ACCOUNT_PAD_EIGHT_DIGIT =8;
        public const char ACCOUNT_PAD_CHAR = 'X';
        public const int REORDER_NOTIFICATION_PERCENTAGE = 80;
        public const int REORDER_NOTIFICATION_DAYS = 45;
        public const int MAX_GMAP_END_ADDRESSES =5;
        public const string GORILLA_EXPENSE_REPORT_CONFIRM_NO_FORMAT = "GEMMDDYYYYXXXXX";
        public const string GORILLA_PRE_EXPENSE_REPORT_CONFIRM_NO_FORMAT = "PEMMDDYYYYXXXXX";
        public const string ENCRYPTION_KEY = "fyiPINcRDYi8/7sRrMFNIR1XqBYyhtjym4SabKgDycJKBPzfBIRJrYXuhayy40mJ";
        public const int CURRENCYRATE_DECIMAL_PRECISION = 10;
        public static string[] GetReceiptAllowedExtentions()
        {
            string[] allowedExtentions = new[] { ".bmp", ".gif", ".jpeg", ".jpg", ".pdf", ".png" };
            return allowedExtentions;
        }
        public const string MULTIPLE_APPROVAL_TEXT = "Multiple";
    }

        #endregion