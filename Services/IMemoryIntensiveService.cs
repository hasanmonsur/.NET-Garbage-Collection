namespace GCoptimizedWebAPI.Services
{
public interface IMemoryIntensiveService
    {
        byte[] ProcessLargeData(int size);
    }

}