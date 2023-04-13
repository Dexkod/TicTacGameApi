namespace WebApplication1.Entities
{
    public class Game
    {
        public readonly Guid Id;
        public readonly string Player1;
        public readonly string Player2;
        public string Result { get; private set; }
        private string Motion;
        private Dictionary<string, string> KeyMotionPlayers;


        private List<List<string>> field = new List<List<string>>()
            {
                new List<string>{ null , null , null },
                new List<string> { null, null, null },
                new List<string> { null, null, null }
            };

        public string this[int column, int row]
        {
            get
            {
                return field[row][column];
            }
        }

        public Game(string player1, string player2)
        {
            Id = Guid.NewGuid();
            Player1 = player1;
            Player2 = player2;
            Motion = "X";
            Result = "";
            LinkingPlayersToSymbols();
            OrganizerMatch.AddGame(this);
        }

        public List<List<string>> GetField()
        {
            return field;
        }

        public List<List<string>> Move(int column, int row, out string message)
        {
            if (field[row][column] != null)
            {
                message = "Ставить можно только на пустом месте";
                return field;
            }

            field[row][column] = Motion;

            if (IsCheckWin())
            {
                message = $"Игра окончена, победил {KeyMotionPlayers[Result.Split(':')[0]]}";
                return field;
            }

            if (IsDraw())
            {
                Result = "Draw";
                message = "Игра окончена, ничья";
                return field;
            }

            message = Result;
            Motion = Motion == "X" ? "O" : "X";
            return field;
        }
        public string MessageStartGame()
        {
            return $"Приятной игры {Player1} и {Player2} " +
                $"Первый ход за {KeyMotionPlayers["X"]}";
        }

        private bool IsDraw()
        {
            return !field.Any(x => x.Any(y => y == null));
        }
        private bool IsCheckWin()
        {
            for (int i = 0; i < 3; i++)
            {
                if (!(field[i].Contains("X") && field[i].Contains("O")) &&
                    !field[i].Contains(null))
                {
                    Result = field[i][0] + ": Win";
                    return true;
                }
            }

            for (int i = 0; i < 3; i++)
            {
                if (field[0][i] == field[1][i] && field[0][i] == field[2][i] &&
                    field[0][i] != null)
                {
                    Result = field[0][i] + ": Win";
                    return true;
                }
            }

            if ((field[0][0] == field[1][1] && field[2][2] == field[0][0] ||
                field[0][2] == field[1][1] && field[0][2] == field[2][0]) &&
                field[1][1] != null)
            {
                Result = field[1][1] + ": Win";
                return true;
            }

            return false;
        }
        private void LinkingPlayersToSymbols()
        {
            Random rnd = new();
            int number = rnd.Next(1, 3);

            switch (number)
            {
                case 1:
                    KeyMotionPlayers = new Dictionary<string, string>()
                    {
                        {"X", Player1},
                        {"O", Player2}
                    };
                    break;
                case 2:
                    KeyMotionPlayers = new Dictionary<string, string>()
                    {
                        {"O", Player1},
                        {"X", Player2}
                    };
                    break;
            }
        }
    }
}
