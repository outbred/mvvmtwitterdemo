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

using System.ComponentModel.Composition;
using Infrastructure.Base;
using MEFedMVVM.ViewModelLocator;

#endregion

namespace MenuModule.ViewModels
{
  [ExportViewModel("MenuViewModel")]
  public class MenuViewModel : ViewModelBase
  {
    [ImportingConstructor]
    public MenuViewModel()
    {
      Aggregator.GetEvent<ToggleViewInjectionEvent>().Subscribe(canInject => CanSwitchViews = canInject, true);
      Aggregator.GetEvent<SwitchViewEvent>().Subscribe(view => CurrentView = view, true);
      CanSwitchViews = true;
    }

    public bool CanSwitchViews
    {
      get { return Get<bool>(); }
      set { Set(value); }
    }

    public ViewType CurrentView
    {
      get { return Get<ViewType>(); }
      set { Set(value); }
    }

    #region Commands

    public void Execute_OnKeepCurrentView()
    {
      Aggregator.GetEvent<ToggleViewInjectionEvent>().Publish(false);
    }

    [DependsUpon("CanSwitchViews")]
    public bool CanExecute_OnKeepCurrentView()
    {
      return CanSwitchViews;
    }

    public void Execute_OnAllowViewsToSwitch()
    {
      Aggregator.GetEvent<ToggleViewInjectionEvent>().Publish(true);
    }

    [DependsUpon("CanSwitchViews")]
    public bool CanExecute_OnAllowViewsToSwitch()
    {
      return !CanSwitchViews;
    }

    public void Execute_OnShowUglyClick()
    {
      Aggregator.GetEvent<SwitchViewEvent>().Publish(ViewType.UglyView);
    }

    [DependsUpon("CanSwitchViews")]
    [DependsUpon("CurrentView")]
    public bool CanExecute_OnShowUglyClick()
    {
      return CurrentView != ViewType.UglyView && CanSwitchViews;
    }

    public void Execute_OnShowLessUglyClick()
    {
      Aggregator.GetEvent<SwitchViewEvent>().Publish(ViewType.PrettyView);
    }

    [DependsUpon("CanSwitchViews")]
    [DependsUpon("CurrentView")]
    public bool CanExecute_OnShowLessUglyClick()
    {
      return CurrentView != ViewType.PrettyView && CanSwitchViews;
    }

    #endregion
  }
}