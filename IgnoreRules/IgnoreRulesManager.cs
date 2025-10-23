using System.Collections.Generic;
using System.IO;

namespace MinimapIcons.IgnoreRules;

public class IgnoreRulesManager
{
    private readonly List<IgnoreRule> _customRules = new();
    private readonly Dictionary<string, bool> _cache = new();
    private readonly string _customRulesFilePath;

    public IReadOnlyList<IgnoreRule> CustomRules => _customRules.AsReadOnly();

    public IgnoreRulesManager(string directoryFullName)
    {
        var configDir = Path.Combine(directoryFullName, "config");
        if (!Directory.Exists(configDir))
            Directory.CreateDirectory(configDir);

        _customRulesFilePath = Path.Combine(configDir, "custom_ignore_rules.txt");
        LoadCustomRules();
    }


    public void LoadCustomRules()
    {
        _customRules.Clear();
        ClearCache();

        if (!File.Exists(_customRulesFilePath))
        {
            SaveCustomRules(); // Create default file
            return;
        }

        try
        {
            var lines = File.ReadAllLines(_customRulesFilePath);
            foreach (var line in lines)
            {
                var trimmedLine = line.Trim();
                if (string.IsNullOrWhiteSpace(trimmedLine) || trimmedLine.StartsWith("#"))
                    continue;

                var rule = ParseRule(trimmedLine);
                if (rule != null)
                    _customRules.Add(rule);
            }
        }
        catch
        {
            // Silent fail - error will be handled by caller
        }
    }

    public void SaveCustomRules()
    {
        try
        {
            var lines = new List<string>
            {
                "# Custom Ignore Rules for MinimapIcons",
                "# Format examples:",
                "# META=Metadata/Monsters/Exact/Path              - Exact metadata match",
                "# META^Metadata/Monsters/Prefix                  - Metadata starts with",
                "# META*PartialMetadata                           - Metadata contains",
                "# NAME=Exact Entity Name                         - Exact name match",
                "# NAME*Partial Name                              - Name contains",
                "# Lines starting with # are comments",
                "# Prefix with ! to disable a rule without deleting it",
                ""
            };

            foreach (var rule in _customRules)
            {
                var prefix = rule.Type switch
                {
                    IgnoreRuleType.MetadataExact => "META=",
                    IgnoreRuleType.MetadataStartsWith => "META^",
                    IgnoreRuleType.MetadataContains => "META*",
                    IgnoreRuleType.NameExact => "NAME=",
                    IgnoreRuleType.NameContains => "NAME*",
                    _ => "META^"
                };

                var line = $"{(rule.IsEnabled ? "" : "!")}{prefix}{rule.Pattern}";
                lines.Add(line);
            }

            File.WriteAllLines(_customRulesFilePath, lines);
        }
        catch
        {
            // Silent fail - error will be handled by caller
        }
    }

    private IgnoreRule ParseRule(string line)
    {
        var isEnabled = !line.StartsWith("!");
        if (!isEnabled)
            line = line.Substring(1);

        IgnoreRuleType type;
        string pattern;

        if (line.StartsWith("META="))
        {
            type = IgnoreRuleType.MetadataExact;
            pattern = line.Substring(5);
        }
        else if (line.StartsWith("META^"))
        {
            type = IgnoreRuleType.MetadataStartsWith;
            pattern = line.Substring(5);
        }
        else if (line.StartsWith("META*"))
        {
            type = IgnoreRuleType.MetadataContains;
            pattern = line.Substring(5);
        }
        else if (line.StartsWith("NAME="))
        {
            type = IgnoreRuleType.NameExact;
            pattern = line.Substring(5);
        }
        else if (line.StartsWith("NAME*"))
        {
            type = IgnoreRuleType.NameContains;
            pattern = line.Substring(5);
        }
        else
        {
            // Default to MetadataStartsWith for backward compatibility
            type = IgnoreRuleType.MetadataStartsWith;
            pattern = line;
        }

        return new IgnoreRule(type, pattern, isEnabled);
    }

    public void AddRule(IgnoreRule rule)
    {
        _customRules.Add(rule);
        ClearCache();
        SaveCustomRules();
    }

    public void RemoveRule(IgnoreRule rule)
    {
        _customRules.Remove(rule);
        ClearCache();
        SaveCustomRules();
    }

    public void ClearCache()
    {
        _cache.Clear();
    }

    public bool ShouldIgnore(string metadata, string renderName)
    {
        var cacheKey = $"{metadata}|{renderName}";
        
        if (_cache.TryGetValue(cacheKey, out var cached))
            return cached;

        // Check only custom rules
        foreach (var rule in _customRules)
        {
            if (rule.Matches(metadata, renderName))
            {
                _cache[cacheKey] = true;
                return true;
            }
        }

        _cache[cacheKey] = false;
        return false;
    }
}
