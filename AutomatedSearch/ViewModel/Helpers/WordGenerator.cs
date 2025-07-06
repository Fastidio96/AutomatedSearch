using System;

namespace AutomatedSearch.ViewModel.Helpers
{
    public class WordGenerator
    {
        private const string CHARSET = "abcdefghijklmnopqrstuvwxyz";

        public static string GetRandomString()
        {
            string result = string.Empty;
            Random rand = new Random();

            for (int i = 0; i < rand.Next(5, 15); i++)
            {
                int index = rand.Next(CHARSET.Length);
                result += CHARSET[index];

            }

            return result;
        }



        //public static string GetStringFromFile(string filePath)
        //{
        //    string result = string.Empty;

        //    if (!File.Exists(filePath))
        //    {
        //        return result;
        //    }


        //    try
        //    {
        //        string[] wordList = File.ReadAllLines(filePath);

        //        Random rand = new Random();
        //        int rand_num = rand.Next(wordList.Length);

        //        result = wordList[rand_num];
        //    }
        //    catch
        //    {
        //    }

        //    return result;
        //}
    }
}
