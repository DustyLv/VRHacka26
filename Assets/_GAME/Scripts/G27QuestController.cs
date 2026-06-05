using UnityEngine;
using System;
using System.Threading;
using System.Text;
using TMPro;

public class G27QuestController : MonoBehaviour
{
    public TextMeshProUGUI DebugText;

    public enum G27Button
    {
        Cross = 0, Square = 1, Circle = 2, Triangle = 3,
        RightPaddle = 4, LeftPaddle = 5, WheelRightUp = 6, WheelLeftUp = 7,
        Select = 8, Start = 9, R3 = 10, L3 = 11,
        Gear1 = 12, Gear2 = 13, Gear3 = 14, Gear4 = 15, Gear5 = 16, Gear6 = 17,
        ShifterRightMiddle = 18, ShifterRightDown = 19, ShifterLeftMiddle = 20, ShifterLeftDown = 21,
        ShifterDepressed = 22, ReverseGear = 23
    }

    public enum DPadDirection { Centered, N, NE, E, SE, S, SW, W, NW }

    public bool IsConnected { get; private set; } = false;
    public float Steering { get; private set; } = 0f;    
    public float Throttle { get; private set; } = 0f;    
    public float Brake { get; private set; } = 0f;       
    public float Clutch { get; private set; } = 0f;      
    public DPadDirection DPad { get; private set; } = DPadDirection.Centered;
    public float ShifterX { get; private set; } = 0f;    
    public float ShifterY { get; private set; } = 0f;    

    private const int LOGITECH_VID = 1133;  
    private const int G27_BOOT_PID = 49812; 

    private AndroidJavaObject deviceConnection;
    private AndroidJavaObject usbInterface;
    private AndroidJavaObject usbEndpoint;
    
    private Thread ioThread;
    private bool isRunning = false;
    private byte[] rawBuffer;
    private readonly object lockToken = new object();
    private int buttonBitmask = 0;
    
    private int hardwarePacketSize = 0;
    private int lastBytesRead = 0;
    private string jniErrorMessage = "Awaiting Connection...";

    void Start()
    {
#if UNITY_ANDROID && !UNITY_EDITOR
        InitializeUSBConnection();
#endif
    }

    private void InitializeUSBConnection()
    {
        try
        {
            AndroidJavaObject usbManager;
            using (AndroidJavaClass unityPlayer = new AndroidJavaClass("com.unity3d.player.UnityPlayer"))
            using (AndroidJavaObject currentActivity = unityPlayer.GetStatic<AndroidJavaObject>("currentActivity"))
            {
                usbManager = currentActivity.Call<AndroidJavaObject>("getSystemService", "usb");
            }

            AndroidJavaObject deviceList = usbManager.Call<AndroidJavaObject>("getDeviceList");
            AndroidJavaObject values = deviceList.Call<AndroidJavaObject>("values");
            AndroidJavaObject iterator = values.Call<AndroidJavaObject>("iterator");
            AndroidJavaObject targetDevice = null;

            while (iterator.Call<bool>("hasNext"))
            {
                AndroidJavaObject device = iterator.Call<AndroidJavaObject>("next");
                if (device.Call<int>("getVendorId") == LOGITECH_VID && device.Call<int>("getProductId") == G27_BOOT_PID)
                {
                    targetDevice = device;
                    break;
                }
            }

            if (targetDevice == null) 
            {
                jniErrorMessage = "Device 1133:49812 not found on USB bus.";
                return;
            }

            usbInterface = targetDevice.Call<AndroidJavaObject>("getInterface", 0);
            
            int endpointCount = usbInterface.Call<int>("getEndpointCount");
            for (int i = 0; i < endpointCount; i++)
            {
                AndroidJavaObject ep = usbInterface.Call<AndroidJavaObject>("getEndpoint", i);
                if (ep.Call<int>("getDirection") == 128) 
                {
                    usbEndpoint = ep;
                    break;
                }
            }

            if (usbEndpoint == null) 
            {
                jniErrorMessage = "No IN endpoint (Direction 128) located on interface 0.";
                return;
            }

            hardwarePacketSize = usbEndpoint.Call<int>("getMaxPacketSize");
            rawBuffer = new byte[hardwarePacketSize];

            deviceConnection = usbManager.Call<AndroidJavaObject>("openDevice", targetDevice);
            if (deviceConnection == null) 
            {
                jniErrorMessage = "openDevice returned null. Manifest permission bypass failed.";
                return;
            }

            deviceConnection.Call<bool>("claimInterface", usbInterface, true);
            
            jniErrorMessage = "Hardware pipeline established. Active streaming.";
            IsConnected = true;
            isRunning = true;

            ioThread = new Thread(ReadPipeAsync) { IsBackground = true };
            ioThread.Start();
        }
        catch (Exception ex)
        {
            jniErrorMessage = $"Init Failure: {ex.Message}";
        }
    }

