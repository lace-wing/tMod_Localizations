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
using BloodSoul;

namespace BloodErosion.NPCs.Bosses.ApostleOfDeath
{
    public class DeathEnergyBomb : BaseProj
    {
        private int interval = 0;
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("DeathEnergyBomb");
        }
        public override void SetDef()
        {
            Projectile.alpha = 255;
            Projectile.scale = 1f;
            Projectile.width = 5;
            Projectile.height = 5;
            Projectile.DamageType = DamageClass.Ranged;
            Projectile.friendly = false;
            Projectile.hostile = true;
            Projectile.ignoreWater = true;
            Projectile.penetrate = 5;
            Projectile.timeLeft = 600;
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
        }
        public override bool OnTileCollide(Vector2 oldVelocity)
        {
            if (Projectile.velocity.X != oldVelocity.X)
            {
                Projectile.velocity.X = -oldVelocity.X * 0.9f;
            }
            if (Projectile.velocity.Y != oldVelocity.Y)
            {
                Projectile.velocity.Y = -oldVelocity.Y * 0.9f;
            }
            return false;
        }
        public override Color? GetAlpha(Color lightColor)
        {
            return new Color(40, 0, 77);
        }
        public override bool PreDraw(ref Color lightColor)
        {
            Texture2D texture = TextureAssets.Projectile[Projectile.type].Value;
            Vector2 drawOrigin;
            drawOrigin = new Vector2(texture.Width * 0.5f, texture.Height * 0.5f);
            Main.spriteBatch.Draw(texture, Projectile.Center - Main.screenPosition, null, new Color(40, 0, 77, 0) * Projectile.Opacity, Projectile.rotation, drawOrigin, Projectile.scale, (Projectile.spriteDirection == 1) ? SpriteEffects.None : SpriteEffects.FlipHorizontally, 0);

            BloodErosionWay.ProjectileDrawTail(Projectile, TextureAssets.Projectile[Projectile.type].Value, new Color(40, 0, 77, 0));

            return false;
        }
    }
}
