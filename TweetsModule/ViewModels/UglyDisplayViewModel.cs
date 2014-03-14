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
using Infrastructure.Base;
using Infrastructure.Helpers;
using Infrastructure.Models;
using MEFedMVVM.ViewModelLocator;
using MvvmTwitter.Interfaces;
using TweetsModule.Views;

#endregion

namespace TweetsModule.ViewModels
{
  [Export(typeof (IDisplayViewModel))] // MEF
  [ExportViewModel("UglyDisplayViewModel")] // MEFedMvvm
  public class UglyDisplayViewModel : DisplayViewModelBase
  {
    protected override ViewType SwitchToView
    {
      get { return ViewType.PrettyView; }
    }

    public string State
    {
      get { return Get<string>(); }
      set { Set(value); }
    }

    private const string Loaded = "Loaded";
    private const string FinishedSearch = "FinishedSearch";
    private const string Unloaded = "Unloaded";
    private bool _injected = false;

    [ImportingConstructor]
    public UglyDisplayViewModel(ITweetService tweetService, IViewAwareStatus viewAwareStatus)
      : base(tweetService)
    {
      viewAwareStatus.ViewLoaded += () =>
                                    {
                                      //MessageBoxService.ShowInformation("You loaded the ugly view!");
                                      Logger.Default.Debug("Ugly loaded");
                                      State = Loaded;
                                      if (!_injected)
                                      {
                                        _injected = true;
                                        RegisterAndActivateView<ITweetBrowser>("BrowserRegion");
                                      }
                                    };
      viewAwareStatus.ViewUnloaded += () => State = Unloaded;
      //Microsoft.Practices.Prism.Regions.RegionManager.SetRegionManager(viewAwareStatus.View as DependencyObject, RegionManager);
      //Microsoft.Practices.Prism.Regions.RegionManager.UpdateRegions();
    }

    #region Overrides of DisplayViewModelBase

    protected override Action OnSearchComplete
    {
      get { return () => State = FinishedSearch; }
      set { }
    }

    #endregion
  }
}