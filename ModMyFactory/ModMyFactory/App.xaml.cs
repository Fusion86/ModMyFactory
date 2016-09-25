﻿using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Threading;
using System.Windows;
using System.Windows.Input;
using ModMyFactory.Lang;

namespace ModMyFactory
{
    public partial class App : Application
    {
        internal static App Instance => (App)Application.Current;

        ResourceDictionary enDictionary;

        internal Settings Settings { get; }

        internal string AppDataPath { get; }

        public App(string appDataPath)
        {
            AppDataPath = appDataPath;

            var appDataDirectory = new DirectoryInfo(AppDataPath);
            if (!appDataDirectory.Exists) appDataDirectory.Create();

            string settingsFile = Path.Combine(appDataDirectory.FullName, "settings.json");
            Settings = Settings.Load(settingsFile, true);
        }

        public App()
            : this(Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "ModMyFactory"))
        { }

        internal List<CultureEntry> GetAvailableCultures()
        {
            var availableCultures = new List<CultureEntry>();
            foreach (var key in Resources.Keys)
            {
                string stringKey = key as string;
                if (stringKey != null && stringKey.StartsWith("Strings."))
                {
                    var dictionary = Resources[key] as ResourceDictionary;
                    if (dictionary != null)
                    {
                        string cultureName = stringKey.Split('.')[1];
                        availableCultures.Add(new CultureEntry(new CultureInfo(cultureName)));
                    }
                }
            }
            return availableCultures;
        }

        internal void SelectCulture(CultureInfo culture)
        {
            if (enDictionary == null)
                enDictionary = (ResourceDictionary)Resources["Strings.en"];

            var mergedDictionaries = Resources.MergedDictionaries;
            mergedDictionaries.Clear();

            mergedDictionaries.Add(enDictionary);
            string resourceName = "Strings." + culture.TwoLetterISOLanguageName;
            if (culture.TwoLetterISOLanguageName != "en" && Resources.Contains(resourceName))
                mergedDictionaries.Add((ResourceDictionary)Resources[resourceName]);

            Thread.CurrentThread.CurrentCulture = culture;
            Thread.CurrentThread.CurrentUICulture = culture;
        }

        private void ExpanderPreviewMouseWheelHandler(object sender, MouseWheelEventArgs e)
        {
            e.Handled = true;

            var element = (UIElement)sender;
            var newArgs = new MouseWheelEventArgs(e.MouseDevice, e.Timestamp, e.Delta);
            newArgs.RoutedEvent = UIElement.MouseWheelEvent;
            element.RaiseEvent(newArgs);
        }
    }
}