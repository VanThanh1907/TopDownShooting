using System;
using System.Collections.Generic;
using System.Linq;
using Assets.HeroEditor.Common.CharacterScripts;
using Assets.HeroEditor.Common.CommonScripts;
using Assets.HeroEditor.FantasyInventory.Scripts.Data;
using Assets.HeroEditor.FantasyInventory.Scripts.Enums;
using HeroEditor.Common.Enums;
using UnityEngine;

namespace Assets.HeroEditor.FantasyInventory.Scripts
{
    public class CharacterInventorySetup
    {
        public static void Setup(Character character, List<Item> equipped)
        {
            character.ResetEquipment();
            character.HideEars = false;
            character.FullHair = true;

            foreach (var item in equipped)
            {
                try
                {
                    switch (item.Params.Type)
                    {
                        case ItemType.Helmet:
                            var entry = character.SpriteCollection.Helmet.Single(i => i.Path == item.Params.Path);

                            character.Helmet = entry.Sprite;
                            character.HideEars = entry.Tags.Contains("HideEars");
                            character.FullHair = entry.Tags.Contains("FullHair");
                            break;
                        case ItemType.Armor:
                            character.Armor = character.SpriteCollection.Armor.FindSpritesByPath(item.Params.Path);
                            break;
                        case ItemType.Shield:
                            character.Shield = character.SpriteCollection.Shield.FindSpriteByPath(item.Params.Path);
                            character.WeaponType = WeaponType.Melee1H;
                            break;
                        case ItemType.Weapon:

                            switch (item.Params.Class)
                            {
                                case ItemClass.Bow:
                                    character.WeaponType = WeaponType.Bow;
                                    character.Bow = character.SpriteCollection.Bow.FindSpritesByPath(item.Params.Path);
                                    break;
                                default:
                                    if (item.IsFirearm)
                                    {
                                        throw new NotImplementedException("Firearm equipping is not implemented. Implement if needed.");
                                    }
                                    else
                                    {
                                        character.WeaponType = item.Params.Tags.Contains(ItemTag.TwoHanded) ? WeaponType.Melee2H : WeaponType.Melee1H;
                                        character.PrimaryMeleeWeapon = (character.WeaponType == WeaponType.Melee1H ? character.SpriteCollection.MeleeWeapon1H : character.SpriteCollection.MeleeWeapon2H).FindSpriteByPath(item.Params.Path);
                                    }
                                    break;
                            }
                            break;
                    }
                }
                catch (Exception e)
                {
                    Debug.LogErrorFormat("Unable to equip {0} ({1})", item.Params.Path, e.Message);
                }
            }

            character.Initialize();
        }
    }
}