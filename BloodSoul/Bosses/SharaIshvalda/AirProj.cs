using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.ModLoader;
using Terraria.ID;
using Terraria.GameContent;
using Terraria.Audio;
using BloodSoul.Projectiles.Arrow;
using BloodSoul.Projectiles;

namespace BloodSoul.NPCs.Bosses.SharaIshvalda
{
    public class AirProj : BaseProj
    {
        private int interval = 0;
        private int i = 0;
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("空气弹");
        }
        public override void SetDef()
        {
            Projectile.alpha = 255;
            Projectile.scale = 1f;
            Projectile.width = 5;
            Projectile.height = 5;
            Projectile.friendly = false;
            Projectile.hostile = true;
            Projectile.ignoreWater = true;
            Projectile.penetrate = -1;
            Projectile.timeLeft = 550;
            Projectile.tileCollide = false;
            Projectile.extraUpdates = 12;
            ProjectileID.Sets.TrailCacheLength[Projectile.type] = 70;
            ProjectileID.Sets.TrailingMode[Projectile.type] = 0;
        }
        public override void AI()
        {
            if (Projectile.timeLeft > 20)
            {
                Projectile.Opacity = 1;
            }
            else
            {
                Projectile.Opacity = Projectile.timeLeft / 15f;
            }
            float v = Projectile.velocity.ToRotation();
            Projectile.rotation = v;
            if (Projectile.timeLeft % 30 == 0)
            {
                Projectile.velocity = Projectile.velocity.RotatedByRandom(2.14f) / 1.4f;
            }
        }
        public override Color? GetAlpha(Color lightColor)
        {
            return new Color(250, 250, 210);
        }
        public override bool PreDraw(ref Color lightColor)
        {
            Texture2D texture = TextureAssets.Projectile[Projectile.type].Value;
            Vector2 drawOrigin;
            drawOrigin = new Vector2(texture.Width * 0.5f, texture.Height * 0.5f);
            Main.spriteBatch.Draw(texture, Projectile.Center - Main.screenPosition, null, new Color(250, 250, 210, 0) * Projectile.Opacity, Projectile.rotation, drawOrigin, Projectile.scale, (Projectile.spriteDirection == 1) ? SpriteEffects.None : SpriteEffects.FlipHorizontally, 0);

            BloodSoulWay.ProjectileDrawTail(Projectile, TextureAssets.Projectile[Projectile.type].Value, new Color(250, 250, 210, 0));

            return false;
        }
    }
}
