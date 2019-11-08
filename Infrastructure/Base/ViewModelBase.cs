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
using System.ComponentModel;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Dynamic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Runtime.Serialization;
using System.Windows;
using CommonServiceLocator;
using Infrastructure.Base;
using Infrastructure.Helpers;
using Infrastructure.Services;
using Infrastructure.Services.Interfaces;
using Microsoft.Practices.Prism.Commands;
using Microsoft.Practices.Prism.Events;
using Microsoft.Practices.Prism.Regions;

#endregion

namespace Infrastructure.Base
{
  [DataContract]
  public abstract class ViewModelBase : DynamicObject, INotifyPropertyChanged
  {
    // not a fool proof way to limit prop change events for static props, but a good start
    // exception is that if you have two inheriting super classes with the same static prop name, but are actually different types, they will
    // all have prop change event notifications, and all are linked together so the value will be corrupted
    private static readonly Dictionary<Type, List<string>> _staticPropMap = new Dictionary<Type, List<string>>();
    // not thread-safe, obviously...so be aware of that (shouldn't be an issue since all VM's should be created on the UI thread)
    private static readonly Dictionary<Type, List<ViewModelBase>> _typeToInstancesLookup = new Dictionary<Type, List<ViewModelBase>>();

    protected ViewModelBase()
    {
      var type = GetType();
      var methods = type.GetMethods(BindingFlags.Instance | BindingFlags.Public);
      var props = type.GetProperties(BindingFlags.Instance | BindingFlags.Public);
      if (!_staticPropMap.ContainsKey(type))
      {
        _staticPropMap.Add(type, props.Select(m => m.Name).ToList());
        _typeToInstancesLookup.Add(type, new List<ViewModelBase>() {this});
      }
      else
      {
        _typeToInstancesLookup[type].Add(this);
      }
      _propertyMap = MapDependencies<DependsUponAttribute>(() => props);
      _methodMap = MapDependencies<DependsUponAttribute>(() => methods.Where(method => !method.Name.StartsWith(CAN_EXECUTE_PREFIX)));
      _commandMap = MapDependencies<DependsUponAttribute>(() => { return methods.Where(method => method.Name.StartsWith(CAN_EXECUTE_PREFIX)); });
      CreateCommands();
      VerifyDependancies();
      _instances.Add(this);
    }

    private static Dictionary<string, object> _staticValues = new Dictionary<string, object>();
    private Dictionary<string, object> _values = new Dictionary<string, object>();
    private readonly IDictionary<string, List<string>> _propertyMap;
    private readonly IDictionary<string, List<string>> _methodMap;
    private readonly IDictionary<string, List<string>> _commandMap;
    private static readonly List<ViewModelBase> _instances = new List<ViewModelBase>();

    [AttributeUsage(AttributeTargets.Property | AttributeTargets.Method, AllowMultiple = true)]
    protected class DependsUponAttribute : Attribute
    {
      public string DependencyName { get; private set; }

      public bool VerifyStaticExistence { get; set; }

      public DependsUponAttribute(string propertyName)
      {
        DependencyName = propertyName;
      }
    }

    /// <summary>
    ///   Gets a value indicating whether the control is in design mode
    ///   (running in Blend or Visual Studio).
    /// </summary>
    [SuppressMessage(
      "Microsoft.Security",
      "CA2122:DoNotIndirectlyExposeMethodsWithLinkDemands",
      Justification = "The security risk here is neglectible.")]
    public static bool IsInDesignModeStatic
    {
      get
      {
        if (!_isInDesignMode.HasValue)
        {
#if SILVERLIGHT
                    _isInDesignMode = DesignerProperties.IsInDesignTool;
#else
          var prop = DesignerProperties.IsInDesignModeProperty;
          _isInDesignMode
            = (bool) DependencyPropertyDescriptor
                       .FromProperty(prop, typeof (FrameworkElement))
                       .Metadata.DefaultValue;

          // Just to be sure
          if (!_isInDesignMode.Value
              && Process.GetCurrentProcess().ProcessName.StartsWith("devenv", StringComparison.Ordinal))
          {
            _isInDesignMode = true;
          }
#endif
        }

        return _isInDesignMode.Value;
      }
    }

