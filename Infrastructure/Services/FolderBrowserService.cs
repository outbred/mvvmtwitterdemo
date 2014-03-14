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
using System.ComponentModel.Composition;
using System.Drawing.Design;
using System.IO;
using System.Runtime.InteropServices;
using System.Text;
using System.Windows;
using System.Windows.Forms;
using System.Windows.Interop;
using Cinch.Utilities.FolderBrowserSupport;
using Cinch.Utilities.FolderBrowserSupport.Interfaces;
using Infrastructure.Services.Interfaces;
using MEFedMVVM.ViewModelLocator;
using IWin32Window = System.Windows.Forms.IWin32Window;

#endregion

namespace Infrastructure.Services
{
  /// <summary>
  ///   Prompts the user to select a folder.
  /// </summary>
  /// <threadsafety instance="false" static="true" />
  [DefaultEvent("HelpRequest"), Designer("System.Windows.Forms.Design.FolderBrowserDialogDesigner, System.Design, Version=2.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a"), DefaultProperty("SelectedPath"), Description("Prompts the user to select a folder.")]
  [PartCreationPolicy(CreationPolicy.Shared)]
  [ExportService(ServiceType.Both, typeof (IFolderBrowserService))]
  public sealed class FolderBrowserService : IFolderBrowserService
  {
    private string _description;
    private string _selectedPath;

    /// <summary>
    ///   Creates a new instance of the <see cref="WindowsFolderBrowserDialog" /> class.
    /// </summary>
    public FolderBrowserService()
    {
      Reset();
    }

    #region Public Properties

    /// <summary>
    ///   If not null, sets the dialog's owner to this ((win)form or WPF window) by default
    /// </summary>
    public static IWin32Window Owner { get; set; }

    /// <summary>
    ///   Gets a value that indicates whether the current OS supports Vista-style common file dialogs.
    /// </summary>
    /// <value>
    ///   <see langword="true" /> on Windows Vista or newer operating systems; otherwise, <see langword="false" />.
    /// </value>
    [Browsable(false)]
    public bool IsVistaFolderDialogSupported
    {
      get { return NativeMethods.IsWindowsVistaOrLater; }
    }

    /// <summary>
    ///   Gets or sets the descriptive text displayed above the tree view control in the dialog box, or below the list view
    ///   control
    ///   in the Vista style dialog.
    /// </summary>
    /// <value>
    ///   The description to display. The default is an empty string ("").
    /// </value>
    [Category("Folder Browsing"), DefaultValue(""), Localizable(true), Browsable(true), Description("The descriptive text displayed above the tree view control in the dialog box, or below the list view control in the Vista style dialog.")]
    public string Description
    {
      get { return _description ?? string.Empty; }
      set { _description = value; }
    }

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
    public Environment.SpecialFolder RootFolder { get; set; }

    /// <summary>
    ///   Gets or sets the path selected by the user.
    /// </summary>
    /// <value>
    ///   The path of the folder first selected in the dialog box or the last folder selected by the user. The default is an
    ///   empty string ("").
    /// </value>
    [Browsable(true), Editor("System.Windows.Forms.Design.SelectedPathEditor, System.Design, Version=2.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a", typeof (UITypeEditor)), Description("The path selected by the user."), DefaultValue(""), Localizable(true), Category("Folder Browsing")]
    public string SelectedPath
    {
      get { return _selectedPath ?? string.Empty; }
      set { _selectedPath = value; }
    }

    /// <summary>
    ///   Gets or sets a value indicating whether the New Folder button appears in the folder browser dialog box. This
    ///   property has no effect if the Vista style dialog is used; in that case, the New Folder button is always shown.
    /// </summary>
    /// <value>
    ///   <see langword="true" /> if the New Folder button is shown in the dialog box; otherwise, <see langword="false" />. The
    ///   default is <see langword="true" />.
    /// </value>
    [Browsable(true), Localizable(false), Description("A value indicating whether the New Folder button appears in the folder browser dialog box. This property has no effect if the Vista style dialog is used; in that case, the New Folder button is always shown."), DefaultValue(true), Category("Folder Browsing")]
    public bool ShowNewFolderButton { get; set; }

    /// <summary>
    ///   Gets or sets a value that indicates whether to use the value of the <see cref="Description" /> property
    ///   as the dialog title for Vista style dialogs. This property has no effect on old style dialogs.
    /// </summary>
    /// <value>
    ///   <see langword="true" /> to indicate that the value of the <see cref="Description" /> property is used as dialog
    ///   title; <see langword="false" />
    ///   to indicate the value is added as additional text to the dialog. The default is <see langword="false" />.
    /// </value>
    [Category("Folder Browsing"), DefaultValue(false), Description("A value that indicates whether to use the value of the Description property as the dialog title for Vista style dialogs. This property has no effect on old style dialogs.")]
    public bool UseDescriptionForTitle { get; set; }

