using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using ExileCore.PoEMemory.Components;
using ExileCore.PoEMemory.MemoryObjects;
using ExileCore.Shared.Enums;
using ExileCore.Shared.Helpers;
using GameOffsets.Native;
using MinimapIcons;

namespace MinimapIcons.IconsBuilder.Icons;

public class MonsterIcon : BaseIcon
{
    public MonsterIcon(Entity entity, IconsBuilderSettings settings, Dictionary<string, Vector2i> modIcons, MapIconsSettings mapSettings)
        : base(entity)
    {
        Update(entity, settings, modIcons, mapSettings);
    }

    public void Update(Entity entity, IconsBuilderSettings settings, Dictionary<string, Vector2i> modIcons, MapIconsSettings mapSettings)
    {
        Show = () => entity.IsAlive;
        if(entity.IsHidden && settings.HideBurriedMonsters)
        {
            Show = () => !entity.IsHidden && entity.IsAlive;
        }

        if (!_HasIngameIcon) MainTexture = new HudTexture("Icons.png");

        MainTexture.Size = Rarity switch
        {
            MonsterRarity.White => settings.SizeEntityWhiteIcon,
            MonsterRarity.Magic => settings.SizeEntityMagicIcon,
            MonsterRarity.Rare => settings.SizeEntityRareIcon,
            MonsterRarity.Unique => settings.SizeEntityUniqueIcon,
            _ => throw new ArgumentException($"{nameof(MonsterIcon)} wrong rarity for {entity.Path}. Dump: {entity.GetComponent<ObjectMagicProperties>()?.DumpObject()}")
        };

        var isMonsterWithIcon = settings.MonstersWithIcons.Content.Any(x => IconsBuilder.GetRegex(x.Value).IsMatch(entity.Path));
        if (isMonsterWithIcon && IngameIconIndex == MapIconsIndex.BlightMonster)
        {
            MainTexture.Size *= 2;
        }

        if (_HasIngameIcon &&
            entity.TryGetComponent<MinimapIcon>(out var mI) &&
            mI.Name != "NPC" &&
            !isMonsterWithIcon &&
            !mapSettings.MonstersIgnoreMinimapIconComponent)
            return;
        if (!entity.IsHostile)
        {
            if (!_HasIngameIcon)
            {
                MainTexture.UV = SpriteHelper.GetUV(MapIconsIndex.LootFilterSmallGreenCircle);
                Priority = IconPriority.Low;
                Show = () => !settings.HideMinions && entity.IsAlive;
            }
        }
        else if (Rarity == MonsterRarity.Unique && entity.Path.Contains("Metadata/Monsters/Spirit/"))
            MainTexture.UV = SpriteHelper.GetUV(MapIconsIndex.LootFilterLargeGreenHexagon);
        else
        {
            string modName = null;
            var objectMagicProperties = entity.GetComponent<ObjectMagicProperties>();
            var mods = objectMagicProperties?.Mods;

            if (mods != null)
            {
                if (mods.Contains("MonsterConvertsOnDeath_")) Show = () => entity.IsAlive && entity.IsHostile;

                modName = mods.FirstOrDefault(modIcons.ContainsKey);
            }

            if (settings.HighlightEldritchMonsters &&
                entity.Path.StartsWith("Metadata/Monsters/AtlasInvaders/", StringComparison.Ordinal))
            {
                BorderColor = settings.EldritchMonstersColor.Value.ToSystem();
            }

            if (modName != null)
            {
                MainTexture = new HudTexture("sprites.png");
                MainTexture.UV = SpriteHelper.GetUV(modIcons[modName], new Vector2i(7, 8));
                Priority = IconPriority.VeryHigh;
            }
            else
            {
                switch (Rarity)
                {
                    case MonsterRarity.White:
                        if (!isMonsterWithIcon) MainTexture.UV = SpriteHelper.GetUV(MapIconsIndex.LootFilterLargeRedCircle);
                        if (settings.MonsterRarityNames.ShowNormalNames)
                        {
                            Text = RenderName.Split(',').FirstOrDefault();
                            TextColor = settings.MonsterRarityNames.TextColor.Value.ToSystem();
                            if (settings.MonsterRarityNames.NameBackground) BackgroundColor = settings.MonsterRarityNames.BackgroundColor.Value.ToSystem();
                        }

                        break;
                    case MonsterRarity.Magic:
                        if (!isMonsterWithIcon) MainTexture.UV = SpriteHelper.GetUV(MapIconsIndex.LootFilterLargeBlueCircle);
                        if (settings.MonsterRarityNames.ShowMagicNames)
                        {
                            Text = RenderName.Split(',').FirstOrDefault();
                            TextColor = settings.MonsterRarityNames.TextColor.Value.ToSystem();
                            if (settings.MonsterRarityNames.NameBackground) BackgroundColor = settings.MonsterRarityNames.BackgroundColor.Value.ToSystem();
                        }

                        break;
                    case MonsterRarity.Rare:
                        if (!isMonsterWithIcon) MainTexture.UV = SpriteHelper.GetUV(MapIconsIndex.LootFilterLargeYellowCircle);
                        if (settings.MonsterRarityNames.ShowRareNames)
                        {
                            Text = RenderName.Split(',').FirstOrDefault();
                            TextColor = settings.MonsterRarityNames.TextColor.Value.ToSystem();
                            if (settings.MonsterRarityNames.NameBackground) BackgroundColor = settings.MonsterRarityNames.BackgroundColor.Value.ToSystem();
                        }

                        break;
                    case MonsterRarity.Unique:
                        if (!isMonsterWithIcon) MainTexture.UV = SpriteHelper.GetUV(MapIconsIndex.LootFilterLargeWhiteHexagon);
                        MainTexture.Color = Color.DarkOrange;
                        if (settings.MonsterRarityNames.ShowUniqueNames)
                        {
                            Text = RenderName.Split(',').FirstOrDefault();
                            TextColor = settings.MonsterRarityNames.TextColor.Value.ToSystem();
                            if (settings.MonsterRarityNames.NameBackground) BackgroundColor = settings.MonsterRarityNames.BackgroundColor.Value.ToSystem();
                        }

                        break;
                    default:
                        throw new ArgumentOutOfRangeException(
                            $"Rarity wrong was is {Rarity}. {objectMagicProperties?.DumpObject()}");
                }
            }
        }
    }
}