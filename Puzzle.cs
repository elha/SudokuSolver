using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SudokuSolver
{
    internal class Puzzle
    {
        public int[][] Points;

        /// <summary>
        /// Strings with Chars from Zero = unknown and 1-9
        /// </summary>
        /// <param name="state"></param>
        /// <exception cref="ArgumentException"></exception>
        public Puzzle(string[] state)
        {
            if (state.Length != 9)
                throw new ArgumentException("needs to have 9 rows");

            foreach (string line in state)
            {
                if (line.Length != 9)
                    throw new ArgumentException("needs to have 9 chars");
            }

            Points = state.Select((line) => line.ToCharArray().Select(c => int.Parse(c.ToString())).ToArray()).ToArray();
        }

        public bool Solve()
        {
            int unsolved = int.MaxValue;
            int unsolvedlast = 0;

            do
            {
                unsolvedlast = unsolved;
                unsolved = 0;

                var Possible = GetPossibles();
                if (Possible == null) return false; // not solveable

                // try every number in every slot if mandatory solution
                for (int x = 0; x < 9; x++)
                    for (int y = 0; y < 9; y++)
                    {      
                        if (Possible[x][y].Count == 1)
                        {
                            Points[x][y] = Possible[x][y][0];
                        }
                        else
                        {
                            var bFound = false;
                            foreach (int n in Possible[x][y])
                            {
                                // line
                                if (!bFound)
                                {
                                    var locations1 = Possible[x].Where(point => point.Contains(n)).ToList();
                                    if (locations1.Count == 1)
                                    {
                                        Points[x][y] = n;
                                        bFound = true;
                                    };
                                }

                                //row
                                if (!bFound)
                                {
                                    var locations2 = Possible.Where(point => point[y].Contains(n)).ToList();
                                    if (locations2.Count == 1)
                                    {
                                        Points[x][y] = n;
                                        bFound = true;
                                    };
                                }

                                //quadrant
                                if (!bFound)
                                {
                                    var x2 = x / 3 * 3;
                                    var y2 = y / 3 * 3;
                                    var locations3 = new List<int>();
                                    for (int dx = 0; dx < 3; dx++)
                                        for (int dy = 0; dy < 3; dy++)
                                            if (Possible[x2 + dx][y2 + dy].Contains(n)) locations3.Add(n);

                                    if (locations3.Count == 1)
                                    {
                                        Points[x][y] = n;
                                        bFound = true;
                                    };
                                }
                            }
                        }

                        if (Points[x][y] == 0) unsolved++;
                    }
            }
            while (unsolvedlast > unsolved);



            if (unsolved == 0)
            {
                return true;
            }
            else
            {
                // add random and recurse
                var Possible = GetPossibles();
                (int x, int y) = FindShortestPossibles(Possible);

                foreach (var n in Possible[x][y])
                {
                    string[] newstate = GetState();

                    // change state with random-try
                    string line = newstate[x];
                    line = line.Substring(0, y) + n.ToString();
                    if (y <= 8) line += newstate[x].Substring(y + 1);
                    newstate[x] = line;

                    // try this
                    var puzzlerecurse = new Puzzle(newstate);

                    if (puzzlerecurse.Solve())
                    {
                        this.Points = puzzlerecurse.Points;
                        return true;
                    }
                }

                return false; // not solveable
            }
        }

        public string[] GetState()
        {
            return Points.Select(line => string.Join("", line.Select(point => point.ToString()))).ToArray();
        }

        private (int x, int y) FindShortestPossibles(List<int>[][] possible)
        {
            int x = -1;
            int y = -1;
            int no = int.MaxValue;

            for (int x2 = 0; x2 < 9; x2++)
                for (int y2 = 0; y2 < 9; y2++)
                {
                    if(no>possible[x2][y2].Count && possible[x2][y2].Count>1)
                    {
                        x = x2;
                        y = y2;
                        no = possible[x2][y2].Count;
                    }
                }

            return (x, y);

        }

        private List<int>[][] GetPossibles()
        {

            List<int>[][] Possible = new List<int>[9][];
            for (int i = 0; i < 9; i++)
                Possible[i] = new List<int>[9];

            // calc possibles
            for (int x = 0; x < 9; x++)
                for (int y = 0; y < 9; y++)
                {
                    Possible[x][y] = FindPossibleNumbers(x, y);
                    if (Possible[x][y].Count == 0)
                        return null; // defunct puzzle
                }

            return Possible;
        }

        public List<int> FindPossibleNumbers(int x, int y)
        {
            List<int> result = new();
            if (Points[x][y] > 0) return new List<int>() { Points[x][y] };

            // loop from 1 to 9 
            // checking if number is possible on x/y slot
            for (int i = 1; i <= 9; i++)
                if (CheckValidSolution(x, y, i))
                    result.Add(i);

            return result;
        }

        private bool CheckValidSolution(int x, int y, int i)
        {
            // line
            foreach (int p in Points[x])
                if (p == i) return false;

            //row
            foreach (int[] row in Points)
                if (row[y] == i) return false;

            //quadrant
            var x2 = x / 3 * 3;
            var y2 = y / 3 * 3;

            for (int dx = 0; dx < 3; dx++)
                for (int dy = 0; dy < 3; dy++)
                    if (Points[x2 + dx][y2 + dy] == i) return false;

            return true;
        }
    }
}
