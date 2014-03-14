﻿// Copyright (c) 2014 Brent Bulla Jr.
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
using System.Windows;

#endregion

namespace Infrastructure.Services.Interfaces
{
  /// <summary>
  ///   This interface defines a interface that will allow
  ///   a ViewModel to save a file
  /// </summary>
  public interface ISaveFileService
  {
    /// <summary>
    ///   Prompt the user to overwrite an existing file
    /// </summary>
    Boolean OverwritePrompt { get; set; }

    /// <summary>
    ///   FileName
    /// </summary>
    String FileName { get; set; }

    /// <summary>
    ///   Filter
    /// </summary>
    String Filter { get; set; }

    /// <summary>
    ///   Initial directory for the dialog
    /// </summary>
    String InitialDirectory { get; set; }

    /// <summary>
    ///   Default extension for the saved file
    /// </summary>
    string DefaultExt { get; set; }

    /// <summary>
    ///   This method should show a window that allows a file to be saved
    /// </summary>
    /// <param name="owner">The owner window of the dialog</param>
    /// <returns>A bool from the ShowDialog call</returns>
    bool? ShowDialog(Window owner);
  }
}