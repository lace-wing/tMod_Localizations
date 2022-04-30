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

namespace BloodSoul.NPCs.Bosses.DarkStarLords
{
    public class DarkStarProj3 : BaseProj
    {
        private int interval = 0;
        private int i = 0;
        private float State
        {
            get => Projectile.ai[0];
            set => Projectile.ai[0] = value;
        }
        private float Timer
        {
            get => Projectile.ai[1];
            set => Projectile.ai[1] = value;
        }
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("暗星能量");
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
            Projectile.timeLeft = 600;
            Projectile.extraUpdates = 12;
            Projectile.tileCollide = false;
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
            Projectile.rotation = Projectile.velocity.ToRotation();
            Timer++;
            if (Timer > 45)
            {
                Projectile.velocity = Projectile.velocity.RotatedBy(0.04f * State);
            }
        }
        public override bool? Colliding(Rectangle projHitbox, Rectangle targetHitbox)
        {
            float s = 0;
            return Collision.CheckAABBvLineCollision(targetHitbox.TopLeft(),
                targetHitbox.Size(),
                Projectile.Center + Projectile.rotation.ToRotationVector2() * 10,
                Projectile.Center + Projectile.rotation.ToRotationVector2() * -10,
                10, ref s);
        }
        public override Color? GetAlpha(Color lightColor)
        {
            return new Color(255, 215, 0);
        }
        public override bool PreDraw(ref Color lightColor)
        {
            Texture2D texture = TextureAssets.Projectile[Projectile.type].Value;
            Vector2 drawOrigin;
            drawOrigin = new Vector2(texture.Width * 0.5f, texture.Height * 0.5f);
            Main.spriteBatch.Draw(texture, Projectile.Center - Main.screenPosition, null, new Color(255, 215, 0, 0) * Projectile.Opacity, Projectile.rotation, drawOrigin, Projectile.scale, (Projectile.spriteDirection == 1) ? SpriteEffects.None : SpriteEffects.FlipHorizontally, 0);

            BloodSoulWay.ProjectileDrawTail(Projectile, TextureAssets.Projectile[Projectile.type].Value, new Color(255, 215, 0, 0));

            return false;
        }
    }
}
