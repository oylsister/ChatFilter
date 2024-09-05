using CounterStrikeSharp.API.Core;
using CounterStrikeSharp.API.Modules.UserMessages;
using System.Text.Json.Serialization;

namespace ChatFilter
{
    public class WordFilter : BasePluginConfig
    {
        [JsonPropertyName("filter_list")]
        public List<string> FilterWordList { get; set; } = [];
    }

    public class ChatFilter : BasePlugin, IPluginConfig<WordFilter>
    {
        public override string ModuleName => "Chat Filter";
        public override string ModuleVersion => "1.0";
        public override string ModuleAuthor => "Oylsister";

        public List<string> defaultFilter = ["fuck", "nigger", "niger"];
        public WordFilter Config { get; set; } = new WordFilter();

        public override void Load(bool hotReload)
        {
            HookUserMessage(118, OnMessage, HookMode.Pre);
        }

        public void OnConfigParsed(WordFilter config)
        {
            if (config.FilterWordList.Count <= 0)
            {
                config.FilterWordList = defaultFilter;
            }

            Config = config;
        }

        public HookResult OnMessage(UserMessage um)
        {
            string message = um.ReadString("param2");
            string originalMessage = message;

            foreach (string word in Config.FilterWordList)
            {
                string replacement = new('*', word.Length);
                message = message.Replace(word, replacement, StringComparison.OrdinalIgnoreCase);
            }

            if (message != originalMessage)
            {
                um.SetString("param2", message);
                return HookResult.Changed;
            }

            return HookResult.Continue;
        }
    }
}