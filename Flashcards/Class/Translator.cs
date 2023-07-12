using System.Collections.Generic;
using System.Text.RegularExpressions;
using System;
using System.Linq;
using System.Net;

namespace Flashcards.Class
{
    public static class Translator
    {
        #region Translate
        /// <summary>
        /// Get Pron from value of div tag
        /// </summary>
        /// <param name="html">html value of div tag</param>
        /// <returns>UTF-8 Encoded Pron</returns>
        public static string Pron(string html)
        {
            string result = "";

            Regex regex = new Regex(@"<span index="".*?"" class=""ipa"">(?<pron>.*?)</span>",
                            RegexOptions.IgnoreCase | RegexOptions.CultureInvariant | RegexOptions.Multiline | RegexOptions.Singleline);
            result = regex.Match(html).Groups["pron"].ToString();
            if (result != "")
                return string.Format("/{0}/", result);
            else
                return string.Empty;
        }

        /// <summary>
        /// Desciption a word
        /// </summary>
        /// <param name="html">div tag that contain the word</param>
        /// <returns>Descriptioning string</returns>
        public static string Description(string html)
        {
            string result;

            Regex regex_Bold = new Regex(@"<b class=""def"">(?<bold>.+?)</b>"
                                        , RegexOptions.IgnoreCase | RegexOptions.CultureInvariant | RegexOptions.Multiline | RegexOptions.Singleline);
            result = regex_Bold.Match(html).Groups["bold"].ToString();

            result = Regex.Replace(result, @"<a .*?>|</a>", " ");
            result = Regex.Replace(result, @"<span .*?>|</span>", " ");
            result.Trim();
            result = Regex.Replace(result, @"[ ]{2,}", " ", RegexOptions.None);

            return result;
        }

        /// <summary>
        /// Return the translate of a word
        /// </summary>
        /// <param name="html">div tag that contain word</param>
        /// <returns>Translated string</returns>
        public static string Translate(string html)
        {
            string result;
            Regex regex_span = new Regex(@"<span class=""trans"" lang=""vi"">(?<trans>.*?)</span>"
                                        , RegexOptions.IgnoreCase | RegexOptions.CultureInvariant | RegexOptions.Multiline | RegexOptions.Singleline);
            result = regex_span.Match(html).Groups["trans"].ToString();

            result = Regex.Replace(result, @"<a .*?>|</a>", " ");
            result.Trim();
            result = Regex.Replace(result, @"[ ]{2,}", " ", RegexOptions.None);
            result = result.Replace("\n", "");

            return result;
        }

        /// <summary>
        /// Return Example Sentences of a word
        /// </summary>
        /// <param name="html">div tag that contain word</param>
        /// <returns>List of Example Sentences</returns>
        public static string[] ExampleSentences(string html)
        {
            List<string> result = new List<string>();

            Regex RE_exSentences = new Regex(@"<span title=""Example"" class=""eg"">(?<ex>.*?)</span>", RegexOptions.IgnoreCase | RegexOptions.CultureInvariant | RegexOptions.Multiline | RegexOptions.Singleline);
            MatchCollection matchCollection = RE_exSentences.Matches(html);
            foreach (Match match in matchCollection)
            {
                string matchValue = match.Groups["ex"].ToString();
                matchValue = Regex.Replace(matchValue, @"<a .*?>|</a>", " ");
                matchValue.Trim();
                matchValue = Regex.Replace(matchValue, @"[ ]{2,}", " ", RegexOptions.None);
                result.Add(matchValue);
            }

            return result.ToArray();
        }

        /// <summary>
        /// Return relatedword of root word
        /// </summary>
        /// <param name="html">value of div tag that contain relatedword</param>
        /// <returns></returns>
        public static string RelatedWord(string html)
        {
            Regex re_word = new Regex(@"<span title=""Derived word"" class=""runon-title""><span class=""w"">(?<w>.*?)</span></span> ",
                RegexOptions.IgnoreCase | RegexOptions.CultureInvariant | RegexOptions.Multiline | RegexOptions.Singleline);
            string word = re_word.Match(html).Groups["w"].ToString();

            Regex re_type = new Regex(@"<span title="".*?"" class=""pos"">(?<t>.*?)</span>",
                RegexOptions.IgnoreCase | RegexOptions.CultureInvariant | RegexOptions.Multiline | RegexOptions.Singleline);
            string type = re_type.Match(html).Groups["t"].ToString();

            return string.Format(@"{0}:{1}", word, type);
        }

