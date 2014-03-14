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

#endregion

namespace Infrastructure.Services.Interfaces
{
  /// <summary>
  ///   Available Button options.
  ///   Abstracted to allow some level of UI Agnosticness
  /// </summary>
  public enum CustomDialogButtons
  {
    OK,
    OKCancel,
    YesNo,
    YesNoCancel
  }

  /// <summary>
  ///   Available Icon options.
  ///   Abstracted to allow some level of UI Agnosticness
  /// </summary>
  public enum CustomDialogIcons
  {
    None,
    Information,
    Question,
    Exclamation,
    Stop,
    Warning
  }

  /// <summary>
  ///   Available DialogResults options.
  ///   Abstracted to allow some level of UI Agnosticness
  /// </summary>
  public enum CustomDialogResults
  {
    None,
    OK,
    Cancel,
    Yes,
    No
  }

  /// <summary>
  ///   This interface defines a interface that will allow
  ///   a ViewModel to show a messagebox
  /// </summary>
  public abstract class IMessageBoxService
  {
    /// <summary>
    ///   Shows an error message
    /// </summary>
    /// <param name="caption">Caption for the messagebox</param>
    /// <param name="message">The error message</param>
    public abstract void ShowError(string message, string caption = null);

    /// <summary>
    ///   Shows an information message
    /// </summary>
    /// <param name="caption">Caption for the messagebox</param>
    /// <param name="message">The information message</param>
    public abstract void ShowInformation(string message, string caption = null);

    /// <summary>
    ///   Shows an warning message
    /// </summary>
    /// <param name="caption">Caption for the messagebox</param>
    /// <param name="message">The warning message</param>
    public abstract void ShowWarning(string message, string caption = null);

    /// <summary>
    ///   Displays a Yes/No dialog and returns the user input.
    /// </summary>
    /// <param name="message">The message to be displayed.</param>
    /// <param name="icon">The icon to be displayed.</param>
    /// <param name="caption">Caption for the messagebox</param>
    /// <returns>User selection.</returns>
    public abstract CustomDialogResults ShowYesNo(string message, CustomDialogIcons icon, string caption = null);

    /// <summary>
    ///   Displays a Yes/No/Cancel dialog and returns the user input.
    /// </summary>
    /// <param name="message">The message to be displayed.</param>
    /// <param name="icon">The icon to be displayed.</param>
    /// <param name="caption">Caption for the messagebox</param>
    /// <returns>User selection.</returns>
    public abstract CustomDialogResults ShowYesNoCancel(string message, CustomDialogIcons icon, string caption = null);

    /// <summary>
    ///   Displays a OK/Cancel dialog and returns the user input.
    /// </summary>
    /// <param name="message">The message to be displayed.</param>
    /// <param name="icon">The icon to be displayed.</param>
    /// <param name="caption">Caption for the messagebox</param>
    /// <returns>User selection.</returns>
    public abstract CustomDialogResults ShowOkCancel(string message, CustomDialogIcons icon, string caption = null);

    /// <summary>
    ///   Fired right before a MessageBox is shown
    /// </summary>
    public EventHandler PreviewShowMessageBox = delegate { };

    /// <summary>
    ///   Fired after a MessageBox closes
    /// </summary>
    public EventHandler MessageBoxClosed = delegate { };
  }
}