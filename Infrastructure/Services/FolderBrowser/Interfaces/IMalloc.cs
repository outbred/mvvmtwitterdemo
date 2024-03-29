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
using System.Runtime.InteropServices;

#endregion

namespace Cinch.Utilities.FolderBrowserSupport.Interfaces
{
  /// <summary>
  ///   C# definition of the IMalloc interface.
  /// </summary>
  [ComImport]
  [InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
  [Guid("00000002-0000-0000-C000-000000000046")]
  internal interface IMalloc
  {
    /// <summary>
    ///   Allocate a block of memory
    /// </summary>
    /// <param name="cb">Size, in bytes, of the memory block to be allocated.</param>
    /// <returns>a pointer to the allocated memory block.</returns>
    [PreserveSig]
    IntPtr Alloc(
      [In] UInt32 cb);

    /// <summary>
    ///   Changes the size of a previously allocated memory block.
    /// </summary>
    /// <param name="pv">Pointer to the memory block to be reallocated</param>
    /// <param name="cb">Size of the memory block, in bytes, to be reallocated.</param>
    /// <returns>reallocated memory block</returns>
    [PreserveSig]
    IntPtr Realloc(
      [In] IntPtr pv,
      [In] UInt32 cb);

    /// <summary>
    ///   Free a previously allocated block of memory.
    /// </summary>
    /// <param name="pv">Pointer to the memory block to be freed.</param>
    [PreserveSig]
    void Free(
      [In] IntPtr pv);


    /// <summary>
    ///   This method returns the size, in bytes, of a memory block previously allocated with IMalloc::Alloc or
    ///   IMalloc::Realloc.
    /// </summary>
    /// <param name="pv">Pointer to the memory block for which the size is requested</param>
    /// <returns>The size of the allocated memory block in bytes.</returns>
    [PreserveSig]
    UInt32 GetSize(
      [In] IntPtr pv);

    /// <summary>
    ///   This method determines whether this allocator was used to allocate the specified block of memory.
    /// </summary>
    /// <param name="pv">Pointer to the memory block</param>
    /// <returns>
    ///   1 - allocated
    ///   0 - not allocated by this IMalloc Instance.
    ///   -1 if DidAlloc is unable to determine whether or not it allocated the memory block.
    /// </returns>
    [PreserveSig]
    Int16 DidAlloc(
      [In] IntPtr pv);

    /// <summary>
    ///   Minimizes the heap by releasing unused memory to the operating system.
    /// </summary>
    [PreserveSig]
    void HeapMinimize();
  }
}