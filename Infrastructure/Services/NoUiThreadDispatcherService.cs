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
using System.Threading.Tasks;
using System.Windows.Threading;
using Infrastructure.Services.Interfaces;

#endregion

namespace Infrastructure.Services
{
  /// <summary>
  ///   Used when unit testing a ViewModel (no UI thread)
  /// </summary>
  public class NoUiThreadDispatcherService : IDispatcherService
  {
    public void SetCurrentDispatcher(Dispatcher dispatcher)
    {
      // DO NOTHING
    }

    /// <summary>
    ///   Processes all UI messages currently in the message queue.
    /// </summary>
    public void WaitForPriority(DispatcherPriority priority = DispatcherPriority.ApplicationIdle)
    {}

    #region Implementation of IDispatcherService

    public void InvokeIfRequired(Action action, DispatcherPriority priority = DispatcherPriority.Normal)
    {
      if (action != null)
      {
        action();
      }
    }

    public void BeginInvoke(Action action)
    {
      if (action == null)
      {
        return;
      }

      Task.Factory.StartNew(action);
    }

    #endregion
  }
}