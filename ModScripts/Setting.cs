using System;
using System.Collections.Generic;
using System.IO;
using Newtonsoft.Json;
using UnityEngine;
using UnityModManagerNet;

namespace YoonKeyViewer
{
    public class Setting : UnityModManager.ModSettings
    {
        public readonly Dictionary<string, Dictionary<SystemLanguage, string>> Localization = new()
        {
            {
                "ykv.size", new()
                {
                    { SystemLanguage.English, "Size" },
                    { SystemLanguage.German, "Größe" },
                    { SystemLanguage.Korean, "크기" },
                }
            },
            {
                "ykv.posx", new()
                {
                    { SystemLanguage.English, "X Location" },
                    { SystemLanguage.German, "X-Position" },
                    { SystemLanguage.Korean, "X 위치" },
                }
            },
            {
                "ykv.posy", new()
                {
                    { SystemLanguage.English, "Y Location" },
                    { SystemLanguage.German, "Y-Position" },
                    { SystemLanguage.Korean, "Y 위치" },
                }
            },
            {
                "ykv.nervousbpm", new()
                {
                    { SystemLanguage.English, "Yoon is nervous if current BPM exceeds" },
                    { SystemLanguage.German, "Yoon wird nervös, wenn dieses BPM übersteigt" },
                    { SystemLanguage.Korean, "굥욷이 힘들어하는 체감 BPM 값" },
                }
            },
            {
                "ykv.fliphorizontal", new()
                {
                    { SystemLanguage.English, "Flip Horizontal" },
                    { SystemLanguage.German, "Horizontal spiegeln" },
                    { SystemLanguage.Korean, "수평 반전" },
                }
            },
            {
                "ykv.hidedesk", new()
                {
                    { SystemLanguage.English, "Hide Desk" },
                    { SystemLanguage.German, "Schreibtisch ausblenden" },
                    { SystemLanguage.Korean, "책상 숨기기" },
                }
            },
            {
                "ykv.pressakey", new()
                {
                    { SystemLanguage.English, "Press a key" },
                    { SystemLanguage.German, "Drücken Sie eine Taste" },
                    { SystemLanguage.Korean, "키를 누르세요" },
                }
            },
            {
                "ykv.settingslabel", new()
                {
                    { SystemLanguage.English, "Key Viewer Settings" },
                    { SystemLanguage.German, "Tastenanzeige-Einstellungen" },
                    { SystemLanguage.Korean, "키뷰어 설정" },
                }
            },
            {
                "ykv.hidelegs", new()
                {
                    { SystemLanguage.English, "Hide Legs" },
                    { SystemLanguage.German, "Beine ausblenden" },
                    { SystemLanguage.Korean, "다리 숨기기" },
                }
            },
            {
                "ykv.hidefeetkeyboard", new()
                {
                    { SystemLanguage.English, "Hide Feet Keyboard" },
                    { SystemLanguage.German, "Fußtastatur ausblenden" },
                    { SystemLanguage.Korean, "발 키보드 숨기기" },
                }
            },
            {
                "ykv.toprow", new()
                {
                    { SystemLanguage.English, "Top Row" },
                    { SystemLanguage.German, "Obere Reihe" },
                    { SystemLanguage.Korean, "상단 줄" },
                }
            },
            {
                "ykv.bottomrow", new()
                {
                    { SystemLanguage.English, "Bottom Row" },
                    { SystemLanguage.German, "Untere Reihe" },
                    { SystemLanguage.Korean, "하단 줄" },
                }
            },
            {
                "ykv.feetkeyboard", new()
                {
                    { SystemLanguage.English, "Feet Keyboard" },
                    { SystemLanguage.German, "Fußtastatur" },
                    { SystemLanguage.Korean, "발 키보드" },
                }
            }
        };
        public float Size = 1;
        public float LocationX = 0;
        public float LocationY = 1;
        public float NervousBPM = 300f;
        public bool FlipHorizontal = false;
        public bool HideDesk = false;
        public bool HideFeet = false;
        public bool HideFeetKeyboard = false;
        public bool ShareJipperResourcePack = true;

        [JsonIgnore] // Replace [DataExclude]
        public bool KeyCodeJipperResourcePack = false;

        public KeyCode[] KeyCodes = new KeyCode[]
        {
            KeyCode.A, KeyCode.S, KeyCode.D, KeyCode.F,
            KeyCode.J, KeyCode.K, KeyCode.L, KeyCode.Semicolon,
            KeyCode.Z, KeyCode.X, KeyCode.C, KeyCode.V,
            KeyCode.N, KeyCode.M, KeyCode.Comma, KeyCode.Period
        };

        public KeyCode[] FKeyCodes = new KeyCode[]
        {
            KeyCode.F2, KeyCode.F3, KeyCode.F7, KeyCode.F8
        };

        public string GetLocalized(string key)
        {
            if (Localization.TryGetValue(key, out var langDict) &&
                langDict.TryGetValue(RDString.language, out var localizedText))
            {
                return localizedText;
            }

            // Fallback to English or key
            return langDict?.TryGetValue(SystemLanguage.English, out var fallback) == true ? fallback : key;
        }

#if !BEPINEX
        public override void Save(UnityModManager.ModEntry modEntry)
        {
            try
            {
                var filepath = GetPath(modEntry);
                var settings = new JsonSerializerSettings
                {
                    Formatting = Formatting.Indented
                };
                var json = JsonConvert.SerializeObject(this, settings);
                File.WriteAllText(filepath, json);

                //if (KeyCodeJipperResourcePack)
                //{
                //    JipperResourcePackAPI.SaveSetting(); // Optional: stub or remove if needed
                //}
            }
            catch (Exception e)
            {
                Main.Logger?.Log($"[Setting.Save] Exception: {e}");
            }
        }

        public override string GetPath(UnityModManager.ModEntry modEntry)
        {
            return Path.Combine(modEntry.Path, GetType().Name + ".json");
        }

        public static Setting Load(UnityModManager.ModEntry modEntry)
        {
            var filepath = Path.Combine(modEntry.Path, typeof(Setting).Name + ".json");

            if (!File.Exists(filepath))
            {
                return new Setting();
            }

            try
            {
                var settings = new JsonSerializerSettings
                {
                    Formatting = Formatting.Indented,
                    ObjectCreationHandling = ObjectCreationHandling.Replace
                };

                var json = File.ReadAllText(filepath);
                var setting = JsonConvert.DeserializeObject<Setting>(json, settings) ?? new Setting();

                // Optional external call
                //JipperResourcePackAPI.CheckJipperResourcePack(); // Remove or replace if needed

                return setting;
            }
            catch (Exception e)
            {
                Main.Logger?.Log($"[Setting.Load] Failed to load settings: {e}");
                return new Setting();
            }
        }
#endif
        //public void ShareJipperKeyCode(bool enable)
        //{
        //    if (enable)
        //    {
        //        KeyCodes = JipperResourcePackAPI.GetKey16(); // External call
        //        KeyCodeJipperResourcePack = true;
        //    }
        //    else
        //    {
        //        var keyCodes = new KeyCode[16];
        //        Array.Copy(KeyCodes, keyCodes, 16);
        //        KeyCodes = keyCodes;
        //        KeyCodeJipperResourcePack = false;
        //    }
        //}
    }
}