using Terraria.ModLoader;
using Terraria;
using Terraria.Localization;
using Terraria.IO;
using Terraria.Audio;
using Microsoft.Xna.Framework;
using Terraria.ID;
using Microsoft.Xna.Framework.Graphics;
using System;
using BloodSoul.Buffs;
using BloodSoul.Projectiles;
using BloodSoul;

namespace BloodSoul.NPCs.Bosses.TheStarGazer
{
    class Comet : BaseProj
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Comet");
            DisplayName.AddTranslation((int)GameCulture.CultureName.Chinese, "妖星");
            ProjectileID.Sets.TrailingMode[Projectile.type] = 2;
            ProjectileID.Sets.TrailCacheLength[Projectile.type] = 5;
            Main.projFrames[Projectile.type] = 5;
        }
        public int i;
        public override void SetDef()
        {
            Projectile.width = 34;
            Projectile.height = 16;
            Projectile.friendly = false;
            Projectile.hostile = true;
            Projectile.timeLeft = 900;
            Projectile.tileCollide = false;
            Projectile.penetrate = 1;
            Projectile.light = 0.5f;
            ProjectileID.Sets.TrailCacheLength[Projectile.type] = 12;
            ProjectileID.Sets.TrailingMode[Projectile.type] = 0;
        }
        public override void AI()
        {
            Projectile.rotation = Utils.ToRotation(Projectile.velocity) + (float)(Math.PI / 2);
            i++;
            switch (i)
            {
                case 30:
                    {
                        Projectile.velocity *= 11f;
                        Projectile.netUpdate = true;
                        break;
                    }
                case 60:
                    {
                        Projectile.velocity *= 2f;
                        Projectile.netUpdate = true;
                        break;
                    }
            }
            Projectile.frameCounter++;
            if (Projectile.frameCounter >= 8)
            {
                Projectile.frame++;
                Projectile.frameCounter = 0;
                if (Projectile.frame >= 4)
                {
                    Projectile.frame = 0;
                }
            }
        }
        public override bool PreDraw(ref Color lightColor)
        {
            BloodSoulWay.ProjectileDrawTail3(Projectile, Color.LightCyan);
            return false;
        }
    }
}
