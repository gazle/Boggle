using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

namespace Boggle
{
    class LetterBag
    {
        readonly Random rng = new Random();
        readonly int[] letterCount = new int[27] { 0, 9, 2, 2, 4, 12, 2, 3, 2, 9, 1, 1, 4, 2, 6, 8, 2, 1, 6, 4, 6, 4, 2, 2, 1, 2, 1 };
        readonly int totalLetters;
        readonly ReadOnlyCollection<string> letters = new ReadOnlyCollection<string>(new List<string>() { "*", "A", "B", "C", "D", "E", "F", "G", "H", "I", "J", "K", "L", "M", "N", "O", "P", "Qu", "R", "S", "T", "U", "V", "W", "X", "Y", "Z" });
        readonly Collection<string> bag = new Collection<string>();

        public ReadOnlyCollection<string> Letters { get { return letters; } }

        public LetterBag()
        {
            totalLetters = letterCount.Sum();
            Fill();
        }

        /// <summary>
        /// Fill the bag with all the letters.
        /// </summary>
        public void Fill()
        {
            bag.Clear();
            for (int i = 0; i < 27; i++)
                for (int j = 0; j < letterCount[i]; j++)
                    bag.Add(letters[i]);
        }

        /// <summary>
        /// Peek a random letter in the bag.
        /// </summary>
        /// <returns>The letter peeked.</returns>
        public string PeekRandom()
        {
            int r = rng.Next(totalLetters);
            return bag[r];
        }
    }
}