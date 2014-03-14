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
using System.Reflection;

#endregion

namespace Infrastructure.Models
{
  /// <summary>
  ///   This class creates a weak delegate of form Action(Of Object)
  /// </summary>
  public class WeakAction
  {
    #region Data

    private readonly WeakReference _target;
    private readonly Type _ownerType;
    private readonly Type _actionType;
    private readonly string _methodName;

    #endregion

    #region Public Properties/Methods

    public WeakAction(object target, Type actionType, MethodBase mi)
    {
      if (target == null)
      {
        Debug.Assert(mi.IsStatic);
        _ownerType = mi.DeclaringType;
      }
      else
      {
        _target = new WeakReference(target);
      }
      _methodName = mi.Name;
      _actionType = actionType;
    }

    public Type ActionType
    {
      get { return _actionType; }
    }

    public bool HasBeenCollected
    {
      get { return (_ownerType == null && (_target == null || !_target.IsAlive)); }
    }

    public Delegate GetMethod()
    {
      if (_ownerType != null)
      {
        return Delegate.CreateDelegate(_actionType, _ownerType, _methodName);
      }

      if (_target != null && _target.IsAlive)
      {
        object target = _target.Target;
        if (target != null)
        {
          return Delegate.CreateDelegate(_actionType, target, _methodName);
        }
      }

      return null;
    }

    #endregion
  }
}