using Assets.HeroEditor.Common.CharacterScripts;
using Assets.HeroEditor.Common.CommonScripts;
using HeroEditor.Common.Enums;
using UnityEngine;

namespace Assets.HeroEditor.Common.ExampleScripts
{
    /// <summary>
    /// An example of how to change character's equipment.
    /// </summary>
    public class EquipmentExample : MonoBehaviour
    {
        public Character Character;

        public void EquipRandomArmor()
        {
            var randomIndex = Random.Range(0, Character.SpriteCollection.Armor.Count);
            var randomItem = Character.SpriteCollection.Armor[randomIndex];

            Character.Equip(randomItem, EquipmentPart.Armor);
        }

        public void RemoveArmor()
        {
            Character.UnEquip(EquipmentPart.Armor);
        }

        public void EquipRandomHelmet()
        {
            Character.Equip(Character.SpriteCollection.Helmet.Random(), EquipmentPart.Helmet);
        }

        public void RemoveHelmet()
        {
            Character.UnEquip(EquipmentPart.Helmet);
        }

        public void EquipRandomShield()
        {
            Character.Equip(Character.SpriteCollection.Shield.Random(), EquipmentPart.Shield);
        }

        public void RemoveShield()
        {
            Character.UnEquip(EquipmentPart.Shield);
        }

        public void EquipRandomWeapon()
        {
            Character.Equip(Character.SpriteCollection.MeleeWeapon1H.Random(), EquipmentPart.MeleeWeapon1H);
        }

        public void RemoveWeapon()
        {
            Character.UnEquip(EquipmentPart.MeleeWeapon1H);
        }

        public void EquipRandomBow()
        {
            Character.Equip(Character.SpriteCollection.Bow.Random(), EquipmentPart.Bow);
        }

        public void RemoveBow()
        {
            Character.UnEquip(EquipmentPart.Bow);
        }

        public void Reset()
        {
            Character.ResetEquipment();
            new CharacterAppearance().Setup(Character);
        }

        public void RandomAppearance()
        {
            Character.SetBody(Character.SpriteCollection.Hair.Random(), BodyPart.Hair, CharacterExtensions.RandomColor);
            Character.SetBody(Character.SpriteCollection.Eyebrows.Random(), BodyPart.Eyebrows);
            Character.SetBody(Character.SpriteCollection.Eyes.Random(), BodyPart.Eyes, CharacterExtensions.RandomColor);
            Character.SetBody(Character.SpriteCollection.Mouth.Random(), BodyPart.Mouth);
        }
    }
}