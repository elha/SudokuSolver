using SudokuSolver;

var x = new Puzzle(new string[] {
    "200800003",
    "050026009",
    "007000005",
    "100034090",
    "000600000",
    "010000300",
    "300092040",
    "000080000",
    "000030004"
   });

if(x.Solve())
{
    foreach (var line in x.GetState())
        Console.WriteLine(line);
}
else
{
    Console.WriteLine("not solvable");
}
