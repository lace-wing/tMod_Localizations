using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.ModLoader;
using Terraria.ID;
using Terraria.GameContent;
using Terraria.Audio;
using BloodSoul.Projectiles.Arrow;
using Terraria.Localization;
using BloodSoul.Projectiles;

namespace BloodErosion.NPCs.Bosses.FlameGhostKing
{
    class BurningBoom2 : BaseProj
    {
        private int Time
        {
            get => (int)Projectile.ai[1];
            set => Projectile.ai[1] = value;
        }
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("AppleBoom");
            DisplayName.AddTranslation((int)GameCulture.CultureName.Chinese, "王炎爆炸");
            ProjectileID.Sets.TrailCacheLength[Projectile.type] = 12;
            ProjectileID.Sets.TrailingMode[Projectile.type] = 0;
            Main.projFrames[Projectile.type] = 7;
        }
        public override void SetDef()
        {
            Projectile.width = 98;
            Projectile.height = 98;
            Projectile.friendly = false;
            Projectile.hostile = true;
            Projectile.timeLeft = 350;
            Projectile.penetrate = -1;
        }
        public override void AI()
        {
            Lighting.AddLight(Projectile.Center, new Vector3(174, 0, 0) * 0.015f);
            Projectile.scale = 2f;
            float v = Projectile.velocity.ToRotation();
            Projectile.rotation = v + MathHelper.Pi / 2;
            Projectile.frameCounter++;
            if (Projectile.frameCounter > 3)
            {
                Projectile.frame += 1;
                Projectile.frameCounter = 0;
            }
            if (Projectile.frame >= Main.projFrames[Projectile.type])
            {
                Projectile.Kill();
            }
            Projectile.velocity *= 0;
        }
        public override void OnHitNPC(NPC target, int damage, float knockback, bool crit)
        {
            target.AddBuff(BuffID.OnFire, 120);
        }
        public override void OnHitPlayer(Player target, int damage, bool crit)
        {
            target.AddBuff(BuffID.OnFire, 300);
        }
    }
}
