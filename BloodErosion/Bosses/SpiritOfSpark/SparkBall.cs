using Microsoft.Xna.Framework;
using System;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.Audio;
using Terraria.GameContent;
using BloodSoul.Projectiles;
using BloodSoul.MyUtils;
using BloodSoul;

namespace BloodErosion.NPCs.Bosses.SpiritOfSpark
{
    public class SparkBall : BaseProj
    {
        private Vector2 OldVec = Vector2.Zero;
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("火花球");
            Main.projFrames[Projectile.type] = 4;
        }
        public override void SetDef()
        {
            Projectile.width = 20;
            Projectile.height = 30;
            Projectile.friendly = false;
            Projectile.hostile = true;
            Projectile.penetrate = -1;
            Projectile.aiStyle = -1;
            Projectile.timeLeft = 30;
            Projectile.ignoreWater = true;
            Projectile.tileCollide = false;
            Projectile.scale = 1f;
            Projectile.light = 0.6f;
            ProjectileID.Sets.TrailCacheLength[Projectile.type] = 12;
            ProjectileID.Sets.TrailingMode[Projectile.type] = 0;
        }
        public override void AI()
        {
            Projectile.rotation = Utils.ToRotation(Projectile.velocity) + (float)(Math.PI / 2);
            /*foreach (NPC npc in Main.npc)
            {
                if (npc.type == ModContent.NPCType<SpiritOfSpark>() && !npc.active)
                {
                    Projectile.active = false;
                }
            }*/
            switch (State)
            {
                case 0://记录速度
                    {
                        OldVec = Projectile.velocity;
                        State = 1;
                        break;
                    }
                case 1://速度变0
                    {
                        Projectile.velocity *= 1;
                        break;
                    }
                case 2://速度恢复
                    {
                        Projectile.velocity = OldVec;
                        Projectile.rotation = Projectile.velocity.ToRotation() + MathHelper.Pi / 4;
                        break;
                    }
            }
        }
        public override void OnHitPlayer(Player Target, int damage, bool crit)
        {
            Target.AddBuff(BuffID.OnFire, 300);
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
            damage = ModifyHitDamage(damage);
        }
        public override bool PreDraw(ref Color lightColor)
        {
            BloodErosionWay.ProjectileDrawTail3(Projectile, Color.White);
            return false;
        }
        public override void Kill(int timeLeft)
        {
            var NPC = Main.npc[Projectile.owner];
            Vector2 plrToMouse = Main.MouseWorld - NPC.Center;
            float r = (float)Math.Atan2(plrToMouse.Y, plrToMouse.X);
            for (int i = -4; i <= 4; i++)
            {
                float r2 = r + i * MathHelper.Pi / 8f;
                Vector2 shootVel = r2.ToRotationVector2() * 10;
                Terraria.Projectile.NewProjectile(Projectile.InheritSource(Projectile), Projectile.Center, shootVel, 328, 15, 10, NPC.whoAmI);
            }
            SoundEngine.PlaySound(SoundID.Item20, Projectile.position);
            for (int i = 0; i < 25; i++)
            {
                Dust.NewDust(Projectile.position + Projectile.velocity, Projectile.width, Projectile.height, DustID.FireflyHit, Projectile.oldVelocity.X * 0.2f, Projectile.oldVelocity.Y * 0.2f);
            }
        }
    }
}