    private void ReadPipeAsync()
    {
        AndroidJNI.AttachCurrentThread();

        IntPtr connectionObject = deviceConnection.GetRawObject();
        IntPtr connectionClass = AndroidJNI.GetObjectClass(connectionObject);
        IntPtr bulkTransferMethodID = AndroidJNI.GetMethodID(connectionClass, "bulkTransfer", "(Landroid/hardware/usb/UsbEndpoint;[BII)I");
        
        IntPtr endpointObject = usbEndpoint.GetRawObject();
        IntPtr jniByteArray = AndroidJNI.NewByteArray(hardwarePacketSize);
        
        jvalue[] methodArgs = new jvalue[4];
        methodArgs[0].l = endpointObject;
        methodArgs[1].l = jniByteArray;
        methodArgs[2].i = hardwarePacketSize; 
        methodArgs[3].i = 50; 

        sbyte[] managedSBytes = new sbyte[hardwarePacketSize];

        while (isRunning)
        {
            int bytesRead = AndroidJNI.CallIntMethod(connectionObject, bulkTransferMethodID, methodArgs);
            
            lock (lockToken)
            {
                lastBytesRead = bytesRead;

                if (bytesRead > 0) 
                {
                    for (int i = 0; i < bytesRead; i++)
                    {
                        managedSBytes[i] = AndroidJNI.GetSByteArrayElement(jniByteArray, i);
                    }
                    Buffer.BlockCopy(managedSBytes, 0, rawBuffer, 0, bytesRead);
                }
            }
            Thread.Sleep(4); 
        }

        AndroidJNI.DeleteLocalRef(jniByteArray);
        AndroidJNI.DeleteLocalRef(connectionClass);
        AndroidJNI.DetachCurrentThread();
    }

    private byte GetByte(int index)
    {
        return index < rawBuffer.Length ? rawBuffer[index] : (byte)0;
    }

    void Update()
    {
        lock (lockToken)
        {
            if (rawBuffer == null) 
            {
                if (DebugText != null) DebugText.text = $"System Status: {jniErrorMessage}";
                return;
            }

            if (DebugText != null)
            {
                StringBuilder sb = new StringBuilder();
                sb.AppendLine($"<b>Max Packet Size:</b> {hardwarePacketSize} Bytes");
                sb.AppendLine($"<b>Last Bytes Read:</b> {lastBytesRead}");
                sb.AppendLine($"<b>Status:</b> {jniErrorMessage}\n");
                
                sb.AppendLine("<b>Live Hex Array:</b>");
                for (int i = 0; i < hardwarePacketSize; i++)
                {
                    sb.AppendLine($"[Byte {i:D2}]  Hex: 0x{rawBuffer[i]:X2}  Dec: {rawBuffer[i]:D3}");
                }
                DebugText.text = sb.ToString();
            }

            if (lastBytesRead > 0)
            {
                // Steering Mapping
                // int rawSteer = (GetByte(4) << 8) | GetByte(3); // old
                // Steering = (rawSteer - 8192f) / 8192f;

                // 10-bit Steering Mapping (Bytes 0 & 1)
                // Mask Byte 1 to extract only the lower 2 bits, shift by 8, and combine with Byte 0
                int rawSteer = GetByte(0) | ((GetByte(1) & 0x03) << 8);
                // Normalize 0-1023 integer range to float: -1.0f (Left) to 1.0f (Right)
                Steering = (rawSteer - 512f) / 512f;
                
                
                // Analog Pedal Mapping
                Throttle = 1.0f - (GetByte(5) / 255f);
                Brake = 1.0f - (GetByte(6) / 255f);
                Clutch = 1.0f - (GetByte(7) / 255f);

                // Shifter Analog Mapping
                ShifterX = (GetByte(8) - 128f) / 128f;
                ShifterY = (GetByte(9) - 128f) / 128f;

                // DPad Digital Mapping
                int dpadRaw = GetByte(0) & 0x0F;
                DPad = dpadRaw <= 8 ? (DPadDirection)dpadRaw : DPadDirection.Centered;

                // Combined Digital Button Bitmask
                int b0 = (GetByte(0) & 0xF0) >> 4; 
                int b1 = GetByte(1);               
                int b2 = GetByte(2);               
                int b3 = (GetByte(3) & 0xC0) >> 6; 
                int b10 = GetByte(10);             

                buttonBitmask = b0 | (b1 << 4) | (b2 << 12) | (b3 << 20) | (b10 << 22);
            }
        }
    }

    public bool IsButtonPressed(G27Button button)
    {
        return (buttonBitmask & (1 << (int)button)) != 0;
    }

    void OnDestroy()
    {
        isRunning = false;
        if (ioThread != null && ioThread.IsAlive) ioThread.Join();

        if (deviceConnection != null)
        {
            deviceConnection.Call<bool>("releaseInterface", usbInterface);
            deviceConnection.Call("close");
        }
    }
}