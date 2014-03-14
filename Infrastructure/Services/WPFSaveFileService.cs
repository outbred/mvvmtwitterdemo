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
using System.ComponentModel.Composition;
using System.Windows;
using Infrastructure.Services.Interfaces;
using MEFedMVVM.ViewModelLocator;
using Microsoft.Win32;

#endregion

namespace Infrastructure.Services
{
  /// <summary>
  ///   This class implements the ISaveFileService for WPF purposes.
  /// </summary>
  [PartCreationPolicy(CreationPolicy.Shared)]
  [ExportService(ServiceType.Both, typeof (ISaveFileService))]
  public class WPFSaveFileService : ISaveFileService
  {
    #region Data

    /// <summary>
    ///   Embedded SaveFileDialog to pass back correctly selected
    ///   values to ViewModel
    /// </summary>
    private readonly SaveFileDialog sfd = new SaveFileDialog();

    #endregion

    #region ISaveFileService Members

    /// <summary>
    ///   This method should show a window that allows a file to be selected
    /// </summary>
    /// <param name="owner">The owner window of the dialog</param>
    /// <returns>A bool from the ShowDialog call</returns>
    public bool? ShowDialog(Window owner)
    {
      //Set embedded SaveFileDialog.Filter
      if (!String.IsNullOrEmpty(Filter))
      {
        sfd.Filter = Filter;
      }

      //Set embedded SaveFileDialog.InitialDirectory
      if (!String.IsNullOrEmpty(InitialDirectory))
      {
        sfd.InitialDirectory = InitialDirectory;
      }

      //Set embedded SaveFileDialog.OverwritePrompt
      sfd.OverwritePrompt = OverwritePrompt;

      //return results
      return sfd.ShowDialog(owner);
    }

    /// <summary>
    ///   FileName : Simply use embedded SaveFileDialog.FileName
    ///   Also allow users to set new FileName, which sets SaveFileDialog.FileName
    /// </summary>
    public string FileName
    {
      get { return sfd.FileName; }
      set { sfd.FileName = value; }
    }

    /// <summary>
    ///   Filter : Simply use embedded SaveFileDialog.Filter
    /// </summary>
    public string Filter
    {
      get { return sfd.Filter; }
      set { sfd.Filter = value; }
    }

    /// <summary>
    ///   Filter : Simply use embedded SaveFileDialog.InitialDirectory
    /// </summary>
    public string InitialDirectory
    {
      get { return sfd.InitialDirectory; }
      set { sfd.InitialDirectory = value; }
    }

    /// <summary>
    ///   OverwritePrompt : Simply use embedded SaveFileDialog.OverwritePrompt
    /// </summary>
    public bool OverwritePrompt
    {
      get { return sfd.OverwritePrompt; }
      set { sfd.OverwritePrompt = value; }
    }

    /// <summary>
    ///   Default extension for the saved file
    /// </summary>
    public string DefaultExt
    {
      get { return sfd.DefaultExt; }
      set { sfd.DefaultExt = value; }
    }

    #endregion
  }
}