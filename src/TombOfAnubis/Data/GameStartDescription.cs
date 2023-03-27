namespace TombOfAnubis
{
    public class GameStartDescription
    {
        private string mapContentName;


        /// <summary>
        /// The content name of the  map for a new game.
        /// </summary>
        public string MapContentName
        {
            get { return mapContentName; }
            set { mapContentName = value; }
        }

        private int numberOfPlayers;
        public int NumberOfPlayers
        {
            get { return numberOfPlayers; }
            set { numberOfPlayers = value; }
        }

    }
}
