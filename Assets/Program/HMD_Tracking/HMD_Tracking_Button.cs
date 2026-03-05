using System.IO;
using UnityEngine;
using System. Globalization;
using UnityEngine.UI;
using UnityEngine.XR;
using System. Collections.Generic;
using TMPro;

public class HMDTrackingButton : MonoBehaviour
{
    [Header("ログ設定")]
    [Tooltip("保存するCSVファイルの名前")]
    public string fileName = "head_log.csv";

    [Tooltip("ログの記録頻度（Hz）")]
    [Range(1f, 120f)]
    public float sampleRateHz = 60f;

    [Header("フィードバック設定")]
    [Tooltip("状態表示用のテキスト（任意）")]
    public TextMeshPro statusText;

    [Tooltip("振動の強さ (0.0 - 1.0)")]
    [Range(0f, 1f)]
    public float vibrationStrength = 0.5f;

    [Tooltip("振動の長さ（秒）")]
    public float vibrationDuration = 0.2f;

    [Header("状態表示（読み取り専用）")]
    public bool isTracking = false;

    private StreamWriter writer;
    private float sampleInterval;
    private float nextSampleTime = 0f;
    private float textDisplayEndTime = 0f;
    private const float TEXT_DISPLAY_DURATION = 1.0f;

    private bool wasXButtonPressed = false;
    private bool wasYButtonPressed = false;
    private InputDevice leftController;

    void Start()
    {
        sampleInterval = (sampleRateHz > 0f) ?  (1f / sampleRateHz) : 0f;
        Debug.Log("HMDTrackingButton initialized.  FileName: " + fileName + ", SampleRate: " + sampleRateHz + "Hz");
        Debug.Log("Press X button to start tracking, Y button to stop.");

        if (statusText != null)
        {
            statusText. text = "";
        }
    }

    void Update()
    {
        if (!leftController.isValid)
        {
            TryGetLeftController();
        }

        CheckButtonInput();

        if (statusText != null && Time.time > textDisplayEndTime && statusText.text != "")
        {
            statusText. text = "";
        }

        if (! isTracking) return;

        if (sampleRateHz > 0f)
        {
            if (Time.realtimeSinceStartup < nextSampleTime) return;
            nextSampleTime = Time.realtimeSinceStartup + sampleInterval;
        }

        Vector3 p = transform.position;
        Vector3 euler = transform.rotation.eulerAngles;
        euler.x = (euler.x > 180f) ? euler. x - 360f : euler.x;
        euler.y = (euler.y > 180f) ? euler.y - 360f : euler.y;
        euler.z = (euler.z > 180f) ? euler.z - 360f : euler.z;

        string ts = System.DateTime. UtcNow. ToString("o", CultureInfo.InvariantCulture);

        string line = string.Format(
            CultureInfo.InvariantCulture,
            "{0},{1:F6},{2:F6},{3:F6},{4:F6},{5:F3},{6:F3},{7:F3}",
            ts,
            Time.realtimeSinceStartup,
            p.x, p.y, p.z,
            euler.x, euler.y, euler. z
        );

        writer.WriteLine(line);
        if (Time.frameCount % 60 == 0) writer.Flush();
    }

    private void TryGetLeftController()
    {
        var devices = new List<InputDevice>();
        InputDevices.GetDevicesAtXRNode(XRNode. LeftHand, devices);
        if (devices.Count > 0)
        {
            leftController = devices[0];
            Debug.Log("Left controller found.");
        }
    }

    private void CheckButtonInput()
    {
        if (! leftController.isValid) return;

        bool xButtonPressed = false;
        if (leftController.TryGetFeatureValue(CommonUsages. primaryButton, out xButtonPressed))
        {
            if (xButtonPressed && !wasXButtonPressed)
            {
                StartTracking();
            }
            wasXButtonPressed = xButtonPressed;
        }

        bool yButtonPressed = false;
        if (leftController.TryGetFeatureValue(CommonUsages.secondaryButton, out yButtonPressed))
        {
            if (yButtonPressed && !wasYButtonPressed)
            {
                StopTracking();
            }
            wasYButtonPressed = yButtonPressed;
        }
    }

    private void StartTracking()
    {
        if (isTracking)
        {
            Debug. Log("Tracking is already running.");
            return;
        }

        sampleInterval = (sampleRateHz > 0f) ? (1f / sampleRateHz) : 0f;

        string path = Path.Combine(Application.persistentDataPath, fileName);

        writer = new StreamWriter(path, false);
        writer.WriteLine("timestamp_utc,unity_time_s,pos_x,pos_y,pos_z,rot_pitch,rot_yaw,rot_roll");
        writer.Flush();

        isTracking = true;
        nextSampleTime = Time.realtimeSinceStartup;

        VibrateController(vibrationDuration);
        ShowStatusText("Start", Color.red);

        Debug. Log("Tracking STARTED.  Logging to: " + path + " at " + sampleRateHz + "Hz");
    }

    private void StopTracking()
    {
        if (! isTracking)
        {
            Debug.Log("Tracking is not running.");
            return;
        }

        if (writer != null)
        {
            writer.Flush();
            writer. Close();
            writer = null;
        }

        isTracking = false;

        VibrateController(vibrationDuration * 2f);
        ShowStatusText("Finish", Color.green);

        Debug. Log("Tracking STOPPED.");
    }

    private void VibrateController(float duration)
    {
        if (! leftController.isValid) return;

        HapticCapabilities capabilities;
        if (leftController.TryGetHapticCapabilities(out capabilities))
        {
            if (capabilities.supportsImpulse)
            {
                leftController.SendHapticImpulse(0, vibrationStrength, duration);
            }
        }
    }

    private void ShowStatusText(string message, Color color)
    {
        if (statusText != null)
        {
            statusText. text = message;
            statusText.color = color;
            textDisplayEndTime = Time.time + TEXT_DISPLAY_DURATION;
        }
    }

    void OnApplicationQuit()
    {
        StopTracking();
    }

    void OnDestroy()
    {
        StopTracking();
    }
}