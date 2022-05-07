using BloodSoul.Projectiles;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.Localization;

namespace BloodErosion.NPCs.Bosses.ApostleOfDeath
{
    class Chop : BaseProj
    {
        public int r;
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Chop");
            DisplayName.AddTranslation((int)GameCulture.CultureName.Chinese, "斩1");
            ProjectileID.Sets.TrailingMode[Projectile.type] = 2;
            ProjectileID.Sets.TrailCacheLength[Projectile.type] = 5;
            Main.projFrames[Projectile.type] = 7;
        }
        public int i;
        public override void SetDef()
        {
            Projectile.width = 120;
            Projectile.height = 116;
            Projectile.friendly = false;
            Projectile.hostile = true;
            Projectile.timeLeft = 900;
            Projectile.tileCollide = false;
            Projectile.penetrate = -1;
            Projectile.light = 0.2f;
            ProjectileID.Sets.TrailCacheLength[Projectile.type] = 12;
            ProjectileID.Sets.TrailingMode[Projectile.type] = 0;
        }
        public override void AI()
        {
            float v = Projectile.velocity.ToRotation();
            Projectile.rotation = v;
            Projectile.velocity *= 0f;
            Projectile.scale = 1.6f;
            NPC npc = Main.npc[Projectile.owner];
            Projectile.Center = npc.Center;
            i++;
            if (i % 10 == 0)
            {
                Projectile.velocity *= 0f;
                Projectile.netUpdate = true;
            }
            Projectile.frameCounter++;
            if (Projectile.frameCounter >= 7)
            {
                Projectile.frame++;
                Projectile.frameCounter = 0;
                if (Projectile.frame == 1)
                {
                    SoundEngine.PlaySound(SoundID.Item71, Projectile.position);
                }
                if (Projectile.frame >= 7)
                {
                    Projectile.Kill();
                }
            }
        }
    }
}
