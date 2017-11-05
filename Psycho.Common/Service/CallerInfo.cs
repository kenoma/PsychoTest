using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace Psycho.Common.Service
{
    public class CallerInfo
    {
        public string CallerFilePath { get; private set; }

        public string CallerMemberName { get; private set; }

        public int CallerLineNumber { get; private set; }

        private CallerInfo(string callerFilePath, string callerMemberName, int callerLineNumber)
        {
            this.CallerFilePath = callerFilePath;
            this.CallerMemberName = callerMemberName;
            this.CallerLineNumber = callerLineNumber;
        }

        public static CallerInfo Create(
            [CallerFilePath] string callerFilePath = null,
            [CallerMemberName] string callerMemberName = null,
            [CallerLineNumber] int callerLineNumber = 0)
        {
            return new CallerInfo(callerFilePath, callerMemberName, callerLineNumber);
        }
    }
}
