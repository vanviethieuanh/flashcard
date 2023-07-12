using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;
using System.IO;

namespace Flashcards.Class
{
    public class Question
    {
        string ques;
        List<string> answers;
        result userSummary;
        int incorrectIndex;

        public string Ques { get => ques; set => ques = value; }
        public List<string> Answers { get => answers; set => answers = value; }
        public result UserSummary { get => userSummary; set => userSummary = value; }
        public int IncorrectIndex { get => incorrectIndex; set => incorrectIndex = value; }

        public enum result { Correct, Incorrect, TimeOver, Skip }
        public static string[] Type = new string[] 
        {
            "noun","adjective","adverb","pronoun","verb",
            "preposition","conjunction",
            "definite article","indefinite article"
        };

        public static List<Question> Generate(int numberOfQues)
        {
            List<Question> result = new List<Question>();
            List<string> librariesPath = Directory.GetFiles(Link.PathOfLibraryFolder).ToList();
            Random r = new Random();
            List<XElement> words = new List<XElement>();

            do
            {
                int randomIndex = r.Next(0,librariesPath.Count-1);
                words.AddRange(XDocument.Load(librariesPath[randomIndex]).Descendants("Word"));
                librariesPath.RemoveAt(randomIndex);
            }
            while (!(words.Count > 10 || librariesPath.Count == 0));

            for (int i = 0; i < 10; i++)
            {
                if (words.Count >= 4)
                {
                    int iCase = r.Next(0, 5);
                    switch (iCase)
                    {
                        case 0:
                            result.Add(QuesDescription(words));
                            break;
                        case 1:
                            result.Add(QuesDescriptions(words));
                            break;
                        case 2:
                            result.Add(QuesTranslation(words));
                            break;
                        case 3:
                            result.Add(QuesTranslations(words));
                            break;
                        default:
                            result.Add(QuesTypes(words));
                            break;
                    }
                }
                else {
                    result.Add(QuesTypes(words));
                }                
            }
            return result;
        }

        public static Question QuesDescriptions(IEnumerable<XElement> Iwords)
        {
            Question result = new Question();
            Random r = new Random();
            List<XElement> words = Iwords.ToList();
            int numOfWord = words.Count;
            int indexAnswer = r.Next(0, numOfWord - 1);
            XElement answer = words[indexAnswer];
            result.Answers = new List<string>();

            while (answer.Descendants("Translation").Attributes("Description").Count() <= 0
                || answer.Descendants("Translation").Count() <= 0)
            {
                indexAnswer = r.Next(0, numOfWord - 1);
                answer = words[indexAnswer];
            }

            words.RemoveAt(indexAnswer);
            result.Ques = answer.Attribute("ThisWord").Value;
            result.Answers.Add(answer.Descendants("Translation").First().Attribute("Description").Value);

            List<XElement> listTrans = words.Descendants("Translation").ToList();

            for (int i = 1; i <= 3; i++)
            {
                int ans = r.Next(0, listTrans.Count);
                result.Answers.Add(listTrans[ans].Attribute("Description").Value);
                listTrans.RemoveAt(ans);
            }

            return result;
        }
        public static Question QuesDescription(IEnumerable<XElement> Iwords)
        {
            Question result = new Question();
            Random r = new Random();
            List<XElement> words = Iwords.ToList();
            int numOfWord = words.Count;
            int indexAnswer = r.Next(0, numOfWord - 1);
            XElement answer = words[indexAnswer];
            result.Answers = new List<string>();

            while (answer.Descendants("Translation").Attributes("Description").Count() <= 0
                || answer.Descendants("Translation").Count() <= 0)
            {
                indexAnswer = r.Next(0, numOfWord - 1);
                answer = words[indexAnswer];
            }

            words.RemoveAt(indexAnswer);
            result.Ques = string.Format(@"""{0}"" is description for:",
                            answer.Descendants("Translation").First().Attribute("Description").Value); 
            result.Answers.Add(answer.Attribute("ThisWord").Value);

            for (int i = 1; i <= 3; i++)
            {
                int ans = r.Next(0, words.Count);
                result.Answers.Add(words[ans].Attribute("ThisWord").Value);
                words.RemoveAt(ans);
            }

            return result;
        }
        public static Question QuesTranslation(IEnumerable<XElement> ws)
        {
            Random r = new Random();
            Question result = new Question();
            result.Answers = new List<string>();
            List<XElement> words = ws.ToList();
            int indexAnswer = r.Next(0, words.Count - 1);
            XElement answer = words[indexAnswer];
            
            while (answer.Descendants("Translation").Count() <= 0)
            {
                indexAnswer = r.Next(0, words.Count - 1);
                answer = words[indexAnswer];
            }
            words.RemoveAt(indexAnswer);

            result.Ques = (string.Format(@"""{0}"" is translation for:",answer.Descendants("Translation").First().Attribute("Trans").Value));
            result.Answers.Add(answer.Attribute("ThisWord").Value);

            for (int i = 1; i <= 3; i++)
            {
                int a = r.Next(0, words.Count - 1);
                result.Answers.Add(words[a].Attribute("ThisWord").Value);
                words.RemoveAt(a);
            }
            
            return result;
        }
        public static Question QuesTranslations(IEnumerable<XElement> Iwords)
        {
            Question result = new Question();
            Random r = new Random();
            List<XElement> words = Iwords.ToList();
            result.Answers = new List<string>();
            int indexAnswer = r.Next(1, words.Count - 1);
            XElement answer = words[indexAnswer];

            while (answer.Descendants("Translation").Attributes("Description").Count() <= 0
                || answer.Descendants("Translation").Count() <= 0)
            {
                indexAnswer = r.Next(0, words.Count - 1);
                answer = words[indexAnswer];
            }
            words.RemoveAt(indexAnswer);

            result.Ques = string.Format(@"""{0}"" is mean:", answer.Attribute("ThisWord").Value);
            result.Answers.Add(answer.Descendants("Translation").First().Attribute("Trans").Value);


            List<XElement> trans = words.Descendants("Translation").ToList();
            for (int i = 1; i <= 3; i++)
            {
                int index = r.Next(0, trans.Count - 1);
                result.Answers.Add(trans[index].Attribute("Trans").Value);
                trans.RemoveAt(index);
            }
            return result;
            
        }
        public static Question QuesTypes(List<XElement> words)
        {
            Question result = new Question();
            Random r = new Random();
            int index_ans = 0;
            XElement answer = words[0];
            result.Answers = new List<string>();

            while (answer.Attributes("Type").Count() <= 0)
            {
                index_ans = r.Next(0, words.Count);
                answer = words[index_ans];
            }
            words.RemoveAt(index_ans);

            result.Ques = string.Format(@"""{0}"" is:",answer.Attribute("ThisWord").Value);
            result.Answers.Add(answer.Attribute("Type").Value.Trim());

            List<string> _type = Type.ToList();
            _type.Remove(result.Answers[0]);
            for (int i = 1; i <= 3 ; i++)
            {
                int indexType = r.Next(0, _type.Count - 1);
                result.Answers.Add(_type[indexType]);
                _type.RemoveAt(indexType);
            }

            return result;
        }
    }
}
