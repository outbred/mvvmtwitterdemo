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
using System.Windows.Threading;

#endregion

namespace Infrastructure.Services.Interfaces
{
  public interface IDispatcherService
  {
    /// <summary>
    ///   A simple threading extension method, to invoke a delegate
    ///   on the correct thread if it is not currently on the correct thread
    ///   which can be used with DispatcherObject types.
    /// </summary>
    /// <param name="dispatcher">
    ///   The Dispatcher object on which to
    ///   perform the Invoke
    /// </param>
    /// <param name="action">The delegate to run</param>
    /// <param name="priority">The DispatcherPriority for the invoke.</param>
    void InvokeIfRequired(Action action, DispatcherPriority priority = DispatcherPriority.Normal);


    /// <summary>
    ///   A simple threading extension method, to invoke a delegate
    ///   on the correct thread asynchronously if it is not currently
    ///   on the correct thread which can be used with DispatcherObject types.
    /// </summary>
    /// <param name="dispatcher">
    ///   The Dispatcher object on which to
    ///   perform the Invoke
    /// </param>
    /// <param name="action">The delegate to run</param>
    void BeginInvoke(Action action);

    void SetCurrentDispatcher(Dispatcher dispatcher);

    /// <summary>
    ///   Processes all UI messages currently in the message queue.
    /// </summary>
    void WaitForPriority(DispatcherPriority priority = DispatcherPriority.ApplicationIdle);
  }
}