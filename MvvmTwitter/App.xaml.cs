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
using System.Windows;

#endregion

namespace MvvmTwitter
{
  /// <summary>
  ///   Interaction logic for App.xaml
  /// </summary>
  public partial class App : Application
  {
    protected override void OnStartup(StartupEventArgs e)
    {
      base.OnStartup(e);
      AppDomain.CurrentDomain.UnhandledException += AppDomainUnhandledException;
    }

    private static void AppDomainUnhandledException(object sender, UnhandledExceptionEventArgs e)
    {
      var ex = e.ExceptionObject as Exception;
      if (ex == null)
      {
        return;
      }


      Trace.TraceError("Unhandled exception in this simple demo app. Did someone forget to eat their Wheaties?\n\n{0}", ex.Message);
      //ExceptionPolicy.HandleException(ex, "Default Policy");
      MessageBox.Show(string.Format("An unhandled exception has occurred: {0}\n{1}\n\n{2}\n{3}", ex.Message, ex.StackTrace, ex.InnerException != null ? ex.InnerException.Message : null, ex.InnerException != null ? ex.InnerException.StackTrace : null), "Error",
                      MessageBoxButton.OK, MessageBoxImage.Error);
      Environment.Exit(1);
    }
  }
}