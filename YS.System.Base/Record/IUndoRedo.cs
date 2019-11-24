using System;
using System.Collections.Generic;
using System.Text;

namespace System.Record
{
    public interface IUndoRedo
    {
        bool CanUndo { get; }
        bool CanRedo { get; }
        void Undo ();
        void Redo ();
    }
    public interface IUndoRedoRecordProvider
    {
        void Push (OperationBase operation);
        void Push (params OperationBase[] operations);
    }
}
