using CounterStrikeSharp.API.Core;
using CounterStrikeSharp.API.Modules.UserMessages;
using System.Text.Json.Serialization;
using System.Text.RegularExpressions;
using TagsApi;

namespace ChatFilter;

public class WordFilter : BasePluginConfig
{
    [JsonPropertyName("filter_list")]
    public List<string> FilterWordList { get; set; } = [];
}

public partial class ChatFilter : BasePlugin, IPluginConfig<WordFilter>
{
    public override string ModuleName => "Chat Filter";
    public override string ModuleVersion => "1.0";
    public override string ModuleAuthor => "Oylsister";

    public WordFilter Config { get; set; } = new WordFilter();
    [GeneratedRegex(@"\d+")] private static partial Regex NumberRegex();

    public override void OnAllPluginsLoaded(bool hotReload)
    {
        try
        {
            var api = ITagApi.Capability.Get();

            if (api == null)
            {
                return;
            }

            api.OnPlayerChatPre += um =>
            {
                OnMessage(um);
            };
        }
        catch (Exception)
        {
            HookUserMessage(118, OnMessage, HookMode.Pre);
        }
    }

    public void OnConfigParsed(WordFilter config)
    {
        Config = config;
    }

    private HookResult OnMessage(UserMessage um)
    {
        var message = um.ReadString("param2");
        var originalMessage = message;

        foreach (string word in Config.FilterWordList)
        {
            string replacement = new('*', word.Length);
            message = message.Replace(word, replacement, StringComparison.OrdinalIgnoreCase);
        }

        string ipOrNumberPattern = @"\b(?:\d{1,3}[.\s/]){3}\d{1,3}\b";
        message = Regex.Replace(message, ipOrNumberPattern, match =>
        {
            return NumberRegex().Replace(match.Value, m => new string('*', m.Value.Length));
        }, RegexOptions.IgnoreCase);

        if (message != originalMessage)
        {
            um.SetString("param2", message);
            return HookResult.Changed;
        }

        return HookResult.Continue;
    }
}