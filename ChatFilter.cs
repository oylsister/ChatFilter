using System.Text.Json.Serialization;
using System.Text.RegularExpressions;
using CounterStrikeSharp.API.Core;
using CounterStrikeSharp.API.Modules.Commands;

namespace ChatFilter
{
    public class WordFilter : BasePluginConfig
    {
        [JsonPropertyName("filter_list")] 
        public List<string> FilterWordList { get; set; } = new List<string>();
    }

    public class ChatFilter : BasePlugin, IPluginConfig<WordFilter>
    {
        public override string ModuleName => "Chat Filter";
        public override string ModuleVersion => "1.0";
        public override string ModuleAuthor => "Oylsister";

        public List<string> defaultFilter = new List<string> { "fuck", "nigger", "niger"};
        public WordFilter Config { get; set; }

        public override void Load(bool hotReload)
        {
            AddCommandListener("say", CommandOnSay, HookMode.Pre);
            AddCommandListener("say_team", CommandOnSayTeam, HookMode.Pre);
        }

        public void OnConfigParsed(WordFilter config)
        {
            if(config.FilterWordList.Count <= 0)
            {
                config.FilterWordList = defaultFilter;
            }

            Config = config;
        }

        public HookResult CommandOnSay(CCSPlayerController? client, CommandInfo info)
        {
            if(client != null)
            {
                var message = info.ArgString;

                bool found = false;

                foreach(var word in Config.FilterWordList)
                {
                    if (message.Contains(word))
                    {
                        string pattern = @"\b" + Regex.Escape(word) + @"\b";
                        message = Regex.Replace(message, pattern, new string('*', word.Length), RegexOptions.IgnoreCase);
                        found = true;
                    }
                }

                if(found)
                {
                    client.ExecuteClientCommand($"say {message}");
                    return HookResult.Handled;
                }
            }

            return HookResult.Continue;
        }

        public HookResult CommandOnSayTeam(CCSPlayerController? client, CommandInfo info)
        {
            if (client != null)
            {
                var message = info.ArgString;

                bool found = false;

                foreach (var word in Config.FilterWordList)
                {
                    if (message.Contains(word))
                    {
                        string pattern = @"\b" + Regex.Escape(word) + @"\b";
                        message = Regex.Replace(message, pattern, new string('*', word.Length), RegexOptions.IgnoreCase);
                        found = true;
                    }
                }

                if (found)
                {
                    client.ExecuteClientCommand($"say_team {message}");
                    return HookResult.Handled;
                }
            }

            return HookResult.Continue;
        }
    }
}
