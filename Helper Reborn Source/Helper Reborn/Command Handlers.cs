using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Speech.Recognition;
using BS;
using UnityEngine;

namespace Helper_Reborn
{
    class giveHandler : commandHandler
    {
        public override void setupGrammar(SpeechRecognitionEngine recognitionEngine)
        {
            base.setupGrammar(recognitionEngine);

            Choices commandWords = new Choices(commandPhrases);

            List<GrammarBuilder> displayNames = new List<GrammarBuilder>();
            foreach (ItemData i in Catalog.current.GetDataList(Catalog.Category.Item))
            {
                if (i.purchasable)
                {
                    displayNames.Add(new SemanticResultValue(i.displayName, i.id));
                }
            }
            foreach (CategoryData.item item in HelperMain.findCategory("Items").members)
            {
                foreach (string phrase in item.recognisablePhrases)
                {
                    displayNames.Add(new SemanticResultValue(phrase, item.id));
                }
            }
            foreach (CategoryData.item item in HelperMain.findCategory("Creatures").members)
            {
                foreach (string phrase in item.recognisablePhrases)
                {
                    displayNames.Add(new SemanticResultValue(phrase, item.id + " NPC"));
                }
            }
            Choices secondaryWords = new Choices(displayNames.ToArray());
            Choices optionalWords = new Choices(new string[] { "left", "left hand", "right", "right hand", " " });
            GrammarBuilder builder = new GrammarBuilder(helperModule.instance.activationWord);
            builder.Append(new SemanticResultKey("cmd", commandWords));
            builder.Append(new SemanticResultKey("item", secondaryWords));
            builder.Append(new SemanticResultKey("optional", optionalWords));

            Grammar spawnCmdGrammar = new Grammar(builder);
            spawnCmdGrammar.Name = "giveCommand";
            recognitionEngine.LoadGrammar(spawnCmdGrammar);
        }

        public override void doCommand(RecognitionResult speechResult)
        {
            base.doCommand(speechResult);
            if (speechResult.Semantics["item"].Value.ToString().Contains("NPC"))
            {
                Creature newCreature = Catalog.current.GetData<CreatureData>(speechResult.Semantics["item"].Value.ToString().Replace(" NPC", "")).Spawn(Player.local.head.transform.position + 0.5f * Player.local.body.transform.right, Quaternion.identity, false);
                return;
            }
            Debug.Log("Spawning you a " + speechResult.Semantics["item"].Value);
            Item item = Catalog.current.GetData<ItemData>(speechResult.Semantics["item"].Value.ToString(), true).Spawn(true, null);
            PlayerHand hand = speechResult.Semantics["optional"].Value.ToString().Contains("left") ? Player.local.handLeft : Player.local.handRight;
            item.transform.position = hand.transform.position;
            if (Creature.player.body.GetHand(hand.side).interactor.grabbedHandle) Creature.player.body.GetHand(hand.side).interactor.UnGrab(true);
            Creature.player.body.GetHand(hand.side).interactor.Grab(item.GetMainHandle(hand.side));
        }
    }

    class spawnHandler : commandHandler
    {
        public override void setupGrammar(SpeechRecognitionEngine recognitionEngine)
        {
            base.setupGrammar(recognitionEngine);

            Choices commandWords = new Choices(commandPhrases);

            List<GrammarBuilder> displayNames = new List<GrammarBuilder>();
            foreach (ItemData i in Catalog.current.GetDataList(Catalog.Category.Item))
            {
                if (i.purchasable)
                {
                    displayNames.Add(new SemanticResultValue(i.displayName, i.id));
                }
            }
            foreach(CategoryData.item item in HelperMain.findCategory("Items").members)
            {
                foreach(string phrase in item.recognisablePhrases)
                {
                    displayNames.Add(new SemanticResultValue(phrase, item.id));
                }
            }
            foreach (CategoryData.item item in HelperMain.findCategory("Creatures").members)
            {
                foreach (string phrase in item.recognisablePhrases)
                {
                    displayNames.Add(new SemanticResultValue(phrase, item.id + " NPC"));
                }
            }
            Choices secondaryWords = new Choices(displayNames.ToArray());
            GrammarBuilder builder = new GrammarBuilder(helperModule.instance.activationWord);
            builder.Append(new SemanticResultKey("cmd", commandWords));
            builder.Append(new SemanticResultKey("item", secondaryWords));

            Grammar spawnCmdGrammar = new Grammar(builder);
            spawnCmdGrammar.Name = "spawnCommand";
            recognitionEngine.LoadGrammar(spawnCmdGrammar);
        }

        public override void doCommand(RecognitionResult speechResult)
        {
            base.doCommand(speechResult);
            if (speechResult.Semantics["item"].Value.ToString().Contains("NPC"))
            {
                Creature newCreature = Catalog.current.GetData<CreatureData>(speechResult.Semantics["item"].Value.ToString().Replace(" NPC","")).Spawn(Player.local.head.transform.position + 0.5f * Player.local.body.transform.right, Quaternion.identity, false);
                return;
            }
            Debug.Log("Spawning you a " + speechResult.Semantics["item"].Value);
            Item item = Catalog.current.GetData<ItemData>(speechResult.Semantics["item"].Value.ToString(), true).Spawn(true, null);
            item.transform.position = Player.local.head.transform.position + 0.5f * Player.local.head.transform.forward;
        }
    }

