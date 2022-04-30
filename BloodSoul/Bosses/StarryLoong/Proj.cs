using Terraria;
using Terraria.ModLoader;
using Terraria.ID;
using Microsoft.Xna.Framework;
using Terraria.Graphics;
using Microsoft.Xna.Framework.Graphics;
using Terraria.GameContent;
using System;

namespace BloodSoul.NPCs.Bosses.StarryLoong
{
    public class Proj : ModProjectile
    {
        public override void SetStaticDefaults()
        {
            ProjectileID.Sets.TrailingMode[Projectile.type] = 2;
            ProjectileID.Sets.TrailCacheLength[Projectile.type] = 12;
        }
        public override void SetDefaults()
        {
            Projectile.width = 1;
            Projectile.height = 1;
            Projectile.friendly = false;
            Projectile.hostile = true;
            Projectile.tileCollide = false;
            Projectile.penetrate = -1;
            Projectile.scale = 1f;
            Projectile.light = 0.01f;
            Projectile.timeLeft = 300;
        }
        public override void AI()
        {
            Projectile.rotation+=0.3f;
            Player player = Main.player[Projectile.owner];

            Projectile.ai[1]++;
            if(Projectile.ai[1] == 1)
            {
                for (int i = 0; i <= 30; i++)
                {
                    int num1 = Dust.NewDust(Projectile.Center, 0, 0, 272, 0, 0);
                    Dust dust = Main.dust[num1];
                    dust.scale = 1.5f;
                    dust.noGravity = true;
                }
            }

            Vector2 vector = Projectile.Center - player.Center;
            vector.Normalize();
            vector *= 0.165f;
            player.velocity += vector;
        }
        public override bool PreDraw(ref Color lightColor)
        {
            Texture2D texture = TextureAssets.Projectile[Projectile.type].Value;
            Vector2 position = Projectile.Center - Main.screenPosition;
            Main.spriteBatch.Draw(texture, position, null, Color.White, Projectile.rotation, texture.Size() * 0.5f, 1f, SpriteEffects.None, 0);
            return false;
        }
        public override void Kill(int timeLeft)
        {
            for(int i = 0; i <= 30; i ++)
            {
                int num1 = Dust.NewDust(Projectile.Center, 0, 0, 272, 0,0);
                Dust dust = Main.dust[num1];
                dust.scale = 1.5f;
                dust.noGravity = true;
            }
        }
    }
}
