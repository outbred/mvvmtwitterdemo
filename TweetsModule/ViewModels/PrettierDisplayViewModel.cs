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
using System.Diagnostics;
using Infrastructure.Base;
using Infrastructure.Helpers;
using Infrastructure.Models;
using MEFedMVVM.ViewModelLocator;
using MvvmTwitter.Interfaces;
using TweetsModule.Models;

#endregion

namespace TweetsModule.ViewModels
{
  [Export(typeof (IDisplayViewModel))] // MEF
  [ExportViewModel("PrettierDisplayViewModel")] // MEFedMvvm
  public class PrettierDisplayViewModel : DisplayViewModelBase
  {
    protected override ViewType SwitchToView
    {
      get { return ViewType.UglyView; }
    }

    private bool _isLoaded = false;

    [ImportingConstructor]
    public PrettierDisplayViewModel(ITweetService tweetService, IViewAwareStatus viewAwareStatus)
      : base(tweetService)
    {
      viewAwareStatus.ViewLoaded += () =>
                                    {
                                      MessageBoxService.ShowInformation("You loaded the pretty view!");
                                      _isLoaded = true;
                                      // use common if you want to loosely couple from the logger type
                                      Logger.Default.Debug("Pretty loaded");
                                    };
      viewAwareStatus.ViewUnloaded += () => _isLoaded = false;

      TweetSearchTweets.CurrentChanged += (s, e) =>
                                          {
                                            if (!_isLoaded)
                                            {
                                              return;
                                            }

                                            var current = TweetSearchTweets.CurrentItem as LinqTweet;
                                            if (current != null)
                                            {
                                              Process.Start(current.Link);
                                            }
                                          };
    }
  }
}