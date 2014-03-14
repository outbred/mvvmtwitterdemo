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
using System.ComponentModel;
using System.Windows.Data;
using Infrastructure.Base;
using Infrastructure.Models;
using MvvmTwitter.Interfaces;
using TweetsModule.Models;

#endregion

namespace TweetsModule.ViewModels
{
  public abstract class DisplayViewModelBase : ViewModelBase, IDisplayViewModel
  {
    private readonly ITweetService _tweetSearchService;
    private static ObservableCollectionEx<LinqTweet> _tweets = null;

    public bool CanSwitchView
    {
      get { return Get<bool>(); }
      set { Set(value); }
    }

    public DisplayViewModelBase(ITweetService tweetService)
    {
      _tweetSearchService = tweetService;
      CanSwitchView = true;
      Searching = false;
      if (_tweets == null)
      {
        DispatcherService.InvokeIfRequired(() =>
                                           {
                                             _tweets = new ObservableCollectionEx<LinqTweet>();
                                             TweetSearchTweets = new ListCollectionView(_tweets);
                                           });
      }
    }

    public string SearchString
    {
      get { return StaticGet<string>(); }
      set { StaticSet(value); }
    }

    /// <summary>
    ///   If you want to handle more than one selection at a time, try this out:
    ///   http://grokys.blogspot.com/2010/07/mvvm-and-multiple-selection-part-i.html
    /// </summary>
    public ICollectionView TweetSearchTweets
    {
      get { return StaticGet<ICollectionView>(); }
      private set { StaticSet(value); }
    }

    protected abstract ViewType SwitchToView { get; }

    public void Execute_OnSwitch()
    {
      Aggregator.GetEvent<SwitchViewEvent>().Publish(SwitchToView);
    }

    [DependsUpon("CanSwitchView")]
    public bool CanExecute_OnSwitch()
    {
      return CanSwitchView;
    }

    public bool Searching
    {
      get { return StaticGet<bool>(); }
      set { StaticSet(value); }
    }

    public async void Execute_OnSearchClick()
    {
      if (Searching)
      {
        return;
      }

      Searching = true;
      DispatcherService.InvokeIfRequired(() => _tweets.Clear());
      if (_tweetSearchService != null && !string.IsNullOrEmpty(SearchString))
      {
        await _tweetSearchService.GetMeSomeTweets<LinqTweet>(newTweets =>
                                                             {
                                                               if (newTweets != null)
                                                               {
                                                                 DispatcherService.InvokeIfRequired(() => _tweets.AddRange(newTweets));
                                                               }
                                                               Searching = false;
                                                               if (OnSearchComplete != null)
                                                               {
                                                                 OnSearchComplete();
                                                               }
                                                             }, SearchString);
      }
    }

    protected virtual Action OnSearchComplete { get; set; }

    [DependsUpon("Searching")]
    [DependsUpon("SearchString")]
    public bool CanExecute_OnSearchClick()
    {
      return !Searching && !string.IsNullOrWhiteSpace(SearchString);
    }
  }
}