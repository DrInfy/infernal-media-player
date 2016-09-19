using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Media;

namespace Imp.DirectShow.Subtitles
{
    public static class FontLoader
    {
        public const string TrueTypeFont = "application/x-truetype-font";
        public const string FontTtf = "application/x-font-ttf";
        public static readonly string AppFolder = Path.GetDirectoryName(System.Reflection.Assembly.GetEntryAssembly().Location) + Path.DirectorySeparatorChar;

        public static void SaveToDisc(byte[] bytes, string fileName)
        {
            //Writing bytes to a temporary file.
            string tempFontFileLocation = AppFolder + "temp\\" + fileName; // "testFont.ttf";
            var folder = AppFolder + "temp\\";
            if (!Directory.Exists(folder))
            {
                Directory.CreateDirectory(AppFolder + "temp\\");
            }

            if (!File.Exists(tempFontFileLocation))
            {
                File.WriteAllBytes(tempFontFileLocation, bytes);
            }

            ////Creating an instance of System.Windows.Media.GlyphTypeface.
            ////From here we will read all the needed font details.
            //var glyphTypeface = new GlyphTypeface(new Uri(tempFontFileLoation));

            ////Reading font family name
            //fontFamilyName = glyphTypeface.FamilyNames[CultureInfo.GetCultureInfo("en-US")];

            ////This is what we actually need... the right font family name, to be able to create a correct FontFamily Uri
            //string fontUri = new Uri(tempFontFileLoation.Replace(Path.GetFileName(tempFontFileLoation), ""), UriKind.RelativeOrAbsolute).AbsoluteUri + "/#" + fontFamilyName;

            ////And here is the instance of System.Windows.Media.FontFamily
            //var fontFamily = new FontFamily(fontUri);
            //return fontFamily;
        }

        public static Dictionary<string, FontFamily> LoadFromDisc()
        {
            string tempFontFileLocation = AppFolder + "temp\\";
            Dictionary<string, FontFamily> fontFamilies = new Dictionary<string, FontFamily>();
            var fonts = Fonts.GetFontFamilies(tempFontFileLocation);
            foreach (FontFamily fontFamily2 in fonts)
            {
                // Perform action.
                //var family = fontFamily2.FamilyNames.Values.First();
                var typeface = new Typeface(fontFamily2, FontStyles.Normal, FontWeights.Normal,
                    FontStretches.SemiCondensed);
                GlyphTypeface glyphs;
                typeface.TryGetGlyphTypeface(out glyphs);
                var family = glyphs.FamilyNames[CultureInfo.GetCultureInfo("en-US")];

                if (!fontFamilies.ContainsKey(family))
                {
                    fontFamilies.Add(family, fontFamily2);
                }
                //return fontFamily2;
            }

            return fontFamilies;
        }

        public static FontFamily LoadSingleGlyphTypeface(byte[] bytes, out string familyName)
        {
            try
            {
                using (var memoryPackage = new MemoryPackage())
                {
                    using (var fontStream = new MemoryStream(bytes))
                    {
                        var typefaceSource = memoryPackage.CreatePart(fontStream, null);

                        var glyphTypeface = new GlyphTypeface(typefaceSource);
                        //Reading font family name, en-us is required here
                        familyName = glyphTypeface.FamilyNames[CultureInfo.GetCultureInfo("en-US")];

                        var typeFace = new FontFamily(typefaceSource, familyName);
                        memoryPackage.DeletePart(typefaceSource);

                        return typeFace;
                    }
                }
            }
            catch (Exception ex)
            {
                Debugger.Break();
            }

            familyName = null;
            return null;
        }

        public static FontFamily LoadFontFamily(IEnumerable<byte[]> bytePackage, string familyName)
        {
            try
            {
                using (var memoryPackage = new MemoryPackage())
                {
                    List<Uri> sourceUris = new List<Uri>();

                    foreach (var bytes in bytePackage)
                    {
                        using (var fontStream = new MemoryStream(bytes))
                        {
                            var typefaceSource = memoryPackage.CreatePart(fontStream, null);
                            //var typefaceSource = memoryPackage.CreatePart(fontStream, "/#" + familyName);
                            sourceUris.Add(typefaceSource);
                            var glyphTypeface = new GlyphTypeface(typefaceSource);
                        }    
                    }

                    var first = sourceUris[0];
                    var index = first.AbsoluteUri.LastIndexOf("/");
                    var combinedUri = new Uri(first.AbsoluteUri.Substring(0, index));
                    //var combinedUri = first;

                    var fonts = Fonts.GetTypefaces(combinedUri);
                    var fontFamily = new FontFamily(first, "" + first.AbsoluteUri.Substring(index)); // + "/" + familyName);
                    fontFamily = new FontFamily(first, "./#" + familyName);
                    var family = fontFamily.FamilyNames.Values.First();
                    foreach (FontFamily fontFamily2 in Fonts.GetFontFamilies(combinedUri, "."+ first.AbsoluteUri.Substring(index)))
                    {
                        // Perform action.
                        family = fontFamily2.FamilyNames.Values.First();
                        return fontFamily2;
                    }

                    foreach (var fontFamily2 in Fonts.GetTypefaces(combinedUri, "." + first.AbsoluteUri.Substring(index)))
                    {
                        // Perform action.
                        family = fontFamily2.FontFamily.FamilyNames.Values.First();
                    }
                    //foreach (var sourceUri in sourceUris)
                    //{
                    //    memoryPackage.DeletePart(sourceUri);
                    //}
                    return fontFamily;
                }
            }
            catch (Exception ex)
            {
                Debugger.Break();
            }

            familyName = null;
            return null;
        }

        public static string LoadFontFamilyName(byte[] bytes)
        {
            try
            {
                using (var memoryPackage = new MemoryPackage())
                {
                    using (var fontStream = new MemoryStream(bytes))
                    {
                        var typefaceSource = memoryPackage.CreatePart(fontStream, null);

                        var glyphTypeface = new GlyphTypeface(typefaceSource);
                        //Reading font family name, en-us is required here
                        var familyName = glyphTypeface.FamilyNames[CultureInfo.GetCultureInfo("en-US")];

                        memoryPackage.DeletePart(typefaceSource);
                        return familyName;
                    }
                }
            }
            catch (Exception ex)
            {
                Debugger.Break();
            }
            
            return null;
        }
    }
}
