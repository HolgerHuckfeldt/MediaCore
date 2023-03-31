using System.Security.Cryptography;

namespace MediaCore.Services
{
    public class Hasher
    {

        // a static Method, that created a Hash from a byte array and return a bytearray.
        public static byte[] CreateHash(byte[] data)
        {
            using SHA256 sha = SHA256.Create();
            return sha.ComputeHash(data);
        }
    }
}
