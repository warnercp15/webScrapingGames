using System;

namespace webScrapingGames
{
    class GameInfo
    {
        public string name;
        public Boolean offer;
        public string price;
        public string score;
        public string timeToBeat;
        public string imageUrl;

        public GameInfo(string name, Boolean offer,string price, string score, string timeToBeat, string imageUrl)
        {
            this.name = name;
            this.offer = offer;
            this.price = price;
            this.score = score;
            this.timeToBeat = timeToBeat;
            this.imageUrl = imageUrl;
        }
    }
}
