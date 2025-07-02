using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Assets.HeroEditor.Common.CharacterScripts;
using Assets.HeroEditor.Common.CharacterScripts.Firearms;
using Assets.HeroEditor.Common.CommonScripts;
using Assets.HeroEditor.Common.Data;
using Assets.HeroEditor.Common.ExampleScripts;
using Assets.HeroEditor.FantasyInventory.Scripts.Data;
using Assets.HeroEditor.FantasyInventory.Scripts.Interface.Elements;
using Assets.HeroEditor4D.SimpleColorPicker.Scripts;
using HeroEditor.Common;
using HeroEditor.Common.Enums;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace Assets.HeroEditor.Common.EditorScripts
{
    /// <summary>
    /// Character editor UI and behaviour.
    /// </summary>
    public class CharacterEditor : CharacterEditorBase
    {
        [Header("Public")]
        public Transform Tabs;
        public ScrollInventory Inventory;
        public Text ItemName;

        [Header("Other")]
        public List<Toggle> EditionToggles;
        public string EditionFilter;
        public List<string> PaintParts;
        public Button PaintButton;
        public ColorPicker ColorPicker;
        public string FilePickerPath;

        [Header("Import/Export")]
        public List<string> ImportParts;
        public Button ImportButton;
        public Button ExportButton;

        public Action<Item> EquipCallback;

        public static string CharacterJson;

        private Toggle ActiveTab => Tabs.GetComponentsInChildren<Toggle>().Single(i => i.isOn);
        
        /// <summary>
        /// Called automatically on app start.
        /// </summary>
        public void Awake()
        {
            RestoreTempCharacter();
        }

        public new void Start()
        {
            base.Start();

            if (Tabs.gameObject.activeSelf)
            {
                Tabs.GetComponentInChildren<Toggle>().isOn = true;
            }

            FilePickerPath = Application.dataPath;
        }

        /// <summary>
        /// This can be used as an example for building your own inventory UI.
        /// </summary>
        public void OnSelectTab(bool value)
        {
            if (value)
            {
                Refresh();
            }
        }

        public void Refresh(int defaultIndex = 0)
        {
            Item.GetParams = null;

            Dictionary<string, SpriteGroupEntry> dict;
            Action<Item> equipAction;
            int equippedIndex;
            var tab = ActiveTab;
            var spriteCollection = Character.SpriteCollection;

            switch (tab.name)
            {
                case "Helmet":
                {
                    dict = spriteCollection.Helmet.ToDictionary(i => i.Id, i => i);
                    equipAction = item => Character.Equip(dict[item.Id], EquipmentPart.Helmet);
                    equippedIndex = spriteCollection.Helmet.FindIndex(i => i.Sprites.Contains(Character.Helmet));
                    break;
                }
                case "Armor":
                {
                    dict = spriteCollection.Armor.ToDictionary(i => i.Id, i => i);
                    equipAction = item => Character.Equip(dict[item.Id], EquipmentPart.Armor);
                    equippedIndex = Character.Armor == null ? -1 : spriteCollection.Armor.FindIndex(i => i.Sprites.SequenceEqual(Character.Armor));
                    break;
                }
                case "Pauldrons":
                case "Vest":
                case "Gloves":
                case "Belt":
                case "Boots":
                {
                    string part;

                    switch (tab.name)
                    {
                        case "Pauldrons": part = "ArmR"; break;
                        case "Vest": part = "Torso"; break;
                        case "Gloves": part = "SleeveR"; break;
                        case "Belt": part = "Pelvis"; break;
                        case "Boots": part = "Shin"; break;
                        default: throw new NotSupportedException(tab.name);
                    }

                    dict = spriteCollection.Armor.ToDictionary(i => i.Id, i => i);
                    equipAction = item => Character.Equip(dict[item.Id], tab.name.ToEnum<EquipmentPart>());
                    equippedIndex = Character.Armor == null ? -1 : spriteCollection.Armor.FindIndex(i => i.Sprites.Contains(Character.Armor.SingleOrDefault(j => j.name == part)));
                    Item.GetParams = item => new ItemParams { Id = item.Id.Replace(".Armor.", $".{tab.name}."), Path = dict[item.Id] == null ? null : dict[item.Id].Path.Replace("Armor/", $"{tab.name}/") + $".{tab.name}", Meta = dict[item.Id] == null ? null : Serializer.Serialize(dict[item.Id].Tags) };
                    break;
                }
                case "Shield":
                {
                    dict = spriteCollection.Shield.ToDictionary(i => i.Id, i => i);
                    equipAction = item => Character.Equip(dict[item.Id], EquipmentPart.Shield);
                    equippedIndex = spriteCollection.Shield.FindIndex(i => i.Sprites.Contains(Character.Shield));
                    break;
                }
                case "Melee1H":
                {
                    dict = spriteCollection.MeleeWeapon1H.ToDictionary(i => i.Id, i => i);
                    equipAction = item => Character.Equip(dict[item.Id], EquipmentPart.MeleeWeapon1H);
                    equippedIndex = spriteCollection.MeleeWeapon1H.FindIndex(i => i.Sprites.Contains(Character.PrimaryMeleeWeapon));
                    break;
                }
                case "Melee2H":
                {
                    dict = spriteCollection.MeleeWeapon2H.ToDictionary(i => i.Id, i => i);
                    equipAction = item => Character.Equip(dict[item.Id], EquipmentPart.MeleeWeapon2H);
                    equippedIndex = spriteCollection.MeleeWeapon2H.FindIndex(i => i.Sprites.Contains(Character.PrimaryMeleeWeapon));
                    break;
                }
                case "MeleePaired":
                {
                    dict = spriteCollection.MeleeWeapon1H.ToDictionary(i => i.Id, i => i);
                    equipAction = item => Character.Equip(dict[item.Id], EquipmentPart.MeleeWeaponPaired);
                    equippedIndex = spriteCollection.MeleeWeapon1H.FindIndex(i => i.Sprites.Contains(Character.SecondaryMeleeWeapon));
                    break;
                }
                case "Bow":
                {
                    dict = spriteCollection.Bow.ToDictionary(i => i.Id, i => i);
                    equipAction = item => Character.Equip(dict[item.Id], EquipmentPart.Bow);
                    equippedIndex = Character.Bow == null ? -1 : spriteCollection.Bow.FindIndex(i => i.Sprites.SequenceEqual(Character.Bow));
                    break;
                }
                case "Firearm1H":
                {
                    dict = spriteCollection.Firearms1H.ToDictionary(i => i.Id, i => i);
                    equipAction = item =>
                    {
                        if (item.Id == "Empty")
                        {
                            Character.Equip(null, EquipmentPart.MeleeWeapon1H);
                        }
                        else
                        {
                            var itemName = item.Id.Split('.')[3];

                            Character.GetFirearm().Params = item.Id == "Empty" ? null : FindFirearmParams(itemName);
                            Character.Equip(dict[item.Id], EquipmentPart.Firearm1H);
                        }
                       
                        Character.Animator.SetBool("Ready", true);
                    };
                    equippedIndex = Character.Firearms == null ? -1 : spriteCollection.Firearms1H.FindIndex(i => i.Sprites.SequenceEqual(Character.Firearms));
                    break;
                }
                case "Firearm2H":
                {
                    dict = spriteCollection.Firearms2H.ToDictionary(i => i.Id, i => i);
                    equipAction = item =>
                    {
                        if (item.Id == "Empty")
                        {
                            Character.Equip(null, EquipmentPart.MeleeWeapon1H);
                        }
                        else
                        {
                            var itemName = item.Id.Split('.')[3];

                            Character.GetFirearm().Params = item.Id == "Empty" ? null : FindFirearmParams(itemName);
                            Character.Equip(dict[item.Id], EquipmentPart.Firearm2H);
                        }
                        
                        Character.Animator.SetBool("Ready", true);
                    };
                    equippedIndex = Character.Firearms == null ? -1 : spriteCollection.Firearms2H.FindIndex(i => i.Sprites.SequenceEqual(Character.Firearms));
                    break;
                }
                case "Cape":
                {
                    dict = spriteCollection.Cape.ToDictionary(i => i.Id, i => i);
                    equipAction = item => Character.Equip(dict[item.Id], EquipmentPart.Cape);
                    equippedIndex = spriteCollection.Cape.FindIndex(i => i.Sprites.Contains(Character.Cape));
                    break;
                }
                case "Back":
                {
                    dict = spriteCollection.Back.ToDictionary(i => i.Id, i => i);
                    equipAction = item => Character.Equip(dict[item.Id], EquipmentPart.Back);
                    equippedIndex = spriteCollection.Back.FindIndex(i => i.Sprites.Contains(Character.Back));
                    break;
                }
                case "Body":
                {
                    dict = spriteCollection.Body.ToDictionary(i => i.Id, i => i);
                    equipAction = item => Character.SetBody(dict[item.Id], BodyPart.Body);
                    equippedIndex = Character.Body == null ? -1 : spriteCollection.Body.FindIndex(i => i.Sprites.SequenceEqual(Character.Body));
                    break;
                }
                case "Head":
                {
                    dict = spriteCollection.Head.ToDictionary(i => i.Id, i => i);
                    equipAction = item => Character.SetBody(dict[item.Id], BodyPart.Head);
                    equippedIndex = spriteCollection.Head.FindIndex(i => i.Sprites.Contains(Character.Head));
                    break;
                }
                case "Ears":
                {
                    dict = spriteCollection.Ears.ToDictionary(i => i.Id, i => i);
                    equipAction = item => Character.SetBody(dict[item.Id], BodyPart.Ears);
                    equippedIndex = spriteCollection.Ears.FindIndex(i => i.Sprites.Contains(Character.Ears));
                    break;
                }
                case "Eyebrows":
                {
                    dict = spriteCollection.Eyebrows.ToDictionary(i => i.Id, i => i);
                    equipAction = item => Character.SetBody(dict[item.Id], BodyPart.Eyebrows);
                    equippedIndex = spriteCollection.Eyebrows.FindIndex(i => i.Sprites.Contains(Character.Expressions[0].Eyebrows));
                    break;
                }
                case "Eyes":
                {
                    dict = spriteCollection.Eyes.ToDictionary(i => i.Id, i => i);
                    equipAction = item => Character.SetBody(dict[item.Id], BodyPart.Eyes);
                    equippedIndex = spriteCollection.Eyes.FindIndex(i => i.Sprites.Contains(Character.Expressions[0].Eyes));
                    break;
                }
                case "Hair":
                {
                    dict = spriteCollection.Hair.ToDictionary(i => i.Id, i => i);
                    equipAction = item => Character.SetBody(dict[item.Id], BodyPart.Hair);
                    equippedIndex = spriteCollection.Hair.FindIndex(i => i.Sprites.Contains(Character.Hair));
                    break;
                }
                case "Beard":
                {
                    dict = spriteCollection.Beard.ToDictionary(i => i.Id, i => i);
                    equipAction = item => Character.SetBody(dict[item.Id], BodyPart.Beard);
                    equippedIndex = spriteCollection.Beard.FindIndex(i => i.Sprites.Contains(Character.Beard));
                    break;
                }
                case "Mouth":
                {
                    dict = spriteCollection.Mouth.ToDictionary(i => i.Id, i => i);
                    equipAction = item => Character.SetBody(dict[item.Id], BodyPart.Mouth);
                    equippedIndex = spriteCollection.Mouth.FindIndex(i => i.Sprites.Contains(Character.Expressions[0].Mouth));
                    break;
                }
                //case "Makeup":
                //    {
                //        dict = SpriteCollection.Makeup.ToDictionary(i => i.FullName, i => i);
                //        equipAction = item => Character.SetBody(dict[item.Id], BodyPart.Makeup);
                //        equippedIndex = SpriteCollection.Makeup.FindIndex(i => i.Sprites.Contains(Character.Makeup));
                //        break;
                //    }
                case "Earrings":
                {
                    dict = spriteCollection.Earrings.ToDictionary(i => i.Id, i => i);
                    equipAction = item => Character.Equip(dict[item.Id], EquipmentPart.Earrings);
                    equippedIndex = spriteCollection.Earrings.FindIndex(i => i.Sprites.Contains(Character.Earrings));
                    break;
                }
                case "Glasses":
                {
                    dict = spriteCollection.Glasses.ToDictionary(i => i.Id, i => i);
                    equipAction = item => Character.Equip(dict[item.Id], EquipmentPart.Glasses);
                    equippedIndex = spriteCollection.Glasses.FindIndex(i => i.Sprites.Contains(Character.Glasses));
                    break;
                }
                case "Mask":
                {
                    dict = spriteCollection.Mask.ToDictionary(i => i.Id, i => i);
                    equipAction = item => Character.Equip(dict[item.Id], EquipmentPart.Mask);
                    equippedIndex = spriteCollection.Mask.FindIndex(i => i.Sprites.Contains(Character.Mask));
                    break;
                }
                case "Supplies":
                {
                    dict = spriteCollection.Supplies.ToDictionary(i => i.Id, i => i);
                    equipAction = item => { Debug.LogWarning("Supplies are present as icons only and are not displayed on a character. Can be used for inventory."); };
                    equippedIndex = -1;
                    break;
                }
                default: throw new NotImplementedException(tab.name);
            }

            var items = dict.Values.Select(i => new Item(i.Id)).ToList();

            items.Insert(0, new Item("Empty"));
            dict.Add("Empty", null);

            if (Item.GetParams == null)
            {
                Item.GetParams = item => new ItemParams { Id = item.Id, Path = dict[item.Id]?.Path, Meta = dict[item.Id] == null ? null : Serializer.Serialize(dict[item.Id].Tags) }; // We override GetParams method because we don't have a database with item params.
            }

            IconCollection.Active = IconCollection.Instances[Character.SpriteCollection.Id];

            if (equippedIndex == -1) equippedIndex = defaultIndex;

            Inventory.OnLeftClick = item =>
            {
                equipAction(item);
                EquipCallback?.Invoke(item);
                ItemName.text = item.Params.Id ?? "Empty";
                SetPaintButton(tab.name, item);
            };

            var equipped = items.Count > equippedIndex + 1 ? items[equippedIndex + 1] : null;

            if (EditionFilter != "")
            {
                items.RemoveAll(i => i.Id != "Empty" && dict[i.Id].Edition != "Common" && dict[i.Id].Edition != "Extensions" && dict[i.Id].Edition != EditionFilter);

                if (equipped != null && !dict.ContainsKey(equipped.Id))
                {
                    equipped = null;
                }
            }

            Inventory.Initialize(ref items, equipped, reset: true);
            Inventory.ScrollRect.verticalNormalizedPosition = 1;
            SetPaintButton(tab.name, equipped);

            if (ImportButton) ImportButton.interactable = ImportParts.Contains(tab.name);
        }

        private void SetPaintButton(string tab, Item item)
        {
            var tags = item?.Params.MetaToList() ?? new List<string>();

            PaintButton.interactable = PaintParts.Contains(tab) && !tags.Contains("NoPaint") || tags.Contains("Paint");
        }

        /// <summary>
        /// Remove all equipment.
        /// </summary>
        public void Reset()
        {
            Character.ResetEquipment();

            var appearance = new CharacterAppearance();

            if (Character.SpriteCollection.Id == "UndeadHeroes")
            {
                appearance.Hair = null;
                appearance.Ears = null;
                appearance.Eyebrows = null;
                appearance.Eyes = "UndeadHeroes.Skeletons.Eyes.Skeleton2EyesGlow";
                appearance.Mouth = null;
                appearance.Head = "UndeadHeroes.Skeletons.Head.Skeleton1";
                appearance.Body = "UndeadHeroes.Skeletons.Body.Skeleton1";
            }

            appearance.Setup(Character);
            Refresh(-1);
        }

        /// <summary>
        /// Randomize character.
        /// </summary>
        public void Randomize()
        {
            Character.Randomize();
            OnSelectTab(true);
        }

        /// <summary>
	    /// Save character to json.
	    /// </summary>
	    public void SaveToJson()
	    {
            StartCoroutine(StandaloneFilePicker.SaveFile("Save as JSON", "", "New character", "json", Encoding.Default.GetBytes(Character.ToJson()), (success, path) => { Debug.Log(success ? $"Saved as {path}" : "Error saving."); }));
		}

		/// <summary>
		/// Load character from json.
		/// </summary>
		public void LoadFromJson()
	    {
            StartCoroutine(StandaloneFilePicker.OpenFile("Open as JSON", "", "json", (success, path, bytes) =>
            {
                if (success)
                {
                    var json = System.IO.File.ReadAllText(path);

                    Character.FromJson(json);
                }
            }));
	    }

        #if UNITY_EDITOR

        /// <summary>
        /// Save character to prefab.
        /// </summary>
        public void Save()
        {
            var path = UnityEditor.EditorUtility.SaveFilePanel("Save character prefab (should be inside Assets folder)", FilePickerPath, "New character", "prefab");

            if (path.Length > 0)
            {
                if (!path.Contains("/Assets/")) throw new Exception("Unity can save prefabs only inside Assets folder!");

                Save("Assets" + path.Replace(Application.dataPath, null));
                FilePickerPath = path;
            }
		}

	    /// <summary>
		/// Load character from prefab.
		/// </summary>
		public void Load()
        {
            var path = UnityEditor.EditorUtility.OpenFilePanel("Load character prefab", FilePickerPath, "prefab");

            if (path.Length > 0)
            {
                Load("Assets" + path.Replace(Application.dataPath, null));
                FilePickerPath = path;
            }
		}

	    public override void Save(string path)
		{
			Character.transform.localScale = Vector3.one;

			#if UNITY_2018_3_OR_NEWER

			UnityEditor.PrefabUtility.SaveAsPrefabAsset(Character.gameObject, path);

			#else

			UnityEditor.PrefabUtility.CreatePrefab(path, Character.gameObject);

			#endif

            Debug.LogFormat("Prefab saved as {0}", path);
        }

        public override void Load(string path)
        {
			var character = UnityEditor.AssetDatabase.LoadAssetAtPath<Character>(path);

            Character.GetFirearm().Params = character.Firearm.Params; // TODO: Workaround
			Load(character);
            Character.GetComponent<CharacterBodySculptor>().OnCharacterLoaded(character);
        }

	    #else

        public override void Save(string path)
        {
            throw new NotSupportedException();
        }

        public override void Load(string path)
        {
            throw new NotSupportedException();
        }

        #endif

        /// <summary>
        /// Load a scene by name.
        /// </summary>
        public void LoadScene(string sceneName)
        {
            #if UNITY_EDITOR

            if (!UnityEditor.EditorBuildSettings.scenes.Any(i => i.path.Contains(sceneName) && i.enabled))
            {
	            UnityEditor.EditorUtility.DisplayDialog("Hero Editor", $"Please add '{sceneName}.scene' to Build Settings!", "OK");
				return;
            }

            #endif

            if (sceneName == "QUICK START")
            {
                QuickStart.ReturnSceneName = SceneManager.GetActiveScene().name;
            }
            else if (sceneName.StartsWith("TestRoom"))
            {
                TestRoom.ReturnSceneName = SceneManager.GetActiveScene().name;
            }

            CharacterJson = Character.ToJson();
            SceneManager.LoadScene(sceneName);
		}

        /// <summary>
		/// Navigate to URL.
		/// </summary>
		public void Navigate(string url)
        {
            #if UNITY_WEBGL && !UNITY_EDITOR

            Application.ExternalEval($"window.open('{url}')");

            #else

			Application.OpenURL(url);

            #endif
        }

		protected override void SetFirearmParams(SpriteGroupEntry entry)
        {
            if (entry == null) return;

            Character.GetFirearm().Params = FindFirearmParams(entry.Name);
		}

        private Color _color;

        public void OpenColorPicker()
        {
            var currentColor = ResolveParts(ActiveTab.name).FirstOrDefault()?.color ?? Color.white;

            ColorPicker.Color = _color = currentColor;
            ColorPicker.OnColorChanged = Paint;
            ColorPicker.SetActive(true);
        }

        public void CloseColorPicker(bool apply)
        {
            if (!apply) Paint(_color);

            ColorPicker.SetActive(false);
        }

        public void Paint(Color color)
        {
            foreach (var part in ResolveParts(ActiveTab.name))
            {
                part.color = color;
                part.sharedMaterial = color == Color.white ? DefaultMaterial : ActiveTab.name == "Eyes" ? EyesPaintMaterial : EquipmentPaintMaterial;
            }

            if (ActiveTab.name == "Eyes")
            {
                Character.Expressions[0].EyesColor = Character.Expressions[1].EyesColor = color;
            }
        }

        public void SetFullHair()
        {
            Character.FullHair = !Character.FullHair;
            Character.HairRenderer.maskInteraction = Character.FullHair ? SpriteMaskInteraction.None : SpriteMaskInteraction.VisibleInsideMask;
        }

        public void OnEditionChanged(bool value)
        {
            if (!value) return;

            foreach (var toggle in EditionToggles)
            {
                if (toggle.isOn) EditionFilter = toggle.name;
            }

            Refresh();
        }

        public void ImportEquipment()
        {
            StartCoroutine(StandaloneFilePicker.OpenFile($"Select {ActiveTab.name}", "", "png", (success, path, bytes) =>
            {
                if (success)
                {
                    var texture = new Texture2D(2, 2, TextureFormat.RGBA32, mipChain: false) { filterMode = FilterMode.Bilinear };

                    texture.LoadImage(bytes);
                    texture.PremultiplyAlpha();

                    Sprite CreateSprite(Texture2D texture2d, Sprite reference)
                    {
                        return Sprite.Create(texture2d, reference.rect, new Vector2(reference.pivot.x / reference.rect.width, reference.pivot.y / reference.rect.height), reference.pixelsPerUnit);
                    }

                    switch (ActiveTab.name)
                    {
                        case "Body": Character.BodyRenderers.ForEach(i => i.sprite = CreateSprite(texture, i.sprite)); break;
                        case "Helmet": Character.HelmetRenderer.sprite = CreateSprite(texture, Character.HelmetRenderer.sprite); break;
                        case "Armor": Character.ArmorRenderers.ForEach(i => i.sprite = CreateSprite(texture, i.sprite)); break;
                        case "Shield": Character.ShieldRenderer.sprite = CreateSprite(texture, Character.ShieldRenderer.sprite); break;
                        case "Melee1H": Character.PrimaryMeleeWeaponRenderer.sprite = CreateSprite(texture, Character.PrimaryMeleeWeaponRenderer.sprite); break;
                        case "Melee2H": Character.PrimaryMeleeWeaponRenderer.sprite = CreateSprite(texture, Character.PrimaryMeleeWeaponRenderer.sprite); break;
                        case "Bow": Character.BowRenderers.ForEach(i => i.sprite = CreateSprite(texture, i.sprite)); break;
                        default: throw new NotImplementedException();
                    }
                }
            }));
        }

        public void ExportEquipment()
        {
            Sprite sprite;

            switch (ActiveTab.name)
            {
                case "Helmet": sprite = Character.Helmet; break;
                case "Armor":
                case "Vest":
                case "Pauldrons":
                case "Gloves":
                case "Belt":
                case "Boots": sprite = Character.Armor[0]; break;
                case "Shield": sprite = Character.Shield; break;
                case "Melee1H":
                case "Melee2H": sprite = Character.PrimaryMeleeWeapon; break;
                case "MeleePaired": sprite = Character.SecondaryMeleeWeapon; break;
                case "Bow": sprite = Character.Bow[0]; break;
                case "Back": sprite = Character.Back; break;
                case "Cape": sprite = Character.Cape; break;
                case "Firearm1H":
                case "Firearm2H": sprite = Character.Firearms[0]; break;
                case "Body": sprite = Character.Body[0]; break;
                case "Head": sprite = Character.Head; break;
                case "Hair": sprite = Character.Hair; break;
                case "Beard": sprite = Character.Beard; break;
                case "Eyebrows": sprite = Character.Expressions[0].Eyebrows; break;
                case "Eyes": sprite = Character.Expressions[0].Eyes; break;
                case "Ears": sprite = Character.Ears; break;
                case "Mouth": sprite = Character.Expressions[0].Mouth; break;
                case "Earrings": sprite = Character.Earrings; break;
                case "Mask": sprite = Character.Mask; break;
                case "Glasses": sprite = Character.Glasses; break;
                default: throw new NotImplementedException();
            }

            var tmp = RenderTexture.GetTemporary(sprite.texture.width, sprite.texture.height, 0, RenderTextureFormat.Default, RenderTextureReadWrite.Linear);

            Graphics.Blit(sprite.texture, tmp);

            var previous = RenderTexture.active;

            RenderTexture.active = tmp;

            var myTexture2D = new Texture2D(sprite.texture.width, sprite.texture.height);

            myTexture2D.ReadPixels(new Rect(0, 0, tmp.width, tmp.height), 0, 0);
            myTexture2D.Apply();

            RenderTexture.active = previous;
            RenderTexture.ReleaseTemporary(tmp);

            var bytes = myTexture2D.EncodeToPNG();
            
            StartCoroutine(StandaloneFilePicker.SaveFile($"Save {ActiveTab.name}", "", ItemName.text, "png", bytes, (success, path) => { Debug.Log(success ? $"Saved as {path}" : "Error saving."); }));
        }

        private void RestoreTempCharacter()
        {
            if (CharacterJson != null)
            {
                Character.FromJson(CharacterJson);
            }
        }

        private static FirearmParams FindFirearmParams(string weaponName)
        {
            foreach (var collection in FirearmCollection.Instances.Values)
            {
                var found = collection.Firearms.SingleOrDefault(i => i.Name == weaponName);

                if (found != null) return found;
            }

            throw new Exception($"Can't find firearm params for {weaponName}.");
        }

        protected override void FeedbackTip()
	    {
			#if UNITY_EDITOR

		    var success = UnityEditor.EditorUtility.DisplayDialog("Hero Editor", "Hi! Thank you for using my asset! I hope you enjoy making your game with it. The only thing I would ask you to do is to leave a review on the Asset Store. It would be awesome support for my asset, thanks!", "Review", "Later");
			
			RequestFeedbackResult(success);

			#endif
	    }
    }
}