using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using BS;
using System.Speech.Recognition;
using System.Speech.Synthesis;
using System.Reflection;
using System.Threading;
using System.IO;
using Newtonsoft.Json;

namespace Helper_Reborn
{
    class HelperMain : MonoBehaviour
    {
        SpeechRecognitionEngine recognizer = null;
        public static List<CommandData> commands = new List<CommandData>();

        public static List<CategoryData> categories = new List<CategoryData>();

        void loadCommandData()
        {
            foreach (string text2 in Directory.GetDirectories(Application.streamingAssetsPath))
            {
                string fileName = Path.GetFileName(text2);
                if (!(fileName.ToLower() == Catalog.current.gameData.defaultJSONFolder.ToLower()) && !fileName.StartsWith("_") && (!(GameManager.local.gameJsonFolder.ToLower() != fileName.ToLower()) || File.Exists(text2 + "/manifest.json")))
                {
                    foreach (FileInfo fileInfo in new DirectoryInfo(text2 + "/").GetFiles("Cmd_*.json", SearchOption.AllDirectories))
                    {
                        Debug.LogError("Helper Reborn JSON loader - Found custom command: " + fileInfo.Name);
                        try
                        {
                            string value = File.ReadAllText(fileInfo.FullName);
                            CommandData catalogData2 = JsonConvert.DeserializeObject<CommandData>(value, new JsonSerializerSettings { TypeNameHandling = TypeNameHandling.All });
                            HelperMain.commands.Add(catalogData2);
                            catalogData2.handler.setupGrammar(recognizer);
                        }
                        catch (Exception ex2)
                        {
                            Debug.LogError(string.Concat(new string[]
                            {
                                    "Cannot read file ",
                                    fileInfo.FullName,
                                    " (",
                                    ex2.Message,
                                    ")"
                            }));
                        }
                    }
                }
            }
        }

        public static CategoryData findCategory(string id)
        {
            foreach(CategoryData data in HelperMain.categories)
            {
                if(data.id == id)
                {
                    return data;
                }
            }
            return null;
        }

        void loadCategories()
        {
            foreach (string text2 in Directory.GetDirectories(Application.streamingAssetsPath))
            {
                string fileName = Path.GetFileName(text2);
                if (!(fileName.ToLower() == Catalog.current.gameData.defaultJSONFolder.ToLower()) && !fileName.StartsWith("_") && (!(GameManager.local.gameJsonFolder.ToLower() != fileName.ToLower()) || File.Exists(text2 + "/manifest.json")))
                {
                    foreach (FileInfo fileInfo in new DirectoryInfo(text2 + "/").GetFiles("Category_*.json", SearchOption.AllDirectories))
                    {
                        Debug.LogError("Helper Reborn JSON loader - Found custom category: " + fileInfo.Name);
                        try
                        {
                            string value = File.ReadAllText(fileInfo.FullName);
                            CategoryData catalogData2 = JsonConvert.DeserializeObject<CategoryData>(value, new JsonSerializerSettings { TypeNameHandling = TypeNameHandling.All });
                            HelperMain.categories.Add(catalogData2);
                        }
                        catch (Exception ex2)
                        {
                            Debug.LogError(string.Concat(new string[]
                            {
                                    "Cannot read file ",
                                    fileInfo.FullName,
                                    " (",
                                    ex2.Message,
                                    ")"
                            }));
                        }
                    }
                }
            }
        }

        void Awake()
        {
            Debug.LogError("Helper Reborn:\nhelperModule.instance.activationWord Word: " + helperModule.instance.activationWord + "\nhelperModule.instance.confidenceNeeded Requirement: " + helperModule.instance.confidenceNeeded);

            recognizer = new SpeechRecognitionEngine();
            recognizer.LoadGrammarCompleted +=
                  new EventHandler<LoadGrammarCompletedEventArgs>(recognizer_LoadGrammarCompleted);
            recognizer.SpeechRecognized +=
              new EventHandler<SpeechRecognizedEventArgs>(recognizer_SpeechRecognized);

            loadCategories();
            loadCommandData();
            recognizer.SetInputToDefaultAudioDevice();
            recognizer.RecognizeAsync(RecognizeMode.Multiple);
            recognizer.UpdateRecognizerSetting("CFGConfidenceRejectionThreshold", helperModule.instance.confidenceNeeded);
        }

        void recognizer_LoadGrammarCompleted(object sender, LoadGrammarCompletedEventArgs e)
        {
            print("Grammar loaded: " + e.Grammar.Name);
        }

        void recognizer_SpeechRecognized(object sender, SpeechRecognizedEventArgs e)
        {
            try
            {
                foreach(CommandData data in commands)
                {
                    if(e.Result.Grammar.Name == data.id)
                    {
                        print(e.Result.Grammar + " recognised " + e.Result.Text + "\nPassing to command handler");
                        data.handler.doCommand(e.Result);
                        return;
                    }
                }
            }
            catch (Exception ee)
            {
                Debug.LogError("An error has occured trying to carry out command: " + e.Result.Text + "\nException: " + ee);
            }
        }
    }

    public class helperModule : LevelModule
    {
        public int confidenceNeeded = 90;
        public string activationWord = "Hey helper";
        public static helperModule instance;

        public override void OnLevelLoaded(LevelDefinition levelDefinition)
        {
            helperModule.instance = this;

            base.OnLevelLoaded(levelDefinition);
            Debug.LogError("Helper Reborn Module:\nhelperModule.activationWord Word: " + activationWord + "\nhelperModule.confidenceNeeded Requirement: " + confidenceNeeded);
            GameObject g = new GameObject();
            HelperMain m = g.AddComponent<HelperMain>();
            GameObject.DontDestroyOnLoad(g);
        }
    }
}