    class restartWaveHandler : commandHandler
    {
        public override void setupGrammar(SpeechRecognitionEngine recognitionEngine)
        {
            base.setupGrammar(recognitionEngine);
            Choices commandWordsWave2 = new Choices(commandPhrases); // Needed to identify first command
            GrammarBuilder builderWave2 = new GrammarBuilder(helperModule.instance.activationWord);
            builderWave2.Append(new SemanticResultKey("cmd", commandWordsWave2));

            Grammar WaveGrammar2 = new Grammar(builderWave2);
            WaveGrammar2.Name = "restartWaveCommand";
            recognitionEngine.LoadGrammar(WaveGrammar2);
        }

        public override void doCommand(RecognitionResult speechResult)
        {
            base.doCommand(speechResult);
            if (LevelDefinition.current.modeRank.mode.GetModule<LevelModuleWave>() != null)
            {
                LevelDefinition.current.modeRank.mode.GetModule<LevelModuleWave>().StartWave(GameObject.FindObjectOfType<UIWaveSelector>().spawnLocation, LevelDefinition.current.modeRank.mode.GetModule<LevelModuleWave>().waveData);
            }
        }
    }

    class stopWaveHandler : commandHandler
    {
        public override void setupGrammar(SpeechRecognitionEngine recognitionEngine)
        {
            base.setupGrammar(recognitionEngine);
            Choices commandWordsWave2 = new Choices(commandPhrases); // Needed to identify first command
            GrammarBuilder builderWave2 = new GrammarBuilder(helperModule.instance.activationWord);
            builderWave2.Append(new SemanticResultKey("cmd", commandWordsWave2));

            Grammar WaveGrammar2 = new Grammar(builderWave2);
            WaveGrammar2.Name = "stopWaveCommand";
            recognitionEngine.LoadGrammar(WaveGrammar2);
        }

        public override void doCommand(RecognitionResult speechResult)
        {
            base.doCommand(speechResult);
            if (LevelDefinition.current.modeRank.mode.GetModule<LevelModuleWave>() != null)
            {
                LevelDefinition.current.modeRank.mode.GetModule<LevelModuleWave>().CancelWave();
            }
        }
    }

    class startWaveHandler : commandHandler
    {
        public override void setupGrammar(SpeechRecognitionEngine recognitionEngine)
        {
            base.setupGrammar(recognitionEngine);
            Choices commandWordsWave = new Choices(commandPhrases); // Needed to identify first command
            List<GrammarBuilder> waves = new List<GrammarBuilder>();
            foreach (WaveData i in Catalog.current.GetDataList(Catalog.Category.Wave))
            {
                waves.Add(new SemanticResultValue(i.title, i.id));
            }
            foreach (CategoryData.item item in HelperMain.findCategory("Waves").members)
            {
                foreach (string phrase in item.recognisablePhrases)
                {
                    waves.Add(new SemanticResultValue(phrase, item.id));
                }
            }
            Choices secondaryWordsWave = new Choices(waves.ToArray());
            GrammarBuilder builderWave = new GrammarBuilder(helperModule.instance.activationWord);
            builderWave.Append(new SemanticResultKey("cmd", commandWordsWave));
            builderWave.Append(new SemanticResultKey("wave", secondaryWordsWave));

            Grammar WaveGrammar = new Grammar(builderWave);
            WaveGrammar.Name = "startWaveCommand";
            recognitionEngine.LoadGrammar(WaveGrammar);
        }

        public override void doCommand(RecognitionResult speechResult)
        {
            base.doCommand(speechResult);
            if (LevelDefinition.current.modeRank.mode.GetModule<LevelModuleWave>() != null)
            {
                LevelDefinition.current.modeRank.mode.GetModule<LevelModuleWave>().StartWave(UnityEngine.Object.FindObjectOfType<UIWaveSelector>().spawnLocation, Catalog.current.GetData<WaveData>(speechResult.Semantics["wave"].Value.ToString(), true), 0f, true);
            }
        }
    }

    class mapHandler : commandHandler
    {
        public override void setupGrammar(SpeechRecognitionEngine recognitionEngine)
        {
            base.setupGrammar(recognitionEngine);

            Choices commandWordsMap = new Choices(commandPhrases); // Needed to identify first command
            List<GrammarBuilder> maps = new List<GrammarBuilder>();
            foreach (LevelData i in Catalog.current.GetDataList(Catalog.Category.Level))
            {
                if (i.mapLocation != null)
                {
                    maps.Add(new SemanticResultValue(i.name, i.id));
                }
            }
            foreach (CategoryData.item item in HelperMain.findCategory("Maps").members)
            {
                foreach (string phrase in item.recognisablePhrases)
                {
                    maps.Add(new SemanticResultValue(phrase, item.id));
                }
            }
            maps.Add(new SemanticResultValue("Home", "Home"));
            Choices secondaryWordsMap = new Choices(maps.ToArray());
            GrammarBuilder builderMap = new GrammarBuilder(helperModule.instance.activationWord);
            builderMap.Append(new SemanticResultKey("cmd", commandWordsMap));
            builderMap.Append(new SemanticResultKey("map", secondaryWordsMap));

            Grammar mapGrammar = new Grammar(builderMap);
            mapGrammar.Name = "mapCommand";
            recognitionEngine.LoadGrammar(mapGrammar);
        }

        public override void doCommand(RecognitionResult speechResult)
        {
            base.doCommand(speechResult);
            Debug.Log("Starting map " + speechResult.Semantics["map"].Value);
            GameManager.LoadLevel(speechResult.Semantics["map"].Value.ToString());
        }
    }
}
