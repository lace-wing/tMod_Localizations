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
    public class StarryLoongProj : ModProjectile
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
        }
        public override void AI()
        {
            Projectile.rotation+=0.1f;
            Player player = Main.player[Projectile.owner];
            NPC npc = Main.npc[NPC.FindFirstNPC(ModContent.NPCType<StarryLoongHead>())];

            Projectile.ai[0]++;
            if(Projectile.ai[0] == 120)
            {
                Vector2 vector = player.Center - Projectile.Center;
                vector.Normalize();
                vector *= 5;
                Projectile.NewProjectile(Projectile.InheritSource(Projectile), Projectile.Center, vector, ModContent.ProjectileType<StarLightT>(), 30, 0, player.whoAmI);
                Projectile.ai[0] = 0;
            }
            if(npc.active)
            {
                Projectile.timeLeft = 2;
            }
            else
            {
                Projectile.Kill();
            }

            float t = Main.GlobalTimeWrappedHourly * 2;
            var targetPos = player.Center + t.ToRotationVector2() * 250f;
            Projectile.velocity = (targetPos - Projectile.Center) * 0.3f;
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
