using System.Diagnostics;
using System.Text.RegularExpressions;
using System.Text;
using System.Runtime.InteropServices;
using System.Management;

using System;

namespace Service1C
{
    public class Renamer
    {
        [DllImport("user32.dll")]
        static extern IntPtr FindWindow(string lpClassName, string lpWindowName);

        [DllImport("user32.dll")]
        static extern bool ShowWindow(IntPtr hWnd, int nCmdShow);

        [DllImport("user32.dll")]
        public static extern int SetWindowText(IntPtr hWnd, string text);

        [DllImport("user32.dll")]
        public static extern int SetWindowTextW(IntPtr hWnd, string text);

        [DllImport("user32.dll")]
        static extern bool SetForegroundWindow(IntPtr hWnd);

        [DllImport("kernel32.dll", SetLastError = true)]
        static extern IntPtr OpenProcess(uint dwDesiredAccess, bool bInheritHandle, uint dwProcessId);

        [DllImport("kernel32.dll")]
        static extern bool CloseHandle(IntPtr hObject);

        [DllImport("advapi32.dll", SetLastError = true)]
        static extern bool OpenProcessToken(IntPtr ProcessHandle, uint DesiredAccess, out IntPtr TokenHandle);

        [DllImport("advapi32.dll", SetLastError = true)]
        static extern bool AdjustTokenPrivileges(IntPtr TokenHandle, bool DisableAllPrivileges, ref TOKEN_PRIVILEGES NewState, uint Zero, IntPtr Null1, IntPtr Null2);

        [DllImport("advapi32.dll", SetLastError = true, CharSet = CharSet.Auto)]
        private static extern bool LookupPrivilegeValue(string lpSystemName, string lpName, ref LUID lpLuid);

        [DllImport("kernel32.dll")]
        static extern uint GetLastError();

        const uint TOKEN_ADJUST_PRIVILEGES = 0x0020;
        const uint TOKEN_QUERY = 0x0008;
        const int SE_PRIVILEGE_ENABLED = 0x00000002;
        const string SE_DEBUG_NAME = "SeDebugPrivilege";
        const int SW_SHOW = 5;


        [StructLayout(LayoutKind.Sequential)]
        struct TOKEN_PRIVILEGES
        {
            public uint PrivilegeCount;
            public LUID Luid;
            public int Attributes;
        }

        [StructLayout(LayoutKind.Sequential, Pack = 1)]
        private struct LUID
        {
            public uint LowPart;
            public int HighPart;
        }

        //private readonly ILogger<Renamer> _logger;

        public Renamer()
        {
            //_logger = logger;
        }

        public async Task ExecuteAsync(CancellationToken stoppingToken)
        {

            //if (_logger.IsEnabled(LogLevel.Information))
            //{
            //    _logger.LogInformation("Worker running at: {time}", DateTimeOffset.Now);
            //}

            IntPtr hToken;
            TOKEN_PRIVILEGES tp = new TOKEN_PRIVILEGES
            {
                PrivilegeCount = 1,
                Attributes = SE_PRIVILEGE_ENABLED

            };

            LUID luid = new LUID();
            if (!LookupPrivilegeValue(null, SE_DEBUG_NAME, ref luid))
            {
                throw new System.ComponentModel.Win32Exception(Marshal.GetLastWin32Error());
            }

            if (OpenProcessToken(Process.GetCurrentProcess().Handle, TOKEN_ADJUST_PRIVILEGES | TOKEN_QUERY, out hToken))
            {
                TOKEN_PRIVILEGES tkp = new TOKEN_PRIVILEGES();
                tkp.PrivilegeCount = 1;
                tkp.Luid = luid;
                tkp.Attributes = SE_PRIVILEGE_ENABLED;

                if (!AdjustTokenPrivileges(hToken, false, ref tkp, 0, IntPtr.Zero, IntPtr.Zero))
                {
                    throw new System.ComponentModel.Win32Exception(Marshal.GetLastWin32Error());
                }
            }
            else
            {
                throw new System.ComponentModel.Win32Exception(Marshal.GetLastWin32Error());
            }

            while (!stoppingToken.IsCancellationRequested)
            {
                    var processes = Process.GetProcessesByName("1cv8");
                    foreach (var process in processes)
                    {
                        if (process.MainWindowTitle.Contains("Конфигуратор")
                            && !process.MainWindowTitle.Contains("Платформа:"))
                        {
                            string comndLine = GetCommandLine(process);
                            string devider = "/IBName";
                            if (comndLine.Contains(devider))
                            {
                                int ind = comndLine.IndexOf(devider);
                                int ind2 = comndLine.IndexOf(@" /", ind);
                                string baseName = comndLine.Substring(ind + devider.Length, ind2 - ind - devider.Length)
                                    .Replace("\"", "");

                                // get version from filename 
                                var match = Regex.Match(process.MainModule.FileName,
                                    "\\d{1}\\.\\d{1}\\.\\d{2}\\.\\d{4}");
                                string winTitle =
                                    $"{process.MainWindowTitle}. Имя базы: {baseName}  Платформа: {(match.Success ? match.Value : "")}";

                                if (!process.MainWindowTitle.Contains(winTitle))
                                {
                                    IntPtr hwnd = FindWindow(null, process.MainWindowTitle);
                                    if (hwnd != IntPtr.Zero)
                                    {
                                        SetWindowText(hwnd, winTitle);
                                    }
                                }
                            }
                        }
                    }
                   await Task.Delay(3500);
            }
        }

        private string GetCommandLine(Process process)
        {
            using (ManagementObjectSearcher searcher = new ManagementObjectSearcher("SELECT CommandLine FROM Win32_Process WHERE ProcessId = " + process.Id))
            using (ManagementObjectCollection objects = searcher.Get())
            {
                return objects.Cast<ManagementBaseObject>().SingleOrDefault()?["CommandLine"]?.ToString();
            }
        }
    }
}
