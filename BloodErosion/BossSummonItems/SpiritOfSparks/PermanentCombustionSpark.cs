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
    class PermanentCombustionSpark : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Permanent Combustion Spark");
            DisplayName.AddTranslation((int)GameCulture.CultureName.Chinese, "永燃火花");
            Tooltip.SetDefault("It will burn forever");
            Tooltip.AddTranslation((int)GameCulture.CultureName.Chinese, "它会永远燃烧下去");
            Main.RegisterItemAnimation(Item.type, new DrawAnimationVertical(3, 4));
            ItemID.Sets.AnimatesAsSoul[Item.type] = true;
            ItemID.Sets.ItemIconPulse[Item.type] = true;
            ItemID.Sets.ItemNoGravity[Item.type] = true;
        }
        public override void SetDefaults()
        {
            Item.maxStack = 1;
            Item.autoReuse = false;
            Item.width = 20;
            Item.height = 30;
            Item.maxStack = 1;
            Item.value = 20000;
            Item.rare = -12;
        }
        public override int BossBagNPC => ModContent.NPCType<SpiritOfSpark>();
        public override bool GrabStyle(Player player)
        {
            Vector2 vectorItemToPlayer = player.Center - Item.Center;
            Vector2 movement = -vectorItemToPlayer.SafeNormalize(default(Vector2)) * 0.1f;
            Item.velocity = Item.velocity + movement;
            Item.velocity = Collision.TileCollision(Item.position, Item.velocity, Item.width, Item.height);
            return true;
        }


        public override void GrabRange(Player player, ref int grabRange)
        {
            grabRange *= 3;
        }

        public override void PostUpdate()
        {
            Lighting.AddLight(Item.Center, Color.Firebrick.ToVector3() * 0.7f * Main.essScale);
        }
    }
}
