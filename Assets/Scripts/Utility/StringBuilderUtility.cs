using System;
using System.Text;
using UnityEngine;

namespace MageBattle.Utility
{
    public class StringBuilderUtility : MonoBehaviour
    {
        private static readonly StringBuilder _builder = new StringBuilder(64);

        public static StringBuilder builder
        {
            get
            {
                _builder.Clear();
                return _builder;
            }
        }

        public static string GetColoredString(Color color, object message)
        {
            var colorHex = ColorToHex(color);
            _builder.Clear();
            _builder.Append("<color=");
            _builder.Append(colorHex);
            _builder.Append('>');
            _builder.Append(message);
            _builder.Append("</color>");
            return _builder.ToString();
        }

        public static string GetBoldString(object message)
        {
            _builder.Clear();
            _builder.Append("<b>");
            _builder.Append(message);
            _builder.Append("</b>");
            return _builder.ToString();
        }

        public static string ColorToHex(Color color)
        {
            _builder.Clear();
            _builder.Append("#");
            _builder.Append(Float01ToHexString(color.r));
            _builder.Append(Float01ToHexString(color.g));
            _builder.Append(Float01ToHexString(color.b));
            return _builder.ToString();
        }

        private static string Float01ToHexString(float f)
        {
            return Convert.ToInt32(Mathf.Clamp01(f) * 255).ToString("X2");
        }
    }
}