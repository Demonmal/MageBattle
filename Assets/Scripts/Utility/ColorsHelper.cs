using UnityEngine;

namespace MageBattle.Utility
{
    public static class ColorsHelper
    {
        //Mesh colors
        public static readonly Color minePlayerColor;
        public static readonly Color defaultMeshColor;
        public static readonly Color activePlayerColor;
        public static readonly string minePlayerColorTag = "#26FF00";
        public static readonly string defaultMeshColorTag = "#BB00FF";
        public static readonly string activePlayerColorTag = "#FF7300";

        static ColorsHelper()
        {
            ColorUtility.TryParseHtmlString(minePlayerColorTag, out minePlayerColor);
            ColorUtility.TryParseHtmlString(defaultMeshColorTag, out defaultMeshColor);
            ColorUtility.TryParseHtmlString(activePlayerColorTag, out activePlayerColor);
        }
    }
}