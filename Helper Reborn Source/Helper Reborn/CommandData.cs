using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Speech.Recognition;
using BS;
using Newtonsoft.Json;

namespace Helper_Reborn
{
    [Serializable]
    public class commandHandler
    {
        [JsonMergeKey]
        [JsonProperty("$type")]
        [JsonIgnore]
        public Type type
        {
            get
            {
                return base.GetType();
            }
            set
            {
            }
        }

        public virtual void setupGrammar(SpeechRecognitionEngine recognitionEngine)
        { }

        public virtual void doCommand(RecognitionResult speechResult)
        { }

        public string[] commandPhrases;
    }
    [Serializable]
    public class CommandData
    {
        public string id;
        public commandHandler handler;
    }

    [Serializable]
    public class CategoryData
    {
        [Serializable]
        public class item
        {
            public string id;
            public string[] recognisablePhrases;
        }

        public string id;
        public List<item> members;
    }
}
