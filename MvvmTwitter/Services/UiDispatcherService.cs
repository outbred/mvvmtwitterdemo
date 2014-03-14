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
using System.Threading;
using System.Windows;
using MvvmTwitter.Interfaces;

#endregion

namespace MvvmTwitter.Utilities
{
  // Architecture prompt: Why have this class at all?  It obviously just wraps around Application.Current.Dispatcher...
  public class UiDispatcherService : IUiDispatcherService
  {
    public void Invoke(Action toInvoke)
    {
      // Prompt: Why have this check at all?
      if (Application.Current != null && Thread.CurrentThread != Application.Current.Dispatcher.Thread)
      {
        Application.Current.Dispatcher.Invoke(toInvoke);
      }
      else
      {
        toInvoke();
      }
    }

    public void InvokeAsync(Action toInvoke)
    {
      if (Application.Current != null && Thread.CurrentThread != Application.Current.Dispatcher.Thread)
      {
        Application.Current.Dispatcher.BeginInvoke(toInvoke);
      }
      else
      {
        toInvoke();
      }
    }

    public Window CurrentApplication
    {
      get { return Application.Current.MainWindow; }
    }
  }
}