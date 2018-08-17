using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MemoryChallenge_Adapter.Common;
using MemoryChallenge_Adapter.Game;

namespace MemoryChallenge.Common
{
    public static class GameCreator
    {
        private const int startTilesCount = 4;

        public static GameZone StartGame()
        {
            return GenerateZone(1);
        }

        public static GameZone GenerateZone(int level)
        {
            var tilesCount = startTilesCount + level;

            var gameZone = new GameZone();
            gameZone.CorrectColor = chooseRandomColor();

            for (int i = 0; i < tilesCount; i++)
            {
                var generatedTile = generateGameTile(i);
                gameZone.GameTiles.Add(generatedTile);

                if(generatedTile.Color == gameZone.CorrectColor)
                {
                    gameZone.CountCorrectTiles++;
                }
            }

            if(!gameZone.GameTiles.Any(x=>x.Color == gameZone.CorrectColor))
            {
                gameZone = GenerateZone(level);
            }

            return gameZone;
        }

        private static GameTile generateGameTile(int position)
        {
            var gameTile = new GameTile();

            gameTile.Position = position;
            gameTile.Color = chooseRandomColor();

            return gameTile;
        }

        private static Random random = new Random();

        private static GameColors chooseRandomColor()
        {
            var colors = Enum.GetValues(typeof(GameColors));
            var randomColor = (GameColors)colors.GetValue(random.Next(colors.Length));

            return randomColor;
        }
    }
}
