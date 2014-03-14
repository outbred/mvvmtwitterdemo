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
using System.Windows.Forms;
using Infrastructure.Services.Interfaces;
using MEFedMVVM.ViewModelLocator;

#endregion

namespace Infrastructure.Services
{
  /// <summary>
  ///   This class implements the IMessageBoxService for WPF purposes.
  /// </summary>
  [PartCreationPolicy(CreationPolicy.Shared)]
  [ExportService(ServiceType.Both, typeof (IMessageBoxService))]
  public class WPFMessageBoxService : IMessageBoxService
  {
    /// <summary>
    ///   If not null, sets the message box's owner to this ((win)form or WPF window) by default
    /// </summary>
    public static IWin32Window Owner { get; set; }

    //static Window WindowOwner { get { return Owner != null ? Owner.ToWindow() : null; } }

    #region IMessageBoxService Members

    /// <summary>
    ///   Displays an error dialog with a given message.
    /// </summary>
    /// <param name="message">The message to be displayed.</param>
    public override void ShowError(string message, string caption = null)
    {
      ShowMessage(message, caption ?? "Error", CustomDialogIcons.Stop);
    }

    /// <summary>
    ///   Displays an error dialog with a given message.
    /// </summary>
    /// <param name="message">The message to be displayed.</param>
    public override void ShowInformation(string message, string caption = null)
    {
      ShowMessage(message, caption ?? "Information", CustomDialogIcons.Information);
    }

    /// <summary>
    ///   Displays an error dialog with a given message.
    /// </summary>
    /// <param name="message">The message to be displayed.</param>
    public override void ShowWarning(string message, string caption = null)
    {
      ShowMessage(message, caption, CustomDialogIcons.Warning);
    }

    /// <summary>
    ///   Displays a Yes/No dialog and returns the user input.
    /// </summary>
    /// <param name="message">The message to be displayed.</param>
    /// <param name="icon">The icon to be displayed.</param>
    /// <returns>User selection.</returns>
    public override CustomDialogResults ShowYesNo(string message, CustomDialogIcons icon, string caption = null)
    {
      return ShowQuestionWithButton(message, caption, icon, CustomDialogButtons.YesNo);
    }

    /// <summary>
    ///   Displays a Yes/No/Cancel dialog and returns the user input.
    /// </summary>
    /// <param name="message">The message to be displayed.</param>
    /// <param name="icon">The icon to be displayed.</param>
    /// <returns>User selection.</returns>
    public override CustomDialogResults ShowYesNoCancel(string message, CustomDialogIcons icon, string caption = null)
    {
      return ShowQuestionWithButton(message, caption, icon, CustomDialogButtons.YesNoCancel);
    }

    /// <summary>
    ///   Displays a OK/Cancel dialog and returns the user input.
    /// </summary>
    /// <param name="message">The message to be displayed.</param>
    /// <param name="icon">The icon to be displayed.</param>
    /// <returns>User selection.</returns>
    public override CustomDialogResults ShowOkCancel(string message, CustomDialogIcons icon, string caption = null)
    {
      return ShowQuestionWithButton(message, caption, icon, CustomDialogButtons.OKCancel);
    }

    #endregion

    #region Private Methods

    /// <summary>
    ///   Shows a standard System.Windows.MessageBox using the parameters requested
    /// </summary>
    /// <param name="message">The message to be displayed.</param>
    /// <param name="heading">The heading to be displayed</param>
    /// <param name="icon">The icon to be displayed.</param>
    private void ShowMessage(string message, string heading, CustomDialogIcons icon)
    {
      PreviewShowMessageBox(this, null);
      if (Owner != null)
      {
        MessageBox.Show(Owner, message, heading, MessageBoxButtons.OK, GetImage(icon));
      }
      else
      {
        MessageBox.Show(message, heading, MessageBoxButtons.OK, GetImage(icon));
      }
      MessageBoxClosed(this, null);
    }

