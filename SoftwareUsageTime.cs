using System.Runtime.InteropServices;
using System;
using System.Collections.Generic;
using System.Timers;
using System.Diagnostics;
using System.Collections.Concurrent;

namespace PicaPico
{
    public class SoftwareUsageTime
    {
        [DllImport("user32.dll")]
        static extern IntPtr GetForegroundWindow();

        [DllImport("user32.dll")]
        static extern uint GetWindowThreadProcessId(IntPtr hWnd, out uint processId);

        [DllImport("kernel32.dll", SetLastError = true)]
        static extern IntPtr OpenProcess(uint dwDesiredAccess, bool bInheritHandle, uint dwProcessId);

        [DllImport("psapi.dll", SetLastError = true)]
        static extern uint GetModuleBaseName(IntPtr hProcess, IntPtr hModule, [Out] char[] lpBaseName, uint nSize);


        private Dictionary<string, int> softwareUsage = new Dictionary<string, int>();

        private Dictionary<IntPtr, string> processNameCache = new Dictionary<IntPtr, string>();
        private readonly object lockObj = new object();
        private Timer timer;

        private int interval = 0;
        public SoftwareUsageTime(int interval = 1)
        {
            this.interval = interval;
        }

        public void Reset()
        {
            processNameCache.Clear();
            softwareUsage.Clear();
        }

        public void Stop()
        {
            timer?.Stop();
            timer?.Dispose();
        }

        public void Start()
        {
            Stop();
            timer = new Timer(TimeSpan.FromSeconds(interval).TotalMilliseconds);
            timer.Elapsed += TrackForegroundWindow;
            timer.Start();
        }

        public Dictionary<string, int> GetSoftwareUsage()
        {
            var usage = new Dictionary<string, int>();
            lock (lockObj)
            {
                foreach (var item in softwareUsage)
                {
                    usage.Add(item.Key, item.Value);
                }
            }
            return usage;
        }

        private void TrackForegroundWindow(object sender, ElapsedEventArgs e)
        {
            IntPtr hwnd = GetForegroundWindow();
            if (hwnd == IntPtr.Zero) return;
            string processName = GetProcessNameByWindowPtr(hwnd);

            if(processName == null) return;

            lock (lockObj)
            {
                if (!softwareUsage.ContainsKey(processName))
                {
                    softwareUsage.Add(processName, 0);
                }
                softwareUsage[processName] += interval;
            }
        }

        const uint PROCESS_QUERY_INFORMATION = 0x0400;
        const uint PROCESS_VM_READ = 0x0010;
        private string GetProcessNameByWindowPtr(IntPtr hwnd)
        {
            lock (lockObj)
            {
                if (processNameCache.ContainsKey(hwnd))
                {
                    return processNameCache[hwnd];
                }
            }
            GetWindowThreadProcessId(hwnd, out uint processId);
            if (processId <= 0)
            {
                return null;
            }
            IntPtr processHandle = OpenProcess(PROCESS_QUERY_INFORMATION | PROCESS_VM_READ, false, processId);
            if (processHandle == IntPtr.Zero) return null;

            char[] processName = new char[1024];
            GetModuleBaseName(processHandle, IntPtr.Zero, processName, (uint)processName.Length);
            var processText = new string(processName).Trim('\0');

            lock (lockObj)
            {
                processNameCache[hwnd] = processText;
            }
            return processText;
        }
    }
}
