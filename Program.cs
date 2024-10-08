using System;
using System.Collections.Generic;
using System.Linq;

class PuzzleState : IComparable<PuzzleState>
{
    public int[,] Board { get; set; }  
    public PuzzleState Parent { get; set; }  
    public int Cost { get; set; }  
    public int EstimatedCost { get; set; }  

    public int ZeroX { get; set; } 
    public int ZeroY { get; set; }  

    public int CompareTo(PuzzleState other)
    {
        return EstimatedCost.CompareTo(other.EstimatedCost);
    }

    public int CalculateManhattanDistance()
    {
        int distance = 0;
        int n = 3;

        for (int i = 0; i < n; i++)
        {
            for (int j = 0; j < n; j++)
            {
                int value = Board[i, j];
                if (value != 0)
                {
                    int targetX = (value - 1) / n;
                    int targetY = (value - 1) % n;
                    distance += Math.Abs(i - targetX) + Math.Abs(j - targetY);
                }
            }
        }
        return distance;
    }

    public bool IsGoalState(int[,] goal)
    {
        for (int i = 0; i < 3; i++)
        {
            for (int j = 0; j < 3; j++)
            {
                if (Board[i, j] != goal[i, j]) return false;
            }
        }
        return true;
    }

    public List<PuzzleState> GenerateNextStates()
    {
        List<PuzzleState> nextStates = new List<PuzzleState>();
        int n = 3;
        int[][] directions = new int[][] 
        {
            new int[] { -1, 0 },  // Up
            new int[] { 1, 0 },   // Down
            new int[] { 0, -1 },  // Left
            new int[] { 0, 1 }    // Right
        };

        foreach (var dir in directions)
        {
            int newX = ZeroX + dir[0];
            int newY = ZeroY + dir[1];

            if (newX >= 0 && newX < n && newY >= 0 && newY < n)
            {
                int[,] newBoard = (int[,])Board.Clone();
                newBoard[ZeroX, ZeroY] = newBoard[newX, newY];
                newBoard[newX, newY] = 0;

                nextStates.Add(new PuzzleState
                {
                    Board = newBoard,
                    ZeroX = newX,
                    ZeroY = newY,
                    Parent = this,
                    Cost = this.Cost + 1
                });
            }
        }

        return nextStates;
    }

    public void PrintState()
    {
        for (int i = 0; i < 3; i++)
        {
            for (int j = 0; j < 3; j++)
            {
                Console.Write(Board[i, j] + " ");
            }
            Console.WriteLine();
        }
        Console.WriteLine();
    }
}

class BFS
{
    public static void Solve(PuzzleState start, int[,] goal)
    {
        Queue<PuzzleState> queue = new Queue<PuzzleState>();
        queue.Enqueue(start);

        HashSet<string> visited = new HashSet<string>();
        visited.Add(GetBoardString(start.Board));

        while (queue.Count > 0)
        {
            PuzzleState current = queue.Dequeue();

            if (current.IsGoalState(goal))
            {
                Console.WriteLine("BFS: Рішення знайдено!");
                PrintSolutionPath(current);
                return;
            }

            foreach (PuzzleState next in current.GenerateNextStates())
            {
                string boardString = GetBoardString(next.Board);
                if (!visited.Contains(boardString))
                {
                    queue.Enqueue(next);
                    visited.Add(boardString);
                }
            }
        }

        Console.WriteLine("BFS: Рішення не знайдено.");
    }

    private static void PrintSolutionPath(PuzzleState state)
    {
        Stack<PuzzleState> path = new Stack<PuzzleState>();
        while (state != null)
        {
            path.Push(state);
            state = state.Parent;
        }

        while (path.Count > 0)
        {
            path.Pop().PrintState();
        }
    }

    private static string GetBoardString(int[,] board)
    {
        return string.Join(",", board.Cast<int>());
    }
}

class RBFS
{
    public static void Solve(PuzzleState start, int[,] goal)
    {
        RecursiveBestFirstSearch(start, goal, int.MaxValue);
    }

