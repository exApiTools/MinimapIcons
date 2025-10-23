using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace MinimapIcons.IgnoreRules;

public class IgnoreRulesManager
{
    private readonly List<IgnoreRule> _customRules = new();
    private readonly List<IgnoreRule> _builtInRules = new();
    private readonly Dictionary<string, bool> _cache = new();
    private readonly string _customRulesFilePath;

    public IReadOnlyList<IgnoreRule> CustomRules => _customRules.AsReadOnly();
    public IReadOnlyList<IgnoreRule> BuiltInRules => _builtInRules.AsReadOnly();

    public IgnoreRulesManager(string directoryFullName)
    {
        var configDir = Path.Combine(directoryFullName, "config");
        if (!Directory.Exists(configDir))
            Directory.CreateDirectory(configDir);

        _customRulesFilePath = Path.Combine(configDir, "custom_ignore_rules.txt");
        LoadBuiltInRules();
        LoadCustomRules();
    }

    private void LoadBuiltInRules()
    {
        _builtInRules.Clear();
        
        // Hardcoded rules from MinimapIcons.cs
        var builtInPatterns = new[]
        {
            "Metadata/Monsters/LeagueAffliction/DoodadDaemons/DoodadDaemonEyes1",
            "Metadata/Monsters/LeagueAffliction/DoodadDaemons/DoodadDaemonEyes2",
            "Metadata/Monsters/LeagueAffliction/DoodadDaemons/DoodadDaemonEyes3",
            "Metadata/Monsters/LeagueAffliction/DoodadDaemons/DoodadDaemonSpikes",
            "Metadata/Monsters/LeagueAffliction/DoodadDaemons/DoodadDaemonSpikes2",
            "Metadata/Monsters/LeagueAffliction/DoodadDaemons/DoodadDaemonSpikes3",
            "Metadata/Monsters/LeagueAffliction/DoodadDaemons/DoodadDaemonPimple1",
            "Metadata/Monsters/LeagueAffliction/DoodadDaemons/DoodadDaemonPimple2",
            "Metadata/Monsters/LeagueAffliction/DoodadDaemons/DoodadDaemonPimple3",
            "Metadata/Monsters/LeagueAffliction/DoodadDaemons/DoodadDaemonGoatFillet1Vanish",
            "Metadata/Monsters/LeagueAffliction/DoodadDaemons/DoodadDaemonGoatFillet2Vanish",
            "Metadata/Monsters/LeagueAffliction/DoodadDaemons/DoodadDaemonGoatRhoa1Vanish",
            "Metadata/Monsters/LeagueAffliction/DoodadDaemons/DoodadDaemonGoatRhoa2Vanish",
            "Metadata/Monsters/AtlasExiles/AtlasExile1@",
            "Metadata/Monsters/AtlasExiles/CrusaderInfluenceMonsters/CrusaderArcaneRune",
            "Metadata/Monsters/AtlasExiles/AtlasExile2_",
            "Metadata/Monsters/AtlasExiles/EyrieInfluenceMonsters/EyrieFrostnadoDaemon",
            "Metadata/Monsters/AtlasExiles/AtlasExile3@",
            "Metadata/Monsters/AtlasExiles/AtlasExile3AcidPitDaemon",
            "Metadata/Monsters/AtlasExiles/AtlasExile3BurrowingViperMelee",
            "Metadata/Monsters/AtlasExiles/AtlasExile3BurrowingViperRanged",
            "Metadata/Monsters/AtlasExiles/AtlasExile4@",
            "Metadata/Monsters/AtlasExiles/AtlasExile4ApparitionCascade",
            "Metadata/Monsters/AtlasExiles/AtlasExile5Apparition",
            "Metadata/Monsters/AtlasExiles/AtlasExile5Throne",
            "Metadata/Monsters/LeagueIncursion/VaalSaucerRoomTurret",
            "Metadata/Monsters/LeagueIncursion/VaalSaucerTurret",
            "Metadata/Monsters/LeagueBetrayal/BetrayalTaserNet",
            "Metadata/Monsters/LeagueBetrayal/FortTurret/FortTurret1Safehouse",
            "Metadata/Monsters/LeagueBetrayal/FortTurret/FortTurret1",
            "Metadata/Monsters/LeagueBetrayal/MasterNinjaCop",
            "Metadata/Monsters/LeagueBetrayal/BetrayalRikerMortarDaemon",
            "Metadata/Monsters/LeagueBetrayal/BetrayalBoneNovaDaemon",
            "Metadata/Monsters/LeagueBetrayal/BetrayalCatarinaPillarDaemon_",
            "Metadata/Monsters/LeagueBetrayal/BetrayalUpgrades/BetrayalDaemonCorpseConsume",
            "Metadata/Monsters/LegionLeague/LegionVaalGeneralProjectileDaemon",
            "Metadata/Monsters/LegionLeague/LegionSergeantStampedeDaemon",
            "Metadata/Monsters/LegionLeague/LegionSandTornadoDaemon",
            "Metadata/Monsters/LegionLeague/LegionVaalGeneralMoveDaemonQuad",
            "Metadata/Monsters/InvisibleFire/InvisibleSandstorm_",
            "Metadata/Monsters/InvisibleFire/InvisibleFrostnado",
            "Metadata/Monsters/InvisibleFire/InvisibleFireAfflictionDemonColdDegen",
            "Metadata/Monsters/InvisibleFire/InvisibleFireAfflictionDemonColdDegenUnique",
            "Metadata/Monsters/InvisibleFire/InvisibleFireAfflictionCorpseDegen",
            "Metadata/Monsters/InvisibleFire/InvisibleFireEyrieHurricane",
            "Metadata/Monsters/InvisibleFire/InvisibleIonCannonFrost",
            "Metadata/Monsters/InvisibleFire/AfflictionBossFinalDeathZone",
            "Metadata/Monsters/InvisibleFire/InvisibleFireDoedreSewers",
            "Metadata/Monsters/InvisibleFire/InvisibleFireDelveFlameTornadoSpiked",
            "Metadata/Monsters/InvisibleFire/InvisibleHolyCannon",
            "Metadata/Monsters/InvisibleFire/DelveVaalBossInvisibleLight",
            "Metadata/Monsters/InvisibleFire/InvisibleChaosstorm",
            "Metadata/Monsters/InvisibleFire/InvisibleKitavaCannon",
            "Metadata/Monsters/InvisibleCurse/InvisibleFrostbiteStationary",
            "Metadata/Monsters/InvisibleCurse/InvisibleConductivityStationary",
            "Metadata/Monsters/InvisibleCurse/InvisibleEnfeeble",
            "Metadata/Monsters/InvisibleAura/InvisibleWrathStationary",
            "Metadata/Monsters/Frog/FrogGod/SilverOrb",
            "Metadata/Monsters/Frog/FrogGod/SilverPool",
            "Metadata/Monsters/LunarisSolaris/SolarisCelestialFormAmbushUniqueMap",
            "Metadata/Monsters/Invisible/MaligaroSoulInvisibleBladeVortex",
            "Metadata/Monsters/Daemon",
            "Metadata/Monsters/Daemon/MaligaroBladeVortexDaemon",
            "Metadata/Monsters/Daemon/SilverPoolChillDaemon",
            "Metadata/Monsters/AvariusCasticus/AvariusCasticusStatue",
            "Metadata/Monsters/Maligaro/MaligaroDesecrate",
            "Metadata/Monsters/Avatar/AvatarMagmaOrbDaemon",
            "Metadata/Monsters/Monkeys/FlameBearerTalismanT2Ghost",
            "Metadata/Monsters/Totems/TalismanTotem/TalismanTotemDeathscape",
            "Metadata/Monsters/BeehiveBehemoth/BeehiveBehemothSwampDaemon",
            "Metadata/Monsters/VaalWraith/VaalWraithChampionMinion",
            "Metadata/Monsters/LeagueSynthesis/SynthesisDroneBossTurret1",
            "Metadata/Monsters/LeagueSynthesis/SynthesisDroneBossTurret2",
            "Metadata/Monsters/LeagueSynthesis/SynthesisDroneBossTurret3",
            "Metadata/Monsters/LeagueSynthesis/SynthesisDroneBossTurret4",
            "Metadata/Monsters/LeagueSynthesis/SynthesisWalkerSpawned_",
            "Metadata/Monsters/LeagueRitual/FireMeteorDaemon",
            "Metadata/Monsters/LeagueRitual/GenericSpeedDaemon",
            "Metadata/Monsters/LeagueRitual/ColdRotatingBeamDaemon",
            "Metadata/Monsters/LeagueRitual/ColdRotatingBeamDaemonUber",
            "Metadata/Monsters/LeagueRitual/GenericEnergyShieldDaemon",
            "Metadata/Monsters/LeagueRitual/GenericMassiveDaemon",
            "Metadata/Monsters/LeagueRitual/ChaosGreenVinesDaemon_",
            "Metadata/Monsters/LeagueRitual/ChaosSoulrendPortalDaemon",
            "Metadata/Monsters/LeagueRitual/VaalAtziriDaemon",
            "Metadata/Monsters/LeagueRitual/LightningPylonDaemon",
            "Metadata/Monsters/LeagueBestiary/RootSpiderBestiaryAmbush",
            "Metadata/Monsters/LeagueBestiary/BlackScorpionBestiaryBurrowTornado",
            "Metadata/Monsters/LeagueBestiary/ModDaemonCorpseEruption",
            "Metadata/Monsters/LeagueBestiary/ModDaemonSandLeaperExplode1",
            "Metadata/Monsters/LeagueBestiary/ModDaemonStampede1",
            "Metadata/Monsters/LeagueBestiary/ModDaemonGraspingPincers1",
            "Metadata/Monsters/LeagueBestiary/ModDaemonPouncingShade1",
            "Metadata/Monsters/LeagueBestiary/ModDaemonPouncingShadeQuickHit",
            "Metadata/Monsters/LeagueBestiary/ModDaemonFire1",
            "Metadata/Monsters/LeagueBestiary/ModDaemonVultureBomb1",
            "Metadata/Monsters/LeagueBestiary/ModDaemonVultureBombCast1",
            "Metadata/Monsters/LeagueBestiary/ModDaemonParasiticSquid1",
            "Metadata/Monsters/LeagueBestiary/ModDaemonBloodRaven1",
            "Metadata/Monsters/LeagueBestiary/SandLeaperBestiaryClone",
            "Metadata/Monsters/LeagueBestiary/SpiderPlagueBestiaryExplode",
            "Metadata/Monsters/LeagueBestiary/ParasiticSquidBestiaryClone",
            "Metadata/Monsters/LeagueBestiary/HellionBestiaryClone",
            "Metadata/Monsters/LeagueBestiary/BestiarySpiderCocoon",
            "Metadata/Monsters/LeagueBestiary/GemFrogBestiaryClone",
            "Metadata/Monsters/LeagueRitual/GoldenCoinDaemon",
            "Metadata/Monsters/LeagueRitual/GenericLifeDaemon",
            "Metadata/Monsters/LeagueRitual/GenericChargesDaemon"
        };

        foreach (var pattern in builtInPatterns)
        {
            _builtInRules.Add(new IgnoreRule(IgnoreRuleType.MetadataStartsWith, pattern, true));
        }
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

        // Check built-in rules first
        foreach (var rule in _builtInRules)
        {
            if (rule.Matches(metadata, renderName))
            {
                _cache[cacheKey] = true;
                return true;
            }
        }

        // Then check custom rules
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
