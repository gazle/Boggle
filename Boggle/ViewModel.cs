using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.IO;
using System.Threading.Tasks;
using System.Windows.Input;
using System.Windows.Threading;

namespace Boggle
{
    class ViewModel : ViewModelBase
    {
        int boardWidth = 4;
        int boardHeight = 4;
        LetterBag letterBag = new LetterBag();
        string[] words;
        readonly SortedSet<string> foundWords = new SortedSet<string>();
        string text;
        string boardSizeText = "";
        int progressPercentage = 0;
        bool searchIsRunning;

        public bool SearchIsRunning
        {
            get { return searchIsRunning; }
            set
            {
                searchIsRunning = value;
                OnPropertyChanged();
                RandomizeCommand.RaiseCanExecuteChanged();
                StartSearchCommand.RaiseCanExecuteChanged();
                BoardSizeCommand.RaiseCanExecuteChanged();
                EditLetterCommand.RaiseCanExecuteChanged();
            }
        }
        public int ProgressPercentage { get { return progressPercentage; } set { progressPercentage = value; OnPropertyChanged(); } }
        public string BoardSizeText { get { return boardSizeText; } set { boardSizeText = value; OnPropertyChanged(); } }
        public string Text { get { return text; } set { text = value; OnPropertyChanged(); } }
        public LetterBag LetterBag { get { return letterBag; } set { letterBag = value; OnPropertyChanged(); } }
        public ObservableCollection<string> Letters { get; set; } = new ObservableCollection<string>();
        public int BoardWidth { get { return boardWidth; } set { boardWidth = value; OnPropertyChanged(); } }
        public int BoardHeight { get { return boardHeight; } set { boardHeight = value; OnPropertyChanged(); } }
        public DelegateCommand InitializeCommand { get; private set; }
        public DelegateCommand RandomizeCommand { get; private set; }
        public DelegateCommand StartSearchCommand { get; private set; }
        public DelegateCommand BoardSizeCommand { get; private set; }
        public DelegateCommand<(int, Key)> EditLetterCommand { get; private set; }

        public ViewModel()
        {
            RandomizeCommand = new DelegateCommand(RandomizeBoard, () => !SearchIsRunning);
            StartSearchCommand = new DelegateCommand(StartSearch, () => !SearchIsRunning);
            InitializeCommand = new DelegateCommand(Initialize);
            BoardSizeCommand = new DelegateCommand(BoardSize, () => !SearchIsRunning);
            EditLetterCommand = new DelegateCommand<(int, Key)>(EditLetter, (p) => !SearchIsRunning);
            RandomizeBoard();
        }

        private void EditLetter((int cell, Key letter) p)
        {
            string s = p.letter.ToString();
            if (p.cell >= 0 && s.Length == 1 && s[0] >= 'A' && s[0] <= 'Z')
                Letters[p.cell] = LetterBag.Letters[p.letter.ToString()[0] - 64];
        }

        void BoardSize()
        {
            if (int.TryParse(boardSizeText, out int size))
            {
                BoardWidth = size;
                BoardHeight = size;
                RandomizeBoard();
            }
        }

        void Initialize()
        {
            using StreamReader stream = new StreamReader(@"..\..\..\WordLists\sowpods EU.txt");
            words = stream.ReadToEnd().Split(new[] { '\r', '\a', '\n' }, StringSplitOptions.RemoveEmptyEntries);
            for (int i = 0; i < words.Length; i++)
                words[i] = words[i].ToLower();
        }

        void RandomizeBoard()
        {
            Letters.Clear();
            for (int i = 0; i < boardWidth * boardHeight; i++)
                Letters.Add(letterBag.PeekRandom());
        }

        async void StartSearch()
        {
            SearchIsRunning = true;
            IProgress<int> progress = new Progress<int>(p => ProgressPercentage = p);
            try
            {
                Stopwatch sw = new Stopwatch();
                sw.Start();
                await Task.Run(() =>
                {
                    int ticks = 0;
                    foundWords.Clear();
                    Parallel.For(0, boardHeight * boardWidth, () => new bool[boardHeight, boardWidth], (i, state, visited) =>
                    {
                        int row = i / boardWidth;
                        int col = i % boardWidth;
                        FindWordsStartingAt("", row, col);
                        ticks++;
                        progress.Report(100 * ticks / (boardHeight * boardWidth));
                        return visited;

                        void FindWordsStartingAt(string prefix, int row, int col)
                        {
                            prefix += Letters[row * boardWidth + col].ToLower();                           // Extend current prefix
                            int x = Array.BinarySearch(words, prefix);       // x=index or the ones complement of where the word would be inserted
                            if (~x == words.Length || x < 0 && !words[~x].StartsWith(prefix)) return;    // Handle edge case, abort if no words start with prefix
                            if (x >= 0)         // Word found
                                lock (this)
                                {
                                    foundWords.Add(prefix);
                                }
                            visited[row, col] = true;     // Visited
                            for (var i = row - 1; i != row + 2; i++)
                                for (var j = col - 1; j != col + 2; j++)
                                    if (i >= 0 & i < boardHeight & j >= 0 & j < boardWidth && !visited[i, j])
                                        FindWordsStartingAt(prefix, i, j);
                            visited[row, col] = false;
                        }
                    },
                    (localData) => { });
                });
                sw.Stop();
                Text = $"Time taken: {sw.ElapsedMilliseconds}ms\n" + $"{foundWords.Count} words found\n" + string.Join(" ", foundWords);
            }
            catch (Exception e)
            {

            }
            finally
            {
                SearchIsRunning = false;
            }
        }
    }
}