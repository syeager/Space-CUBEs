// Little Byte Games
// Author: Steve Yeager
// Created: 2014.08.24
// Edited: 2014.09.17

using System.Collections.Generic;
using System.Linq;

namespace SpaceCUBEs
{
    public static class Scenes
    {
        #region Public Fields

        public enum Levels
        {
            TheAbyss,
            NebulaForest,
            ForsakenColonies,
            TheCapital,
            GalacticCore,
        }

        public enum Menus
        {
            MainMenu,
            LevelSelectMenu,
            Store,
            Garage,
            Workshop,
        }

        #endregion

        #region Const Fields

        private static readonly Dictionary<Levels, string> LevelNames = new Dictionary<Levels, string>
                                                                        {
                                                                            {Levels.TheAbyss, "The Abyss"},
                                                                            {Levels.NebulaForest, "Nebula Forest"},
                                                                            {Levels.ForsakenColonies, "Forsaken Colonies"},
                                                                            {Levels.TheCapital, "The Capital"},
                                                                            {Levels.GalacticCore, "Galactic Core"},
                                                                        };

        private static readonly Dictionary<Menus, string> MenuNames = new Dictionary<Menus, string>
                                                                        {
                                                                            {Menus.MainMenu, "Main Menu"},
                                                                            {Menus.LevelSelectMenu, "Level Select Menu"},
                                                                            {Menus.Store, "Store"},
                                                                            {Menus.Garage, "Garage"},
                                                                            {Menus.Workshop, "Workshop"},
                                                                        };

        #endregion

        public static string Scene(Levels level)
        {
            return LevelNames[level];
        }


        public static string Scene(Menus menu)
        {
            return MenuNames[menu];
        }


        public static Levels Level(string level)
        {
            return LevelNames.First(levelName => levelName.Value == level).Key;
        }


        public static Menus Menu(string menu)
        {
            return MenuNames.First(key => key.Value == menu).Key;
        }
    }
}