    private static IDispatcherService _dispatcherService;
    private static IEventAggregator _aggregator;
    private static IRegionManager _regionManager;
    private static IServiceLocator _container;
    private static readonly object _locker = new object();

    public static IRegionManager RegionManager
    {
      get
      {
        if (_regionManager == null)
        {
          _regionManager = new RegionManager();
        }
        return _regionManager;
      }
      set { _regionManager = value; }
    }

    /// <summary>
    ///   Access to other objects/services that you may need to inject at runtime
    /// </summary>
    public static IServiceLocator Container
    {
      get { return _container; }
      set { _container = value; }
    }

    public static IEventAggregator Aggregator
    {
      get
      {
        // cover unit testing only!
        if (_aggregator == null)
        {
          _aggregator = new EventAggregator();
        }
        return _aggregator;
      }
      set { _aggregator = value; }
    }

    private static IMessageBoxService _messageBoxService;

    public static IMessageBoxService MessageBoxService
    {
      get
      {
        // cover unit testing only!
        if (_messageBoxService == null)
        {
          _messageBoxService = new TestMessageBoxService();
        }
        return _messageBoxService;
      }
      set { _messageBoxService = value; }
    }

    /// <summary>
    ///   Should be set somewhere near initialization
    /// </summary>
    public static IDispatcherService DispatcherService
    {
      get
      {
        if (_dispatcherService == null)
        {
          _dispatcherService = new NoUiThreadDispatcherService();
        }
        return _dispatcherService;
      }
      set { _dispatcherService = value; }
    }

    private static IFolderBrowserService _folderBrowserService;

    public static IFolderBrowserService FolderBrowserService
    {
      get
      {
        if (_folderBrowserService == null)
        {
          _folderBrowserService = new FolderBrowserService();
        }
        return _folderBrowserService;
      }
      set { _folderBrowserService = value; }
    }

    private static bool? _isInDesignMode;
    private IEnumerable<string> _commandNames = null;

    public bool IsInDesignMode
    {
      get { return IsInDesignModeStatic; }
    }

    protected T Get<T>([CallerMemberName] string name = null)
    {
      return Get(name, default(T));
    }

    /// <summary>
    ///   For static properties accessed in an instance context (for binding, etc.), use this Get'er and the StaticSet'er
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="name"></param>
    /// <returns></returns>
    protected static T StaticGet<T>([CallerMemberName] string name = null)
    {
      return StaticGet(name, default(T));
    }

    protected T Get<T>(string name, T defaultValue)
    {
      // weird unit test check
      if (_values == null)
      {
        _values = new Dictionary<string, object>();
      }

      if (_values.ContainsKey(name))
      {
        return (T) _values[name];
      }

      return defaultValue;
    }

    protected static T StaticGet<T>(string name, T defaultValue)
    {
      // weird unit test check
      if (_staticValues == null)
      {
        _staticValues = new Dictionary<string, object>();
      }

      if (_staticValues.ContainsKey(name))
      {
        return (T) _staticValues[name];
      }

      return defaultValue;
    }

    protected T Get<T>(string name, Func<T> initialValue)
    {
      // weird unit test check
      if (_values == null)
      {
        _values = new Dictionary<string, object>();
      }

      if (_values.ContainsKey(name))
      {
        return (T) _values[name];
      }

      Set(initialValue(), name);
      return Get<T>(name);
    }

