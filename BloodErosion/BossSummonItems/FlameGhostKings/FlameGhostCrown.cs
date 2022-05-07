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

namespace BloodErosion.Items.Boss.FlameGhostKings
{
    class FlameGhostCrown : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Flame Ghost Crown");
            DisplayName.AddTranslation((int)GameCulture.CultureName.Chinese, "炎鬼王冠");
            Tooltip.SetDefault("Attract the most powerful presence in the flame ghost");
            Tooltip.AddTranslation((int)GameCulture.CultureName.Chinese, "吸引炎鬼中最为强大的存在");
            ItemID.Sets.SortingPriorityBossSpawns[Item.type] = 13;
        }
        public override void SetDefaults()
        {
            Item.autoReuse = false;
            Item.width = 30;
            Item.height = 22;
            Item.maxStack = 1;
            Item.value = 1000;
            Item.rare = ItemRarityID.Red;
            Item.useAnimation = 30;
            Item.useTime = 30;
            Item.useStyle = 4;
            Item.consumable = false;
        }
        public override bool CanUseItem(Player player)
        {
            foreach (NPC npc in Main.npc)
            {
                if (player.ZoneUnderworldHeight && npc.type == ModContent.NPCType<FlameGhostKing>() && npc.active)
                {
                    return false;
                }
            }
            return (player.ZoneUnderworldHeight);
        }
        public override bool? UseItem(Player player)
        {
            NPC.NewNPC(player.GetNPCSource_TileInteraction((int)player.position.X / 16, (int)(player.position.Y - 300) / 16),(int)player.position.X, (int)player.position.Y - 300, ModContent.NPCType<FlameGhostKing>());
            SoundEngine.PlaySound(SoundID.Roar, player.position, 0);
            return true;
        }
        public override void AddRecipes()
        {
            CreateRecipe()
       .AddIngredient(174, 20)
       .AddIngredient(ItemID.SoulofNight, 3)
       .AddIngredient(ModContent.ItemType<HolyFragment>(), 1)
       .AddTile(TileID.Anvils)
       .Register();
        }
    }
}
