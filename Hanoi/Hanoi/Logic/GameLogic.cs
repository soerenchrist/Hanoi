using Hanoi.Models;
using Hanoi.Util;
using ReactiveUI;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace Hanoi.Logic
{
    public class GameLogic : ReactiveObject
    {
        public ResumableStopWatch Stopwatch = new ResumableStopWatch();

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

        public GameLogic(SavedGame savedGame)
        {
            Stopwatch = new ResumableStopWatch(savedGame.CurrentTime);
            MoveCount = savedGame.MoveCount;
            NumberOfDiscs = savedGame.NumberOfDiscs;

            var left = DeserializeStack(savedGame.LeftStack);
            foreach (var disc in left)
                _left.Push(disc);

            var middle = DeserializeStack(savedGame.MiddleStack);
            foreach (var disc in middle)
                _middle.Push(disc);

            var right = DeserializeStack(savedGame.RightStack);
            foreach (var disc in right)
                _right.Push(disc);
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

        public SavedGame ToSavedGame()
        {
            return new SavedGame
            {
                CurrentTime = Stopwatch.ElapsedMilliseconds,
                LeftStack = string.Join(",", Left.Select(x => x.Size)),
                MiddleStack = String.Join(",", Middle.Select(x => x.Size)),
                RightStack = String.Join(",", Right.Select(x => x.Size)),
                MoveCount = MoveCount,
                NumberOfDiscs = NumberOfDiscs,
            };
        }


        private IEnumerable<Disc> DeserializeStack(string serialized)
        {
            var discSizes = serialized.Split(',');
            foreach (var item in discSizes.Reverse())
            {
                if (int.TryParse(item, out var size))
                    yield return new Disc(size);
            }
        }
    }

    public enum StackName
    {
        Left, Middle, Right
    }
}
