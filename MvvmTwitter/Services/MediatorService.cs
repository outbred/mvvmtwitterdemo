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
using System.Collections.Concurrent;
using System.Threading.Tasks;
using MvvmTwitter.Interfaces;
using MvvmTwitter.Utilities;

#endregion

namespace MvvmTwitter.Services
{
  public class MediatorService : IMediatorService
  {
    /// <summary>
    ///   Precisely one action per token
    /// </summary>
    private static readonly ConcurrentDictionary<MediatorToken, Action<object>> RegisteredEvents = new ConcurrentDictionary<MediatorToken, Action<object>>();

    /// <summary>
    ///   Possibly multiple tokens for one event
    /// </summary>
    private static readonly ConcurrentDictionary<ViewModelEvents, ConcurrentList<MediatorToken>> SubscriptionTokens = new ConcurrentDictionary<ViewModelEvents, ConcurrentList<MediatorToken>>();

    public MediatorToken Subscribe(ViewModelEvents eventType, Action<object> onTriggered)
    {
      if (onTriggered == null)
      {
        return null;
      }

      MediatorToken token = null;
      if (!SubscriptionTokens.ContainsKey(eventType))
      {
        token = new MediatorToken();
        RegisteredEvents.TryAdd(token, onTriggered);
        SubscriptionTokens.TryAdd(eventType, new ConcurrentList<MediatorToken>() {token});
      }
      else
      {
        token = new MediatorToken();
        RegisteredEvents[token] = onTriggered;
        SubscriptionTokens[eventType].Add(token);
      }
      return token;
    }

    public bool Unsubscribe(ViewModelEvents eventType, MediatorToken token)
    {
      if (token == null)
      {
        return false;
      }

      if (SubscriptionTokens.ContainsKey(eventType) && SubscriptionTokens[eventType].Contains(token))
      {
        var killedIt = SubscriptionTokens[eventType].Remove(token);
        Action<object> someAction;
        killedIt = killedIt && RegisteredEvents.TryRemove(token, out someAction);
        return killedIt;
      }
      return false;
    }

    /// <summary>
    ///   Colleagues are notified in the order that they subscribed
    /// </summary>
    /// <param name="eventType"></param>
    /// <param name="argument"></param>
    public void NotifyColleagues(ViewModelEvents eventType, object argument)
    {
      // find all tokens for this event
      // fire off the Action<> for each one in succession
      if (SubscriptionTokens.ContainsKey(eventType) && SubscriptionTokens[eventType] != null)
      {
        var tokens = SubscriptionTokens[eventType];
        tokens.ForEach(t =>
                       {
                         if (RegisteredEvents.ContainsKey(t))
                         {
                           RegisteredEvents[t](argument);
                         }
                       });
      }
    }

    /// <summary>
    ///   Colleagues are notified in the order that they subscribed, asynchronously
    /// </summary>
    /// <param name="eventType"></param>
    /// <param name="argument"></param>
    public void NotifyColleaguesAsync(ViewModelEvents eventType, object argument)
    {
      Task.Factory.StartNew(() => NotifyColleagues(eventType, argument));
    }
  }
}