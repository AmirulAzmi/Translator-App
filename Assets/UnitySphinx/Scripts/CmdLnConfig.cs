using System;
using System.Runtime.InteropServices;

namespace Pocketsphinx {
    public class CmdLineConfig {
        internal readonly IntPtr _cmdln;

        [DllImport("pocketsphinx")]
        private extern static IntPtr ps_args();

        [DllImport("sphinxbase")]
        private static extern IntPtr cmd_ln_init(
            IntPtr inout_cmdln,
            IntPtr defn,
            Int32 strict,
            IntPtr _nl);

        public CmdLineConfig() {
            _cmdln = cmd_ln_init(IntPtr.Zero, ps_args(), 1,
                IntPtr.Zero);

            if (IntPtr.Zero.Equals(_cmdln)) {
                throw new Exception("cmd_ln_init returned NULL");
            }
        }

        [DllImport("sphinxbase")]
        private static extern void cmd_ln_set_str_r(
            IntPtr cmdln,
            [MarshalAs(UnmanagedType.LPStr)] string name,
            [MarshalAs(UnmanagedType.LPStr)] string str);

        /// <summary>
        /// log output file
        /// </summary>
        public string Logfn {
            set {
                cmd_ln_set_str_r(_cmdln, "-logfn", value);
            }
        }

        /// <summary>
        /// folder containing acoustic model files
        /// </summary>
        public string Hmm {
            set {
                cmd_ln_set_str_r(_cmdln, "-hmm", value);
            }
        }

        /// <summary>
        /// trigram language model input file
        /// </summary>
        public string Lm {
            set {
                cmd_ln_set_str_r(_cmdln, "-lm", value);
            }
        }

        /// <summary>
        /// keyphrase spotting input file
        /// </summary>
        public string Kws {
            set {
                cmd_ln_set_str_r(_cmdln, "-kws", value);
            }
        }

        /// <summary>
        /// jsgf grammar input file
        /// </summary>
        public string Jsgf {
            set {
                cmd_ln_set_str_r(_cmdln, "-jsgf", value);
            }
        }

        /// <summary>
        /// pronunciation dictionary input file
        /// </summary>
        public string Dict {
            set {
                cmd_ln_set_str_r(_cmdln, "-dict", value);
            }
        }
    }
}
