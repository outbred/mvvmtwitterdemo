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
using System.ComponentModel.Composition;
using System.Windows.Threading;
using Infrastructure.Services.Interfaces;
using MEFedMVVM.ViewModelLocator;

#endregion

namespace Infrastructure.Services
{
  [PartCreationPolicy(CreationPolicy.Shared)]
  [ExportService(ServiceType.Both, typeof (IDispatcherService))]
  public class DispatcherService : IDispatcherService
  {
    private Dispatcher _currentDispatcher = null;

    [ImportingConstructor]
    public DispatcherService()
    {
#if SILVERLIGHT
      _currentDispatcher = Deployment.Current.Dispatcher;
#else
      _currentDispatcher = Dispatcher.CurrentDispatcher;
#endif
    }

    #region Implementation of IDispatcherService

    public void BeginInvoke(Action method)
    {
      if (method == null)
      {
        return;
      }

      if (_currentDispatcher != null)
      {
        _currentDispatcher.BeginInvoke(method);
      }
      else
      {
        method();
      }
    }

    public void InvokeIfRequired(Action method, DispatcherPriority priority = DispatcherPriority.Normal)
    {
      if (_currentDispatcher != null && !_currentDispatcher.CheckAccess())
      {
        _currentDispatcher.Invoke(method, priority);
      }
      else
      {
        method();
      }
    }

    public void SetCurrentDispatcher(Dispatcher dispatcher)
    {
      _currentDispatcher = dispatcher;
    }

    private static readonly DispatcherOperationCallback _exitFrameCallback = ExitFrame;

    public void WaitForPriority(DispatcherPriority priority = DispatcherPriority.ApplicationIdle)
    {
      RenderThenExecute(priority);
    }

    /// <summary>
    ///   Processes all UI messages currently in the message queue.
    /// </summary>
    private static void RenderThenExecute(DispatcherPriority priority)
    {
      try
      {
        // Create new nested message pump.
        DispatcherFrame nestedFrame = new DispatcherFrame();

        // Dispatch a callback to the current message queue, when getting called,
        // this callback will end the nested message loop.
        // The priority of this callback should be lower than that of event message you want to process.
        DispatcherOperation exitOperation = Dispatcher.CurrentDispatcher.BeginInvoke(priority, _exitFrameCallback, nestedFrame);

        // pump the nested message loop, the nested message loop will immediately
        // process the messages left inside the message queue.
        Dispatcher.PushFrame(nestedFrame);

        // If the "exitFrame" callback is not finished, abort it.
        if (exitOperation.Status != DispatcherOperationStatus.Completed)
        {
          exitOperation.Abort();
        }
      }
        // threads can be out of sync depending on usage, but don't let that kill the app - bbulla
      catch
      {}
    }

    private static Object ExitFrame(Object state)
    {
      var frame = state as DispatcherFrame;

      // Exit the nested message loop.
      if (frame != null)
      {
        frame.Continue = false;
      }
      return null;
    }

    #endregion
  }
}