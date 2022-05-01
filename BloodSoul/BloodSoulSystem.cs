using BloodSoul.UI;
using BloodSoul.UI.UIStates;
using System.Collections.Generic;
using System.IO;
using Terraria;
using Terraria.ModLoader;
using Terraria.ModLoader.IO;
using Terraria.UI;

namespace BloodSoul
{
    public class BloodSoulSystem : ModSystem
    {
        public static BloodSoulSystem Instance { get; private set; }
        public BloodSoulSystem()
        {
            Instance = this;
        }
        public UserInterface TitleUILayer;
        public TitleCard TitleCardUIElement;
        public static bool downedHolyLightSword;//击败阴凌剑
        public static bool downedBloodCrystalEye;//击败血眼
        public static bool downedGelSpider;//击败毒蛛
        public static bool downedStarGazer;//击败观星者
        public static bool downedMechanicalBoss;//击败机械Boss
        public static bool Plant;//击败花花
        public static bool FirstEntry;
        public static bool MagicAids;
        public static bool SharaIshvalda;//弟弟龙
        public static bool GodLoveYou;//神佑模式
        public static bool downedUang;
        public static bool downedRockSnake;
        public static bool downedTideSpirit;
        public static bool downedDarkStarLord;
        public override void SaveWorldData(TagCompound tag)
        {
            if (downedHolyLightSword)
            {
                tag["downedHolyLightSword"] = true;
            }
            if (downedBloodCrystalEye)
            {
                tag["downedBloodCrystalEye"] = true;
            }
            if (FirstEntry == true)
            {
                tag["FirstEntry"] = true;
            }
            if (downedMechanicalBoss)
            {
                tag["downedMechanicalBoss"] = true;
            }
            if (downedGelSpider)
            {
                tag["downedGelSpider"] = true;
            }
            if (Plant)
            {
                tag["Plant"] = true;
            }
            if (downedStarGazer)
            {
                tag["downedStarGazer"] = true;
            }
            if (SharaIshvalda)
            {
                tag["SharaIshvalda"] = true;
            }
            if (GodLoveYou)
            {
                tag["GodLoveYou"] = true;
            }
            if (downedUang)
            {
                tag["downedUang"] = true;
            }
            if (downedRockSnake)
            {
                tag["downedRockSnake"] = true;
            }
            if (downedTideSpirit)
            {
                tag["downedTideSpirit"] = true;
            }
            if (downedDarkStarLord)
            {
                tag["downedDarkStarLord"] = true;
            }
        }
        public override void OnWorldLoad()
        {
            downedHolyLightSword = false;//重置世界击败
            downedBloodCrystalEye = false;
            FirstEntry = false;
            downedMechanicalBoss = false;
            downedGelSpider = false;
            Plant = false;
            downedStarGazer = false;
            MagicAids = false;
            SharaIshvalda = false;
            GodLoveYou = false;
            downedUang = false;
            downedRockSnake = false;
            downedTideSpirit = false;
            downedDarkStarLord = false;
        }
        public override void OnWorldUnload()
        {
            downedHolyLightSword = false;
            downedBloodCrystalEye = false;
            downedGelSpider = false;
            downedMechanicalBoss = false;
            FirstEntry = false;
            Plant = false;
            downedStarGazer = false;
            MagicAids = false;
            SharaIshvalda = false;
            GodLoveYou = false;
            downedUang = false;
            downedRockSnake = false;
            downedTideSpirit= false;
            downedDarkStarLord= false;
        }
        public override void NetSend(BinaryWriter writer)
        {
            BitsByte flags = new();//最多只有8个索引
            flags[0] = downedHolyLightSword;//向多人服务器写入击败信息
            flags[1] = downedBloodCrystalEye;
            flags[2] = downedMechanicalBoss;
            flags[3] = downedGelSpider;
            flags[4] = Plant;
            flags[5] = downedStarGazer;
            flags[6] = SharaIshvalda;
            flags[7] = GodLoveYou;
            writer.Write(flags);
            BitsByte flags2 = new();
            flags2[0] = downedUang;
            flags2[1] = downedRockSnake;
            flags2[2] = downedTideSpirit;
            flags2[3] = downedDarkStarLord;
            writer.Write(flags2);
        }
        public override void NetReceive(BinaryReader reader)
        {
            BitsByte flags = reader.ReadByte();//与上面的对应
            downedHolyLightSword = flags[0];
            downedBloodCrystalEye = flags[1];
            downedMechanicalBoss = flags[2];
            downedGelSpider = flags[3];
            Plant = flags[4];
            downedStarGazer = flags[5];
            SharaIshvalda = flags[6];
            GodLoveYou = flags[7];
            BitsByte flags2 = reader.ReadByte();
            downedUang = flags2[0];
            downedRockSnake = flags2[1];
            downedTideSpirit = flags2[2];
            downedDarkStarLord = flags2[3];
        }
        public override void LoadWorldData(TagCompound tag)
        {
            downedHolyLightSword = tag.ContainsKey("downedHolyLightSword");
            downedBloodCrystalEye = tag.ContainsKey("downedBloodCrystalEye");
            downedMechanicalBoss = tag.ContainsKey("downedMechanicalBoss");
            downedGelSpider = tag.ContainsKey("downedGelSpider");
            FirstEntry = tag.ContainsKey("FirstEntry");
            Plant = tag.ContainsKey("Plant");
            downedStarGazer = tag.ContainsKey("downedStarGazer");
            SharaIshvalda = tag.ContainsKey("SharaIshvalda");
            GodLoveYou = tag.ContainsKey("GodLoveYou");
            downedUang = tag.ContainsKey("downedUang");
            downedRockSnake = tag.ContainsKey("downedRockSnake");
            downedTideSpirit = tag.ContainsKey("downedTidespirit");
            downedDarkStarLord = tag.ContainsKey("downedDarkStarLord");
        }
        public override void Load()
        {
            if (!Main.dedServ)
            {
                TitleUILayer = new UserInterface();
                TitleCardUIElement = new TitleCard();
                TitleUILayer.SetState(TitleCardUIElement);
            }
        }
        public override void ModifyInterfaceLayers(List<GameInterfaceLayer> layers)
        {
            layers.Insert(layers.FindIndex(layer => layer.Name.Equals("Vanilla: Mouse Text")), new LegacyGameInterfaceLayer("GUI Menus",
                delegate
                {
                    return true;
                }, InterfaceScaleType.UI));
            int MouseTextIndex = layers.FindIndex(layer => layer.Name.Equals("Vanilla: Mouse Text"));
            if (MouseTextIndex != -1)
            {
                AddInterfaceLayer(layers, TitleUILayer, TitleCardUIElement, MouseTextIndex + 3, TitleCard.Showing, "Title Card");
            }
        }
        public static void AddInterfaceLayer(List<GameInterfaceLayer> layers, UserInterface userInterface, UIState state, int index, bool visible, string customName = null) //Code created by Scalie
        {
            string name;
            if (customName == null)
            {
                name = state.ToString();
            }
            else
            {
                name = customName;
            }
            layers.Insert(index, new LegacyGameInterfaceLayer("BloodSoul: " + name,
                delegate
                {
                    if (visible)
                    {
                        userInterface.Update(Main._drawInterfaceGameTime);
                        state.Draw(Main.spriteBatch);
                    }
                    return true;
                }, InterfaceScaleType.UI));
        }
    }
}