    #endregion

    #region Public Methods

    /// <summary>
    ///   Resets all properties to their default values (except Owner).
    /// </summary>
    public void Reset()
    {
      _description = string.Empty;
      UseDescriptionForTitle = false;
      _selectedPath = string.Empty;
      RootFolder = Environment.SpecialFolder.Desktop;
      ShowNewFolderButton = true;
    }

    /// <summary>
    ///   Displays the folder browser dialog using whatever window handle it can find, or none if there are none.
    /// </summary>
    /// <param name="owner">Handle to the window that owns the dialog.</param>
    /// <param name="ownerWin32">Handle to the wpf or winform window </param>
    /// <returns>If the user clicks the OK button, <see langword="true" /> is returned; otherwise, <see langword="false" />.</returns>
    public bool? ShowDialog(Window owner = null, IWin32Window ownerWin32 = null)
    {
      IntPtr ownerHandle;
      if (owner != null)
      {
        ownerHandle = new WindowInteropHelper(owner).Handle;
      }
      else if (ownerWin32 != null)
      {
        ownerHandle = ownerWin32.Handle;
      }
      else if (Owner != null)
      {
        ownerHandle = Owner.Handle;
      }
      else
      {
        ownerHandle = NativeMethods.GetActiveWindow();
      }

      return (IsVistaFolderDialogSupported ? RunDialog(ownerHandle) : RunDialogClassic());
    }

    #endregion

    #region Private Methods

    private bool RunDialog(IntPtr owner)
    {
      IFileDialog dialog = null;
      try
      {
        dialog = new NativeFileOpenDialog();
        SetDialogProperties(dialog);
        int result = dialog.Show(owner);
        if (result < 0)
        {
          if ((uint) result == (uint) HRESULT.ERROR_CANCELLED)
          {
            return false;
          }
          else
          {
            throw Marshal.GetExceptionForHR(result);
          }
        }
        GetResult(dialog);
        return true;
      }
      finally
      {
        if (dialog != null)
        {
          Marshal.FinalReleaseComObject(dialog);
        }
      }
    }

    private bool RunDialogClassic()
    {
      var dialog = new FolderBrowserDialog();
      dialog.Description = Description;
      dialog.ShowNewFolderButton = ShowNewFolderButton;
      dialog.RootFolder = RootFolder;

      DialogResult result = dialog.ShowDialog();

      if (result == DialogResult.OK)
      {
        SelectedPath = dialog.SelectedPath;
        return true;
      }
      else
      {
        return false;
      }
    }

    private void SetDialogProperties(IFileDialog dialog)
    {
      // Description
      if (!string.IsNullOrEmpty(_description))
      {
        if (UseDescriptionForTitle)
        {
          dialog.SetTitle(_description);
        }
        else
        {
          IFileDialogCustomize customize = (IFileDialogCustomize) dialog;
          customize.AddText(0, _description);
        }
      }

      dialog.SetOptions(NativeMethods.FOS.FOS_PICKFOLDERS | NativeMethods.FOS.FOS_FORCEFILESYSTEM | NativeMethods.FOS.FOS_FILEMUSTEXIST);

      if (!string.IsNullOrEmpty(_selectedPath))
      {
        string parent = Path.GetDirectoryName(_selectedPath);
        if (parent == null || !Directory.Exists(parent))
        {
          dialog.SetFileName(_selectedPath);
        }
        else
        {
          string folder = Path.GetFileName(_selectedPath);
          dialog.SetFolder(NativeMethods.CreateItemFromParsingName(parent));
          dialog.SetFileName(folder);
        }
      }
    }

    private void GetResult(IFileDialog dialog)
    {
      IShellItem item;
      dialog.GetResult(out item);
      item.GetDisplayName(NativeMethods.SIGDN.SIGDN_FILESYSPATH, out _selectedPath);
    }

    private int BrowseCallbackProc(IntPtr hwnd, NativeMethods.FolderBrowserDialogMessage msg, IntPtr lParam, IntPtr wParam)
    {
      switch (msg)
      {
        case NativeMethods.FolderBrowserDialogMessage.Initialized:
          if (SelectedPath.Length != 0)
          {
            NativeMethods.SendMessage(hwnd, NativeMethods.FolderBrowserDialogMessage.SetSelection, new IntPtr(1), SelectedPath);
          }
          break;
        case NativeMethods.FolderBrowserDialogMessage.SelChanged:
          if (lParam != IntPtr.Zero)
          {
            StringBuilder path = new StringBuilder(260);
            bool validPath = NativeMethods.SHGetPathFromIDList(lParam, path);
            NativeMethods.SendMessage(hwnd, NativeMethods.FolderBrowserDialogMessage.EnableOk, IntPtr.Zero, validPath ? new IntPtr(1) : IntPtr.Zero);
          }
          break;
      }
      return 0;
    }

    #endregion
  }
}