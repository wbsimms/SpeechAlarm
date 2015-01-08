using System;
using System.Collections;
using System.Threading;
using Microsoft.SPOT;
using Microsoft.SPOT.Presentation;
using Microsoft.SPOT.Presentation.Controls;
using Microsoft.SPOT.Presentation.Media;
using Microsoft.SPOT.Presentation.Shapes;
using Microsoft.SPOT.Touch;

using Gadgeteer.Networking;
using GT = Gadgeteer;
using GTM = Gadgeteer.Modules;
using Gadgeteer.Modules.GHIElectronics;
using Gadgeteer.SocketInterfaces;

namespace SpeechAlarm
{
    public partial class Program
    {
        Distance_US3 distance = new Distance_US3(10);
        private Serial serial;
        GT.Timer monitor = new GT.Timer(1000);

        void ProgramStarted()
        {
            Debug.Print("Program Started");
            GT.Socket socket = GT.Socket.GetSocket(11, true, null, null);
            serial = GT.SocketInterfaces.SerialFactory.Create(socket, 9600, SerialParity.None, SerialStopBits.One, 8, HardwareFlowControl.UseIfAvailable, extender);
            Debug.Print(serial.PortName);
            serial.DataReceived +=ser_DataReceived;
            serial.Open();
            button.ButtonPressed += MonitorIntruders;
            monitor.Tick += monitor_Tick;
        }

        void monitor_Tick(GT.Timer timer)
        {
            int currentdistance = distance.GetDistanceInCentimeters();
            Debug.Print("Current distance : " + currentdistance);
            if (currentdistance <= 20)
            {
                serial.WriteLine("SYou are too close at " + currentdistance + " centimeters. Please move back");
            }
        }

        private bool isRunning = false;
        void MonitorIntruders(Button sender, Button.ButtonState state)
        {
            if (!isRunning)
            {
                isRunning = true;
                monitor.Start();
            }
            else
            {
                isRunning = false;
                monitor.Stop();
            }
        }

        void ser_DataReceived(Serial sender)
        {
            Debug.Print(sender.PortName);
        }
    }
}
