using BloodErosion.NPCs.Bosses.GoldAndSilverDoubleSwords;
using BloodSoul.NPCs.Bosses.HolyLightSwords;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;
using BloodSoul.Items;
using BloodErosion.NPCs.Bosses.FlameGhostKing;
using BloodErosion.NPCs.Bosses.DivineGlow;
using BloodErosion.NPCs.Bosses.ApostleOfDeath;
using BloodErosion.NPCs.Bosses.SpearOfCanglanGod;

namespace BloodErosion.Items.Boss.SpearOfCanglanGods
{
    class ArcContract : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Arc Contract");
            DisplayName.AddTranslation((int)GameCulture.CultureName.Chinese, "电弧契约");
            Tooltip.SetDefault("Summon the companion spirit of canglan God");
            Tooltip.AddTranslation((int)GameCulture.CultureName.Chinese, "召唤苍岚神的伴生器灵");
            ItemID.Sets.SortingPriorityBossSpawns[Item.type] = 13;
        }
        public override void SetDefaults()
        {
            Item.autoReuse = false;
            Item.width = 46;
            Item.height = 64;
            Item.maxStack = 1;
            Item.value = 1000;
            Item.rare = -12;
            Item.useAnimation = 30;
            Item.useTime = 30;
            Item.useStyle = 4;
            Item.consumable = false;
        }
        public override bool CanUseItem(Player player)
        {
            foreach (NPC npc in Main.npc)
            {
                if (npc.type == ModContent.NPCType<SpearOfCanglanGod>() && npc.active)
                {
                    return false;
                }
            }
            return true;
        }
        public override bool? UseItem(Player player)
        {
            NPC.NewNPC(player.GetNPCSource_TileInteraction((int)player.position.X / 16, (int)(player.position.Y - 350) / 16),(int)player.position.X, (int)player.position.Y - 350, ModContent.NPCType<SpearOfCanglanGod>());
            SoundEngine.PlaySound(SoundID.Thunder, player.position, 0);
            return true;
        }
        public override void AddRecipes()
        {
            CreateRecipe()
       .AddIngredient(ModContent.ItemType<ElectricArcCrystal>(), 5)
       .AddIngredient(ModContent.ItemType<ContractAncientInk>(), 1)
       .AddTile(412)
       .Register();
        }
    }
}
