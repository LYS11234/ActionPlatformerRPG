using UnityEngine;
using UnityEngine.Profiling;

public class MemoryChecker : MonoBehaviour
{
    void Update()
    {
        // 바이트(Byte) 단위로 반환되므로 MB 단위로 변환합니다.
        float allocatedMemory = Profiler.GetTotalAllocatedMemoryLong() / 1048576f;
        float reservedMemory = Profiler.GetTotalReservedMemoryLong() / 1048576f;

        Debug.Log($"현재 사용 중인 메모리: {allocatedMemory:F1} MB");
        Debug.Log($"유니티가 확보한 총 메모리: {reservedMemory:F1} MB");
    }
}
