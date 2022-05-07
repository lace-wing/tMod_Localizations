using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;
using BloodErosion.NPCs.Bosses.FlameGhostKing;
using BloodErosion.NPCs.Bosses.SnowDemonEmperor;
using BloodSoul.Items;

namespace BloodErosion.Items.Boss.SnowDemonEmperors
{
    class IceCrown : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Ice Crown");
            DisplayName.AddTranslation((int)GameCulture.CultureName.Chinese, "冰王冠");
            Tooltip.SetDefault("Summon an ice demon emperor who hates humans\n" + "“It's made with her soul...”");
            Tooltip.AddTranslation((int)GameCulture.CultureName.Chinese, "召唤一个痛恨人类的冰雪妖皇\n" + "“这是用她的灵魂做的...”");
            ItemID.Sets.SortingPriorityBossSpawns[Item.type] = 13;
        }
        public override void SetDefaults()
        {
            Item.autoReuse = false;
            Item.width = 18;
            Item.height = 12;
            Item.maxStack = 1;
            Item.value = 1000;
            Item.rare = ItemRarityID.Blue;
            Item.useAnimation = 30;
            Item.useTime = 30;
            Item.useStyle = 4;
            Item.consumable = false;
        }
        public override bool CanUseItem(Player player)
        {
            foreach (NPC npc in Main.npc)
            {
                if (player.ZoneSnow && npc.type == ModContent.NPCType<SnowDemonEmperor>() && npc.active)
                {
                    return false;
                }
            }
            return (player.ZoneSnow);
        }
        public override bool? UseItem(Player player)
        {
            NPC.NewNPC(player.GetNPCSource_TileInteraction((int)player.position.X / 16, (int)(player.position.Y - 200) / 16),(int)player.position.X, (int)player.position.Y - 300, ModContent.NPCType<SnowDemonEmperor>());
            SoundEngine.PlaySound(SoundID.Roar, player.position, 2);
            return true;
        }
        public override void AddRecipes()
        {
            CreateRecipe()
       .AddIngredient(ItemID.SoulofNight, 5)
       .AddIngredient(ItemID.SoulofLight, 5)
       .AddIngredient(ItemID.IceBlock, 10)
       .AddIngredient(ModContent.ItemType<LostSoul>(), 5)
       .AddTile(134)
       .Register();
        }
    }
}
