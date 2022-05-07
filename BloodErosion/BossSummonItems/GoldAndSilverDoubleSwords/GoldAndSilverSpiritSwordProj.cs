using Microsoft.Xna.Framework;
using System;
using Terraria;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;
using Terraria.Audio;
using BloodSoul.MyUtils;
using BloodSoul;
using BloodSoul.Projectiles;
using BloodSoul.Items;
using Terraria.DataStructures;

namespace BloodErosion.Items.Boss.GoldAndSilverDoubleSwords
{
    public class GoldAndSilverSpiritSwordProj : BaseProj
    {
        private int interval = 0;
        public override void SetStaticDefaults()
        {
            SetName("Gold And Silver Spirit Sword Proj", "金银剑气");
            Main.projFrames[Projectile.type] = 28;
        }
        public override void SetDef()
        {
            Projectile.width = 64;
            Projectile.height = 60;
            Projectile.friendly = true;
            Projectile.penetrate = -1;
            Projectile.tileCollide = false;
            Projectile.ownerHitCheck = true;
            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = 5;
            Projectile.DamageType = DamageClass.Melee;
            Projectile.alpha = 100;
            Projectile.light = 0.3f;
        }
        public override void AI()
        {
            Player player = Main.player[Projectile.owner];
            float num = 0f;
            Vector2 vector = player.RotatedRelativePoint(player.MountedCenter, true);
            if (Projectile.spriteDirection == -1)
            {
                num = 3.14159274f;
            }
            Projectile.frameCounter++;
            if (Projectile.frameCounter > 2)
            {
                Projectile.frame += 1;
                Projectile.frameCounter = 0;
            }
            if (Projectile.frame >= Main.projFrames[Projectile.type])
            {
                Projectile.frame = 0;
            }
            Projectile.soundDelay--;
            if (Projectile.soundDelay <= 0)
            {
                SoundEngine.PlaySound(SoundID.Item1, base.Projectile.Center);
                Projectile.soundDelay = 24;
            }
            if (Main.myPlayer == Projectile.owner)
            {
                if (player.channel && !player.noItems && !player.CCed)
                {
                    float scaleFactor6 = 1f;
                    if (player.inventory[player.selectedItem].shoot == Projectile.type)
                    {
                        scaleFactor6 = player.inventory[player.selectedItem].shootSpeed * Projectile.scale;
                    }
                    Vector2 vector13 = Main.MouseWorld - vector;
                    vector13.Normalize();
                    if (vector13.HasNaNs())
                    {
                        vector13 = Vector2.UnitX * player.direction;
                    }
                    vector13 *= scaleFactor6;
                    if (vector13.X != Projectile.velocity.X || vector13.Y != Projectile.velocity.Y)
                    {
                        Projectile.netUpdate = true;
                    }
                    Projectile.velocity = vector13;
                }
                else
                {
                    Projectile.Kill();
                }
            }
            Vector2 vector14 = Projectile.Center + Projectile.velocity * 3f;
            Lighting.AddLight(vector14, 0.25f, 0.22f, 0.13f);
            if (Main.rand.Next(3) == 0)
            {
                int num30 = Dust.NewDust(vector14 - Projectile.Size / 2f, Projectile.width, Projectile.height, DustID.Gold, Projectile.velocity.X, Projectile.velocity.Y, 100, Color.White, 1f);
                Main.dust[num30].noGravity = true;
                Main.dust[num30].position -= Projectile.velocity;
            }
            Projectile.position = player.RotatedRelativePoint(player.MountedCenter, true) - Projectile.Size / 2f;
            Projectile.rotation = Projectile.velocity.ToRotation() + num;
            Projectile.spriteDirection = Projectile.direction;
            Projectile.timeLeft = 2;
            player.ChangeDir(Projectile.direction);
            player.heldProj = Projectile.whoAmI;
            player.itemTime = 2;
            player.itemAnimation = 2;
            player.itemRotation = (float)Math.Atan2(Projectile.velocity.Y * Projectile.direction, Projectile.velocity.X * Projectile.direction);
            Timer++;
            if (Timer == 1000)
            {
                Timer = 0;
            }
            if (Timer % 20 == 0)
            {
                Vector2 plrToMouse = Main.MouseWorld - player.Center;
                float r = (float)Math.Atan2(plrToMouse.Y, plrToMouse.X);
                for (int i = 1; i <= 1; i++)
                {
                        float r2 = r + i * MathHelper.Pi / 36f;
                        Vector2 shootVel = r2.ToRotationVector2() * 10;
                        Terraria.Projectile.NewProjectile(Projectile.InheritSource(Projectile), Projectile.Center, shootVel, ModContent.ProjectileType<SilverGoldenSpiritSwordWindW>(), 50, 10, player.whoAmI);
                        SoundEngine.PlaySound(SoundID.Item71, player.position);
                        interval++;
                }
                return;
                
            }
        }
    }
}
