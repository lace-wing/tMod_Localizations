using BloodSoul.Projectiles;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.GameContent;
using Terraria.ID;

namespace BloodErosion.NPCs.Bosses.FinalDeathSickle.FinalDeathSickle2s
{
    public class DeathStar : BaseProj
    {
        private int interval = 0;
        private int i = 0;
        private int Z = 0;
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
            Projectile.tileCollide = false;
            ProjectileID.Sets.TrailCacheLength[Projectile.type] = 70;
            ProjectileID.Sets.TrailingMode[Projectile.type] = 0;
        }
        public override void AI()
        {
            Player player = Main.player[Projectile.owner];
            Player Target = null;
            Vector2 ToTarget = (player.position - Projectile.position).SafeNormalize(Vector2.UnitX) * 15;
            i++;
            if (Projectile.timeLeft > 20)
            {
                Projectile.Opacity = 1;
            }
            else
            {
                Projectile.Opacity = Projectile.timeLeft / 15f;
            }
            float v = Projectile.velocity.ToRotation();
            if (i < 30)
            {
                Projectile.rotation = v;
                if (Projectile.timeLeft % 30 == 0)
                {
                    Projectile.velocity = Projectile.velocity.RotatedByRandom(2.14f) / 1.4f;
                }
            }
            Z++;
            if(Z > 30 && Z < 60)
            {
                Projectile.velocity = ToTarget;
            }
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
            Main.spriteBatch.Draw(texture, Projectile.Center - Main.screenPosition, null, new Color(148, 0, 211, 0) * Projectile.Opacity, Projectile.rotation, drawOrigin, Projectile.scale, (Projectile.spriteDirection == 1) ? SpriteEffects.None : SpriteEffects.FlipHorizontally, 0);

            BloodErosionWay.ProjectileDrawTail(Projectile, TextureAssets.Projectile[Projectile.type].Value, new Color(148, 0, 211, 0));

            return false;
        }
    }
}
