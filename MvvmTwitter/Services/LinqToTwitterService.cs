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
using System.Linq;
using LinqToTwitter;
using MvvmTwitter.Interfaces;
using MvvmTwitter.Models;
using MvvmTwitter.Utilities;

#endregion

namespace MvvmTwitter.Services
{
  internal class LinqToTwitterService : ITweetService
  {
    private static readonly IMessageBoxService MessageBoxService;
    private static bool _gettingThemTweets = false;
    private static readonly object Locker = new object();
    private static readonly ApplicationOnlyAuthorizer _pinAuth;

    static LinqToTwitterService()
    {
      MessageBoxService = Container.GetSharedInstance<IMessageBoxService>();
      var store = new InMemoryCredentials()
                  {
                    ConsumerKey = "8SBo35KV2hEXeOQaW6ZDaw",
                    ConsumerSecret = "fhNwwxui5hd8yeJJb6ZGrsKyxHEPk3d2dk1dLnFo2hM",
                  };
      _pinAuth = new ApplicationOnlyAuthorizer()
                 {
                   Credentials = store
                 };
      _pinAuth.Authorize();
    }

    public void GetMeSomeTweetsAsync<T>(Action<IEnumerable<T>> onComplete, string searchTerm = "Bulla") where T : class
    {
      if (!_gettingThemTweets)
      {
        lock (Locker)
        {
          _gettingThemTweets = true;
        }
        using (var context = new TwitterContext(_pinAuth))
        {
          var searchResponse =
            (from search in context.Search
             where search.Type == SearchType.Search && search.Query == (searchTerm ?? "utahcodecamp")
             select search).FirstOrDefault();

          if (searchResponse != null && searchResponse.Statuses != null)
          {
            var results = (from status in searchResponse.Statuses
                           let url = status.User != null && status.User.Identifier != null
                                       ? string.Format("http://twitter.com/{0}/statuses/{1}", status.User.Identifier.ScreenName, status.StatusID)
                                       : null
                           select new LinqTweet()
                                  {
                                    Author = new Author()
                                             {
                                               Name = status.User != null ? status.User.Name : (status.ScreenName ?? "unknown"),
                                               Uri = status.User != null ? status.User.Url : null
                                             },
                                    Content = status.Text,
                                    Id = status.StatusID,
                                    Image = status.User != null ? status.User.ProfileImageUrl : null,
                                    Link = url,
                                    Published = status.CreatedAt,
                                  }).ToList();
            onComplete(results.Cast<T>());
          }
        }

        lock (Locker)
        {
          _gettingThemTweets = false;
        }
      }
    }

    public bool IsSupported<T>() where T : class
    {
      return typeof (T) == typeof (LinqTweet);
    }
  }
}