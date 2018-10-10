using System;
using UIKit;

namespace Messenger.iOS.Extensions {

    /// <summary>
    /// String extensions
    /// </summary>
    public static class StringExtensions {

        /// <summary>
        /// Convert a hex string to a <c>UIColor</c>
        /// </summary>
        /// <param name="hexString">The hex color string to convert</param>
        /// <returns></returns>
        public static UIColor ToUIColor(this string hexString) {
            hexString = hexString.Replace("#", "");

            if (hexString.Length == 3)
                hexString = hexString + hexString;

            if (hexString.Length != 6)
                throw new Exception("Invalid hex string");

            int red = Int32.Parse(hexString.Substring(0, 2), System.Globalization.NumberStyles.AllowHexSpecifier);
            int green = Int32.Parse(hexString.Substring(2, 2), System.Globalization.NumberStyles.AllowHexSpecifier);
            int blue = Int32.Parse(hexString.Substring(4, 2), System.Globalization.NumberStyles.AllowHexSpecifier);

            return UIColor.FromRGB(red, green, blue);
        }
    }
}