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

namespace BloodErosion.Items.Boss.ApostleOfDeaths
{
    class InvertedCrossNecklace : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Inverted Cross Necklace");
            DisplayName.AddTranslation((int)GameCulture.CultureName.Chinese, "黑暗倒十字项链");
            Tooltip.SetDefault("Welcome death! (highly difficult)");
            Tooltip.AddTranslation((int)GameCulture.CultureName.Chinese, "迎接死亡的到来吧！(高难)");
            ItemID.Sets.SortingPriorityBossSpawns[Item.type] = 13;
        }
        public override void SetDefaults()
        {
            Item.autoReuse = false;
            Item.width = 26;
            Item.height = 32;
            Item.maxStack = 1;
            Item.value = 1000;
            Item.rare = ItemRarityID.Purple;
            Item.useAnimation = 30;
            Item.useTime = 30;
            Item.useStyle = 4;
            Item.consumable = false;
        }
        public override bool CanUseItem(Player player)
        {
            foreach (NPC npc in Main.npc)
            {
                if (npc.type == ModContent.NPCType<ApostleOfDeath>() && npc.active)
                {
                    return false;
                }
            }
            return true;
        }
        public override bool? UseItem(Player player)
        {
            NPC.NewNPC(player.GetNPCSource_TileInteraction((int)player.position.X / 16, (int)(player.position.Y - 200) / 16),(int)player.position.X, (int)player.position.Y - 350, ModContent.NPCType<ApostleOfDeath>());
            SoundEngine.PlaySound(SoundID.Roar, player.position, 0);
            return true;
        }
        public override void AddRecipes()
        {
            CreateRecipe()
       .AddIngredient(ItemID.SoulofNight, 5)
       .AddIngredient(1225, 5)
       .AddIngredient(3467, 5)
       .AddIngredient(ModContent.ItemType<MagicFragment>(), 1)
       .AddTile(412)
       .Register();
        }
    }
}
