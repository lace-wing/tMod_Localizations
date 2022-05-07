using BloodErosion.NPCs.Bosses.GoldAndSilverDoubleSwords;
using BloodSoul.NPCs.Bosses.HolyLightSwords;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;
using BloodSoul.Items;

namespace BloodErosion.Items.Boss.GoldAndSilverDoubleSwords
{
    class SpiritMeltingSword : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Spirit Melting Sword");
            DisplayName.AddTranslation((int)GameCulture.CultureName.Chinese, "灵融之剑");
            Tooltip.SetDefault("Use it and see what you've done!");
            Tooltip.AddTranslation((int)GameCulture.CultureName.Chinese, "使用它,然后瞧瞧你干了什么好事！");
            ItemID.Sets.SortingPriorityBossSpawns[Item.type] = 13;
        }
        public override void SetDefaults()
        {
            Item.autoReuse = false;
            Item.width = 20;
            Item.height = 20;
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
                if (npc.type == ModContent.NPCType<SilverSpiritSword>() && npc.type == ModContent.NPCType<GoldenSpiritSword>() && npc.active)
                {
                    return false;
                }
            }
            return true;
        }
        public override bool? UseItem(Player player)
        {
            NPC.NewNPC(player.GetNPCSource_TileInteraction((int)player.position.X / 16, (int)(player.position.Y - 200) / 16), (int)player.position.X - 150, (int)player.position.Y - 300, ModContent.NPCType<SilverSpiritSword>());
            NPC.NewNPC(player.GetNPCSource_TileInteraction((int)player.position.X / 16, (int)(player.position.Y - 200) / 16), (int)player.position.X + 150, (int)player.position.Y - 300, ModContent.NPCType<GoldenSpiritSword>());
            SoundEngine.PlaySound(SoundID.Roar, player.position, 0);
            return true;
        }
        public override void AddRecipes()
        {
            CreateRecipe()
       .AddIngredient(ItemID.GoldBroadsword, 1)
       .AddIngredient(ItemID.SilverBroadsword, 1)
       .AddIngredient(ModContent.ItemType<MagicFragment>(), 2)
       .AddTile(TileID.Anvils)
       .Register();
        }
    }
}
