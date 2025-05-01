using System.Buffers;
namespace GCoptimizedWebAPI.Services
{
  public class MemoryIntensiveService : IMemoryIntensiveService, IDisposable
   {
        private readonly ArrayPool<byte> _arrayPool = ArrayPool<byte>.Shared;
        private bool _disposed;

        public byte[] ProcessLargeData(int size)
        {
            // Rent buffer from pool instead of allocating
            byte[] buffer = _arrayPool.Rent(size);
            
            try
            {
                // Simulate processing
                new Random().NextBytes(buffer.AsSpan(0, size));
                
                // Return copy (since we can't return rented buffer)
                var result = new byte[size];
                buffer.AsSpan(0, size).CopyTo(result);
                return result;
            }
            finally
            {
                _arrayPool.Return(buffer);
            }
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!_disposed)
            {
                if (disposing)
                {
                    // Clean up managed resources
                }
                _disposed = true;
            }
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        ~MemoryIntensiveService() => Dispose(false);
    }

}