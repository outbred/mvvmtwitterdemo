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
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

#endregion

namespace MvvmTwitter.Utilities
{
  public static class Container
  {
    private static readonly ConcurrentDictionary<Type, object> AlreadyCreated = new ConcurrentDictionary<Type, object>();

    /// <summary>
    ///   By reflection, finds the first instance of a particular interface in this assembly, creates it and returns it
    /// </summary>
    /// <typeparam name="TInterface"></typeparam>
    /// <returns></returns>
    public static TInterface GetSharedInstance<TInterface>() where TInterface : class
    {
      var typeInQuestion = typeof (TInterface);
      if (AlreadyCreated.ContainsKey(typeInQuestion))
      {
        var list = AlreadyCreated[typeInQuestion] as List<TInterface>;
        if (list != null && list.Count > 0)
        {
          return list[0];
        }
      }
      else
      {
        var instance = (from t in Assembly.GetExecutingAssembly().GetTypes()
                        //Activator.CreateInstance demands an empty ctor
                        where t.GetInterfaces().Contains(typeInQuestion) && t.GetConstructor(Type.EmptyTypes) != null
                        select Activator.CreateInstance(t) as TInterface).ToList();
        AlreadyCreated.GetOrAdd(typeInQuestion, instance);
        return instance.Count > 0 ? instance[0] : null;
      }
      return null;
    }

    /// <summary>
    ///   By reflection, finds all instances of a particular interface in this assembly, creates and returns them
    /// </summary>
    /// <typeparam name="TInterface"></typeparam>
    /// <returns></returns>
    public static List<TInterface> GetSharedInstances<TInterface>() where TInterface : class
    {
      var typeInQuestion = typeof (TInterface);
      if (AlreadyCreated.ContainsKey(typeInQuestion))
      {
        return AlreadyCreated[typeInQuestion] as List<TInterface>;
      }

      var instances = (from t in Assembly.GetExecutingAssembly().GetTypes()
                       //Activator.CreateInstance demands an empty ctor
                       where t.GetInterfaces().Contains(typeof (TInterface)) && t.GetConstructor(Type.EmptyTypes) != null
                       select Activator.CreateInstance(t) as TInterface).ToList();

      AlreadyCreated.GetOrAdd(typeInQuestion, instances);

      return instances;
    }
  }
}