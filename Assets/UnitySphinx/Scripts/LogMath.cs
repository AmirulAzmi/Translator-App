using System;
using System.Runtime.InteropServices;

namespace Pocketsphinx {
    public class LogMath {
        readonly internal IntPtr _logmath;

        internal LogMath(IntPtr logmath) {
            _logmath = logmath;
        }

        [DllImport("sphinxbase")]
        private static extern double logmath_exp(IntPtr logmath, Int32 logb);

        public double Exp(Int32 logb) {
            return logmath_exp(_logmath, logb);
        }
    }
}
