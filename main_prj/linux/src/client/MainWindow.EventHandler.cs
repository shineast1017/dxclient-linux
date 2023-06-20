using System;
using client;

public partial class MainWindow {

	private System.Timers.Timer _timerCheckBrokerServerState = null;
	private const int CHECK_INTERVAL_SERVER_STATE = 300 * 1000;    // 2 sec.


	public void startCheckBrokerServerStateTimer ()
	{
		try {

			if (_timerCheckBrokerServerState == null) {
				_timerCheckBrokerServerState = new System.Timers.Timer ();
				_timerCheckBrokerServerState.Interval = CHECK_INTERVAL_SERVER_STATE;
				_timerCheckBrokerServerState.Elapsed += new System.Timers.ElapsedEventHandler (runCheckBrokerServerState);
			}

			if (_timerCheckBrokerServerState.Enabled)
				_timerCheckBrokerServerState.Stop ();

			_timerCheckBrokerServerState.Start ();


		} catch (Exception ex) {
			_logger.Error (string.Format ("Exception[0x{0:X8}] : {1}", ex.HResult, ex.ToString ()));
		}

	}

	private void runCheckBrokerServerState (object sender, EventArgs e)
	{
		if(MainFunc.requestCheckServerState != null) {
			MainFunc.requestCheckServerState ();
		}
	}




}