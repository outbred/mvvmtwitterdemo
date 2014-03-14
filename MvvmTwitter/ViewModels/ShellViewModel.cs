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

using MvvmTwitter.Interfaces;
using MvvmTwitter.Utilities;

#endregion

namespace MvvmTwitter.ViewModels
{
  public class ShellViewModel : ViewModelBase, IShellViewModel
  {
    #region Private fields/props

    private static readonly IMediatorService Mediator = null;
    private static readonly IUiDispatcherService UiDispatcherService = null;
    private MediatorToken _switchViewToken;

    private MediatorToken SwitchViewToken
    {
      get { return _switchViewToken; }
      set
      {
        _switchViewToken = value;
        RaiseCanExecuteOnCommands();
      }
    }

    #endregion

    #region ctor

    static ShellViewModel()
    {
      Mediator = MediatorLocator.GetMediator();
      UiDispatcherService = UiDispatcherLocator.GetDispatcher();
    }

    public ShellViewModel()
    {
      SubscribeToSwitchViewEvent();
      CurrentView = ViewLocator.GetSharedInstance<IUglyDisplayView>();
    }

    #endregion

    #region Bindable Properties

    private object _currentView;

    public object CurrentView
    {
      get { return _currentView; }
      private set
      {
        UiDispatcherService.InvokeAsync(() =>
                                        {
                                          _currentView = value;
                                          NotifyPropertyChanged("CurrentView");
                                        });
      }
    }

    #region Commands

    public DelegateCommand OnKeepCurrentView
    {
      get
      {
        return new DelegateCommand(ignore =>
                                   {
                                     Mediator.Unsubscribe(ViewModelEvents.SwitchView, SwitchViewToken);
                                     SwitchViewToken = null;
                                     Mediator.NotifyColleaguesAsync(ViewModelEvents.SwitchViewBlocked, null);
                                   }, ignore => SwitchViewToken != null);
      }
    }

    public DelegateCommand OnAllowViewsToSwitch
    {
      get
      {
        return new DelegateCommand(ignore =>
                                   {
                                     SubscribeToSwitchViewEvent();
                                     Mediator.NotifyColleaguesAsync(ViewModelEvents.SwitchViewUnblocked, null);
                                   }, (ignore) => SwitchViewToken == null);
      }
    }

    public DelegateCommand OnShowUglyClick
    {
      get { return new DelegateCommand(ignore => { Mediator.NotifyColleaguesAsync(ViewModelEvents.SwitchView, ViewLocator.GetSharedInstance<IUglyDisplayView>()); }, ignore => SwitchViewToken != null); }
    }

    public DelegateCommand OnShowLessUglyClick
    {
      get { return new DelegateCommand(ignore => { Mediator.NotifyColleaguesAsync(ViewModelEvents.SwitchView, ViewLocator.GetSharedInstance<IPrettierDisplayView>()); }, ignore => SwitchViewToken != null); }
    }

    #endregion

    #endregion

    #region Private Helpers

    private void RaiseCanExecuteOnCommands()
    {
      UiDispatcherService.InvokeAsync(() =>
                                      {
                                        OnAllowViewsToSwitch.RaiseCanExecuteChanged(null);
                                        OnKeepCurrentView.RaiseCanExecuteChanged(null);
                                        OnShowUglyClick.RaiseCanExecuteChanged(null);
                                        OnShowLessUglyClick.RaiseCanExecuteChanged(null);
                                      });
    }

    private void SubscribeToSwitchViewEvent()
    {
      if (Mediator != null)
      {
        SwitchViewToken = Mediator.Subscribe(ViewModelEvents.SwitchView, (view) => CurrentView = view);
      }
    }

    #endregion
  }
}