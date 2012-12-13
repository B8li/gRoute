using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Navigation;
using System.Windows.Shapes;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Shell;
using gRouteTrack.ViewModels;
using gRouteTrack.Model;
using System.IO.IsolatedStorage;
using System.IO;
using System.Collections.ObjectModel;
using System.Windows.Media.Imaging;
using Microsoft.Phone;
namespace gRouteTrack
{
    public partial class App : Application
    {
        private static gSystemViewModel _gRouteTrackViewModel = null;
        public static gSystemViewModel gRouteTrackViewModel
        {
            get
            {
                if (_gRouteTrackViewModel == null)
                {
                    _gRouteTrackViewModel = new gSystemViewModel();
                }

                return _gRouteTrackViewModel;
            }
        }

        private static ConfigurationViewModel _configurationViewModel = null;
        public static ConfigurationViewModel configurationViewModel
        {
            get
            {
                if (_configurationViewModel == null)
                {
                    _configurationViewModel = new ConfigurationViewModel();
                }

                return _configurationViewModel;
            }
        }
        /// <summary>
        /// Provides easy access to the root frame of the Phone Application.
        /// </summary>
        /// <returns>The root frame of the Phone Application.</returns>
        public PhoneApplicationFrame RootFrame { get; private set; }

        /// <summary>
        /// Constructor for the Application object.
        /// </summary>
        public App()
        {
            // Global handler for uncaught exceptions. 
            UnhandledException += Application_UnhandledException;

            // Standard Silverlight initialization
            InitializeComponent();

            // Phone-specific initialization
            InitializePhoneApplication();

            // Show graphics profiling information while debugging.
            if (System.Diagnostics.Debugger.IsAttached)
            {
                // Display the current frame rate counters
                Application.Current.Host.Settings.EnableFrameRateCounter = true;

                // Show the areas of the app that are being redrawn in each frame.
                //Application.Current.Host.Settings.EnableRedrawRegions = true;

                // Enable non-production analysis visualization mode, 
                // which shows areas of a page that are handed off to GPU with a colored overlay.
                //Application.Current.Host.Settings.EnableCacheVisualization = true;

                // Disable the application idle detection by setting the UserIdleDetectionMode property of the
                // application's PhoneApplicationService object to Disabled.
                // Caution:- Use this under debug mode only. Application that disables user idle detection will continue to run
                // and consume battery power when the user is not using the phone.
                PhoneApplicationService.Current.UserIdleDetectionMode = IdleDetectionMode.Disabled;
            }
        }

        // Code to execute when the application is launching (eg, from Start)
        // This code will not execute when the application is reactivated
        private void Application_Launching(object sender, LaunchingEventArgs e)
        {
            LoadFromIsolatedStorage();
            LoadSettings();
        }

        // Code to execute when the application is activated (brought to foreground)
        // This code will not execute when the application is first launched
        private void Application_Activated(object sender, ActivatedEventArgs e)
        {
            if (e.IsApplicationInstancePreserved)
            {
                LoadFromStateObject();
            }
            else
            {
                LoadFromIsolatedStorage();
                LoadSettings();
            }
        }

        // Code to execute when the application is deactivated (sent to background)
        // This code will not execute when the application is closing
        private void Application_Deactivated(object sender, DeactivatedEventArgs e)
        {
            SaveToStateObject();
        }

        // Code to execute when the application is closing (eg, user hit Back)
        // This code will not execute when the application is deactivated
        private void Application_Closing(object sender, ClosingEventArgs e)
        {
            // Ensure that required application state is persisted here.
        }

        // Code to execute if a navigation fails
        private void RootFrame_NavigationFailed(object sender, NavigationFailedEventArgs e)
        {
            if (System.Diagnostics.Debugger.IsAttached)
            {
                // A navigation has failed; break into the debugger
                System.Diagnostics.Debugger.Break();
            }
        }

        // Code to execute on Unhandled Exceptions
        private void Application_UnhandledException(object sender, ApplicationUnhandledExceptionEventArgs e)
        {
            if (System.Diagnostics.Debugger.IsAttached)
            {
                // An unhandled exception has occurred; break into the debugger
                System.Diagnostics.Debugger.Break();
            }
            else
            {
                MessageBox.Show(e.ExceptionObject.Message);
            }
        }

