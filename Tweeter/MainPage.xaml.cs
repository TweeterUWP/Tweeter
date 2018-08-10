using System;
using System.Threading.Tasks;
using Windows.Security.Authentication.Web;
using Windows.Security.Cryptography;
using Windows.Security.Cryptography.Core;
using Windows.Storage.Streams;
using Windows.UI.Core;
using Windows.UI.Popups;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.Web.Http;
using Windows.Web.Http.Headers;
using Microsoft.Toolkit.Uwp.Services.Twitter;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Toolkit.Uwp.UI.Controls;
using Windows.UI.Xaml.Media;

namespace Tweeter
{
	/// <summary>
	/// An empty page that can be used on its own or navigated to within a Frame.
	/// </summary>
	public sealed partial class MainPage : Page
	{
		public MainPage()
		{
			this.InitializeComponent();

			// Set an app variable to track logged-in status
			// There's probably a better way to do this.
			(App.Current as App).IsLoggedIn = false;

			// This might be better?
			Utils.Login theLogin = new Utils.Login();
			theLogin.TwitterLogin();

            // if login was successful, IsLoggedIn is true

            if ((App.Current as App).IsLoggedIn == true)
			{
				// this means login was successful
				// load the twitter feed at launch
				ContentFrame.Navigate(typeof(FeedPage));
                LoadMenu();
			}
			else
			{
				// this means the user never logged in
				// load the settings page at launch
				ContentFrame.Navigate(typeof(SettingsPage));
			}
		}

		private void navMain_ItemInvoked(object sender, HamburgerMenuItemInvokedEventArgs e)
		{
            MenuItem item = e.InvokedItem as MenuItem;
            ContentFrame.Navigate(item.PageType);
		}

        private void LoadMenu()
        {
            List <MenuItem> theMenu = new List<MenuItem>();
            MenuItem m = new MenuItem { Glyph = "&#xf099;", Tag = "Feed", Label = "Twitter Feed", PageType=typeof(FeedPage) };

            theMenu.Add(m);
            navMain.ItemsSource = theMenu;
        }
	}

    public class MenuItem : HamburgerMenuGlyphItem
    {
        public Type PageType { get; set; }
    }

    public class FontAwesome : FontIcon
    {
        public FontAwesome()
        {
            this.FontFamily = new FontFamily("./Assets/Fonts/FontAwesome.otf");
        }
    }
}