    /// <summary>
    ///   Set the value of a property
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="name"></param>
    /// <param name="value"></param>
    /// <returns>True if changed; false if not</returns>
    public bool Set<T>(T value, [CallerMemberName] string name = null)
    {
      // weird unit test check
      if (_values == null)
      {
        _values = new Dictionary<string, object>();
      }

      if (_values.ContainsKey(name))
      {
        if (_values[name] == null && value == null)
        {
          return false;
        }

        if (_values[name] != null && _values[name].Equals(value))
        {
          return false;
        }

        _values[name] = value;
      }
      else
      {
        _values.Add(name, value);
      }

      RaisePropertyChanged(name);

      return true;
    }

    /// <summary>
    ///   For static properties accessed in an instance context (for binding, etc.), use this Get'er and the StaticSet'er
    ///   This will fire property changed events for the affected properties on all instances of this class
    /// 
    ///  ** Take care when using this b/c Type's are not enforced for static prop's with the same name! **
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="name"></param>
    /// <param name="value"></param>
    /// <returns>True if changed; false if not</returns>
    public static bool StaticSet<T>(T value, [CallerMemberName] string name = null)
    {
      // weird unit test check
      if (_staticValues == null)
      {
        _staticValues = new Dictionary<string, object>();
      }

      if (_staticValues.ContainsKey(name))
      {
        if (_staticValues[name] == null && value == null)
        {
          return false;
        }

        if (_staticValues[name] != null && _staticValues[name].Equals(value))
        {
          return false;
        }

        _staticValues[name] = value;
      }
      else
      {
        _staticValues.Add(name, value);
      }

      StaticRaisePropertyChanged(name);

      return true;
    }

    protected void RaisePropertyChanged(string name)
    {
      _dispatcherService.BeginInvoke(() =>
                                     {
                                       PropertyChanged.Raise(this, name);
#if SILVERLIGHT
				PropertyChanged.Raise(this, "");
#endif

                                       // unit test friendly - bbulla
                                       if (_propertyMap != null && _propertyMap.ContainsKey(name))
                                       {
                                         _propertyMap[name].Each(this.RaisePropertyChanged);
                                       }

                                       ExecuteDependentMethods(name);
                                       FireChangesOnDependentCommands(name);
                                     });
    }

    private static void StaticRaisePropertyChanged(string propName)
    {
      var vmGroupMatches = (from vm in _staticPropMap.Keys
                            where _staticPropMap[vm].Contains(propName)
                            select _typeToInstancesLookup[vm]).ToList();

      vmGroupMatches.ForEach(vmGroup => vmGroup.ForEach(vm => vm.RaisePropertyChanged(propName)));
    }

    /// <summary>
    ///   Raises the PropertyChanged event if needed, and broadcasts a
    ///   PropertyChangedMessage using the Messenger instance (or the
    ///   static default instance if no Messenger instance is available).
    /// </summary>
    /// <typeparam name="T">
    ///   The type of the property that
    ///   changed.
    /// </typeparam>
    /// <param name="i_propertyExpression">
    ///   An expression identifying the property
    ///   that changed.
    /// </param>
    [SuppressMessage("Microsoft.Design", "CA1030:UseEventsWhereAppropriate",
      Justification = "This cannot be an event")]
    [SuppressMessage("Microsoft.Design", "CA1006:GenericMethodsShouldProvideTypeParameter",
      Justification = "This syntax is more convenient than other alternatives.")]
    protected virtual void RaisePropertyChanged<T>(Expression<Func<T>> i_propertyExpression)
    {
      if (IsInDesignMode)
      {
        return;
      }

      if (i_propertyExpression == null)
      {
        return;
      }

      var body = PropertyName(i_propertyExpression);
      if (body != null)
      {
        RaisePropertyChanged(body);
      }
    }

    private void ExecuteDependentMethods(string name)
    {
      // unit test friendly - bbulla
      if (_methodMap != null && _methodMap.ContainsKey(name))
      {
        _methodMap[name].Each(ExecuteMethod);
      }
    }

    private void FireChangesOnDependentCommands(string i_name)
    {
      // unit test friendly - bbulla
      if (_commandMap != null && _commandMap.ContainsKey(i_name))
      {
        _commandMap[i_name].Each(RaiseCanExecuteChangedEvent);
      }
    }

