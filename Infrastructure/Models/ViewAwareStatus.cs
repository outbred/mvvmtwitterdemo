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
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;
using System.Windows;
using System.Windows.Threading;
using MEFedMVVM.ViewModelLocator;

#endregion

namespace Infrastructure.Models
{
  /// <summary>
  ///   View aware service that provides the following
  ///   1. Events for ViewLoaded / ViewUnloaded (WPF and SL)
  ///   2. Events for ViewActivated / ViewDeactivated (WPF Only)
  ///   3. Views current Dispatcher
  ///   4. If the view implements <c>IViewCreationContextProvider</c>
  ///   the current Views Context will also be available to allow
  ///   the ViewModel to obtain some view specific contextual information
  /// </summary>
  [PartCreationPolicy(CreationPolicy.NonShared)]
  [ExportService(ServiceType.Both, typeof (IViewAwareStatus))]
  public class ViewAwareStatus : IViewAwareStatus
  {
    #region Data

    private WeakReference _weakViewInstance;

    #endregion

    #region IViewAwareStatus Members

    private readonly IList<WeakAction> _loadedHandlers = new List<WeakAction>();

    public event Action ViewLoaded
    {
      add { _loadedHandlers.Add(new WeakAction(value.Target, typeof (Action), value.Method)); }
      remove { }
    }


    private readonly IList<WeakAction> unloadedHandlers = new List<WeakAction>();

    public event Action ViewUnloaded
    {
      add { unloadedHandlers.Add(new WeakAction(value.Target, typeof (Action), value.Method)); }
      remove { }
    }


    private readonly IList<WeakAction> activatedHandlers = new List<WeakAction>();

    public event Action ViewActivated
    {
      add { activatedHandlers.Add(new WeakAction(value.Target, typeof (Action), value.Method)); }
      remove { }
    }


    private readonly IList<WeakAction> deactivatedHandlers = new List<WeakAction>();

    public event Action ViewDeactivated
    {
      add { deactivatedHandlers.Add(new WeakAction(value.Target, typeof (Action), value.Method)); }
      remove { }
    }

    public Dispatcher ViewsDispatcher { get; private set; }

    public Object View
    {
      get { return (Object) _weakViewInstance.Target; }
    }

    #endregion

    #region IContextAware Members

    public void InjectContext(object view)
    {
      if (_weakViewInstance != null)
      {
        if (_weakViewInstance.Target == view)
        {
          return;
        }
      }

      // unregister before hooking new events
      if (_weakViewInstance != null && _weakViewInstance.Target != null)
      {
        object targ = _weakViewInstance.Target;

        if (targ != null)
        {
          ((FrameworkElement) targ).Loaded -= OnViewLoaded;
          ((FrameworkElement) targ).Unloaded -= OnViewUnloaded;

          Window w = targ as Window;
          if (w != null)
          {
            w.Activated -= OnViewActivated;
            w.Deactivated -= OnViewDeactivated;
          }
        }
      }

      var x = view as FrameworkElement;

      if (x != null)
      {
        x.Loaded += OnViewLoaded;
        x.Unloaded += OnViewUnloaded;

        Window w = x as Window;
        if (w != null)
        {
          w.Activated += OnViewActivated;
          w.Deactivated += OnViewDeactivated;
        }

        //get the Views Dispatcher
        ViewsDispatcher = x.Dispatcher;
        _weakViewInstance = new WeakReference(x);
      }
    }

    #endregion

    #region Private Helpers

    private void OnViewLoaded(object sender, RoutedEventArgs e)
    {
      var methods = (from handler in _loadedHandlers
                     let method = handler.GetMethod()
                     where method != null
                     select method).ToList();
      foreach (var method in methods)
      {
        method.DynamicInvoke();
      }
    }

    private void OnViewUnloaded(object sender, RoutedEventArgs e)
    {
      foreach (var unloadedHandler in unloadedHandlers)
      {
        unloadedHandler.GetMethod().DynamicInvoke();
      }
    }


    private void OnViewActivated(object sender, EventArgs e)
    {
      foreach (var activatedHandler in activatedHandlers)
      {
        activatedHandler.GetMethod().DynamicInvoke();
      }
    }

    private void OnViewDeactivated(object sender, EventArgs e)
    {
      foreach (var deactivatedHandler in deactivatedHandlers)
      {
        deactivatedHandler.GetMethod().DynamicInvoke();
      }
    }

    #endregion
  }
}