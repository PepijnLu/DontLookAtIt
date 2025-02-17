using System;
using System.IO;
using UnityEngine;

public class EyeTrackingReader : MonoBehaviour
{
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

    public string filePath = "C:/path/to/your/eye_tracking_data.json";  // Update with the correct path to your JSON file
    private long lastReadPosition = 0;

    void Start()
    {
        // Initialize last read position
        if (File.Exists(filePath))
        {
            lastReadPosition = new FileInfo(filePath).Length;
        }
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
                    ProcessNewData(newData);
                }
            }

            // Update the last read position to the end of the file
            lastReadPosition = fileInfo.Length;
        }
    }

    // Method to process new content from the JSON
    private void ProcessNewData(string json)
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
                Debug.Log("Timestamp: " + data.timestamp + ", Eye State: " + data.eye_state);
            }
        }
        catch (Exception ex)
        {
            Debug.LogError("JSON Parsing error: " + ex.Message);
        }
    }
}
