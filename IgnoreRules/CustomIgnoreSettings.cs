using ExileCore.Shared.Attributes;
using ExileCore.Shared.Interfaces;
using ExileCore.Shared.Nodes;

namespace MinimapIcons.IgnoreRules;

public class CustomIgnoreSettings : ISettings
{
    [Menu("Enable Custom Ignore Rules")]
    public ToggleNode EnableCustomIgnoreRules { get; set; } = new ToggleNode(true);

    [Menu("New Rule Type")]
    public ListNode NewRuleType { get; set; } = new ListNode
    {
        Values = new System.Collections.Generic.List<string>
        {
            "Metadata Exact",
            "Metadata Starts With", 
            "Metadata Contains",
            "Name Exact",
            "Name Contains"
        },
        Value = "Metadata Starts With"
    };

    [Menu("New Rule Pattern")]
    public TextNode NewRulePattern { get; set; } = new TextNode("");

    [Menu("Add Rule")]
    public ButtonNode AddRule { get; set; } = new ButtonNode();

    [Menu("Reload Rules from File")]
    public ButtonNode ReloadRules { get; set; } = new ButtonNode();

    [Menu("Open Config Folder")]
    public ButtonNode OpenConfigFolder { get; set; } = new ButtonNode();

    public ToggleNode Enable { get; set; } = new ToggleNode(true);
}
