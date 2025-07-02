using System;
using System.Linq;
using HeroEditor.Common;
using HeroEditor.Common.Data;
using UnityEngine;

namespace Assets.HeroEditor.Common.CharacterScripts
{
    [Serializable]
    public class CharacterAppearance
    {
        public string Hair = "Common.Basic.Hair.BuzzCut";
        public string Ears = "Common.Basic.Ears.HumanEars";
        public string Eyebrows = "Common.Basic.Eyebrows.Eyebrows1";
        public string Eyes = "Common.Basic.Eyes.Male";
        public string Mouth = "Common.Basic.Mouth.Normal";
        public string Head = "Common.Basic.Head.Human";
        public string Body = "Common.Basic.Body.Human";

        public Color32 HairColor = new Color32(150, 50, 0, 255);
        public Color32 EyesColor = new Color32(0, 200, 255, 255);
        public Color32 BodyColor = new Color32(255, 200, 120, 255);
        
        public void Setup(CharacterBase character, bool initialize = true)
        {
            character.Hair = Hair == null ? null : character.SpriteCollection.Hair.Single(i => i.Id == Hair)?.Sprite;
            character.HairRenderer.color = HairColor;
            character.Ears = Ears == null ? null : character.SpriteCollection.Ears.Single(i => i.Id == Ears)?.Sprite;

            if (character.Expressions.Count > 0)
            {
                character.Expressions[0] = new Expression { Name = "Default" };
                character.Expressions[0].Eyebrows = Eyebrows == null ? null : character.SpriteCollection.Eyebrows.Single(i => i.Id == Eyebrows)?.Sprite;
                character.Expressions[0].Eyes = Eyes == null ? null : character.SpriteCollection.Eyes.Single(i => i.Id == Eyes)?.Sprite;
                character.Expressions[0].Mouth = Mouth == null ? null : character.SpriteCollection.Mouth.Single(i => i.Id == Mouth)?.Sprite;

                foreach (var expression in character.Expressions)
                {
                    if (expression.Name != "Dead") expression.EyesColor = EyesColor;
                }
            }

            character.EyesRenderer.color = EyesColor;
            character.HeadRenderer.color = BodyColor;
            character.BodyRenderers.ForEach(i => i.color = BodyColor);
            character.EarsRenderer.color = BodyColor;

            var head = character.SpriteCollection.Head.Single(i => i.Id == Head);
            var body = character.SpriteCollection.Body.Single(i => i.Id == Body);

            character.Head = head.Sprite;
            character.Body = body.Sprites;

            if (body.Tags.Contains("NoMouth"))
            {
                character.Expressions.ForEach(i => i.Mouth = null);
            }

            if (initialize) character.Initialize();
        }

        public string ToJson()
        {
            return JsonUtility.ToJson(this);
        }

        public static CharacterAppearance FromJson(string json)
        {
            return JsonUtility.FromJson<CharacterAppearance>(json);
        }
    }
}