    /// <summary>
    ///   Shows a standard System.Windows.MessageBox using the parameters requested
    ///   but will return a translated result to enable adhere to the IMessageBoxService
    ///   implementation required.
    ///   This abstraction allows for different frameworks to use the same ViewModels but supply
    ///   alternative implementations of core service interfaces
    /// </summary>
    /// <param name="message">The message to be displayed.</param>
    /// <param name="heading">Heading for the message </param>
    /// <param name="icon">The icon to be displayed.</param>
    /// <param name="button"></param>
    /// <returns>CustomDialogResults results to use</returns>
    private CustomDialogResults ShowQuestionWithButton(string message, string heading, CustomDialogIcons icon, CustomDialogButtons button)
    {
      PreviewShowMessageBox(this, null);
      if (Owner != null)
      {
        var result = MessageBox.Show(Owner, message, heading, GetButton(button), GetImage(icon));
        MessageBoxClosed(this, null);

        return GetResult(result);
      }
      else
      {
        var result = MessageBox.Show(message, heading, GetButton(button), GetImage(icon));
        MessageBoxClosed(this, null);

        return GetResult(result);
      }
    }


    /// <summary>
    ///   Translates a CustomDialogIcons into a standard WPF System.Windows.MessageBox MessageBoxImage.
    ///   This abstraction allows for different frameworks to use the same ViewModels but supply
    ///   alternative implementations of core service interfaces
    /// </summary>
    /// <param name="icon">The icon to be displayed.</param>
    /// <returns>A standard WPF System.Windows.MessageBox MessageBoxImage</returns>
    private MessageBoxIcon GetImage(CustomDialogIcons icon)
    {
      var image = MessageBoxIcon.None;

      switch (icon)
      {
        case CustomDialogIcons.Information:
          image = MessageBoxIcon.Information;
          break;
        case CustomDialogIcons.Question:
          image = MessageBoxIcon.Question;
          break;
        case CustomDialogIcons.Exclamation:
          image = MessageBoxIcon.Exclamation;
          break;
        case CustomDialogIcons.Stop:
          image = MessageBoxIcon.Stop;
          break;
        case CustomDialogIcons.Warning:
          image = MessageBoxIcon.Warning;
          break;
      }
      return image;
    }


    /// <summary>
    ///   Translates a CustomDialogButtons into a standard WPF System.Windows.MessageBox MessageBoxButton.
    ///   This abstraction allows for different frameworks to use the same ViewModels but supply
    ///   alternative implementations of core service interfaces
    /// </summary>
    /// <param name="btn">The button type to be displayed.</param>
    /// <returns>A standard WPF System.Windows.MessageBox MessageBoxButton</returns>
    private MessageBoxButtons GetButton(CustomDialogButtons btn)
    {
      MessageBoxButtons button = MessageBoxButtons.OK;

      switch (btn)
      {
        case CustomDialogButtons.OK:
          button = MessageBoxButtons.OK;
          break;
        case CustomDialogButtons.OKCancel:
          button = MessageBoxButtons.OKCancel;
          break;
        case CustomDialogButtons.YesNo:
          button = MessageBoxButtons.YesNo;
          break;
        case CustomDialogButtons.YesNoCancel:
          button = MessageBoxButtons.YesNoCancel;
          break;
      }
      return button;
    }


    /// <summary>
    ///   Translates a standard WPF System.Windows.MessageBox MessageBoxResult into a
    ///   CustomDialogIcons.
    ///   This abstraction allows for different frameworks to use the same ViewModels but supply
    ///   alternative implementations of core service interfaces
    /// </summary>
    /// <param name="result">The standard WPF System.Windows.MessageBox MessageBoxResult</param>
    /// <returns>CustomDialogResults results to use</returns>
    private CustomDialogResults GetResult(DialogResult result)
    {
      CustomDialogResults customDialogResults = CustomDialogResults.None;

      switch (result)
      {
        case DialogResult.Cancel:
          customDialogResults = CustomDialogResults.Cancel;
          break;
        case DialogResult.No:
          customDialogResults = CustomDialogResults.No;
          break;
        case DialogResult.None:
          customDialogResults = CustomDialogResults.None;
          break;
        case DialogResult.OK:
          customDialogResults = CustomDialogResults.OK;
          break;
        case DialogResult.Yes:
          customDialogResults = CustomDialogResults.Yes;
          break;
      }
      return customDialogResults;
    }

    #endregion
  }
}