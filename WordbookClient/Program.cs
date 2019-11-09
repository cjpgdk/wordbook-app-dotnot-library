using System;
using System.Linq;
using System.Threading.Tasks;

namespace WordbookClient
{
    class Program
    {
        static wordbook.cjpg.app.client.WordbookClient client = new wordbook.cjpg.app.client.WordbookClient();

        static async Task Main(string[] args)
        {
            if (args.Contains("--help") || args.Contains("-h"))
            {
                ShowHelp();
            }
            else if (args.Contains("--dictionaries"))
            {
                await ListDictionaries();
            }
            else if (args.Contains("--info"))
            {
                await ListDictionaryInfo(args);
            }
            else if (args.Contains("-w"))
            {
                await GetWordSuggestions(args);
            }
            else
            {
                ShowHelp();
            }
            Console.WriteLine("** Hit the return key to exit.");
            Console.ReadLine();

        }

        private static async Task GetWordSuggestions(string[] args)
        {
            // filter the word/phrase and language to use!
            string phrase = "";
            string language = null; // must be null or empty if not used!
            foreach (var item in args)
            {
                if (item == "-w")
                {
                    continue;
                }
                else if (item.StartsWith("-l:"))
                {
                    language = item.Replace("-l:", "");
                    // must be a valid dictionary id!
                    if (!IsDictionaryId(language))
                    {
                        language = null;
                    }
                }
                else
                {
                    phrase += $" {item}";
                }
            }

            // get suggestions for the provided word/phrase and dictionary id.
            wordbook.cjpg.app.client.Suggestions suggestions = await client.Suggestions(phrase.Trim(), language);
            if (suggestions.Items == null)
            {
                // hmm, can happen!
                Console.WriteLine("Thats wired! we got an empty result!");
            }
            else
            {
                foreach (wordbook.cjpg.app.client.Suggestion suggestion in suggestions.Items)
                {
                    Console.WriteLine($"WordId: {suggestion.Data.WordId}");
                    Console.WriteLine($"Word .: {suggestion.Word}");
                    // get definitions
                    wordbook.cjpg.app.client.Definitions definitions;
                    if (language == null)
                    {
                        // load all definitions
                        definitions = await suggestion.GetDefinitions();
                    }
                    else
                    {
                        // get definitions for chosen dictionary
                        string[] lngs = language.Split('-');
                        definitions = await suggestion.GetDefinitions(int.Parse(lngs[1]));
                    }

                    // print definitions
                    if (definitions.Items != null)
                    {
                        Console.WriteLine("============================");
                        Console.WriteLine("Definitions:");
                        foreach (var item in definitions.Items)
                        {
                            // item == KeyValuePair<string, List<wordbook.cjpg.app.client.Definition>>
                            Console.WriteLine($"Dictionary ID: {item.Key}");
                            item.Value.ForEach(def =>
                            {
                                Console.WriteLine($"Dictionary ..:{def.dictionary}:\n{def.definition}\n");
                            });

                        }
                    }
                    Console.WriteLine("============================\n");
                }
            }
        }

        /// <summary>
        /// List extended information for a dictionary
        /// </summary>
        /// <param name="args"></param>
        /// <returns></returns>
        private static async Task ListDictionaryInfo(string[] args)
        {
            // for simplicity we just fect all dictionaries, and loop for the ones we need!
            wordbook.cjpg.app.client.Dictionaries dictionaries = await client.Dictionaries();
            foreach (string dictId in args)
            {
                if (!IsDictionaryId(dictId))
                {
                    continue;
                }
                // get the matching dictionary
                wordbook.cjpg.app.client.Dictionary dict;
                dict = dictionaries.Items.Find(i => i.Id == dictId);
                if (dict != null)
                {
                    // load info and print it!
                    string info = await dict.Info();
                    string alphabet = await dict.Alphabet();
                    Console.WriteLine($"Id ......: {dict.Id}");
                    Console.WriteLine($"ShortName: {dict.ShortName}");
                    Console.WriteLine($"LongName : {dict.LongName}");
                    Console.WriteLine($"Alphabet : {alphabet}");
                    Console.WriteLine("============================\n");
                    Console.WriteLine(info);
                    Console.WriteLine("============================\n");
                }
            }
        }

        /// <summary>
        /// Check if the id is a valid dictionary id!
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        private static bool IsDictionaryId(string id)
        {
            // format must be 'sID-dID'
            string[] ids = id.Split('-');
            if (ids.Length != 2)
            {
                return false;
            }
            // we must be able to convert them to int!
            foreach (string i in ids)
            {
                int res;
                if (!Int32.TryParse(i, out res))
                {
                    return false;
                }
            }
            // good, it's vaild.
            return true;
        }

        /// <summary>
        /// List all available dictionaries
        /// </summary>
        private static async Task ListDictionaries()
        {
            wordbook.cjpg.app.client.Dictionaries dictionaries = await client.Dictionaries();
            if (dictionaries.Items == null)
            {
                Console.WriteLine("Thats wired! we got an empty result!");
                return;
            }
            foreach (wordbook.cjpg.app.client.Dictionary dict in dictionaries.Items)
            {
                Console.WriteLine($"Id ......: {dict.Id}");
                Console.WriteLine($"ShortName: {dict.ShortName}");
                Console.WriteLine($"LongName : {dict.LongName}");
                Console.WriteLine("============================\n");
            }
        }

        /// <summary>
        /// Show the help text for this app!
        /// </summary>
        private static void ShowHelp()
        {
            Console.WriteLine("WordbookClient.exe -- Help");
            string helpMsg = @"
--dictionaries Prints a list of available dictionaries.
--info dictionary-id [....] Prints extended info about the provided dictionary ids
-w Word .... [-l:DICT-ID] get auggestions for a word or phrase. -l flag is the dictionary to look up.
";
            Console.WriteLine(helpMsg);
        }
    }
}
