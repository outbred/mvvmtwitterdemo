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

#endregion

namespace MvvmTwitter.Interfaces
{
  public enum ViewModelEvents
  {
    SwitchView,
    SwitchViewBlocked,
    SwitchViewUnblocked
  }

  public interface IMediatorService
  {
    /// <summary>
    ///   Subscribes to a certain ViewModelEvent
    /// </summary>
    /// <param name="eventType">Which event to be subscribed to</param>
    /// <param name="onTriggered">Action to be performed when event is triggered - strong reference</param>
    /// <returns>Subscription token (for unsubscribing)</returns>
    MediatorToken Subscribe(ViewModelEvents eventType, Action<object> onTriggered);

    /// <summary>
    ///   Allows a consumer to unsubscribe from events by passing in the type and token
    /// </summary>
    /// <param name="eventType">Which event to be unsubscribed from</param>
    /// <param name="token"></param>
    /// <returns>True if able to unsubscribe</returns>
    bool Unsubscribe(ViewModelEvents eventType, MediatorToken token);

    /// <summary>
    ///   Triggers the event with an payload
    /// </summary>
    /// <param name="eventType">Which event to publish</param>
    /// <param name="argument"></param>
    void NotifyColleagues(ViewModelEvents eventType, object argument);

    /// <summary>
    ///   Triggers the event with payload async from the calling thread
    /// </summary>
    /// <param name="eventType">Which event to publish asynchronously</param>
    /// <param name="argument"></param>
    void NotifyColleaguesAsync(ViewModelEvents eventType, object argument);
  }

  /// <summary>
  ///   Simple reference mechanism for keeping track of event subscriptions
  /// </summary>
  public class MediatorToken
  {}
}