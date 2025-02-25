using System;
using System.Diagnostics;
using System.IO;
using UnityEngine;

public class EyeTrackingReader : MonoBehaviour
{
    Process eyeTracking;

    [System.Serializable]
    public class EyeTrackingData
    {
        public float timestamp;
        public string eye_state;
    }

    [System.Serializable]
    public class EyeTrackingDataList
    {
        public EyeTrackingData[] data;
    }

    string filePath = System.IO.Path.Combine(Application.streamingAssetsPath, "EyeTracking", "eye_tracking_data.json");
    private long lastReadPosition = 0;

    void Start()
    {
        // Initialize last read position
        if (File.Exists(filePath))
        {
            lastReadPosition = new FileInfo(filePath).Length;
        }

        string exePath = System.IO.Path.Combine(Application.streamingAssetsPath, "EyeTracking", "EyeTracking.exe");
        // Process.Start(exePath);

        eyeTracking = new Process();
        eyeTracking.StartInfo.FileName = exePath;
        eyeTracking.StartInfo.WorkingDirectory = System.IO.Path.Combine(Application.streamingAssetsPath, "EyeTracking");
        eyeTracking.StartInfo.UseShellExecute = true; // Required when setting WorkingDirectory
        eyeTracking.Start();
    }

    void Update()
    {
        // Only process new data if the file size has changed
        FileInfo fileInfo = new FileInfo(filePath);
        if (fileInfo.Length > lastReadPosition)
        {
            // Open the file and read the new content, allowing shared access for read/write
            using (FileStream fs = new FileStream(filePath, FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
            {
                fs.Seek(lastReadPosition, SeekOrigin.Begin); // Start from the last read position
                using (StreamReader reader = new StreamReader(fs))
                {
                    string newData = reader.ReadToEnd(); // Read new data
                    ReadNewData(newData);
                }
            }

            // Update the last read position to the end of the file
            lastReadPosition = fileInfo.Length;
        }
    }

    // Method to process new content from the JSON
    private void ReadNewData(string json)
    {
        // We will now correctly format the new data to avoid issues with missing commas
        string formattedJson = "{\"data\":[" + json.TrimStart('[', ',').TrimEnd(']') + "]}";

        try
        {
            // Deserialize the new JSON content
            EyeTrackingDataList dataList = JsonUtility.FromJson<EyeTrackingDataList>(formattedJson);

            // Display the new data in the Unity console
            foreach (EyeTrackingData data in dataList.data)
            {
                ProcessData(data);
            }
        }
        catch (Exception ex)
        {
            UnityEngine.Debug.LogError("JSON Parsing error: " + ex.Message);
        }
    }

    private void ProcessData(EyeTrackingData data)
    {
        //UnityEngine.Debug.Log("Timestamp: " + data.timestamp + ", Eye State: " + data.eye_state);

        if(data.eye_state == "open")
        {
            if(!EyeData.areEyesOpen) 
            {
                EyeData.areEyesOpen = true;
                UnityEngine.Debug.Log("Eyes Opened");
            }

        }
        else if (data.eye_state == "closed")
        {
            if(EyeData.areEyesOpen) 
            {
                EyeData.areEyesOpen = false;
                UnityEngine.Debug.Log("Eyes Closed");
            }
        }
        else throw new Exception("Unexpected Data Processed");
    }

    void OnApplicationQuit()
    {
        if (eyeTracking != null && !eyeTracking.HasExited)
        {
            eyeTracking.Kill(); // Force closes the program
            eyeTracking.Dispose(); // Cleanup resources
        }
    }
}
