using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace Percheron.API.Utility
{
    public static class AuthState
    {
        private static char[] stateCharacters = new char[64];

        static AuthState()
        {
            for (int i = 0; i < 10; i++)
            {
                stateCharacters[i] = (char)('0' + i);
            }
            for (int i = 0; i < 26; i++)
            {
                stateCharacters[i + 10] = (char)('A' + i);
                stateCharacters[i + 10 + 26] = (char)('a' + i);
            }
            stateCharacters[62] = '-';
            stateCharacters[63] = '_';
        }

        public static string Generate()
        {
            var bytes = new byte[32];
            using (var crypto = new RNGCryptoServiceProvider())
            {
                crypto.GetBytes(bytes);
            }
            var chars = new char[32];
            for (var i = 0; i < bytes.Length; i++)
            {
                chars[i] = stateCharacters[bytes[i] % stateCharacters.Length];
            }
            var state = new string(chars);
            return state;
        }
    }
}
