using System;
using System.Runtime.InteropServices;
namespace System
{
      public class UnmanagedMemory<T> : IDisposable where T : struct
      {

            private bool disposed;

            // Methods
            public UnmanagedMemory(int length)
            {
                  if (length < 0)
                  {
                        throw new ArgumentOutOfRangeException("length");
                  }
                  this.Length = length;
                  this.SizeOfType = this.SizeOfT();
                  this.TotalSize = this.SizeOfType * length;
                  this.Start = Marshal.AllocHGlobal(this.TotalSize);
            }

            public void Dispose()
            {
                  this.Dispose(true);
                  GC.SuppressFinalize(this);
            }

            protected virtual void Dispose(bool disposing)
            {
                  if (!this.disposed)
                  {
                        this.disposed = true;
                        Marshal.FreeHGlobal(this.Start);
                  }
            }

            ~UnmanagedMemory()
            {
                  this.Dispose(false);
            }

            private int SizeOfT()
            {
                  return Marshal.SizeOf(typeof(T));
            }

            // Properties
            public int Length { get; private set; }


            public int SizeOfType { get; private set; }

            public IntPtr Start { get; private set; }


            public int TotalSize { get; set; }

      }
}


 
