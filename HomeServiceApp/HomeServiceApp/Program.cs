// Use this code inside a project created with the Visual C# > Windows Desktop > Console Application template.
// Replace the code in Program.cs with this code.

using System;
using System.Diagnostics;
using System.IO.Ports;
using System.Threading;

public class ArduinoCommunicator
{
    static bool _continue;
    static SerialPort _serialPort;

    public static void Main()
    {
        string message;
        StringComparer stringComparer = StringComparer.OrdinalIgnoreCase;
        Thread readThread = new Thread(Read);

        // Create a new SerialPort object with default settings.
        _serialPort = new SerialPort();
        string _oldSongPlaying = "";

        // Allow the user to set the appropriate properties.
        _serialPort.PortName = SetPortName("COM5");
        _serialPort.BaudRate = SetPortBaudRate(_serialPort.BaudRate);
        _serialPort.Parity = SetPortParity(_serialPort.Parity);
        _serialPort.DataBits = SetPortDataBits(_serialPort.DataBits); 
        _serialPort.StopBits = SetPortStopBits(_serialPort.StopBits);
        _serialPort.Handshake = SetPortHandshake(_serialPort.Handshake);

        // Set the read/write timeouts
        _serialPort.ReadTimeout = 500;
        _serialPort.WriteTimeout = 500;

        _serialPort.Open();
        _continue = true;
        readThread.Start();

        Console.WriteLine("Type QUIT to exit");

        while (_continue)
        {
            message = Console.ReadLine();

            if (stringComparer.Equals("quit", message))
            {
                _continue = false;
            }
            if (stringComparer.Equals("update", message))
            {
                while (true)
                {
                    // Spotify status update
                    Process[] processes = Process.GetProcesses();
                    foreach (Process p in processes)
                    {
                        if (p.ProcessName == "Spotify")
                        {
                            if (!String.IsNullOrEmpty(p.MainWindowTitle))
                            {
                                if (p.MainWindowTitle.ToUpper() != "SPOTIFY PREMIUM") {
                                    if (_oldSongPlaying != p.MainWindowTitle)
                                    {
                                        _serialPort.WriteLine(
                                            String.Format("{0}", p.MainWindowTitle.ToUpper())
                                        );
                                        _oldSongPlaying = p.MainWindowTitle;
                                    }
                                } else {
                                    _serialPort.WriteLine(String.Format(""));
                                }
                            }
                        }
                    }

                    Thread.Sleep(1000);
                }
            }
            else
            {
                _serialPort.WriteLine(
                    String.Format("{0}", message));
            }
        }

        readThread.Join();
        _serialPort.Close();
    }

    public static void Read()
    {
        while (_continue)
        {
            try
            {
                string message = _serialPort.ReadLine();
                Console.WriteLine(message);
            }
            catch (TimeoutException) { }
        }
    }

    // Display Port values and prompt user to enter a port.
    public static string SetPortName(string defaultPortName)
    {
        return defaultPortName;
    }

    // Display BaudRate values and prompt user to enter a value.
    public static int SetPortBaudRate(int defaultPortBaudRate)
    {
        return int.Parse(defaultPortBaudRate.ToString());
    }

    // Display PortParity values and prompt user to enter a value.
    public static Parity SetPortParity(Parity defaultPortParity)
    {
        return (Parity)Enum.Parse(typeof(Parity), defaultPortParity.ToString(), true);
    }

    // Display DataBits values and prompt user to enter a value.
    public static int SetPortDataBits(int defaultPortDataBits)
    {
        return int.Parse(defaultPortDataBits.ToString());
    }

    // Display StopBits values and prompt user to enter a value.
    public static StopBits SetPortStopBits(StopBits defaultPortStopBits)
    {
        string stopBits;

        stopBits = defaultPortStopBits.ToString();

        return (StopBits)Enum.Parse(typeof(StopBits), stopBits, true);
    }
    public static Handshake SetPortHandshake(Handshake defaultPortHandshake)
    {
        string handshake;

        handshake = defaultPortHandshake.ToString();
        
        return (Handshake)Enum.Parse(typeof(Handshake), handshake, true);
    }
}