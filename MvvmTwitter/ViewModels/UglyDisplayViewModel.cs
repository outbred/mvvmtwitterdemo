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
using MvvmTwitter.Interfaces;
using MvvmTwitter.Models;
using MvvmTwitter.Utilities;

#endregion

namespace MvvmTwitter.ViewModels
{
  public class UglyDisplayViewModel : ViewModelBase, IUglyDisplayViewModel
  {
    private readonly IMediatorService _mediator = null;
    private readonly ITweetService _twitterizerService = null;
    private readonly IUiDispatcherService _uiDispatcherService = null;

    public UglyDisplayViewModel()
    {
      _twitterizerService = TweetServiceLocator.GetDataService<LinqTweet>();
      _uiDispatcherService = UiDispatcherLocator.GetDispatcher();
      _mediator = MediatorLocator.GetMediator();
      _mediator.Subscribe(ViewModelEvents.SwitchViewBlocked, ignore =>
                                                             {
                                                               _allowSwitch = false;
                                                               OnSwitchToPretty.RaiseCanExecuteChanged("OnSwitchToPretty");
                                                             });

      _mediator.Subscribe(ViewModelEvents.SwitchViewUnblocked, ignore =>
                                                               {
                                                                 _allowSwitch = true;
                                                                 OnSwitchToPretty.RaiseCanExecuteChanged("OnSwitchToPretty");
                                                               });
    }

    private bool _allowSwitch = true;

    public DelegateCommand OnSwitchToPretty
    {
      get { return new DelegateCommand(ignore => { _mediator.NotifyColleaguesAsync(ViewModelEvents.SwitchView, ViewLocator.GetSharedInstance<IPrettierDisplayView>()); }, ignore => _allowSwitch); }
    }

    private IEnumerable<LinqTweet> _twitterizerTweets;

    public IEnumerable<LinqTweet> TwitterizerTweets
    {
      get { return _twitterizerTweets; }
      private set
      {
        _uiDispatcherService.InvokeAsync(() =>
                                         {
                                           _twitterizerTweets = value;
                                           NotifyPropertyChanged("TwitterizerTweets");
                                         });
      }
    }

    private bool _gettingTweets = false;

    public DelegateCommand OnHitMeClick
    {
      get
      {
        return new DelegateCommand(ignore =>
                                   {
                                     if (_twitterizerService != null)
                                     {
                                       _gettingTweets = true;
                                       OnHitMeClick.RaiseCanExecuteChanged(null);
                                       _twitterizerService.GetMeSomeTweetsAsync<LinqTweet>(newTweets =>
                                                                                           {
                                                                                             TwitterizerTweets = newTweets;
                                                                                             _gettingTweets = false;
                                                                                             OnHitMeClick.RaiseCanExecuteChanged(null);
                                                                                           }, null);
                                     }
                                   }, ignore => !_gettingTweets);
      }
    }
  }
}