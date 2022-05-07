using BloodErosion.NPCs.Bosses.GoldAndSilverDoubleSwords;
using BloodSoul.NPCs.Bosses.HolyLightSwords;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;
using BloodSoul.Items;
using BloodErosion.NPCs.Bosses.SpiritOfSpark;
using Terraria.DataStructures;

namespace BloodErosion.Items.Boss.SpiritOfSparks
{
    class ManicSpark : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Manic Spark");
            DisplayName.AddTranslation((int)GameCulture.CultureName.Chinese, "狂躁火花");
            Tooltip.SetDefault("It becomes very manic because it absorbs a lot of energy");
            Tooltip.AddTranslation((int)GameCulture.CultureName.Chinese, "它因吸收了大量能量而变得异常狂躁");
            ItemID.Sets.SortingPriorityBossSpawns[Item.type] = 13;
            Main.RegisterItemAnimation(Item.type, new DrawAnimationVertical(5, 6));
        }
        public override void SetDefaults()
        {
            Item.autoReuse = false;
            Item.width = 16;
            Item.height = 30;
            Item.maxStack = 1;
            Item.value = 1000;
            Item.rare = ItemRarityID.Red;
            Item.useAnimation = 30;
            Item.useTime = 30;
            Item.useStyle = 4;
            Item.consumable = false;
            Item.noUseGraphic = true;
        }
        public override bool CanUseItem(Player player)
        {
            foreach (NPC npc in Main.npc)
            {
                if (npc.type == ModContent.NPCType<SpiritOfSpark>() && npc.active)
                {
                    return false;
                }
            }
            return true;
        }
        public override bool? UseItem(Player player)
        {
            NPC.NewNPC(player.GetNPCSource_TileInteraction((int)player.position.X / 16, (int)(player.position.Y - 300) / 16),(int)player.position.X, (int)player.position.Y - 300, ModContent.NPCType<SpiritOfSpark>());
            SoundEngine.PlaySound(SoundID.Roar, player.position, 0);
            return true;
        }
        public override void AddRecipes()
        {
            CreateRecipe()
       .AddIngredient(ModContent.ItemType<MagicFragment>(), 2)
       .AddTile(TileID.Campfire)
       .Register();
        }
    }
}