        /// <summary>
        /// return the Main word and its type [word:type]
        /// </summary>
        /// <param name="html">container</param>
        /// <returns></returns>
        public static Tuple<string,string> MainWord(string html)
        {
            Regex regex_word = new Regex(@"<h2 class=""di-title cdo-section-title-hw"">(?<word>.*?)</h2>",
                                    RegexOptions.IgnoreCase | RegexOptions.CultureInvariant | RegexOptions.Multiline | RegexOptions.Singleline);
            Regex regex_type = new Regex(@"<span title="".*?"" class=""pos"">(?<type>.*?)</span>",
                                    RegexOptions.IgnoreCase | RegexOptions.CultureInvariant | RegexOptions.Multiline | RegexOptions.Singleline);

            string word = regex_word.Match(html).Groups["word"].ToString();
            string type = regex_type.Match(html).Groups["type"].ToString();

            return new Tuple<string, string>(word,type);
        }

        /// <summary>
        /// Return list of input's translation and related's
        /// </summary>
        /// <param name="word">word</param>
        /// <returns> Input's translation is the st element</returns>
        public static List<Word> Translation(string word)
        {
            List<Word> result = new List<Word>();

            bool Extended = false;

            Word mainword = new Word();
            mainword.ThisWord = word;
            mainword.Translations = new List<Translation>();
            
            Word related_word = new Word();
            related_word.Translations = new List<Translation>();

            string html = HTML.GetHTMLForTranslate(word);

            Regex regex_container = new Regex(@"<div class=""di-head normal-entry"">(?<mainword>.*?)</div>",
                                        RegexOptions.IgnoreCase | RegexOptions.CultureInvariant | RegexOptions.Multiline | RegexOptions.Singleline);
            string container = regex_container.Match(html).Groups["mainword"].ToString();

            Tuple<string, string> t_mainword = MainWord(container);
            mainword.ThisWord = t_mainword.Item1;
            mainword.Type = t_mainword.Item2;
            mainword.Pron = Pron(container);

            Regex div = new Regex(@"<div class=""pos-body"">(?<trans>.*?)^</div>$",
                                    RegexOptions.IgnoreCase | RegexOptions.CultureInvariant | RegexOptions.Multiline | RegexOptions.Singleline);
            string Sdiv = div.Match(html).Groups["trans"].ToString();

            Regex trans = new Regex(@"<div class=""def-block pad-indent"" .*?>(?<trans>.*?)</span></div>|<div class=""runon pad-indent"">(?<extend>.*?)</div>",
                                    RegexOptions.IgnoreCase | RegexOptions.CultureInvariant | RegexOptions.Multiline | RegexOptions.Singleline);
            MatchCollection matchCollection = trans.Matches(Sdiv);
            foreach (Match match in matchCollection)
            {
                Translation _trans = new Translation();

                if (match.Groups["trans"].Success)
                {
                    string input = match.Groups["trans"].ToString();

                    _trans.Trans = Translate(input);
                    _trans.Description = Description(input);
                    _trans.ExampleSentences = ExampleSentences(match.ToString());

                    if (!Extended)
                    {
                        mainword.Translations.Add(_trans);
                    }
                    else
                    {
                        related_word.Translations.Add(_trans);
                    }

                }
                else
                {
                    if (related_word.Translations.Count > 0)
                    {
                        result.Add(related_word);
                        related_word = new Word();
                        related_word.Translations = new List<Translation>();
                    }
                    else
                    {
                        related_word = new Word();
                        related_word.Translations = new List<Translation>();
                    }

                    string input = match.Groups["extend"].ToString();

                    _trans.Description = Description(input);
                    _trans.ExampleSentences = ExampleSentences(match.ToString());
                    _trans.Trans = Translate(input);

                    related_word.Pron = Pron(input);
                    related_word.ThisWord = RelatedWord(input).Split(':')[0];
                    related_word.Type = RelatedWord(input).Split(':')[1];
                    related_word.Translations.Add(_trans);

                    Extended = true;
                }

            }
            if (related_word.ThisWord != null)
            {
                result.Add(related_word);
            }
            result.Add(mainword);

            Word seaching = result.Where(p => p.ThisWord == word).FirstOrDefault();
            result.Remove(seaching);
            result.Insert(0, seaching);

            return result;
        }

        /// <summary>
        /// Translate a long string
        /// </summary>
        /// <param name="Litter"></param>
        /// <param name="LitterLanguage"></param>
        /// <param name="OutputLanguage"></param>
        /// <returns></returns>
        public static string TranslateString(this string Litter, string LitterLanguage, string OutputLanguage)
        {
            string address = string.Format("http://www.google.com/translate_t?hl=" + OutputLanguage + "&ie=UTF8&text={0}&langpair={1}", Litter.ToURL(), LitterLanguage);
            WebClient webClient = new WebClient();
            string text = webClient.DownloadString(address);
            text = text.Substring(text.IndexOf("<span title=\"") + "<span title=\"".Length);
            text = text.Substring(text.IndexOf(">") + 1);
            text = text.Substring(0, text.IndexOf("</span>"));
            return text;
        }
        #endregion
    }
}
