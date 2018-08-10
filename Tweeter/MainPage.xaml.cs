using System.Linq;
using Microsoft.UI.Xaml.Controls;

namespace Tweeter
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainPage : Windows.UI.Xaml.Controls.Page
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

				// change the navMain selected item to Feed
				// this is for UX completeness...otherwise the selected bar won't appear in the right place in navMain
				// sauce: https://stackoverflow.com/questions/48361741/windows-10-uwp-navigationview-update-selected-menuitem-on-backnavigation
				var pageName = "Feed";
				//find menu item that has the matching tag
				var menuItem = navMain.MenuItems.OfType<NavigationViewItem>().Where(item => item.Tag.ToString() == pageName).First();
				//select
				navMain.SelectedItem = menuItem;
			}
			else
			{
				// this means the user never logged in
				// load the settings page at launch
				ContentFrame.Navigate(typeof(SettingsPage));
			}
		}

		private void navMain_Invoke(NavigationView sender, NavigationViewItemInvokedEventArgs args)
		{
			if (args.IsSettingsInvoked)
			{
				ContentFrame.Navigate(typeof(SettingsPage));
			}
			else
			{
				// find NavigationViewItem with Content that equals InvokedItem
				var item = sender.MenuItems.OfType<NavigationViewItem>().First(x => (string)x.Content == (string)args.InvokedItem);
				navMain_Navigate(item as NavigationViewItem);
			}
		}
		private void navMain_Navigate(NavigationViewItem item)
		{
			switch (item.Tag)
			{
				case "Feed":
					ContentFrame.Navigate(typeof(FeedPage));
					break;
			}
		}
	}
}
