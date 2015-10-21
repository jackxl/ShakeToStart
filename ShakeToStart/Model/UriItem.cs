using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Xaml.Controls;
using System.Runtime.Serialization;
using System.Xml.Serialization;
using System.IO;
using System.Collections.ObjectModel;

namespace ShakeToStart.Model
{
    [DataContract]
    public class UriItem
    {
        [DataMember]
        public string name { get; set; }
        [DataMember]
        public Uri uri { get; set; }
        [DataMember]
        public Symbol symbol { get; set; }

        //TODO: this thing
        //DataContractJsonSerializer

        public const string SERIALIZEDHEADER = "UriItem;;";
        public const string SERIALIZEDFOOTER = "/UriItem";

        private const int SEPARATOR_SIZE = 2;

        public String GetSerializedUriObject()
        {
            String result = SERIALIZEDHEADER;

            result += name;
            result += ";;";
            result += uri.ToString();
            result += ";;";
            result += ((int)symbol).ToString();
            result += ";;";
            result += SERIALIZEDFOOTER;

            return result;
        }

        public static UriItem DesirializeUriString(String UriObjectString)
        {

            if(UriObjectString.Substring(0,9).Equals(SERIALIZEDHEADER))
            {
                string name = "";
                string uri = "";
                string symbol = "";

                int strl = UriObjectString.Length;
                UriObjectString = UriObjectString.Substring(9, strl-9);


                //the string that has been supplied starts with the uriitem string suffix.

                //name part
                Tuple<string, string> nameAndRemainder = getUriStringPart(UriObjectString); //pass the UriObjectString to the method that returns it in two parts. part 1 is the part and part 2 is the remainder
                name = nameAndRemainder.Item1;                                              //store the first part in the name.
                UriObjectString = nameAndRemainder.Item2;                                   //overwrite the second part in the objectString for the next parts.

                //URI Part
                Tuple<string, string> uriAndRemainder = getUriStringPart(UriObjectString);
                uri = uriAndRemainder.Item1;
                UriObjectString = uriAndRemainder.Item2;

                //symbol Part
                Tuple<string, string> symbolAndRemainder = getUriStringPart(UriObjectString);
                symbol = symbolAndRemainder.Item1;
                UriObjectString = symbolAndRemainder.Item2;

                //if all went well the parsed in string that consisted of "\(SERIALIZEDHEADER)uriname::uriuri::urisymbol\(SERIALIZEDFOOTER)" has been unwrapped
                //and now only contains the SERIALIZEDFOOTER

                if (UriObjectString.Equals(SERIALIZEDFOOTER))
                {
                    //TODO Or not TODO! tell success
                }
                else
                {
                    //no success
                }

                UriItem uriItem = new UriItem();
                uriItem.name = name;
                uriItem.uri = new Uri(uri);
                uriItem.symbol = (Symbol)Convert.ToInt32(symbol);

                return uriItem;

            }

            return new UriItem();
        }

        // This method checks each char in the uriObject until it hits two ':' chars. when the two ':' chars are being hit the method returns the passed string in two parts
        // part 1 contains the string that consists of the chars that precede the '::' sequence
        // part 2 contains the string that consits of the chars following the '::' sequence
        private static Tuple<string,string> getUriStringPart(String UriObjectBody)
        {
            string part = "";
            bool previousCharWasSemicolon = false;

            foreach (char c in UriObjectBody)
            {
                if (c == ';') //if the char is ; check if the last char also was ;
                {
                    if (previousCharWasSemicolon) // if yes. break. cause the name is finished
                    {

                        part = part.Remove(part.Length - 1);
                        break;
                    }
                    else //if no, just set the bool.
                    {
                        previousCharWasSemicolon = true;
                    }
                }
                else
                {
                    previousCharWasSemicolon = false;
                }

                part += c;
            }
            int subStringStart = part.Length + SEPARATOR_SIZE;
            int subStringEnd = UriObjectBody.Length - subStringStart;
            return new Tuple<string, string>(part, UriObjectBody.Substring(subStringStart, subStringEnd ) );
        }
    }
}
