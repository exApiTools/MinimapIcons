using ExileCore.Shared.Attributes;
using ExileCore.Shared.Interfaces;
using ExileCore.Shared.Nodes;
using MinimapIcons.IconsBuilder;
using System;

namespace MinimapIcons;

/// <summary>
/// One "never draw an icon for this" entry, editable from the plugin menu.
/// </summary>
/// <remarks>
/// The existing ignore mechanisms are both applied when the icon is *built*: entries in
/// <c>ignored_entities.txt</c> and in <see cref="MinimapIcons.Ignored"/> make
/// <c>IconsBuilder.SkipIcon</c> return before an icon object exists. That is fine for a
/// permanent exclusion, but it means an entry cannot be undone without an area change, and
/// the hardcoded list cannot be undone at all. These rules are applied at draw time instead,
/// so toggling one takes effect on the next frame in both directions.
/// </remarks>
[Submenu]
public class IconIgnoreRule
{
    public IconIgnoreRule()
    {
    }

    public IconIgnoreRule(string label, string metadataRegex, bool hide)
    {
        Label = new TextNode(label);
        MetadataRegex = new TextNode(metadataRegex);
        Hide = new ToggleNode(hide);
    }

    [Menu("Hide", "On = matching entities get no icon. Off = they draw as normal.")]
    public ToggleNode Hide { get; set; } = new ToggleNode(true);

    [Menu("Label", "Free text, so the row is recognisable in this list. Not used for matching.")]
    public TextNode Label { get; set; } = new TextNode("");

    [Menu("Metadata regex", "Unanchored regex tested against the entity metadata path.")]
    public TextNode MetadataRegex { get; set; } = new TextNode("");

    // Names the row in the settings UI, which otherwise falls back to object.ToString() and shows
    // every row as "MinimapIcons.IconIgnoreRule". The trailing ### keeps the ImGui id of the row
    // stable while the visible part of the label changes, so the row does not collapse itself
    // while you are typing in the label or pattern field.
    public override string ToString()
    {
        var label = !string.IsNullOrWhiteSpace(Label?.Value)
            ? Label.Value
            : !string.IsNullOrWhiteSpace(MetadataRegex?.Value)
                ? MetadataRegex.Value
                : "(empty rule)";

        return $"{label}###";
    }
}

public class MapIconsSettings : ISettings
{
    public ToggleNode DrawMonsters { get; set; } = new ToggleNode(true);
    public RangeNode<float> ZForText { get; set; } = new RangeNode<float>(-10, -50, 50);
    public ToggleNode DrawOnlyOnLargeMap { get; set; } = new ToggleNode(true);
    public ToggleNode DrawCachedEntities { get; set; } = new ToggleNode(true);
    public ToggleNode DrawReplacementsForGameIconsWhenOutOfRange { get; set; } = new ToggleNode(true);
    public ToggleNode IgnoreFullscreenPanels { get; set; } = new ToggleNode(false);
    public ToggleNode IgnoreLargePanels { get; set; } = new ToggleNode(false);

    [Menu("Cache breach entities", "Breaches spawn lots of entities, to avoid cluttering your minimap you can turn off this setting")]
    public ToggleNode CacheBreachEntities { get; set; } = new ToggleNode(true);
    public ToggleNode Enable { get; set; } = new ToggleNode(true);
    public RangeNode<int> IconListRefreshPeriod { get; set; } = new RangeNode<int>(100, 0, 1000);
    public ToggleNode HighlightHiddenMonsters { get; set; } = new ToggleNode(true);

    [Menu("Hidden icons", "Entities you never want an icon for, each with its own on/off. Unlike " +
                          "ignored_entities.txt these are applied at draw time, so a toggle takes effect " +
                          "immediately and can be undone without changing areas. Empty by default.",
        CollapsedByDefault = true)]
    public ContentNode<IconIgnoreRule> HiddenIcons { get; set; } =
        new ContentNode<IconIgnoreRule>()
        {
            Content =
            [
            ],
            EnableControls = true,
            EnableItemCollapsing = true,
            ItemFactory = () => new IconIgnoreRule(),
            ItemFilter = (o, s) => o.Label.Value.Contains(s, StringComparison.OrdinalIgnoreCase) ||
                                   o.MetadataRegex.Value.Contains(s, StringComparison.OrdinalIgnoreCase),
        };

    [Menu(null, CollapsedByDefault = true)]
    public ContentNode<TextNode> AlwaysShownIngameIcons { get; set; } =
        new ContentNode<TextNode>()
        {
            Content =
            [
                "Metadata/Terrain/Leagues/Delve/Objects/DelveWall"
            ],
            EnableControls = true, 
            ItemFactory = () => new TextNode(""),
            UseFlatItems = true,
        };

    [Menu(null, CollapsedByDefault = true)]
    public ContentNode<TextNode> IgnoreHiddenStatusMinimapIcons { get; set; } =
        new ContentNode<TextNode>()
        {
            Content =
            [
            ],
            EnableControls = true, 
            ItemFactory = () => new TextNode(""),
            UseFlatItems = true,
        };

    public IconsBuilderSettings IconsBuilderSettings { get; set; } = new();
}