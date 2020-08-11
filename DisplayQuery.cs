﻿using System;
using System.Collections.Generic;
using System.Diagnostics.Eventing;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace StageViewPpt
{
    class DisplayQuery
    {
        [DllImport("user32.dll", CharSet = CharSet.Auto)]
        static extern bool EnumDisplayDevices(string lpDevice, uint iDevNum, ref DISPLAY_DEVICE lpDisplayDevice, uint dwFlags);
        [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Auto)]
        public struct DISPLAY_DEVICE
        {
            [MarshalAs(UnmanagedType.U4)]
            public int cb;
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 32)]
            public string DeviceName;
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 128)]
            public string DeviceString;
            [MarshalAs(UnmanagedType.U4)]
            public DisplayDeviceStateFlags StateFlags;
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 128)]
            public string DeviceID;
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 128)]
            public string DeviceKey;
        }
        [Flags()]
        public enum DisplayDeviceStateFlags : int
        {
            /// <summary>The device is part of the desktop.</summary>
            AttachedToDesktop = 0x1,
            MultiDriver = 0x2,
            /// <summary>The device is part of the desktop.</summary>
            PrimaryDevice = 0x4,
            /// <summary>Represents a pseudo device used to mirror application drawing for remoting or other purposes.</summary>
            MirroringDriver = 0x8,
            /// <summary>The device is VGA compatible.</summary>
            VGACompatible = 0x10,
            /// <summary>The device is removable; it cannot be the primary display.</summary>
            Removable = 0x20,
            /// <summary>The device has more display modes than its output devices support.</summary>
            ModesPruned = 0x8000000,
            Remote = 0x4000000,
            Disconnect = 0x2000000
        }

        static void ShowNames()
        {
            var device = new DISPLAY_DEVICE();
            device.cb = Marshal.SizeOf(device);
            try
            {
                for (uint id = 0; EnumDisplayDevices(null, id, ref device, 0); id++)
                {
                    Console.WriteLine(String.Format("{0}, {1}, {2}, {3}, {4}, {5}", id, device.DeviceName, device.DeviceString, device.StateFlags, device.DeviceID, device.DeviceKey));
                    Console.WriteLine();
                    device.cb = Marshal.SizeOf(device);

                    EnumDisplayDevices(device.DeviceName, 0, ref device, 0);

                    Console.WriteLine(String.Format("{0}, {1}, {2}, {3}, {4}, {5}", id, device.DeviceName, device.DeviceString, device.StateFlags, device.DeviceID, device.DeviceKey));
                    device.cb = Marshal.SizeOf(device);

                    device.cb = Marshal.SizeOf(device);
                    return;
                }

            }
            catch (Exception ex)
            {
                Console.WriteLine(String.Format("{0}", ex.ToString()));
            }
        }

        public static string DeviceFriendlyName(Screen screen)
        {
            var device = new DISPLAY_DEVICE();
            device.cb = Marshal.SizeOf(device);
            for (uint id = 0; EnumDisplayDevices(null, id, ref device, 0); id++)
            {
                if (device.DeviceName == screen.DeviceName)
                {
                    EnumDisplayDevices(device.DeviceName, 0, ref device, 0);
                    return device.DeviceString;
                }
            }

            return screen.DeviceName;
        }
    }
}