    private static string PropertyName<T>(Expression<Func<T>> i_expression)
    {
      if (i_expression == null)
      {
        return null;
      }

      var memberExpression = i_expression.Body as MemberExpression;

      if (memberExpression == null)
      {
        throw new ArgumentException("expression must be a property expression");
      }

      return memberExpression.Member.Name;
    }

    public override bool TryGetMember(GetMemberBinder i_binder, out object i_result)
    {
      i_result = Get<object>(i_binder.Name);

      if (i_result != null)
      {
        return true;
      }

      return base.TryGetMember(i_binder, out i_result);
    }

    public override bool TrySetMember(SetMemberBinder binder, object value)
    {
      var result = base.TrySetMember(binder, value);
      if (result)
      {
        return true;
      }

      Set(value, binder.Name);
      return true;
    }

    public event PropertyChangedEventHandler PropertyChanged = delegate { };

    private void CreateCommands()
    {
      if (!IsInDesignModeStatic)
      {
        CommandNames.Each(name => Set(new DelegateCommand<object>(x => ExecuteCommand(name, x), x => CanExecuteCommand(name, x)), name));
      }
    }

    private const string EXECUTE_PREFIX = "Execute_";
    private const string CAN_EXECUTE_PREFIX = "CanExecute_";

    private IEnumerable<string> CommandNames
    {
      get
      {
        if (_commandNames == null)
        {
          _commandNames = from method in GetType().GetMethods(BindingFlags.Public | BindingFlags.Instance)
                          where method.Name.StartsWith(EXECUTE_PREFIX)
                          select method.Name.StripLeft(EXECUTE_PREFIX.Length);
        }
        return _commandNames;
      }
    }

    private void ExecuteCommand(string name, object parameter)
    {
      var methodInfo = GetType().GetMethod(EXECUTE_PREFIX + name);
      if (methodInfo == null)
      {
        return;
      }

      methodInfo.Invoke(this, methodInfo.GetParameters().Length == 1 ? new[] {parameter} : null);
    }

    private bool CanExecuteCommand(string i_name, object i_parameter)
    {
      var methodInfo = GetType().GetMethod(CAN_EXECUTE_PREFIX + i_name);
      if (methodInfo == null)
      {
        return true;
      }

      return (bool) methodInfo.Invoke(this, methodInfo.GetParameters().Length == 1 ? new[] {i_parameter} : null);
    }

    protected void RaiseCanExecuteChangedEvent(string canExecute_name)
    {
      var commandName = canExecute_name.Contains(CAN_EXECUTE_PREFIX) ? canExecute_name.StripLeft(CAN_EXECUTE_PREFIX.Length) : canExecute_name;
      var command = Get<DelegateCommand<object>>(commandName);
      if (command == null)
      {
        return;
      }

      command.RaiseCanExecuteChanged();
    }

#if SILVERLIGHT
        public object this[string key]
        {
            get { return Get<object>(key);}
            set { Set(key, value); }
        }
#endif

    private static IDictionary<string, List<string>> MapDependencies<T>(Func<IEnumerable<MemberInfo>> getInfo) where T : DependsUponAttribute
    {
      var dependencyMap = getInfo().ToDictionary(
                                                 p => p.Name,
                                                 p => p.GetCustomAttributes(typeof (T), true)
                                                       .Cast<T>()
                                                       .Select(a => a.DependencyName)
                                                       .ToList());

      return Invert(dependencyMap);
    }

    private static IDictionary<string, List<string>> Invert(IDictionary<string, List<string>> map)
    {
      var flattened = from key in map.Keys
                      from value in map[key]
                      select new {Key = key, Value = value};

      var uniqueValues = flattened.Select(x => x.Value).Distinct();

      return uniqueValues.ToDictionary(
                                       x => x,
                                       x => (from item in flattened
                                             where item.Value == x
                                             select item.Key).ToList());
    }

