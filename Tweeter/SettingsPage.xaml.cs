using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;
using Microsoft.Toolkit.Uwp.Services.Twitter;
using Windows.UI.Popups;

// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=234238

namespace Tweeter
{
	/// <summary>
	/// An empty page that can be used on its own or navigated to within a Frame.
	/// </summary>
	public sealed partial class SettingsPage : Page
	{
		public SettingsPage()
		{
			this.InitializeComponent();
			if ((App.Current as App).IsLoggedIn == true)
				UpdateControls();
		}

		private void cmdLogin_Click(object sender, RoutedEventArgs e)
		{
			Utils.Login theLogin = new Utils.Login();

			//now that we're logged in we can look at the Twitter daters.
			theLogin.TwitterLogin();
			UpdateControls();
		}

		private void UpdateControls()
		{
			this.txbStatus.Text = "Logged in as: " + (App.Current as App).TheUser.ScreenName;
			this.cmdLogin.Content = "Log out of twitter";
		}
	}
}
