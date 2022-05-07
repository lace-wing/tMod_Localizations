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

namespace BloodErosion.Items.Boss.DivineGlows
{
    class AwakeningSacredIngot : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Awakening·Sacred Ingot");
            DisplayName.AddTranslation((int)GameCulture.CultureName.Chinese, "真·神圣锭");
            Tooltip.SetDefault("Summon true divine power");
            Tooltip.AddTranslation((int)GameCulture.CultureName.Chinese, "召唤真正的神圣力量");
            ItemID.Sets.SortingPriorityBossSpawns[Item.type] = 13;
        }
        public override void SetDefaults()
        {
            Item.autoReuse = false;
            Item.width = 38;
            Item.height = 32;
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
                if (npc.type == ModContent.NPCType<DivineGlow>() && npc.active)
                {
                    return false;
                }
            }
            return true;
        }
        public override bool? UseItem(Player player)
        {
            NPC.NewNPC(player.GetNPCSource_TileInteraction((int)player.position.X / 16, (int)(player.position.Y - 200) / 16),(int)player.position.X, (int)player.position.Y - 300, ModContent.NPCType<DivineGlow>());
            SoundEngine.PlaySound(SoundID.Roar, player.position, 0);
            return true;
        }
        public override void AddRecipes()
        {
            CreateRecipe()
       .AddIngredient(ItemID.SoulofNight, 5)
       .AddIngredient(ItemID.SoulofLight, 5)
       .AddIngredient(1225, 5)
       .AddIngredient(ModContent.ItemType<MagicFragment>(), 1)
       .AddTile(134)
       .Register();
        }
    }
}
