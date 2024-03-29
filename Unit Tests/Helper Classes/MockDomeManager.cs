﻿using System;
using System.Threading;
using ASCOM.DeviceHub;
using ASCOM.DeviceInterface;
using ASCOM.DeviceHub.MvvmMessenger;
using ASCOM;

namespace Unit_Tests
{
	public class MockDomeManager : IDomeManager
	{
		public MockDomeManager()
		{
			IsConnected = false;
			RaiseError = false;	
		}

		public string MockDomeID { get; set; }
		public string MockDomeName { get; set; }

		public bool MockIsConnected { get; set; }
		public double MockAltitude { get; set; }
		public double MockAzimuth { get; set; }

		public DomeCapabilities Capabilities { get; set; }
		public DomeParameters Parameters { get; set; }
		public DevHubDomeStatus Status { get; set; }

		public bool RaiseError { get; set; }

		public bool IsScopeReadyToSlave => false;
		public bool IsDomeReadyToSlave
		{
			get
			{
				bool retval = false;

				if ( Status != null && Status.Connected )
				{
					if ( Capabilities != null && ( Capabilities.CanSetAltitude || Capabilities.CanSetAzimuth ) )
					{
						retval = true;
					}
				}

				return retval;
			}
		}

		public double ParkAzimuth => 0.0;
		public double HomeAzimuth = 180.0;

		public string DomeID => MockDomeID;
		public string DomeName => MockDomeName;

		public string ConnectError { get; private set; }
		public Exception ConnectException { get; private set; }

		public bool IsConnected
		{
			get => MockIsConnected;
			private set => MockIsConnected = value;
		}

		public void CloseDomeShutter()
		{
			if ( RaiseError )
			{
				throw new DriverException( "Error raised, as requested." );
			}

			Status = DevHubDomeStatus.GetEmptyStatus();
			Status.ShutterStatus = ShutterState.shutterClosing;
			Status.Slewing = true;

			Messenger.Default.Send( new DomeStatusUpdatedMessage( Status ) );

			Thread.Sleep( 1500 );

			Status.ShutterStatus = ShutterState.shutterClosed;
			Status.Slewing = false;

			Messenger.Default.Send( new DomeStatusUpdatedMessage( Status ) );
		}

		public bool Connect()
		{
			return Connect( MockDomeID, false );
		}
		public bool Connect( string domeID, bool interactiveConnect = true )
		{
			MockDomeID = domeID;
			IsConnected = true;

			return IsConnected;
		}

		public void Disconnect( bool interactiveDisconnect = false )
		{
			IsConnected = false;
		}

		public void FindHomePosition()
		{
			if ( RaiseError )
			{
				throw new DriverException( "Error raised, as requested." );
			}

			Status = DevHubDomeStatus.GetEmptyStatus();
			Status.Connected = true;
			Status.ShutterStatus = ShutterState.shutterOpen;
			Status.Slewing = true;
			Status.Azimuth = MockAzimuth;

			Messenger.Default.Send( new DomeStatusUpdatedMessage( Status ) );

			Thread.Sleep( 1500 );

			Status.Slewing = false;
			Status.Azimuth = HomeAzimuth;
			Status.AtHome = true;

			Messenger.Default.Send( new DomeStatusUpdatedMessage( Status ) );
		}

		public void OpenDomeShutter()
		{
			if ( RaiseError )
			{
				throw new DriverException( "Error raised, as requested." );
			}

			Status = DevHubDomeStatus.GetEmptyStatus();
			Status.ShutterStatus = ShutterState.shutterOpening;
			Status.Slewing = true;

			Messenger.Default.Send( new DomeStatusUpdatedMessage( Status ) );

			Thread.Sleep( 1500 );

			Status.ShutterStatus = ShutterState.shutterOpen;
			Status.Slewing = false;

			Messenger.Default.Send( new DomeStatusUpdatedMessage( Status ) );
		}

		public void StopDomeMotion()
		{
			if ( RaiseError )
			{
				throw new DriverException( "Error raised, as requested." );
			}

			// Really nothing to do here.
		}

		public void ParkTheDome()
		{
			if ( RaiseError )
			{
				throw new DriverException( "Error raised, as requested." );
			}

			Status = DevHubDomeStatus.GetEmptyStatus();
			Status.Connected = true;
			Status.Slewing = true;

			Messenger.Default.Send( new DomeStatusUpdatedMessage( Status ) );

			Thread.Sleep( 1500 );

			Status.ShutterStatus = ShutterState.shutterOpen;
			Status.Slewing = false;
			Status.Azimuth = ParkAzimuth;
			Status.AtPark = true;

			Messenger.Default.Send( new DomeStatusUpdatedMessage( Status ) );
		}

		public void SlewDomeShutter( double targetAltitude )
		{
			if ( RaiseError )
			{
				throw new DriverException( "Error raised, as requested." );
			}

			Status = DevHubDomeStatus.GetEmptyStatus();
			Status.Connected = true;
			Status.ShutterStatus = ShutterState.shutterOpen;
			Status.Altitude = MockAltitude;
			Status.Slewing = true;

			Messenger.Default.Send( new DomeStatusUpdatedMessage( Status ) );

			Thread.Sleep( 1500 );

			Status.ShutterStatus = ShutterState.shutterOpen;
			Status.Slewing = false;
			Status.Altitude = targetAltitude;

			Messenger.Default.Send( new DomeStatusUpdatedMessage( Status ) );
		}

		public void SlewDomeToAzimuth( double targetAzimuth )
		{
			if ( RaiseError )
			{
				throw new DriverException( "Error raised, as requested." );
			}

			Status = DevHubDomeStatus.GetEmptyStatus();
			Status.Connected = true;
			Status.ShutterStatus = ShutterState.shutterOpen;
			Status.Slewing = true;
			Status.Azimuth = MockAzimuth;

			Messenger.Default.Send( new DomeStatusUpdatedMessage( Status ) );

			Thread.Sleep( 1500 );

			Status.Slewing = false;
			Status.Azimuth = targetAzimuth;

			Messenger.Default.Send( new DomeStatusUpdatedMessage( Status ) );
		}

		public void SyncDomeToAzimuth( double azimuth )
		{
			if ( RaiseError )
			{
				throw new DriverException( "Error raised, as requested." );
			}

			Status = DevHubDomeStatus.GetEmptyStatus();
			Status.Connected = true;
			Status.ShutterStatus = ShutterState.shutterOpen;
			Status.Slewing = true;
			Status.Azimuth = MockAzimuth;

			Messenger.Default.Send( new DomeStatusUpdatedMessage( Status ) );

			Thread.Sleep( 1500 );

			Status.Slewing = false;
			Status.Azimuth = azimuth;

			Messenger.Default.Send( new DomeStatusUpdatedMessage( Status ) );
		}

		public void SetSlavedState( bool state )
		{
			if ( state != Globals.IsDomeSlaved )
			{
				Globals.IsDomeSlaved = state;
				Messenger.Default.Send( new DomeSlavedChangedMessage( state ) );
			}
		}

	}
}
