// ================================================================================================================================
// File:        MemoryMonitor.cs
// Description: Displays amount of memory currently used, and amount of memory that is free on the UI
// Author:      Harley Laurie https://blog.kongregate.com/unity-webgl-memory-and-performance-optimization/
// ================================================================================================================================

using UnityEngine;
using UnityEngine.UI;

public class MemoryMonitor : MonoBehaviour
{
    public Text UIText;

    private void Start()
    {
        int TotalSystemMemory = SystemInfo.systemMemorySize;
        int TotalGraphicsMemory = SystemInfo.graphicsMemorySize;
        string ProcessorType = SystemInfo.processorType;
        int ProcessorCount = SystemInfo.processorCount;
        int ProcessorFrequency = SystemInfo.processorFrequency;

        UIText.text = "Total System Memory: " + TotalSystemMemory + "\n" +
            "Total Graphics Memory: " + TotalGraphicsMemory + "\n" +
            "Processor Type: " + ProcessorType + "\n" +
            "Processor Count: " + ProcessorCount + "\n" +
            "Processor Frequency: " + ProcessorFrequency;
    }
}
