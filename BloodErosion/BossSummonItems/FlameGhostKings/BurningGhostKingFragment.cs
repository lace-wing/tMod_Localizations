using BloodSoul.MyUtils;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.Localization;
using Terraria.ModLoader;

namespace BloodErosion.Items.Boss.FlameGhostKings
{

    public class BurningGhostKingFragment : ModItem
    {
        float r = 0;
        public override void Update(ref float gravity, ref float maxFallSpeed)
        {
            r += 0.01f;
        }
        public override void SetStaticDefaults()
        {

            DisplayName.SetDefault("Burning Ghost King Fragment");
            DisplayName.AddTranslation((int)GameCulture.CultureName.Chinese, "王炎碎片");
        }

        public override void SetDefaults()
        {
            Item.width = 24;
            Item.height = 24;
            Item.maxStack = 999;
            Item.value = 7500;
            Item.rare = -12;
        }
        public override bool PreDrawInWorld(SpriteBatch spriteBatch, Color lightColor, Color alphaColor, ref float rotation, ref float scale, int whoAmI)
        {
            Texture2D texture2 = BloodSoulUtils.GetTexture("Images/Extra_98").Value;
            Vector2 drawOrigin2;
            drawOrigin2 = new Vector2(texture2.Width * 0.5f, texture2.Height * 0.5f);
            Main.spriteBatch.Draw(texture2, Item.Center - Main.screenPosition, null, new Color(255, 45, 45, 0), -r, drawOrigin2, new Vector2(0.8f, 0.8f), SpriteEffects.None, 0);

            Texture2D texture3 = BloodSoulUtils.GetTexture("Images/TailStar").Value;
            Vector2 drawOrigin3;
            drawOrigin3 = new Vector2(texture3.Width * 0.5f, texture3.Height * 0.5f);
            Main.spriteBatch.Draw(texture3, Item.Center - Main.screenPosition, null, new Color(255, 45, 45, 0), -r, drawOrigin3, new Vector2(0.8f, 0.8f), SpriteEffects.None, 0);

            return true;
        }
    }
}
