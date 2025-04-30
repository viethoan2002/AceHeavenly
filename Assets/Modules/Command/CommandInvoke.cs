using System.Collections.Generic;
using UnityEngine;

namespace Modules.Command
{
    public class CommandInvoke : MonoBehaviour
    {
        private static Stack<ICommand> _undoStack = new Stack<ICommand>();
        private static Stack<ICommand> _redoStack = new Stack<ICommand>();

        public static void ClearCommands()
        {
            _undoStack.Clear();
            _redoStack.Clear();
        }

        public static void ExecuteCommand(ICommand command)
        {
            command.Execute();
            
            _undoStack.Push(command);
            _redoStack.Clear();
        }

        public static void UndoCommand()
        {
            if (_undoStack.Count > 0)
            {
                ICommand command = _undoStack.Pop();
                command.Undo();
                _redoStack.Push(command);
            }
        }

        public static void RedoCommand()
        {
            if (_redoStack.Count > 0)
            {
                ICommand command = _redoStack.Pop();
                _undoStack.Push(command);
                command.Execute();
            }
        }

        public static bool CheckEmptyCommand()
        {
            return _undoStack.Count == 0;
        }

        public static void UndoAllCommands()
        {
            while (_undoStack.Count > 0)
            {
                UndoCommand();
            }
        }
    }
}
