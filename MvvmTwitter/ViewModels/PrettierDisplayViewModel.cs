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
  public class PrettierDisplayViewModel : ViewModelBase, IPrettierDisplayViewModel
  {
    private readonly IMediatorService _mediator;
    private bool _allowSwitch = true;
    private readonly ITweetService _tweetSearchService;
    private readonly IUiDispatcherService _uiDispatcherService;


    public PrettierDisplayViewModel()
    {
      _tweetSearchService = TweetServiceLocator.GetDataService<LinqTweet>();
      _mediator = MediatorLocator.GetMediator();
      _uiDispatcherService = UiDispatcherLocator.GetDispatcher();

      _mediator.Subscribe(ViewModelEvents.SwitchViewBlocked, ignore =>
                                                             {
                                                               _allowSwitch = false;
                                                               OnSwitchToUgly.RaiseCanExecuteChanged("OnSwitchToUgly");
                                                             });

      _mediator.Subscribe(ViewModelEvents.SwitchViewUnblocked, ignore =>
                                                               {
                                                                 _allowSwitch = true;
                                                                 OnSwitchToUgly.RaiseCanExecuteChanged("OnSwitchToUgly");
                                                               });
    }

    private string _searchString = null;

    public string SearchString
    {
      get { return _searchString; }
      set
      {
        _uiDispatcherService.InvokeAsync(() =>
                                         {
                                           _searchString = value;
                                           NotifyPropertyChanged("SearchString");
                                         });
      }
    }

    private IEnumerable<LinqTweet> _tweets;

    public IEnumerable<LinqTweet> TweetSearchTweets
    {
      get { return _tweets; }
      private set
      {
        _uiDispatcherService.InvokeAsync(() =>
                                         {
                                           _tweets = value;
                                           NotifyPropertyChanged("TweetSearchTweets");
                                         });
      }
    }

    public DelegateCommand OnSwitchToUgly
    {
      get { return new DelegateCommand(ignore => { _mediator.NotifyColleaguesAsync(ViewModelEvents.SwitchView, ViewLocator.GetSharedInstance<IUglyDisplayView>()); }, ignore => _allowSwitch); }
    }

    public DelegateCommand OnSearchClick
    {
      get
      {
        return new DelegateCommand(ignore =>
                                   {
                                     if (_tweetSearchService != null && !string.IsNullOrEmpty(SearchString))
                                     {
                                       _tweetSearchService.GetMeSomeTweetsAsync<LinqTweet>(newTweets => TweetSearchTweets = newTweets, SearchString);
                                     }
                                   });
      }
    }
  }
}