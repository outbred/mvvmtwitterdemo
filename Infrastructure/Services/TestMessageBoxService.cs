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
using Infrastructure.Services.Interfaces;

#endregion

namespace Infrastructure.Services
{
  /// <summary>
  ///   This class implements the IMessageBoxService for Unit testing purposes.
  /// </summary>
  /// <example>
  ///   <![CDATA[
  ///  
  ///         //Queue up the response we expect for our given TestMessageBoxService
  ///         //for a given ICommand/Method call within the test ViewModel
  ///         testMessageBoxService.ShowYesNoResponders.Enqueue
  ///             (() =>
  ///                 {
  /// 
  ///                     return CustomDialogResults.Yes;
  ///                 }
  ///             );
  ///  ]]>
  /// </example>
  public class TestMessageBoxService : IMessageBoxService
  {
    #region Data

    public Queue<Func<CustomDialogResults>> ShowOkCancelResponders { get; set; }
    public Queue<Func<CustomDialogResults>> ShowYesNoCancelResponders { get; set; }
    public Queue<Func<CustomDialogResults>> ShowYesNoResponders { get; set; }

    #endregion

    #region Ctor

    /// <summary>
    ///   Ctor
    /// </summary>
    public TestMessageBoxService()
    {
      ShowOkCancelResponders = new Queue<Func<CustomDialogResults>>();
    }

    #endregion

    #region IMessageBoxService Members

    /// <summary>
    ///   Does nothing, as nothing required for testing
    /// </summary>
    /// <param name="message">The message to be displayed.</param>
    public override void ShowError(string message, string caption = null)
    {
      //Nothing to do, as there will never be a UI
      //as we are testing the VMs
    }

    /// <summary>
    ///   Does nothing, as nothing required for testing
    /// </summary>
    /// <param name="message">The message to be displayed.</param>
    public override void ShowInformation(string message, string caption = null)
    {
      //Nothing to do, as there will never be a UI
      //as we are testing the VMs
    }

    /// <summary>
    ///   Does nothing, as nothing required for testing
    /// </summary>
    /// <param name="message">The message to be displayed.</param>
    public override void ShowWarning(string message, string caption = null)
    {
      //Nothing to do, as there will never be a UI
      //as we are testing the VMs        
    }

    /// <summary>
    ///   Returns the next Dequeue ShowOkCancel response expected. See the tests for
    ///   the Func callback expected values
    /// </summary>
    /// <param name="message">The message to be displayed.</param>
    /// <returns>User selection.</returns>
    public override CustomDialogResults ShowOkCancel(string message, CustomDialogIcons icon, string caption = null)
    {
      if (ShowOkCancelResponders.Count == 0)
      {
        throw new Exception(
          "TestMessageBoxService ShowOkCancel method expects a Func<CustomDialogResults> callback \r\n" +
          "delegate to be enqueued for each Show call");
      }
      else
      {
        var responder = ShowOkCancelResponders.Dequeue();
        return responder();
      }
    }

    public override CustomDialogResults ShowYesNo(string message, CustomDialogIcons icon, string caption = null)
    {
      if (ShowYesNoResponders.Count == 0)
      {
        throw new Exception(
          "TestMessageBoxService ShowOkCancel method expects a Func<CustomDialogResults> callback \r\n" +
          "delegate to be enqueued for each Show call");
      }
      else
      {
        var responder = ShowYesNoResponders.Dequeue();
        return responder();
      }
    }

    public override CustomDialogResults ShowYesNoCancel(string message, CustomDialogIcons icon, string caption = null)
    {
      if (ShowYesNoCancelResponders.Count == 0)
      {
        throw new Exception(
          "TestMessageBoxService ShowOkCancel method expects a Func<CustomDialogResults> callback \r\n" +
          "delegate to be enqueued for each Show call");
      }
      else
      {
        var responder = ShowYesNoCancelResponders.Dequeue();
        return responder();
      }
    }

    #endregion
  }
}