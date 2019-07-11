using System;
using System.Windows;

namespace ASCOM.DeviceHub
{
	/// <summary>
	/// Interaction logic for App.xaml
	/// </summary>
	public partial class App : Application
	{
		protected override void OnStartup( StartupEventArgs e )
		{
			base.OnStartup( e );

			try
			{
				// Launch the ASCOM Local Server

				Server.Startup( e.Args );
			}
			catch ( Exception xcp )
			{
				string msg = String.Format( "DeviceHub caught a fatal exception during startup\r\n{0}", xcp.Message );
				MessageBox.Show( msg, "DeviceHub", MessageBoxButton.OK, MessageBoxImage.Stop );

				if ( Application.Current != null )
				{
					Application.Current.Shutdown();
				}
			}
		}
    }
}
