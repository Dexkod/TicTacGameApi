namespace WebApplication1.Entities
{
    public static class OrganizerMatch
    {
        private static List<Game> allGames = new List<Game>();
        public static void AddGame(Game game)
        {
            allGames.Add(game);
        }

        public static Game SearchGame(string id)
        {
            return allGames.FirstOrDefault(x => x.Id.ToString().Equals(id));
        }

    }
}
