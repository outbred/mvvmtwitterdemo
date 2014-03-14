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
using System.Diagnostics.Contracts;

#endregion

namespace MvvmTwitter.Utilities
{
  /// <summary>
  ///   A command that executes delegates to determine whether the command can execute, and to execute the command.
  /// </summary>
  /// <remarks>
  ///   <para>
  ///     This command implementation is useful when the command simply needs to execute a method on a view model. The
  ///     delegate for
  ///     determining whether the command can execute is optional. If it is not provided, the command is considered always
  ///     eligible
  ///     to execute.
  ///     Unscrupiously snatched from http://www.users.on.net/~kentcb/mvvm/Command.cs
  /// </remarks>
  /// </para>
  /// </remarks>
  public class DelegateCommand : Command
  {
    private readonly Predicate<object> _canExecute;
    private readonly Action<object> _execute;

    /// <summary>
    ///   Constructs an instance of <c>DelegateCommand</c>.
    /// </summary>
    /// <remarks>
    ///   This constructor creates the command without a delegate for determining whether the command can execute. Therefore,
    ///   the
    ///   command will always be eligible for execution.
    /// </remarks>
    /// <param name="execute">
    ///   The delegate to invoke when the command is executed.
    /// </param>
    public DelegateCommand(Action<object> execute)
      : this(execute, null)
    {}

    /// <summary>
    ///   Constructs an instance of <c>DelegateCommand</c>.
    /// </summary>
    /// <param name="execute">
    ///   The delegate to invoke when the command is executed.
    /// </param>
    /// <param name="canExecute">
    ///   The delegate to invoke to determine whether the command can execute.
    /// </param>
    public DelegateCommand(Action<object> execute, Predicate<object> canExecute)
    {
      Contract.Requires(execute != null);
      _execute = execute;
      _canExecute = canExecute;
    }

    /// <summary>
    ///   Determines whether this command can execute.
    /// </summary>
    /// <remarks>
    ///   If there is no delegate to determine whether the command can execute, this method will return <see langword="true" />
    ///   . If a delegate was provided, this
    ///   method will invoke that delegate.
    /// </remarks>
    /// <param name="parameter">
    ///   The command parameter.
    /// </param>
    /// <returns>
    ///   <see langword="true" /> if the command can execute, otherwise <see langword="false" />.
    /// </returns>
    public override bool CanExecute(object parameter)
    {
      if (_canExecute == null)
      {
        return true;
      }

      return _canExecute(parameter);
    }

    /// <summary>
    ///   Executes this command.
    /// </summary>
    /// <remarks>
    ///   This method invokes the provided delegate to execute the command.
    /// </remarks>
    /// <param name="parameter">
    ///   The command parameter.
    /// </param>
    public override void Execute(object parameter)
    {
      _execute(parameter);
    }

    /// <summary>
    ///   Informs the view to re-evaluate whether a command can be executed or not
    /// </summary>
    /// <param name="parameter"></param>
    public void RaiseCanExecuteChanged(object parameter)
    {
      base.OnCanExecuteChanged();
    }
  }
}