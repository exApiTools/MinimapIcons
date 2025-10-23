using System;

namespace MinimapIcons.IgnoreRules;

public class IgnoreRule
{
    public IgnoreRuleType Type { get; set; }
    public string Pattern { get; set; }
    public bool IsEnabled { get; set; } = true;

    public IgnoreRule()
    {
    }

    public IgnoreRule(IgnoreRuleType type, string pattern, bool isEnabled = true)
    {
        Type = type;
        Pattern = pattern;
        IsEnabled = isEnabled;
    }

    public bool Matches(string metadata, string renderName)
    {
        if (!IsEnabled || string.IsNullOrWhiteSpace(Pattern))
            return false;

        return Type switch
        {
            IgnoreRuleType.MetadataExact => metadata.Equals(Pattern, StringComparison.OrdinalIgnoreCase),
            IgnoreRuleType.MetadataStartsWith => metadata.StartsWith(Pattern, StringComparison.OrdinalIgnoreCase),
            IgnoreRuleType.MetadataContains => metadata.Contains(Pattern, StringComparison.OrdinalIgnoreCase),
            IgnoreRuleType.NameExact => renderName.Equals(Pattern, StringComparison.OrdinalIgnoreCase),
            IgnoreRuleType.NameContains => renderName.Contains(Pattern, StringComparison.OrdinalIgnoreCase),
            _ => false
        };
    }

    public override string ToString()
    {
        var prefix = Type switch
        {
            IgnoreRuleType.MetadataExact => "[META=]",
            IgnoreRuleType.MetadataStartsWith => "[META^]",
            IgnoreRuleType.MetadataContains => "[META*]",
            IgnoreRuleType.NameExact => "[NAME=]",
            IgnoreRuleType.NameContains => "[NAME*]",
            _ => "[???]"
        };
        return $"{prefix} {Pattern}";
    }
}

public enum IgnoreRuleType
{
    MetadataExact,          // Exact metadata match
    MetadataStartsWith,     // Metadata starts with
    MetadataContains,       // Metadata contains
    NameExact,              // Exact name match
    NameContains            // Name contains
}
