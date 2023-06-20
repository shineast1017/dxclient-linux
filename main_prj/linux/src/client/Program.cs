using System;
using System.Globalization;
using System.Reflection;
using Gtk;
using log4net;
using Mono.Unix;
using Mono.Unix.Native;
using System.Diagnostics;
using System.IO;

public class DxSignals {
	public static DxSignals dxSignals = new DxSignals ();
	public UnixSignal [] signals = null;
	public DxSignals() {
		this.signals = new UnixSignal [] {
				new UnixSignal(Signum.SIGUSR2),
				};
	}
}

namespace client {
	class MainClass {
		private static readonly ILog _logger = LogManager.GetLogger (MethodBase.GetCurrentMethod ().DeclaringType);

		private static string _strProgramName = string.Empty;

		public static void Main (string [] args)
		{
			ProgramStartOptions (args);

			bool bRunProgram = true;

			string procesIdFilePath = System.Environment.GetFolderPath (System.Environment.SpecialFolder.Personal) + "/daasxpertclient.processid";

			if(_strProgramName != string.Empty) {
				procesIdFilePath = procesIdFilePath + "." + _strProgramName;
			}

			var myProcessFileName = Process.GetCurrentProcess ().MainModule.FileName;
			Console.WriteLine ("Process File Name : " + myProcessFileName);
			Console.WriteLine ("Process StartInfo : " + Process.GetCurrentProcess ().StartInfo.Arguments);

			var myProcessName = Process.GetCurrentProcess ().StartInfo.Arguments;
			var myProcessId = Process.GetCurrentProcess ().Id;
			string readProcessID = "";
			if (File.Exists (procesIdFilePath)) {
				using (var sr = new StreamReader (procesIdFilePath)) {
					readProcessID = sr.ReadToEnd ();
					Console.WriteLine ("Read ProcessID in file : " + readProcessID);
					sr.Close ();
				}
				Process ps = null;

				if (readProcessID != "") {
					try {
						ps = Process.GetProcessById (Convert.ToInt32 (readProcessID)); // 프로세스가 없으면 exception 발생!
						Console.WriteLine ("Process is alive : " + ps.ProcessName );
						if(ps.MainModule.FileName == myProcessFileName) {
							Syscall.kill (Convert.ToInt32 (readProcessID), Signum.SIGUSR2);
							Console.WriteLine ("Send signal(SIGUSR2) to : " + readProcessID);
							bRunProgram = false;
						}

					} catch {
						// 프로세스 없음
						Console.WriteLine ("No Process ID: " + readProcessID);
						using (var sw = new StreamWriter (procesIdFilePath)) {
							sw.WriteLine (myProcessId);
							sw.Close ();
						}
						Console.WriteLine ("Write ProcessId in file: " + myProcessId);
					}
				}

			} else {
				Console.WriteLine ("No Process ID: " + readProcessID);
				using (var sw = new StreamWriter (procesIdFilePath)) {
					sw.WriteLine (myProcessId);
					sw.Close ();
				}
				Console.WriteLine ("Write ProcessId in file: " + myProcessId);
			}

			if (bRunProgram == true) {
				//log4net.Config.XmlConfigurator.Configure ();
				//Gtk.Rc.ParseString("style \"user-font\" {    font_name = \"NanumGothic Regular 10\" } widget_class \"*\" style \"user-font\"");
				Gtk.Rc.ParseString ("style \"user-font\" {    font_name = \"Ubuntu 11\" } widget_class \"*\" style \"user-font\"");

				Application.Init ();
				Gtk.Settings.Default.SetLongProperty ("gtk-button-images", 1, ""); // button image show
				Gtk.Settings.Default.ThemeName = "Ambiance";

				MainWindow win = new MainWindow();
				///win.Show ();
				Application.Run ();
			} 

		}

		public static void ProgramStartOptions (string [] options)
		{
			if (options.Length > 0) {
				string mode = "";
				for (int idx = 0; idx < options.Length; idx++) {
					mode = options [idx];

					switch (mode) {

					case "--logname":
						var temp = options [++idx];
						temp = temp.Trim ();
						_strProgramName = temp;
						break;

					default:
						break;
					}

				}
			}
		}
	}
}
