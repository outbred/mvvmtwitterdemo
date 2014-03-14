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
using System.ComponentModel;
using System.Drawing.Design;
using System.Windows;
using System.Windows.Forms;

#endregion

namespace Infrastructure.Services.Interfaces
{
  public interface IFolderBrowserService
  {
    #region Properties

    /// <summary>
    ///   Gets a value that indicates whether the current OS supports Vista-style common file dialogs.
    /// </summary>
    /// <value>
    ///   <see langword="true" /> on Windows Vista or newer operating systems; otherwise, <see langword="false" />.
    /// </value>
    [Browsable(false)]
    bool IsVistaFolderDialogSupported { get; }

    /// <summary>
    ///   Gets or sets the descriptive text displayed above the tree view control in the dialog box, or below the list view
    ///   control
    ///   in the Vista style dialog.
    /// </summary>
    /// <value>
    ///   The description to display. The default is an empty string ("").
    /// </value>
    [Category("Folder Browsing")]
    [DefaultValue("")]
    [Localizable(true)]
    [Browsable(true)]
    [Description("The descriptive text displayed above the tree view control in the dialog box, or below the list view control in the Vista style dialog.")]
    string Description { get; set; }

    /// <summary>
    ///   Gets or sets the root folder where the browsing starts from. This property has no effect if the Vista style
    ///   dialog is used.
    /// </summary>
    /// <value>
    ///   One of the <see cref="System.Environment.SpecialFolder" /> values. The default is Desktop.
    /// </value>
    /// <exception cref="System.ComponentModel.InvalidEnumArgumentException">
    ///   The value assigned is not one of the
    ///   <see cref="System.Environment.SpecialFolder" /> values.
    /// </exception>
    [Localizable(false), Description("The root folder where the browsing starts from. This property has no effect if the Vista style dialog is used."), Category("Folder Browsing"), Browsable(true), DefaultValue(typeof (Environment.SpecialFolder), "Desktop")]
    Environment.SpecialFolder RootFolder { get; set; }

    /// <summary>
    ///   Gets or sets the path selected by the user.
    /// </summary>
    /// <value>
    ///   The path of the folder first selected in the dialog box or the last folder selected by the user. The default is an
    ///   empty string ("").
    /// </value>
    [Browsable(true)]
    [Editor("System.Windows.Forms.Design.SelectedPathEditor, System.Design, Version=2.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a", typeof (UITypeEditor))]
    [Description("The path selected by the user.")]
    [DefaultValue("")]
    [Localizable(true)]
    [Category("Folder Browsing")]
    string SelectedPath { get; set; }

    /// <summary>
    ///   Gets or sets a value indicating whether the New Folder button appears in the folder browser dialog box. This
    ///   property has no effect if the Vista style dialog is used; in that case, the New Folder button is always shown.
    /// </summary>
    /// <value>
    ///   <see langword="true" /> if the New Folder button is shown in the dialog box; otherwise, <see langword="false" />. The
    ///   default is <see langword="true" />.
    /// </value>
    [Browsable(true)]
    [Localizable(false)]
    [Description("A value indicating whether the New Folder button appears in the folder browser dialog box. This property has no effect if the Vista style dialog is used; in that case, the New Folder button is always shown.")]
    [DefaultValue(true)]
    [Category("Folder Browsing")]
    bool ShowNewFolderButton { get; set; }

    /// <summary>
    ///   Gets or sets a value that indicates whether to use the value of the <see cref="Description" /> property
    ///   as the dialog title for Vista style dialogs. This property has no effect on old style dialogs.
    /// </summary>
    /// <value>
    ///   <see langword="true" /> to indicate that the value of the <see cref="Description" /> property is used as dialog
    ///   title; <see langword="false" />
    ///   to indicate the value is added as additional text to the dialog. The default is <see langword="false" />.
    /// </value>
    [Category("Folder Browsing")]
    [DefaultValue(false)]
    [Description("A value that indicates whether to use the value of the Description property as the dialog title for Vista style dialogs. This property has no effect on old style dialogs.")]
    bool UseDescriptionForTitle { get; set; }

    #endregion

    #region Methods

    /// <summary>
    ///   Resets all properties to their default values (except Owner).
    /// </summary>
    void Reset();

    /// <summary>
    ///   Displays the folder browser dialog using whatever window handle it can find, or none if there are none.
    /// </summary>
    /// <param name="owner">Handle to the window that owns the dialog.</param>
    /// <returns>If the user clicks the OK button, <see langword="true" /> is returned; otherwise, <see langword="false" />.</returns>
    bool? ShowDialog(Window owner = null, IWin32Window ownerWin32 = null);

    #endregion
  }
}