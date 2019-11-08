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
using System.Linq;
using CommonServiceLocator;
using Infrastructure.Helpers;
using Infrastructure.Services.Interfaces;
using Microsoft.Practices.Prism.Events;
using Microsoft.Practices.Prism.Regions;

#endregion

namespace Infrastructure.Base
{
  public abstract class RegionControllerBase
  {
    protected readonly IEventAggregator _aggregator;
    protected readonly IRegionManager _regionManager;
    protected readonly IServiceLocator _container;
    protected readonly IDispatcherService _dispatcherService;
    private readonly object _locker = new object();

    protected RegionControllerBase(IRegionManager regionManager, IEventAggregator aggregator, IServiceLocator container, IDispatcherService dispatcherService)
    {
      _dispatcherService = dispatcherService;
      _container = container;
      _aggregator = aggregator;
      _regionManager = regionManager;
      _aggregator = aggregator;

      _aggregator.GetEvent<SwitchViewEvent>().Subscribe(OnSwitchViewEvent, true);
    }

    protected abstract void OnSwitchViewEvent(ViewType type);

    /// <summary>
    ///   Meant to be used by an inheriting controller to inject a view from the SwitchViewEvent, handled by OnSwitchViewEvent
    /// </summary>
    /// <typeparam name="TView"></typeparam>
    /// <param name="region"></param>
    /// <returns></returns>
    protected TView RegisterAndActivateView<TView>(string region = RegionNames.MainRegion) where TView : class
    {
      if (string.IsNullOrWhiteSpace(region))
      {
        region = RegionNames.MainRegion;
      }

      TView view = null;
      _dispatcherService.InvokeIfRequired(() =>
                                          {
                                            lock (_locker)
                                            {
                                              try
                                              {
                                                view = _container.GetInstance<TView>();

                                                try
                                                {
                                                  _regionManager.Regions[region].ActiveViews.ToList().ForEach(v => _regionManager.Regions[region].Remove(v));
                                                  // Add it, then activate
                                                  _regionManager.AddToRegion(region, view);
                                                  _regionManager.Regions[region].Activate(view);
                                                }
                                                catch (Exception ex)
                                                {
                                                  Logger.Default.Error("Unable to activate view.", ex);
                                                  view = null;
                                                }
                                              }
                                              catch (Exception ex)
                                              {
                                                Logger.Default.Error("unable to switch view", ex);
                                              }
                                            }
                                          });

      return view;
    }
  }
}