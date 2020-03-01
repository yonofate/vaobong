/*
 * Copyright (c) 2011-2015 Håkan Edling
 *
 * This software may be modified and distributed under the terms
 * of the MIT license.  See the LICENSE file for details.
 * 
 * http://github.com/piranhacms/piranha
 * 
 */

using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Piranha.Globalization
{
	public static class GlobalizationUtils
	{
		private static readonly Dictionary<char, string> RussianCharacters;

		static GlobalizationUtils() {
			RussianCharacters = new Dictionary<char, string>()
            {
                {'А', "A"},
                {'Б', "B"},
                {'В', "V"},
                {'Г', "G"},
                {'Д', "D"},
                {'Е', "E"},
                {'Ё', "Yo"},
                {'Ж', "Zh"},
                {'З', "Z"},
                {'И', "I"},
                {'Й', "J"},
                {'К', "K"},
                {'Л', "L"},
                {'М', "M"},
                {'Н', "N"},
                {'О', "O"},
                {'П', "P"},
                {'Р', "R"},
                {'С', "S"},
                {'Т', "T"},
                {'У', "U"},
                {'Ф', "F"},
                {'Х', "X"},
                {'Ц', "C"},
                {'Ч', "Ch"},
                {'Ш', "Sh"},
                {'Щ', "Shh"},
                {'Ъ', "``"},
                {'Ы', "Y`"},
                {'Ь', "`"},
                {'Э', "E`"},
                {'Ю', "Yu"},
                {'Я', "Ya"},

                {'а', "a"},
                {'б', "b"},
                {'в', "v"},
                {'г', "g"},
                {'д', "d"},
                {'е', "e"},
                {'ё', "yo"},
                {'ж', "zh"},
                {'з', "z"},
                {'и', "i"},
                {'й', "j"},
                {'к', "k"},
                {'л', "l"},
                {'м', "m"},
                {'н', "n"},
                {'о', "o"},
                {'п', "p"},
                {'р', "r"},
                {'с', "s"},
                {'т', "t"},
                {'у', "u"},
                {'ф', "f"},
                {'х', "x"},
                {'ц', "c"},
                {'ч', "ch"},
                {'ш', "sh"},
                {'щ', "shh"},
                {'ъ', "``"},
                {'ы', "y`"},
                {'ь', "`"},
                {'э', "e`"},
                {'ю', "yu"},
                {'я', "ya"}
            };
		}

		/// <summary>
		/// Replcace russian characters by latin equivalent according to GOST(2002) transliteration table
		/// http://en.wikipedia.org/wiki/Romanization_of_Russian
		/// </summary>
		/// <param name="input">String to process</param>
		/// <returns></returns>
		public static string TransliterateRussianToLatin(this string input) {
			if (string.IsNullOrEmpty(input)) {
				return "";
			}

			if (input.All(x => x < 0x80)) {
				return input;
			}

			var sb = new StringBuilder(input.Length * 2);
			foreach (char c in input) {
				if (c < 0x80) {
					sb.Append(c);
				} else {
					string latinEquivalent;
					if (RussianCharacters.TryGetValue(c, out latinEquivalent)) {
						sb.Append(latinEquivalent);
					} else {
						sb.Append(c);
					}
				}
			}

			return sb.ToString();
		}
	}
}