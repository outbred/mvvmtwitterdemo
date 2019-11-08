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
using System.ComponentModel.Composition;
using System.Linq;
using System.Threading.Tasks;
using Infrastructure.Services.Interfaces;
using LinqToTwitter;
using MvvmTwitter.Interfaces;
using TweetsModule.Models;

#endregion

namespace TweetsModule.DataServices
{
    [Export(typeof(ITweetService))]
    internal class LinqToTwitterService : ITweetService
    {
        private readonly IMessageBoxService MessageBoxService;
        private static bool _gettingThemTweets = false;
        private static readonly object Locker = new object();
        private static readonly ApplicationOnlyAuthorizer _appAuth;

        static LinqToTwitterService()
        {
            var store = new InMemoryCredentialStore()
            {
                ConsumerKey = "8SBo35KV2hEXeOQaW6ZDaw",
                ConsumerSecret = "fhNwwxui5hd8yeJJb6ZGrsKyxHEPk3d2dk1dLnFo2hM",
            };
            _appAuth = new ApplicationOnlyAuthorizer()
            {
                CredentialStore = store
            };
            _appAuth.AuthorizeAsync();
        }

        [ImportingConstructor]
        public LinqToTwitterService(IMessageBoxService messageBoxService)
        {
            MessageBoxService = messageBoxService;
        }

        public Task GetMeSomeTweets<T>(Action<IEnumerable<T>> onComplete, string searchTerm = "Bulla") where T : class
        {
            return Task.Factory.StartNew(() =>
                                         {
                                             if (!_gettingThemTweets)
                                             {
                                                 lock (Locker)
                                                 {
                                                     _gettingThemTweets = true;
                                                 }
                                                 try
                                                 {
                                                     using (var context = new TwitterContext(_appAuth))
                                                     {
                                                         var searchResponse =
                                                     (from search in context.Search
                                                    where search.Type == SearchType.Search && search.Query == (searchTerm ?? "utahcodecamp")
                                                    select search).FirstOrDefault();

                                                         if (searchResponse != null && searchResponse.Statuses != null)
                                                         {
                                                             var results = (from status in searchResponse.Statuses
                                                                            let url = status.User != null
                                                                                  ? string.Format("http://twitter.com/{0}/statuses/{1}", status.User.ScreenName, status.StatusID)
                                                                                  : null
                                                                            select new LinqTweet()
                                                                            {
                                                                                Author = new Author()
                                                                                {
                                                                                    Name = status.User != null ? status.User.Name : (status.ScreenName ?? "unknown"),
                                                                                    Uri = status.User != null ? status.User.Url : null
                                                                                },
                                                                                Content = status.Text,
                                                                                Id = status.ID.ToString(),
                                                                                Image = status.User != null ? status.User.ProfileImageUrl : null,
                                                                                Link = url,
                                                                                Published = status.CreatedAt,
                                                                            }).ToList();
                                                             onComplete(results.Cast<T>());
                                                         }
                                                     }
                                                 }
                                                 finally
                                                 {
                                                     lock (Locker)
                                                     {
                                                         _gettingThemTweets = false;
                                                     }
                                                 }
                                             }
                                         });
        }

        public bool IsSupported<T>() where T : class
        {
            return typeof(T) == typeof(LinqTweet);
        }
    }
}