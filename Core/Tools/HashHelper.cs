namespace Core.Tools
{
    using System;
    using System.Runtime.CompilerServices;
    using System.Security.Cryptography;

    using Core.Enums;

    /// <summary>
    /// Class HashHelper
    /// </summary>
    public class HashHelper
    {
        #region Constants

        /// <summary>
        /// The current hash type
        /// </summary>
        public const HashType CurrentHashType = HashType.HmacSha1S18I1000;

        private const int SaltSize = 18;
        private const int Iterations = 1000;
        private const int HashSize = 32;

        #endregion

        #region Public Methods and Operators

        /// <summary>
        /// Gets the password hash.
        /// </summary>
        /// <param name="password">The password.</param>
        /// <param name="hashType">Type of the hash.</param>
        /// <returns>System.String.</returns>
        /// <exception cref="System.ArgumentNullException">password</exception>
        public static string GetPasswordHash(string password, HashType hashType = CurrentHashType)
        {
            /*switch (hashType)
            {
                default: // nothing to change
                    break;
            }*/

            byte[] salt;
            byte[] buffer2;
            if (string.IsNullOrEmpty(password))
            {
                throw new ArgumentNullException("password");
            }

            using (var bytes = new Rfc2898DeriveBytes(password, SaltSize, Iterations))
            {
                salt = bytes.Salt;
                buffer2 = bytes.GetBytes(HashSize);
            }

            var dst = new byte[SaltSize + HashSize + 1];
            Buffer.BlockCopy(salt, 0, dst, 1, SaltSize);
            Buffer.BlockCopy(buffer2, 0, dst, SaltSize + 1, HashSize);
            return Convert.ToBase64String(dst);
        }

        /// <summary>
        /// Verifies the hashed password.
        /// </summary>
        /// <param name="hashedPassword">The hashed password.</param>
        /// <param name="password">The password.</param>
        /// <param name="hashType">Type of the hash.</param>
        /// <returns><c>true</c> if XXXX, <c>false</c> otherwise</returns>
        /// <exception cref="System.ArgumentNullException">
        /// hashedPassword
        /// or
        /// password
        /// </exception>
        public static bool VerifyHashedPassword(string hashedPassword, string password, HashType hashType = CurrentHashType)
        {
           /*switch (hashType)
            {
                default: // nothing to change
                    break;
            }*/

            byte[] buffer4;
            if (hashedPassword == null)
            {
                throw new ArgumentNullException("hashedPassword");
            }

            if (password == null)
            {
                throw new ArgumentNullException("password");
            }

            byte[] src = Convert.FromBase64String(hashedPassword);
            if ((src.Length != SaltSize + HashSize + 1) || (src[0] != 0))
            {
                return false;
            }

            var dst = new byte[SaltSize];
            Buffer.BlockCopy(src, 1, dst, 0, SaltSize);
            var buffer3 = new byte[HashSize];
            Buffer.BlockCopy(src, SaltSize + 1, buffer3, 0, HashSize);
            using (var bytes = new Rfc2898DeriveBytes(password, dst, Iterations))
            {
                buffer4 = bytes.GetBytes(HashSize);
            }

            return ByteArraysEqual(buffer3, buffer4);
        }

        #endregion

        #region Methods

        [MethodImpl(MethodImplOptions.NoOptimization)]
        private static bool ByteArraysEqual(byte[] a, byte[] b)
        {
            if (ReferenceEquals(a, b))
            {
                return true;
            }

            if (((a == null) || (b == null)) || (a.Length != b.Length))
            {
                return false;
            }

            bool flag = true;
            for (int i = 0; i < a.Length; i++)
            {
                flag &= a[i] == b[i];
            }

            return flag;
        }

        #endregion
    }
}