        #region Phone application initialization

        // Avoid double-initialization
        private bool phoneApplicationInitialized = false;

        // Do not add any additional code to this method
        private void InitializePhoneApplication()
        {
            if (phoneApplicationInitialized)
                return;

            // Create the frame but don't set it as RootVisual yet; this allows the splash
            // screen to remain active until the application is ready to render.
            RootFrame = new PhoneApplicationFrame();
            RootFrame.Navigated += CompleteInitializePhoneApplication;

            // Handle navigation failures
            RootFrame.NavigationFailed += RootFrame_NavigationFailed;

            // Ensure we don't initialize again
            phoneApplicationInitialized = true;
        }

        // Do not add any additional code to this method
        private void CompleteInitializePhoneApplication(object sender, NavigationEventArgs e)
        {
            // Set the root visual to allow the application to render
            if (RootVisual != RootFrame)
                RootVisual = RootFrame;

            // Remove this handler since it is no longer needed
            RootFrame.Navigated -= CompleteInitializePhoneApplication;
        }

        #endregion

        

        public static void SaveToIsolatedStorage()
        {
            using (IsolatedStorageFile isf = IsolatedStorageFile.GetUserStoreForApplication())
            {
                if (!gRouteTrackViewModel.CurrentRoute.IsFinished)
                {
                    using (IsolatedStorageFileStream fileStream = isf.CreateFile("CurrentRoute.xml"))
                    {
                        StreamWriter writter = new StreamWriter(fileStream);
                        writter.Write(GPointConverter.FromGRouteToIsolatedStorageFormat(gRouteTrackViewModel.CurrentRoute));
                        writter.Close();
                    }
                }

                using (IsolatedStorageFileStream fileStream = isf.CreateFile("Routes.xml"))
                {
                    StreamWriter writter = new StreamWriter(fileStream);
                    writter.Write(GPointConverter.FromGRoutesToIsolatedStorageFormat(gRouteTrackViewModel.RoutesOnPhone.ToList<GRoute>()));
                    writter.Close();
                }
            }
        }
        public static void LoadFromIsolatedStorage()
        {
            String result = "";
            result = App.LoadStringFromIsolatedStorage("CurrentRoute.xml");
            if (!String.IsNullOrWhiteSpace(result))
            {
                GRoute currRoute;
                GPointConverter.FromIsolatedStorageFormatToGRoute(result, out currRoute);

                if (currRoute.Coordinates.Count > 0)
                {
                    gRouteTrackViewModel.CurrentRoute = currRoute;
                    MessageBox.Show("Loaded CurrentRoute from isolatedStorage"); //TO-DO Log File
                }
            }
            App.DeleteFileFromIsolatedStorage("CurrentRoute.xml");

            result = App.LoadStringFromIsolatedStorage("Routes.xml");
            if (!String.IsNullOrWhiteSpace(result))
            {
                List<GRoute> routes = new List<GRoute>();
                GPointConverter.FromIsolatedStorageFormatToGRoutes(result, out routes);

                if (routes.Count > 0)
                {
                    gRouteTrackViewModel.RoutesOnPhone = new ObservableCollection<GRoute>(routes);
                    MessageBox.Show("Loaded Routes from isolatedStorage"); //TO-DO Log File
                }
            }
            App.DeleteFileFromIsolatedStorage("Routes.xml");
        }

