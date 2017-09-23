using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Text.RegularExpressions;

namespace SKClassLibrary
{
    public class SKValidations
    {
        /// <summary>
        /// To capitalise given string
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public string Capitalise(string input)
        {
            if (!isEmpty(input))
            {
                input = input.Trim().ToLower();

                string[] words = input.Split(new char[] { ' ', '\t', '\n' });
                input = "";

                foreach(string word in words)
                {
                    input += toUpperFirstLetter(word) + " ";
                }
            }
            return input.TrimEnd(' ');
        }

        /// <summary>
        /// To upper only first letter
        /// </summary>
        /// <param name="word">To be capitalised</param>
        /// <returns>Capitalised string</returns>
        public string toUpperFirstLetter(string word)
        {
            int forTest = 0;
            if(int.TryParse(word, out forTest))
            {
                return word;
            }
                
            return word.Insert(0, ((char)((int)word[0] - 32)).ToString()).Remove(1,1);
        }


        /// <summary>
        /// To check if given obeject is empty
        /// </summary>
        /// <param name="value">Object to be checked</param>
        /// <returns>True when the object is empty</returns>
        public bool isEmpty(object value)
        {
            if (value == null || value.ToString() == "")
                return true;
            return false;
        }

        /// <summary>
        /// Extract only number from given obejct
        /// </summary>
        /// <param name="value"></param>
        /// <returns>Sequence of number</returns>
        public string convertToNumber(object value)
        {
            string temp = "";
            Regex digit = new Regex(@"\d");

            if(!isEmpty(value))
            {
                MatchCollection matches = digit.Matches(value.ToString());
                foreach(Match match in matches)
                {
                    temp += match.Value;
                }
            }
            return temp;
        }

        /// <summary>
        /// To convert 
        /// </summary>
        /// <param name="str"></param>
        /// <param name="digitNumbers"></param>
        /// <param name="splitor"></param>
        /// <returns></returns>
        public string convertToPhoneNumber(object str, int[] digitNumbers, char splitor = '-')
        {
            string givenNumbers = "";
            string temp = "";
            givenNumbers = convertToNumber(str);

            int i = 0;
            int j = 0;
            int count = 0;
            while (i < givenNumbers.Length)
            {
                temp += givenNumbers[i++];
                count++;
                if (count == digitNumbers[j] && j < digitNumbers.Length - 1)
                {
                    count = 0;
                    j++;
                    temp += splitor;
                }
            }
            return temp;
        }

        /// <summary>
        /// To count how many integer numbers are contained in the given string
        /// </summary>
        /// <param name="str">String to be checked</param>
        /// <returns>Number of integer</returns>
        public int countIntger(string str)
        {
            int count = 0;
            int len = str.Length;

            for (int i = 0; i < len; i++)
            {
                int strAscii = (int)str[i];
                if (strAscii >= (int)'0' && strAscii <= '9') count++;
            }
            return count;
        }

    }
}
