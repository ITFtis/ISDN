using NAudio.Lame;
using NAudio.Wave;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Speech.Synthesis;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;

namespace WebSiteProject.Areas.webadmin.Controllers
{
    public class HomeController : AppController
    {
        // GET: Home
        public ActionResult Index(string menutype)
        {
            Session["menutype"] = menutype;
            Session.Timeout = 600;
            return View();
        }

        public async Task<ActionResult> TSS(string text = "1234")
        {
            //string fileName = "fileName";
            //Task<ViewResult> task = Task.Run(() =>
            //{
            //    using (SpeechSynthesizer speechSynthesizer = new SpeechSynthesizer())
            //    {
            //        speechSynthesizer.SetOutputToWaveFile(Server.MapPath("/UploadImage/fileName.wav"));
            //        speechSynthesizer.Speak(text);

            //        ViewBag.FileName = fileName + ".wav";
            //        return View();
            //    }
            //});
            //return await task;
            Task<FileContentResult> task = Task.Run(() =>
            {
                using (SpeechSynthesizer speechSynthesizer = new SpeechSynthesizer())
                {
                    MemoryStream stream = new MemoryStream();
                  
                    speechSynthesizer.SetOutputToWaveStream(stream);
                    speechSynthesizer.Speak("E");
                    speechSynthesizer.Speak("A");
                    speechSynthesizer.Speak("2");
                    speechSynthesizer.Speak("B");
               
                    var bytes = stream.GetBuffer();
                    var mp3bytes = ConvertWavStreamToMp3File(ref stream, Server.MapPath("/UploadImage/fileName.mp3"));
                    return File(mp3bytes, "audio/mpeg");
                }
            });
            return await task;
        }

        private Task SomeJobBy3rdPtyLibrary()
        {
            return Task.Factory.StartNew(() =>
            {
                var voice = new System.Speech.Synthesis.SpeechSynthesizer();
                voice.GetInstalledVoices()
                    .ToList().ForEach((v) =>
                    {
                        Console.WriteLine(
                            v.VoiceInfo.Name + " " +
                            v.VoiceInfo.Culture.DisplayName);
                    });
            });
        }

        private byte[] ConvertWavStreamToMp3File(ref MemoryStream ms, string savetofilename)
        {
            //rewind to beginning of stream
            CheckAddBinPath();
            ms.Seek(0, SeekOrigin.Begin);
            MemoryStream msmp3 = new MemoryStream();
            using (var retMs = new MemoryStream())
            using (var rdr = new WaveFileReader(ms))
            using (var wtr = new LameMP3FileWriter(msmp3, rdr.WaveFormat, LAMEPreset.VBR_90))
            {
                rdr.CopyTo(wtr);
            }
            return msmp3.ToArray();
        }
        public  void CheckAddBinPath()
        {
            // find path to 'bin' folder
            var binPath = Path.Combine(new string[] { AppDomain.CurrentDomain.BaseDirectory, "bin" });
            // get current search path from environment
            var path = Environment.GetEnvironmentVariable("PATH") ?? "";

            // add 'bin' folder to search path if not already present
            if (!path.Split(Path.PathSeparator).Contains(binPath, StringComparer.CurrentCultureIgnoreCase))
            {
                path = string.Join(Path.PathSeparator.ToString(), new string[] { path, binPath });
                Environment.SetEnvironmentVariable("PATH", path);
            }
        }
    }
}