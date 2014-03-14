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

#endregion

namespace MvvmTwitter.Utilities
{
  /// <summary>
  ///   Exposes some thread-safe methods for adding/removing items
  /// </summary>
  /// <typeparam name="T"></typeparam>
  public class ConcurrentList<T> : List<T>
  {
    private readonly object Locker = new object();

    public new void AddRange(IEnumerable<T> items)
    {
      lock (Locker)
      {
        base.AddRange(items);
      }
    }

    public new void RemoveRange(int index, int count)
    {
      lock (Locker)
      {
        base.RemoveRange(index, count);
      }
    }

    public new void Add(T item)
    {
      lock (Locker)
      {
        base.Add(item);
      }
    }

    public new bool Remove(T item)
    {
      if (Contains(item))
      {
        lock (Locker)
        {
          return base.Remove(item);
        }
      }
      else
      {
        return false;
      }
    }
  }
}