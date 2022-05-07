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
using BloodErosion.NPCs.Bosses.DivineGlow;
using Microsoft.Xna.Framework.Graphics;
using BloodSoul.MyUtils;

namespace BloodErosion.Items.Boss.DivineGlows
{
    class HolyLightFragment : ModItem
    {
        float r = 0;
        public override void Update(ref float gravity, ref float maxFallSpeed)
        {
            r += 0.01f;
        }
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Holy Light Fragment");
            DisplayName.AddTranslation((int)GameCulture.CultureName.Chinese, "圣光碎片");
            Tooltip.SetDefault("True divine power!");
            Tooltip.AddTranslation((int)GameCulture.CultureName.Chinese, "真正的神圣力量！");
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
        public override int BossBagNPC => ModContent.NPCType<DivineGlow>();
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
        public override bool PreDrawInWorld(SpriteBatch spriteBatch, Color lightColor, Color alphaColor, ref float rotation, ref float scale, int whoAmI)
        {

            Texture2D texture2 = BloodSoulUtils.GetTexture("Images/Ray").Value;
            Vector2 drawOrigin2;
            drawOrigin2 = new Vector2(texture2.Width * 0.5f, texture2.Height * 0.5f);
            Main.spriteBatch.Draw(texture2, Item.Center - Main.screenPosition, null, new Color(255, 255, 170, 0), r, drawOrigin2, new Vector2(1.7f, 1.7f), SpriteEffects.None, 0);

            Texture2D texture3 = BloodSoulUtils.GetTexture("Images/Start").Value;
            Vector2 drawOrigin3;
            drawOrigin3 = new Vector2(texture3.Width * 0.5f, texture3.Height * 0.5f);
            Main.spriteBatch.Draw(texture3, Item.Center - Main.screenPosition, null, new Color(255, 255, 170, 0), -r, drawOrigin3, new Vector2(0.6f, 0.6f), SpriteEffects.None, 0);

            return true;
        }
    }
}
