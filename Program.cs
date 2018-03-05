/*
 * Created by SharpDevelop.
 * User: Wojtas
 * Date: 05.03.2018
 * Time: 14:09
 * 
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */
using System;

namespace STR_utf8_utf16_unicode_converter
{
	class Program
	{
		public static void Main(string[] args)
		{
			string s = "😀";
			
			int wartoscUnicode = dekodujUTF16DoWartosciUnicode(s);
			int[] utf8 = dekodujWartoscUnicodeDoUTF8(wartoscUnicode);
			utf8 = dekodujUTF16doUTF8(s);
			
			Console.WriteLine("Znak: " + s);
			Console.WriteLine("Dalsze zapisy heksadecymalne w big-endian!!!");
			Console.WriteLine("Wartosc znaku w unicode: {0:X}", wartoscUnicode);
			
			Console.Write("Zapis w utf-16: ");
			for (int i = 0; i < s.Length; i++) {
				Console.Write("{0:X} ", (int)s[i]);
			}
			Console.WriteLine();
			
			Console.Write("Zapis w utf-8: ");
			for (int i = 0; i < utf8.Length; i++) {
				Console.Write("{0:X} ", utf8[i]);
			}
			Console.WriteLine();
			
			Console.Write("Press any key to continue . . . ");
			Console.ReadKey(true);
		}
		
		private static int dekodujUTF16DoWartosciUnicode(string s){
			if(s[0] >= 0xD800){
				int starszeBity = (s[0] - 0xD800) << 10;
				int mlodszeBity = (s[1] - 0xDC00);
				return 0x10000 | starszeBity | mlodszeBity;
			}
			return (int)s[0];
		}
		
		private static int dekodujUTF8DoWartosciUnicode(int[] utf8){
			if (utf8.Length == 1) {
				return utf8[0];
			}
			
			int maskaBitowaPierwszegoWyrazu = 0;
			for (int i = 0; i < 7 - utf8.Length; i++) {
				maskaBitowaPierwszegoWyrazu = maskaBitowaPierwszegoWyrazu << 1;
				maskaBitowaPierwszegoWyrazu = maskaBitowaPierwszegoWyrazu | 1;
			}
			int wartosc = utf8[0] & maskaBitowaPierwszegoWyrazu;
			
			for (int i = 1; i < utf8.Length; i++) {
				wartosc = wartosc << 6;
				wartosc = wartosc | (utf8[i] & 0x3f);
			}
			
			return wartosc;
		}
		
		private static int[] dekodujWartoscUnicodeDoUTF8(int wartosc){
			int dlugosc;
			int maskaPierwszegoBajtu;
			
			if (wartosc <= 0x7f) {
				int[] temp = new int[1];
				temp[0] = wartosc;
				return temp;
			}
			
			if (wartosc <= 0x7ff) {
				dlugosc = 2;
				maskaPierwszegoBajtu = 0xc0;
			}else if (wartosc <= 0xffff) {
				dlugosc = 3;
				maskaPierwszegoBajtu = 0xe0;
			}else if (wartosc <= 0x1fffff) {
				dlugosc = 4;
				maskaPierwszegoBajtu = 0xf0;
			}else if (wartosc <= 0x3ffffff) {
				dlugosc = 5;
				maskaPierwszegoBajtu = 0xf8;
			}else{
				dlugosc = 6;
				maskaPierwszegoBajtu = 0xfc;
			}
			
			int[] utf8 = new int[dlugosc];
			
			utf8[0] = maskaPierwszegoBajtu;
			
			for (int i = 1; i < dlugosc; i++) {
				utf8[i] = 0x80;
			}
			
			for (int i = dlugosc-1; i >= 0; i--) {
				int tmp_wartosc = wartosc & 0x3f;
				wartosc = wartosc >> 6;
				utf8[i] = utf8[i] | tmp_wartosc;
			}
			
			return utf8;
		}
		
		private static string dekodujWartoscUnicodeDoUTF16(int wartosc){
			if(wartosc < 0xd800){
				char c = (char)wartosc;
				return c.ToString();
			}
			wartosc -= 0x10000;
			int starszeBity = (wartosc >> 10) + 0xd800;
			int mlodszeBity = (wartosc & 0x3ff) + 0xdc00;
			
			char starszy = (char)starszeBity;
			char mlodszy = (char)mlodszeBity;
			return starszy.ToString() + mlodszy.ToString();
		}
		
		private static int[] dekodujUTF16doUTF8(string s){
			return dekodujWartoscUnicodeDoUTF8(dekodujUTF16DoWartosciUnicode(s));
		}
		
		private static string dekodujUTF8doUTF16(int[] utf8){
			return dekodujWartoscUnicodeDoUTF16(dekodujUTF8DoWartosciUnicode(utf8));
		}
	}
}