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
using System.Diagnostics;
using System.Threading;
using System.Windows;
using System.Windows.Input;
using System.Windows.Threading;

#endregion

namespace MvvmTwitter.Utilities
{
  /// <summary>
  ///   A base class for command implementations.
  /// </summary>
  /// <remarks>Unscrupiously snatched from http://www.users.on.net/~kentcb/mvvm/Command.cs</remarks>
  public abstract class Command : ICommand
  {
    private readonly Dispatcher _dispatcher;


    /// <summary>
    ///   Constructs an instance of <c>CommandBase</c>.
    /// </summary>
    protected Command()
    {
      if (Application.Current != null)
      {
        _dispatcher = Application.Current.Dispatcher;
      }
      else
      {
        //this is useful for unit tests where there is no application running
        _dispatcher = Dispatcher.CurrentDispatcher;
      }

      Debug.Assert(_dispatcher != null);
    }

    /// <summary>
    ///   Occurs whenever the state of the application changes such that the result of a call to <see cref="CanExecute" /> may
    ///   return a different value.
    /// </summary>
    public event EventHandler CanExecuteChanged
    {
      add { CommandManager.RequerySuggested += value; }
      remove { CommandManager.RequerySuggested -= value; }
    }

    /// <summary>
    ///   Determines whether this command can execute.
    /// </summary>
    /// <param name="parameter">
    ///   The command parameter.
    /// </param>
    /// <returns>
    ///   <see langword="true" /> if the command can execute, otherwise <see langword="false" />.
    /// </returns>
    public abstract bool CanExecute(object parameter);

    /// <summary>
    ///   Executes this command.
    /// </summary>
    /// <param name="parameter">
    ///   The command parameter.
    /// </param>
    public abstract void Execute(object parameter);

    /// <summary>
    ///   Raises the <see cref="CanExecuteChanged" /> event.
    /// </summary>
    protected virtual void OnCanExecuteChanged()
    {
      if (!_dispatcher.CheckAccess())
      {
        _dispatcher.Invoke((ThreadStart) OnCanExecuteChanged, DispatcherPriority.Normal);
      }
      else
      {
        CommandManager.InvalidateRequerySuggested();
      }
    }
  }
}