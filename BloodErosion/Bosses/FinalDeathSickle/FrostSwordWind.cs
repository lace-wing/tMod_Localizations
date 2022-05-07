using Terraria.ModLoader;
using Terraria;
using Terraria.Localization;
using Microsoft.Xna.Framework;
using Terraria.ID;
using BloodSoul.Projectiles;
using BloodSoul;

namespace BloodErosion.NPCs.Bosses.FinalDeathSickle
{
    class FrostSwordWind : BaseProj
    {
        private Vector2 OldVec = Vector2.Zero;
        private int TimeV = 0;
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("FrostSwordWind");
            DisplayName.AddTranslation((int)GameCulture.CultureName.Chinese, "最终寒霜剑风");
            ProjectileID.Sets.TrailingMode[Projectile.type] = 2;
            ProjectileID.Sets.TrailCacheLength[Projectile.type] = 18;
        }
        public override void SetDef()
        {
            Projectile.width = 58;
            Projectile.height = 42;
            Projectile.friendly = false;
            Projectile.hostile = true;
            Projectile.timeLeft = 600;
            Projectile.tileCollide = false;
            Projectile.penetrate = -1;
            Projectile.light = 0.3f;
        }
        public override void AI()
        {
            TimeV++;
            Projectile.rotation = TimeV * 7;
            switch(State)
            {
                case 0://记录速度
                    {
                        Projectile.rotation = TimeV * 7;
                        OldVec = Projectile.velocity;
                        State = 1;
                        break;
                    }
                case 1://速度变0
                    {
                        Projectile.rotation = TimeV * 7;
                        Projectile.velocity *= 1f;
                        break;
                    }
                case 2://速度恢复
                    {
                        Projectile.rotation = TimeV * 7;
                        Projectile.velocity = OldVec;
                        break;
                    }
            }
        }
        public override bool? Colliding(Rectangle projHitbox, Rectangle TargetHitbox)
        {
            float r = 0;
            if (Collision.CheckAABBvLineCollision(TargetHitbox.TopLeft(), TargetHitbox.Size(),
                Projectile.Center + (Projectile.rotation - MathHelper.Pi / 4).ToRotationVector2() * 16,
                Projectile.Center + (Projectile.rotation - MathHelper.Pi / 4).ToRotationVector2() * -16, 10, ref r))
            {
                return true;
            }
            return false;
        }
        public override void ModifyHitPlayer(Player Target, ref int damage, ref bool crit)
        {
            Target.AddBuff(BuffID.Frostburn, 90);
            damage = ModifyHitDamage(damage);
        }
        public override bool CanHitPlayer(Player Target)
        {
            return Projectile.Distance(Target.Center) < (Projectile.width / 2 + Projectile.height / 2);
        }
        public override bool PreDraw(ref Color lightColor)
        {
            BloodErosionWay.ProjectileDrawTail3(Projectile, Color.LightCyan);
            return false;
        }
    }
}
