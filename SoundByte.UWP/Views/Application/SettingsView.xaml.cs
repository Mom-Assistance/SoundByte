//*********************************************************
// Copyright (c) Dominic Maas. All rights reserved.
// This code is licensed under the MIT License (MIT).
// THIS CODE IS PROVIDED *AS IS* WITHOUT WARRANTY OF
// ANY KIND, EITHER EXPRESS OR IMPLIED, INCLUDING ANY
// IMPLIED WARRANTIES OF FITNESS FOR A PARTICULAR
// PURPOSE, MERCHANTABILITY, OR NON-INFRINGEMENT.
//*********************************************************

using System;
using System.IO;
using System.Threading.Tasks;
using Windows.ApplicationModel;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Navigation;
using Microsoft.Services.Store.Engagement;
using Newtonsoft.Json;
using SoundByte.Core.API.Endpoints;
using SoundByte.UWP.Dialogs;
using SoundByte.UWP.Helpers;
using SoundByte.UWP.Services;
using SoundByte.UWP.Views.General;
using SoundByte.UWP.ViewModels;

namespace SoundByte.UWP.Views.Application
{
    /// <summary>
    /// This is the main settings/about page for the app.
    /// is handled here
    /// </summary>
    public sealed partial class SettingsView
    {
        // The settings object, we bind to this to change values
        public SettingsService SettingsService { get; set; } = SettingsService.Current;

        // View model for the settings page
        public ViewModels.SettingsViewModel ViewModel = new ViewModels.SettingsViewModel();
        private ISettingsView_Bindings bindings;

        /// <summary>
        /// Setup the page
        /// </summary>
        public SettingsView()
        {
            // Initialize XAML Components
            InitializeComponent();
            // This page must be cached
            NavigationCacheMode = NavigationCacheMode.Enabled;
            // Set the datacontext
            DataContext = ViewModel;
            // Page has been unloaded from UI
            Unloaded += (s, e) => ViewModel.Dispose();
        }

        public SettingsView(SettingsService settingsService, SettingsViewModel viewModel, Grid rootGrid, VisualStateGroup deviceStates, VisualStateGroup windowSize, VisualState xbox, Grid mobileGrid, TextBlock textHeader, Pivot infoPivot, StackPanel stackPanel, TextBlock appBuildTime, TextBlock appBuildBranch, TextBlock appVersion, Button disconnectSoundCloudButton, Button connectSoundCloudButton, Button disconnectFanBurstButton, Button connectFanBurstButton, ComboBox themeComboBox, ComboBox languageComboBox, ComboBoxItem language_English_US, ComboBoxItem language_French_FR, ComboBoxItem language_Dutch_NL, ComboBoxItem defaultTheme, ComboBoxItem darkTheme, ComboBoxItem lightTheme, bool contentLoaded)
        {
            SettingsService = settingsService;
            ViewModel = viewModel;
            RootGrid = rootGrid;
            DeviceStates = deviceStates;
            WindowSize = windowSize;
            Xbox = xbox;
            MobileGrid = mobileGrid;
            TextHeader = textHeader;
            InfoPivot = infoPivot;
            StackPanel = stackPanel;
            AppBuildTime = appBuildTime;
            AppBuildBranch = appBuildBranch;
            AppVersion = appVersion;
            DisconnectSoundCloudButton = disconnectSoundCloudButton;
            ConnectSoundCloudButton = connectSoundCloudButton;
            DisconnectFanBurstButton = disconnectFanBurstButton;
            ConnectFanBurstButton = connectFanBurstButton;
            this.themeComboBox = themeComboBox;
            LanguageComboBox = languageComboBox;
            Language_English_US = language_English_US;
            Language_French_FR = language_French_FR;
            Language_Dutch_NL = language_Dutch_NL;
            this.defaultTheme = defaultTheme;
            this.darkTheme = darkTheme;
            this.lightTheme = lightTheme;
            _contentLoaded = contentLoaded;
            Bindings = bindings;
        }

        /// <summary>
        /// Called when the user navigates to the page
        /// </summary>
        protected override async void OnNavigatedTo(NavigationEventArgs e)
        {
            // Set the last visited frame (crash handling)
            SettingsService.Current.LastFrame = typeof(SettingsView).FullName;
            // Track Event
            TelemetryService.Current.TrackPage("Info Page");
            // TEMP, Load the page
            LoadSettingsPage();

            // Set the app version
            AppVersion.Text = $"{Package.Current.Id.Version.Major}.{Package.Current.Id.Version.Minor}.{Package.Current.Id.Version.Build}.{Package.Current.Id.Version.Revision}";
            AppBuildBranch.Text = "...";
            AppBuildTime.Text = "...";

            var dataFile = await Package.Current.InstalledLocation.GetFileAsync(@"Assets\build_info.json");
            var buildData = await Task.Run(() => JsonConvert.DeserializeObject<BuildInfo>(File.ReadAllText(dataFile.Path)));

            AppBuildBranch.Text = buildData.BuildBranch;
            AppBuildTime.Text = buildData.BuildTime;


            DisconnectSoundCloudButton.Visibility = SoundByteService.Current.IsSoundCloudAccountConnected ? Visibility.Visible : Visibility.Collapsed;
            ConnectSoundCloudButton.Visibility = SoundByteService.Current.IsSoundCloudAccountConnected ? Visibility.Collapsed : Visibility.Visible;

            DisconnectFanBurstButton.Visibility = SoundByteService.Current.IsFanBurstAccountConnected ? Visibility.Visible : Visibility.Collapsed;
            ConnectFanBurstButton.Visibility = SoundByteService.Current.IsFanBurstAccountConnected ? Visibility.Collapsed : Visibility.Visible;
        }

