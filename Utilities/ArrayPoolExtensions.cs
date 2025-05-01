using System.Buffers;

namespace GCoptimizedWebAPI.Utilities
{
    public static class ArrayPoolExtensions
    {
        public static byte[] RentAndInitialize(this ArrayPool<byte> pool, int size)
        {
            var buffer = pool.Rent(size);
            Array.Clear(buffer, 0, size); // Ensure clean buffer
            return buffer;
        }

        public static void ReturnAndClear(this ArrayPool<byte> pool, byte[] buffer)
        {
            Array.Clear(buffer, 0, buffer.Length);
            pool.Return(buffer);
        }
    }
}