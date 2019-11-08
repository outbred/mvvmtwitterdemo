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
using System.ComponentModel;
using System.ComponentModel.Composition;
using System.Linq;
using System.Reactive.Linq;
using CommonServiceLocator;
using Infrastructure.Base;
using Infrastructure.Helpers;
using Infrastructure.Services.Interfaces;
using Microsoft.Practices.Prism.Events;
using Microsoft.Practices.Prism.Regions;
using TweetsModule.ViewModels;
using TweetsModule.Views;

#endregion

namespace TweetsModule
{
  [Export]
  public class TweetsController : RegionControllerBase
  {
    private readonly List<IDisplayViewModel> _viewModels;

    [ImportingConstructor]
    public TweetsController(IRegionManager regionManager, IEventAggregator aggregator, IServiceLocator container, IDispatcherService dispatcherService,
                            [ImportMany] IEnumerable<IDisplayView> views)
      : base(regionManager, aggregator, container, dispatcherService)
    {
      // Pull in the views so that the IViewAwareStatus object is set correctly with the View on the ViewModels
      // Then extract the VM's from the Views
      _viewModels = views.Select(v => v.DataContext as IDisplayViewModel).ToList();

      // Fire an event so the Menu module can listen for it, instead of calling RegisterAndActivateView directly
      aggregator.GetEvent<SwitchViewEvent>().Publish(ViewType.UglyView);
      aggregator.GetEvent<ToggleViewInjectionEvent>().Subscribe(canInject => { _viewModels.ForEach(vm => vm.CanSwitchView = canInject); }, true);

      _viewModels.ForEach(vm =>
                          {
                            Observable.FromEventPattern<PropertyChangedEventArgs>(vm, "PropertyChanged")
                                      .Where(args => args.EventArgs.PropertyName == "Searching")
                                      .Subscribe(args => Logger.Default.Debug("{0} may be searching (could be true or false)", vm.GetType().Name));
                          });
    }

    #region Overrides of RegionControllerBase

    protected override void OnSwitchViewEvent(ViewType type)
    {
      switch (type)
      {
        case ViewType.UglyView:
          RegisterAndActivateView<IUglyDisplayView>();
          break;
        case ViewType.PrettyView:
          RegisterAndActivateView<IPrettierDisplayView>();
          break;
      }
    }

    #endregion
  }
}