    private void ExecuteMethod(string i_name)
    {
      var memberInfo = GetType().GetMethod(i_name);
      if (memberInfo == null)
      {
        return;
      }

      memberInfo.Invoke(this, null);
    }

    private void VerifyDependancies()
    {
      var methods = GetType().GetMethods(BindingFlags.Instance | BindingFlags.Public).Cast<MemberInfo>();
      var properties = GetType().GetProperties(BindingFlags.Instance | BindingFlags.Public);

      var propertyNames = methods.Union(properties)
                                 .SelectMany(i_method => i_method.GetCustomAttributes(typeof (DependsUponAttribute), true).Cast<DependsUponAttribute>())
                                 .Where(i_attribute => i_attribute.VerifyStaticExistence)
                                 .Select(i_attribute => i_attribute.DependencyName);

      propertyNames.Each(VerifyDependancy);
    }

    private void VerifyDependancy(string i_propertyName)
    {
      var property = GetType().GetProperty(i_propertyName);
      if (property == null)
      {
        throw new ArgumentException("DependsUpon Property Does Not Exist: " + i_propertyName);
      }
    }

    /// <summary>
    ///   Raises the PropertyChanged event if needed.
    /// </summary>
    /// <remarks>
    ///   If the propertyName parameter
    ///   does not correspond to an existing property on the current class, an
    ///   exception is thrown in DEBUG configuration only.
    /// </remarks>
    /// <param name="i_PropertyName">
    ///   The name of the property that
    ///   changed.
    /// </param>
    [SuppressMessage("Microsoft.Design", "CA1030:UseEventsWhereAppropriate", Justification = "This cannot be an event")]
    protected virtual void Raise(string i_PropertyName)
    {
      var handler = PropertyChanged;

      if (handler != null)
      {
        handler(this, new PropertyChangedEventArgs(i_PropertyName));
      }
    }

    /// <summary>
    ///   If this ViewModel's View houses other injectable View's, this method will be useful
    /// </summary>
    /// <param name="type"></param>
    /// <param name="region"></param>
    /// <returns></returns>
    /// <remarks>Only call this for a nested control once the parent has been loaded</remarks>
    protected virtual TView RegisterAndActivateView<TView>(string region = null) where TView : class
    {
      if (!string.IsNullOrWhiteSpace(region))
      {
        Logger.Default.Debug("Activating view {0}", typeof (TView).Name);

        TView view = null;
        DispatcherService.InvokeIfRequired(() => SwitchView(region, out view));
        return view;
      }
      return null;
    }

    private void SwitchView<TView>(string region, out TView view) where TView : class
    {
      lock (_locker)
      {
        try
        {
          _regionManager.Regions[region].ActiveViews.ToList().ForEach(v => _regionManager.Regions[region].Remove(v));
          // Add it, then activate
          view = _container.GetInstance<TView>();
          _regionManager.AddToRegion(region, view);
          _regionManager.Regions[region].Activate(view);
        }
        catch (Exception ex)
        {
          Logger.Default.Error("Unable to activate view.", ex);
          view = null;
        }
      }
    }
  }
}

public static class Extensions
{
  public static void Each<T>(this IEnumerable<T> i_items, Action<T> i_action)
  {
    foreach (var item in i_items)
    {
      i_action(item);
    }
  }

  public static string StripLeft(this string i_value, int i_length)
  {
    return i_value.Substring(i_length, i_value.Length - i_length);
  }

  public static void Raise(this PropertyChangedEventHandler i_eventHandler, object i_source, string i_propertyName, bool onceThroughAlready = false)
  {
    var handlers = i_eventHandler;
    if (handlers != null)
    {
      try
      {
        handlers(i_source, new PropertyChangedEventArgs(i_propertyName));
      }
      catch
      {
        if (!onceThroughAlready)
        {
          ViewModelBase.DispatcherService.WaitForPriority();
          Raise(i_eventHandler, i_source, i_propertyName, true);
        }
      }
    }
  }
}