using Isopoh.Cryptography.Argon2;
using System;
using System.Collections.Generic;
using System.Security.Cryptography;
using System.Text;


namespace MyHeart.Core.Helpers
{
    public static class PasswordHasher
    {

        public static string CreatePasswordHash(string password, string pepper)
        {
            byte[] saltBytes;
            var passwordBytes = Encoding.UTF8.GetBytes(password);
            var PepperBytes = Encoding.UTF8.GetBytes(pepper);

            using (var rng = new RNGCryptoServiceProvider()) 
            {
                rng.GetBytes(saltBytes = new byte[16]);
            }

            var hashConfig = new Argon2Config {
                Type = Argon2Type.DataIndependentAddressing,
                Version = Argon2Version.Nineteen,
                TimeCost = 1,
                MemoryCost = 32768,
                Lanes = 5,
                Threads = Environment.ProcessorCount,
                Password = passwordBytes,
                Salt = saltBytes,
                Secret = PepperBytes,
                AssociatedData = new byte[] { },
                HashLength = 20
            };
            var hash = Argon2.Hash(hashConfig);

            /*using (var pbkdf2 = new Rfc2898DeriveBytes(password, salt, 10000))
            {
                byte[] hash = pbkdf2.GetBytes(20);
                Array.Copy(salt, 0, passwordHashBytes, 0, 16);
                Array.Copy(hash, 0, passwordHashBytes, 16, 20);
            }*/



            return hash;
        }

        public static bool CheckPassword(string password, string passwordHash, string pepper)
        {
            if(Argon2.Verify(passwordHash, password, pepper)) {
                return true;
            }

            return false;
            /*byte[] hashBytes = Convert.FromBase64String(passwordHash);

            byte[] salt = GetSalt(hashBytes);

            using (var pbkdf2 = new Rfc2898DeriveBytes(password, salt, 10000))
            {
                byte[] hash = pbkdf2.GetBytes(20);


                for (int i = 0; i < 20; i++)
                    if (hashBytes[i + 16] != hash[i])
                        return false;
            }

            return true;*/
        }

        public static byte[] GetSalt(byte[] hashBytes)
        { 
            byte[] salt = new byte[16];
            Array.Copy(hashBytes, 0, salt, 0, 16);

            return salt;
        }
    }
}