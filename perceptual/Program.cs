using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace perceptual
{
    class Program
    {
        private static List<PXCMAudioSource.DeviceInfo> devices = new List<PXCMAudioSource.DeviceInfo>();
        static void Main(string[] args)
        {
            PXCMSession session = PXCMSession.CreateInstance();
            Console.WriteLine("SDK Version {0}.{1}", session.QueryVersion().major, session.QueryVersion().minor);
            PXCMSpeechRecognition sr;
            pxcmStatus status = session.CreateImpl<PXCMSpeechRecognition>(out sr);
            Console.WriteLine("STATUS : " + status);
            PXCMSpeechRecognition.ProfileInfo pinfo;
            sr.QueryProfile(0, out pinfo);
            sr.SetProfile(pinfo);

            String[] cmds = new String[3] { "One", "Two", "Three" };
            // Build the grammar.
            sr.BuildGrammarFromStringList(1, cmds, null);
            // Set the active grammar.
            sr.SetGrammar(1);

            //sr.SetDictation();
            PXCMAudioSource source;

            source = session.CreateAudioSource();
            source.ScanDevices();

            for (int i = 0; ; i++)
            {
                PXCMAudioSource.DeviceInfo dinfo;
                if (source.QueryDeviceInfo(i, out dinfo) < pxcmStatus.PXCM_STATUS_NO_ERROR) break;
                devices.Add(dinfo);
                Console.WriteLine("Device : " + dinfo.name);
            }


            source.SetDevice(GetCheckedSource());
  
            PXCMSpeechRecognition.Handler handler = new PXCMSpeechRecognition.Handler();
            handler.onRecognition = OnRecognition;
            // sr is a PXCMSpeechRecognition instance
            status = sr.StartRec(source, handler);
            Console.WriteLine("AFTER start : " + status);
            while (true)
            {

                System.Threading.Thread.Sleep(5);
            }

            session.Dispose();

      
        }

        public static PXCMAudioSource.DeviceInfo GetCheckedSource()
        {
            Console.WriteLine("SELECTED : " + devices[1].name);
            return devices[1];
        }


        static void OnRecognition(PXCMSpeechRecognition.RecognitionData data) {
            Console.WriteLine("RECOGNIZED sentence : " + data.scores[0].sentence);
            Console.WriteLine("RECOGNIZED tags : " + data.scores[0].tags);
        }
    }
}
