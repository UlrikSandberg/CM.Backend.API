using System;
using System.Collections.Generic;
using System.Linq;
using CM.Backend.Domain.SharedValueObjects;

namespace CM.Backend.Domain.Aggregates.User.ValueObjects
{
    public class Language : SingleValueObject<string>
    {
        public Language(string value) : base(value)
        {
            if (!_twoLetterISOLanguageCode.Contains(value) && !_threeLetterISOLanguageCode.Contains(value))
            {
                throw new ArgumentException(nameof(value) + ": Incompatible LanguageCode");
            }
        }
        
        //Language code have been generated by the .Net assembly System.Globalization --> Culture:
        //The codes where generated below code: -->
        
        /*CultureInfo[] cultures = CultureInfo.GetCultures(CultureTypes.SpecificCultures);

            foreach (var culture in cultures)
            {
                try
                {
                    var region = new RegionInfo(culture.LCID);
    
                    twoLetterISOLanguageCode.Add(culture.TwoLetterISOLanguageName);
				    threeLetterISOLanguageCode.Add(culture.ThreeLetterISOLanguageName);
                }
                catch (CultureNotFoundException)
                {
                    Console.WriteLine("Culture not found: " + culture.Name);
                }
        }*/
        
        private HashSet<string> _twoLetterISOLanguageCode = new HashSet<string> {
            "af",
            "am",
            "ar",
            "as",
            "az",
            "be",
            "bg",
            "bn",
            "bo",
            "br",
            "bs",
            "ca",
            "cs",
            "cy",
            "da",
            "de",
            "dsb",
            "dz",
            "el",
            "en",
            "es",
            "et",
            "eu",
            "fa",
            "fi",
            "fil",
            "fo",
            "fr",
            "fy",
            "ga",
            "gd",
            "gl",
            "gsw",
            "gu",
            "haw",
            "he",
            "hi",
            "hr",
            "hsb",
            "hu",
            "hy",
            "id",
            "ig",
            "ii",
            "is",
            "it",
            "ja",
            "ka",
            "kk",
            "kl",
            "km",
            "kn",
            "ko",
            "kok",
            "ky",
            "lb",
            "lo",
            "lt",
            "lv",
            "mk",
            "ml",
            "mn",
            "mr",
            "ms",
            "mt",
            "my",
            "nb",
            "ne",
            "nl",
            "nn",
            "om",
            "or",
            "pa",
            "pl",
            "ps",
            "pt",
            "rm",
            "ro",
            "ru",
            "rw",
            "sah",
            "se",
            "si",
            "sk",
            "sl",
            "smn",
            "so",
            "sq",
            "sr",
            "sv",
            "sw",
            "ta",
            "te",
            "th",
            "ti",
            "tk",
            "tr",
            "ug",
            "uk",
            "ur",
            "uz",
            "vi",
            "yi",
            "yo",
            "zu"};
        
        private HashSet<string> _threeLetterISOLanguageCode = new HashSet<string> {
            "afr",
            "amh",
            "ara",
            "asm",
            "aze",
            "bel",
            "bul",
            "ben",
            "bod",
            "bre",
            "bos",
            "cat",
            "ces",
            "cym",
            "dan",
            "deu",
            "dsb",
            "dzo",
            "ell",
            "eng",
            "spa",
            "est",
            "eus",
            "fas",
            "fin",
            "fil",
            "fao",
            "fra",
            "fry",
            "gle",
            "gla",
            "glg",
            "gsw",
            "guj",
            "haw",
            "heb",
            "hin",
            "hrv",
            "hsb",
            "hun",
            "hye",
            "ind",
            "ibo",
            "iii",
            "isl",
            "ita",
            "jpn",
            "kat",
            "kaz",
            "kal",
            "khm",
            "kan",
            "kor",
            "kok",
            "kir",
            "ltz",
            "lao",
            "lit",
            "lav",
            "mkd",
            "mal",
            "mon",
            "mar",
            "msa",
            "mlt",
            "mya",
            "nob",
            "nep",
            "nld",
            "nno",
            "orm",
            "ori",
            "pan",
            "pol",
            "pus",
            "por",
            "roh",
            "ron",
            "rus",
            "kin",
            "sah",
            "sme",
            "sin",
            "slk",
            "slv",
            "smn",
            "som",
            "sqi",
            "srp",
            "swe",
            "swa",
            "tam",
            "tel",
            "tha",
            "tir",
            "tuk",
            "tur",
            "uig",
            "ukr",
            "urd",
            "uzb",
            "vie",
            "yid",
            "yor",
            "zul"};
    }
}