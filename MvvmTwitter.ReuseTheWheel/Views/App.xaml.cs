// Copyright (c) 2014 Brent Bulla Jr.
//
// Permission is hereby granted, free of charge, to any person obtaining a copy
// of this software and associated documentation files (the "Software"), to deal
// in the Software without restriction, including without limitation the rights
// to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
// copies of the Software, and to permit persons to whom the Software is
// furnished to do so, subject to the following conditions:
//
// The above copyright notice and this permission notice shall be included in
// all copies or substantial portions of the Software.
//
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
// IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
// FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
// AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
// LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
// OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN
// THE SOFTWARE.

#region

using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Windows;
using Infrastructure.Helpers;

#endregion

namespace MvvmTwitter.ReuseTheWheel.Views
{
  /// <summary>
  ///   Interaction logic for App.xaml
  /// </summary>
  public partial class App : Application
  {
    #region Imports

    // Sets the window to be foreground
    [DllImport("User32")]
    private static extern int SetForegroundWindow(IntPtr hwnd);

    // Activate or minimize a window
    [DllImport("User32.DLL")]
    private static extern bool ShowWindow(IntPtr hWnd, int nCmdShow);

    private const int SW_SHOW = 5;
    private const int SW_MINIMIZE = 6;
    private const int SW_RESTORE = 9;

    [DllImport("user32.dll", SetLastError = true)]
    private static extern IntPtr GetWindow(IntPtr hWnd, GetWindow_Cmd uCmd);

    private enum GetWindow_Cmd : uint
    {
      GW_HWNDFIRST = 0,
      GW_HWNDLAST = 1,
      GW_HWNDNEXT = 2,
      GW_HWNDPREV = 3,
      GW_OWNER = 4,
      GW_CHILD = 5,
      GW_ENABLEDPOPUP = 6
    }

    [DllImport("user32.dll")]
    [return: MarshalAs(UnmanagedType.Bool)]
    private static extern bool IsWindowVisible(IntPtr hWnd);

    #endregion

    private bool IsMainWindow(IntPtr handle)
    {
      return (!(GetWindow(handle, GetWindow_Cmd.GW_OWNER) != IntPtr.Zero) && IsWindowVisible(handle));
    }

    #region Overrides of Application

    /// <summary>
    ///   Raises the <see cref="E:System.Windows.Application.Startup" /> event.
    /// </summary>
    /// <param name="e">A <see cref="T:System.Windows.StartupEventArgs" /> that contains the event data.</param>
    protected override void OnStartup(StartupEventArgs e)
    {
      base.OnStartup(e);
      AppDomain.CurrentDomain.UnhandledException += AppDomainUnhandledException;

      if (!IsUniqueInstance)
      {
        var appName = Path.GetFileNameWithoutExtension(Assembly.GetEntryAssembly().Location);
        var process = Process.GetProcessesByName(appName).FirstOrDefault();
        if (process != null && process.MainWindowHandle != (IntPtr) 0 && IsMainWindow(process.MainWindowHandle))
        {
          ShowWindow(process.MainWindowHandle, SW_RESTORE);
          SetForegroundWindow(process.MainWindowHandle);
          Logger.Default.Debug("Shutting down app b/c another instance is running.");
          Shutdown();
          return;
        }
        else if (process != null)
        {
          // kill it
          process.Kill();
        }
      }

#if (DEBUG)
      RunInDebugMode();
#else
			RunInReleaseMode();
#endif
    }

    private static void AppDomainUnhandledException(object sender, UnhandledExceptionEventArgs e)
    {
      var ex = e.ExceptionObject as Exception;
      if (ex == null)
      {
        return;
      }

      HandleException(ex);
    }

    #endregion

    private static void HandleException(Exception ex)
    {
      Trace.TraceError("Unhandled exception in this simple demo app. Did someone forget to eat their Wheaties?\n\n{0}", ex.Message);
      //ExceptionPolicy.HandleException(ex, "Default Policy");
      MessageBox.Show(string.Format("An unhandled exception has occurred: {0}\n{1}\n\n{2}\n{3}", ex.Message, ex.StackTrace, ex.InnerException != null ? ex.InnerException.Message : null, ex.InnerException != null ? ex.InnerException.StackTrace : null), "Error",
                      MessageBoxButton.OK, MessageBoxImage.Error);
      Environment.Exit(1);
    }

    private static void RunInDebugMode()
    {
      var bootstrapper = new Bootstrapper();
      bootstrapper.Run();
    }

    private static void RunInReleaseMode()
    {
      try
      {
        var bootstrapper = new Bootstrapper();
        bootstrapper.Run();
      }
      catch (Exception ex)
      {
        HandleException(ex);
      }
    }

    private bool _unique;

    private bool IsUniqueInstance
    {
      get
      {
        if (!_unique)
        {
          _unique = Process.GetProcessesByName(Process.GetCurrentProcess().ProcessName).Count() == 1;
        }

        return _unique;
      }
    }
  }
}