        public async void NavigateBugs()
        {
            await Windows.System.Launcher.LaunchUriAsync(new Uri("https://gridentertainment.net/fwlink/GvC5iXmJSo"));
        }

        public async void NavigateFeedback()
        {
            var launcher = StoreServicesFeedbackLauncher.GetDefault();
            await launcher.LaunchAsync();
        }

        public async void NavigatePrivacy()
        {
            await Windows.System.Launcher.LaunchUriAsync(new Uri("https://gridentertainment.net/fwlink/Y5jGLtoFXs"));
        }

        public async void NavigateReddit()
        {
            await Windows.System.Launcher.LaunchUriAsync(new Uri("https://gridentertainment.net/fwlink/68vfoKLYJS"));
        }

        public async void NavigateFacebook()
        {
            await Windows.System.Launcher.LaunchUriAsync(new Uri("https://gridentertainment.net/fwlink/rOye5hzCXt"));
        }

        public async void NavigateGitHub()
        {
            await Windows.System.Launcher.LaunchUriAsync(new Uri("https://gridentertainment.net/fwlink/O3i37tbVVO"));
        }
        public void NavigateNew()
        {
            App.NavigateTo(typeof(WhatsNewView));
        }

        /// <summary>
        /// Called when the user taps on the rate_review button
        /// </summary>
        public async void RateAndReview()
        {
            TelemetryService.Current.TrackPage("Rate and Review App");

            await Windows.System.Launcher.LaunchUriAsync(new Uri($"ms-windows-store:REVIEW?PFN={Package.Current.Id.FamilyName}"));
        }

        private void LoadSettingsPage()
        {
            ViewModel.IsComboboxBlockingEnabled = true;
            // Get the saved language
            var appLanguage = SettingsService.Current.CurrentAppLanguage;
            // Check that the string is not empty
            if (!string.IsNullOrEmpty(appLanguage))
            {
                switch (appLanguage)
                {
                    case "en-US":
                        LanguageComboBox.SelectedItem = Language_English_US;
                        break;
                    case "fr":
                        LanguageComboBox.SelectedItem = Language_French_FR;
                        break;
                    case "nl":
                        LanguageComboBox.SelectedItem = Language_Dutch_NL;
                        break;
                    default:
                        LanguageComboBox.SelectedItem = Language_English_US;
                        break;
                }
            }
            else
            {
                LanguageComboBox.SelectedItem = Language_English_US;
            }

            switch (SettingsService.Current.ApplicationThemeType)
            {
                case AppTheme.Default:
                    themeComboBox.SelectedItem = defaultTheme;
                    break;
                case AppTheme.Light:
                    themeComboBox.SelectedItem = lightTheme;
                    break;
                case AppTheme.Dark:
                    themeComboBox.SelectedItem = darkTheme;
                    break;
                default:
                    themeComboBox.SelectedItem = defaultTheme;
                    break;
            }

            // Enable combo boxes
            ViewModel.IsComboboxBlockingEnabled = false;
        }

        private async void AppThemeComboBoxChanged(object sender, SelectionChangedEventArgs e)
        {
            if (ViewModel.IsComboboxBlockingEnabled)
                return;

            switch (((ComboBoxItem)(sender as ComboBox)?.SelectedItem)?.Name)
            {
                case "defaultTheme":
                    SettingsService.Current.ApplicationThemeType = AppTheme.Default;
                    ((MainShell)Window.Current.Content).RequestedTheme = ElementTheme.Default;
                    break;
                case "darkTheme":
                    SettingsService.Current.ApplicationThemeType = AppTheme.Dark;
                    ((MainShell)Window.Current.Content).RequestedTheme = ElementTheme.Dark;
                    break;
                case "lightTheme":
                    SettingsService.Current.ApplicationThemeType = AppTheme.Light;
                    ((MainShell)Window.Current.Content).RequestedTheme = ElementTheme.Light;
                    break;
                default:
                    SettingsService.Current.ApplicationThemeType = AppTheme.Default;
                    ((MainShell)Window.Current.Content).RequestedTheme = ElementTheme.Default;
                    break;
            }



            var restartDialog = new ContentDialog
            {
                Title = "App Restart",
                Content = new TextBlock { TextWrapping = TextWrapping.Wrap, Text = "The app needs to be restarted in order for the changes to correctly take effect." },
                IsPrimaryButtonEnabled = true,
                PrimaryButtonText = "Close"
            };

            await restartDialog.ShowAsync();
        }

        private void TextBlock_SelectionChanged(object sender, RoutedEventArgs e)
        {

        }
    }
}