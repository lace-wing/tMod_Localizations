using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.ModLoader;
using Terraria.Localization;
using Terraria.ID;
using Microsoft.Xna.Framework;
using System.IO;
using Terraria.ModLoader.Utilities;
using Terraria.DataStructures;
using BloodSoul.NPCs;
using Terraria.GameContent.ItemDropRules;
using BloodErosion.Items.MasterTrophy.GoldAndSilverDoubleSwordsRelics;
using BloodErosion.Items.Boss.GoldAndSilverDoubleSwords;

namespace BloodErosion.NPCs.Bosses.GoldAndSilverDoubleSwords
{
    [AutoloadBossHead]
    class GoldenSpiritSword : GoldAndSilverDoubleSword
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Golden Spirit Sword");
            DisplayName.AddTranslation((int)GameCulture.CultureName.Chinese, "金灵剑");
            Main.npcFrameCount[NPC.type] = 8;
        }
        public override void SetDefaults()
        {
            base.SetDefaults();
            NPC.lifeMax = 3100;
            NPC.defense = 15;
            NPC.damage = 85 / 3;
            BossBag = ModContent.ItemType<GoldAndSilverSpiritSwordI>();
            if (!Main.dedServ)
            {
                Music = MusicLoader.GetMusicSlot(Mod, "Sounds/Music/Double");
            }
            //offsetBasePoint = new Vector2(-240f, 0f);
        }
        public override void ModifyNPCLoot(NPCLoot npcLoot)
        {
            npcLoot.Add(ItemDropRule.BossBag(BossBag));
        }
        public override bool CheckDead()
        {
            foreach (NPC n in Main.npc)
            {
                if (n.type == ModContent.NPCType<SilverSpiritSword>() && n.active)
                {
                    return false;
                }
            }
            if (Main.masterMode == true)
            {
                Item.NewItem(NPC.GetSpawnSource_NPCHurt(), new Rectangle((int)NPC.Center.X, (int)NPC.Center.Y, NPC.width, NPC.height), ModContent.ItemType<GoldAndSilverDoubleSwordsRelicItem>(), Main.rand.Next(1, 1));
            }
            return base.CheckDead();
        }
    }
}