        public static void SaveSettings()
        {
            IsolatedStorageSettings settings = IsolatedStorageSettings.ApplicationSettings;
            settings.Clear();

            settings.Add("UserName", App.configurationViewModel.UserName);
            settings.Add("Password", App.configurationViewModel.Password);
            settings.Add("DistanceMeters", (Double)Decimal.Round((Decimal)App.configurationViewModel.DistanceInMeters, 0));

            settings.Save();

        }
        public static void LoadSettings()
        {
            IsolatedStorageSettings settings = IsolatedStorageSettings.ApplicationSettings;

            String searchValue = "";
            settings.TryGetValue<String>("UserName", out searchValue);
            if (string.IsNullOrWhiteSpace(searchValue) == false)
            {
                App.configurationViewModel.UserName = searchValue;
                searchValue = "";
                settings.TryGetValue<String>("Password", out searchValue);
                if (string.IsNullOrWhiteSpace(searchValue) == false)
                {
                    App.configurationViewModel.Password = searchValue;
                }
            }

            Double metersDistance;
            if (settings.TryGetValue<Double>("DistanceMeters", out metersDistance))
            {
                App.configurationViewModel.DistanceInMeters = metersDistance;
            }
        }
        private void SaveToStateObject()
        {
            IDictionary<string, object> stateStore = PhoneApplicationService.Current.State;

            stateStore.Remove("CurrentRoute");
            stateStore.Add("CurrentRoute", gRouteTrackViewModel.CurrentRoute);

            stateStore.Remove("Routes");
            stateStore.Add("Routes", gRouteTrackViewModel.RoutesOnPhone.ToList<GRoute>());

            stateStore.Remove("UserName");
            stateStore.Add("UserName", configurationViewModel.UserName);

            stateStore.Remove("Password");
            stateStore.Add("Password", configurationViewModel.Password);

            stateStore.Remove("DistanceMeters");
            stateStore.Add("DistanceMeters", Double.Parse(Decimal.Round((Decimal)configurationViewModel.DistanceInMeters, 2).ToString()));
        }
        private static String LoadStringFromIsolatedStorage(String FileName)
        {
            String result = "";

            using (IsolatedStorageFile isf = IsolatedStorageFile.GetUserStoreForApplication())
            {
                if (isf.FileExists(FileName))
                {
                    try
                    {
                        using (IsolatedStorageFileStream stream = isf.OpenFile(FileName, FileMode.Open))
                        {
                            StreamReader reader = new StreamReader(stream);
                            result = reader.ReadToEnd();
                            reader.Close();
                        }
                    }
                    catch (Exception)
                    {
                        result = "";
                    }
                }
            }
            return result;
        }
        private static void LoadFromStateObject()
        {
            if (PhoneApplicationService.Current.State.ContainsKey("CurrentRoute"))
            {
                gRouteTrackViewModel.CurrentRoute = (GRoute)PhoneApplicationService.Current.State["CurrentRoute"];
                MessageBox.Show("Loaded CurrentRoute from State object"); //TO-DO Log File
            }

            if (PhoneApplicationService.Current.State.ContainsKey("Routes"))
            {
                gRouteTrackViewModel.RoutesOnPhone = new ObservableCollection<GRoute>((List<GRoute>)PhoneApplicationService.Current.State["Routes"]);
                MessageBox.Show("Loaded Routes from State object"); //TO-DO Log File
            }

            if (PhoneApplicationService.Current.State.ContainsKey("UserName"))
            {
                configurationViewModel.UserName = (String)PhoneApplicationService.Current.State["UserName"];

                if (PhoneApplicationService.Current.State.ContainsKey("Password"))
                {
                    configurationViewModel.Password = (String)PhoneApplicationService.Current.State["Password"];
                }
            }

            if (PhoneApplicationService.Current.State.ContainsKey("DistanceMeters"))
            {
                configurationViewModel.DistanceInMeters = (Double)PhoneApplicationService.Current.State["DistanceMeters"];
            }


            gRouteTrackViewModel.LocationServiceStatus = gSystemSettings.GLocationServiceStatus.Paused;

        }
        private static void DeleteFileFromIsolatedStorage(String FileName)
        {
            IsolatedStorageFile.GetUserStoreForApplication().DeleteFile(FileName);
        }

        public static WriteableBitmap ReadImageFromIsolatedStorage(string fileName)
        {
            WriteableBitmap bitmap = new WriteableBitmap(200, 200);
            using (IsolatedStorageFile myIsolatedStorage = IsolatedStorageFile.GetUserStoreForApplication())
            {
                using (IsolatedStorageFileStream fileStream = myIsolatedStorage.OpenFile(fileName, FileMode.Open, FileAccess.Read))
                {
                    // Decode the JPEG stream.
                    bitmap = PictureDecoder.DecodeJpeg(fileStream);
                }
            }

            return bitmap;
        }
    }
}