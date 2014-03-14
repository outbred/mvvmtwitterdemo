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
  ///   This class implements the IOpenFileService for WPF purposes.
  /// </summary>
  [PartCreationPolicy(CreationPolicy.Shared)]
  [ExportService(ServiceType.Both, typeof (IOpenFileService))]
  public class WPFOpenFileService : IOpenFileService
  {
    #region Data

    /// <summary>
    ///   Embedded OpenFileDialog to pass back correctly selected
    ///   values to ViewModel
    /// </summary>
    private readonly OpenFileDialog ofd = new OpenFileDialog();

    #endregion

    #region IOpenFileService Members

    /// <summary>
    ///   This method should show a window that allows a file to be selected
    /// </summary>
    /// <param name="owner">The owner window of the dialog</param>
    /// <returns>A bool from the ShowDialog call</returns>
    public bool? ShowDialog(Window owner)
    {
      //Set embedded OpenFileDialog.Filter
      if (!String.IsNullOrEmpty(Filter))
      {
        ofd.Filter = Filter;
      }

      //Set embedded OpenFileDialog.InitialDirectory
      if (!String.IsNullOrEmpty(InitialDirectory))
      {
        ofd.InitialDirectory = InitialDirectory;
      }

      //return results
      return ofd.ShowDialog(owner);
    }

    /// <summary>
    ///   FileName : Simply use embedded OpenFileDialog.FileName
    ///   Also allow users to set new FileName, which sets OpenFileDialog.FileName
    /// </summary>
    public string FileName
    {
      get { return ofd.FileName; }
      set { ofd.FileName = value; }
    }

    /// <summary>
    ///   Filter : Simply use embedded OpenFileDialog.Filter
    /// </summary>
    public string Filter
    {
      get { return ofd.Filter; }
      set { ofd.Filter = value; }
    }

    /// <summary>
    ///   Filter : Simply use embedded OpenFileDialog.InitialDirectory
    /// </summary>
    public string InitialDirectory
    {
      get { return ofd.InitialDirectory; }
      set { ofd.InitialDirectory = value; }
    }

    #endregion
  }
}