using Sparebeat.Utilities;
using System.Runtime.InteropServices;

namespace Sparebeat.Core
{
    class SparebeatClient
    {
        private readonly string _userAgent;

        public SparebeatClient()
        {
            _userAgent = $"Sparebeat/{AssemblyUtility.GetVersion()} ({RuntimeInformation.OSDescription})";
        }
    }
}
