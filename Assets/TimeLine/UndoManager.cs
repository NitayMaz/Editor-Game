using System;
using System.Collections.Generic;

public static class UndoManager
{
    private static Stack<Action> undoStack = new Stack<Action>();

    public static void Push(Action undoAction) => undoStack.Push(undoAction);
    
    
    public static void Undo()
    {
        if (undoStack.Count > 0)
            undoStack.Pop().Invoke();
    }

    public static void Clear() => undoStack.Clear();
}