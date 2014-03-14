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

using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;

#endregion

namespace Infrastructure.Models
{
  /// <summary>
  ///   Much faster AddRange than a manual Add for many items
  /// </summary>
  /// <typeparam name="T"></typeparam>
  public class ObservableCollectionEx<T> : ObservableCollection<T>
  {
    public ObservableCollectionEx()
      : base()
    {
      _suspendCollectionChangeNotification = false;
    }

    public ObservableCollectionEx(IEnumerable<T> list)
    {
      if (list == null)
      {
        return;
      }

      AddRange(list);
    }


    private bool _suspendCollectionChangeNotification;

    protected override void OnCollectionChanged(NotifyCollectionChangedEventArgs e)
    {
      if (!_suspendCollectionChangeNotification)
      {
        base.OnCollectionChanged(e);
      }
    }

    public void SuspendCollectionChangeNotification()
    {
      _suspendCollectionChangeNotification = true;
    }

    public void ResumeCollectionChangeNotification()
    {
      _suspendCollectionChangeNotification = false;
    }


    public void AddRange(IEnumerable<T> items)
    {
      SuspendCollectionChangeNotification();
      try
      {
        foreach (var i in items)
        {
          base.InsertItem(base.Count, i);
        }
      }
      finally
      {
        ResumeCollectionChangeNotification();
        var arg = new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Reset);
        OnCollectionChanged(arg);
      }
    }
  }
}