    private static (PuzzleState, int) RecursiveBestFirstSearch(PuzzleState node, int[,] goal, int f_limit)
    {
        if (node.IsGoalState(goal))
        {
            Console.WriteLine("RBFS: Рішення знайдено!");
            PrintSolutionPath(node);
            return (node, 0);
        }

        List<PuzzleState> successors = node.GenerateNextStates();
        if (successors.Count == 0)
        {
            return (null, int.MaxValue);
        }

        foreach (var succ in successors)
        {
            succ.EstimatedCost = Math.Max(succ.Cost + succ.CalculateManhattanDistance(), node.EstimatedCost);
        }

        while (true)
        {
            successors.Sort();

            PuzzleState best = successors[0];
            if (best.EstimatedCost > f_limit)
            {
                return (null, best.EstimatedCost);
            }

            int alternative = successors.Count > 1 ? successors[1].EstimatedCost : int.MaxValue;

            var result = RecursiveBestFirstSearch(best, goal, Math.Min(f_limit, alternative));

            if (result.Item1 != null)
            {
                return result;
            }

            best.EstimatedCost = result.Item2;
        }
    }

    private static void PrintSolutionPath(PuzzleState state)
    {
        Stack<PuzzleState> path = new Stack<PuzzleState>();
        while (state != null)
        {
            path.Push(state);
            state = state.Parent;
        }

        while (path.Count > 0)
        {
            path.Pop().PrintState();
        }
    }
}

class PuzzleGenerator
{
    public static int[,] GenerateRandomPuzzle(int moves)
    {
        Random random = new Random();

        int[,] board = {
            { 1, 2, 3 },
            { 4, 5, 6 },
            { 7, 8, 0 }
        };

        int zeroX = 2, zeroY = 2;

        int[][] directions = new int[][] 
        {
            new int[] { -1, 0 },  // UP
            new int[] { 0, 1 },   // Right
            new int[] { 1, 0 },   // Down
            new int[] { 0, -1 }   // Left
        };

        for (int i = 0; i < moves; i++)
        {
            int direction = random.Next(4);

            int newX = zeroX + directions[direction][0];
            int newY = zeroY + directions[direction][1];

            if (newX >= 0 && newX < 3 && newY >= 0 && newY < 3)
            {
                board[zeroX, zeroY] = board[newX, newY];
                board[newX, newY] = 0;

                zeroX = newX;
                zeroY = newY;
            }
            else
            {
                i--;
            }
        }

        return board;
    }
}

class Program
{
    static void Main(string[] args)
    {
        int moves = 100; // Количество случайных перемещений
        int[,] startState = PuzzleGenerator.GenerateRandomPuzzle(moves);

        int[,] goalState = {
            { 1, 2, 3 },
            { 4, 5, 6 },
            { 7, 8, 0 }
        };

        // Поиск координат пустой клетки (0)
        int zeroX = 0, zeroY = 0;
        for (int i = 0; i < 3; i++)
        {
            for (int j = 0; j < 3; j++)
            {
                if (startState[i, j] == 0)
                {
                    zeroX = i;
                    zeroY = j;
                }
            }
        }

        PuzzleState start = new PuzzleState
        {
            Board = startState,
            ZeroX = zeroX,
            ZeroY = zeroY,
            Cost = 0,
            EstimatedCost = 0
        };

        Console.WriteLine("Початковий стан:");
        PrintBoard(startState);
        
        Console.WriteLine("Рішення з використанням BFS:");
        BFS.Solve(start, goalState);

        Console.WriteLine("\nРішення з використанням RBFS:");
        RBFS.Solve(start, goalState);
    }
    
    public static void PrintBoard(int[,] board)
    {
        for (int i = 0; i < 3; i++)
        {
            for (int j = 0; j < 3; j++)
            {
                Console.Write(board[i, j] + " ");
            }
            Console.WriteLine();
        }
        Console.WriteLine();
    }
}
