using Hanoi.Models;
using ReactiveUI;
using System.Collections.Generic;
using System.Linq;

namespace Hanoi.Logic
{
    public class GameLogic : ReactiveObject
    {
        private Disc? _selectedDisc;
        public Disc? SelectedDisc
        {
            get => _selectedDisc;
            private set => this.RaiseAndSetIfChanged(ref _selectedDisc, value);
        }

        private int _moveCount;
        public int MoveCount
        {
            get => _moveCount;
            private set => this.RaiseAndSetIfChanged(ref _moveCount, value);
        }

        private bool _gameWon;
        public bool GameWon
        {
            get => _gameWon;
            private set => this.RaiseAndSetIfChanged(ref _gameWon, value);
        }

        private StackName? _selectedStack;
        public StackName? SelectedStack
        {
            get => _selectedStack;
            private set => this.RaiseAndSetIfChanged(ref _selectedStack, value);
        }

        private Stack<Disc>? _originStack;

        public int NumberOfDiscs { get; }

        public IEnumerable<Disc> Left => _left.ToArray();
        public IEnumerable<Disc> Right => _right.ToArray();
        public IEnumerable<Disc> Middle => _middle.ToArray();

        private readonly Stack<Disc> _left = new ();
        private readonly Stack<Disc> _right = new ();
        private readonly Stack<Disc> _middle = new ();

        public GameLogic(int numberOfDiscs)
        {
            NumberOfDiscs = numberOfDiscs;
            for (int i = numberOfDiscs; i >= 1; i--)
            {
                var disc = new Disc(i);
                _left.Push(disc);
            }
        }

        public void SelectStack(StackName stack)
        {
            var selectedStack = stack switch
            {
                StackName.Left => _left,
                StackName.Right => _right,
                _ => _middle
            };

            if (SelectedDisc == null || _originStack == null)
            {
                if (selectedStack.Count == 0)
                    return;

                SelectedStack = stack;
                SelectedDisc = selectedStack.Peek();
                _originStack = selectedStack;
            } 
            else
            {
                var valid = CanMoveDiscTo(selectedStack);
                if (valid)
                {
                    SelectedDisc = _originStack.Pop();
                    selectedStack.Push(SelectedDisc);
                    MoveCount++;
                    if (HasWon())
                        GameWon = true;
                }

                SelectedStack = null;
                SelectedDisc = null;
                _originStack = null;
            }
        }

        private bool HasWon()
            => _right.Count == NumberOfDiscs;
        

        private bool CanMoveDiscTo(Stack<Disc> stack)
        {
            if (SelectedDisc == null)
                return false;
            if (stack.Count == 0)
                return true;

            var topDisc = stack.Peek();
            return SelectedDisc.Size < topDisc.Size;
        }


    }

    public enum StackName
    {
        Left, Middle, Right
    }
}
