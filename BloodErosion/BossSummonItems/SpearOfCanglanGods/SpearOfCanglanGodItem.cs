using Terraria.ID;
using Terraria.ModLoader;
using Microsoft.Xna.Framework;
using Terraria.Localization;
using Terraria;
using Terraria.DataStructures;
using Terraria.Audio;
using System;
using Microsoft.Xna.Framework.Graphics;
using BloodErosion.MyUtils;
using BloodSoul.Buffs;

namespace BloodErosion.Items.Boss.SpearOfCanglanGods
{
    public class SpearOfCanglanGodItem : ModItem
    {
        public int ticksperframe = 0;
        public int frameCount = 1;
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("SpearOfCanglanGod");
            DisplayName.AddTranslation((int)GameCulture.CultureName.Chinese, "苍岚神矛");
            Tooltip.SetDefault("Right click to release thunder raid");
            Tooltip.AddTranslation((int)GameCulture.CultureName.Chinese, "右键释放狂雷突袭");
            Main.RegisterItemAnimation(base.Item.type, new DrawAnimationVertical(8, 4));
        }
        public override void SetDefaults()
        {
            Item.damage = 95;
            Item.DamageType = DamageClass.NoScaling;
            Item.width = 128;
            Item.height = 128;
            Item.useTime = 21;
            Item.useAnimation = 21; 
            Item.rare = -12;
            Item.noMelee = true;
            Item.noUseGraphic = true;
            Item.useStyle = 5;
            Item.knockBack = 1.1f;
            Item.UseSound = SoundID.Item1;
            Item.crit = 6;
            Item.value = 800;
            Item.scale = 1f;
            Item.shoot = ModContent.ProjectileType<SpearOfCanglanGodItemProj>();
            Item.channel = true;
        }
        public override void Update(ref float gravity, ref float maxFallSpeed)
        {
            ticksperframe++;
            if (ticksperframe > 8)
            {
                frameCount++;
                ticksperframe = 0;
            }
            if (frameCount > 3)
            {
                frameCount = 1;
            }
        }
        public override bool AltFunctionUse(Player player)
        {
            return true;
        }
        public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {
            if (player.altFunctionUse == 2 && !player.HasBuff(ModContent.BuffType<Cooldown>()))
            {
                Item.shoot = ModContent.ProjectileType<SpearOfCanglanGodItemProj3>();
            }
            else if (player.altFunctionUse == 2 && player.HasBuff(ModContent.BuffType<Cooldown>()))
            {
                return false;
            }
            if (player.altFunctionUse != 2)
            {
                Projectile.NewProjectile(source, position, velocity, ModContent.ProjectileType<SpearOfCanglanGodItemProj>(), damage, knockback, player.whoAmI);
                return false;
            }
            return true;
        }
        public bool Dash = false;
        public int timer1 = 0;
        public override void HoldItem(Player player)
        {
            if (player.altFunctionUse == 2 && !player.HasBuff(ModContent.BuffType<Cooldown>()))
            {
                Dash = true;
                player.velocity = Vector2.Normalize(Main.MouseWorld - player.position) * 60;
                player.AddBuff(ModContent.BuffType<Cooldown>(), 420);
            }
            if (Dash)
            {
                player.immune = true;
                player.immuneTime = 120;
                timer1++;
                if (timer1 == 10)
                {
                    player.velocity = Vector2.Zero;
                    timer1 = 0;
                    Dash = false;
                }
            }
        }
    }
    public class SpearOfCanglanGodItemProj3 : ModProjectile
    {
        private Vector2 OldPlayer;
        private Vector2 OldProj;
        private Vector2 Proj;

        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("苍雷冲刺");
        }

        public override void SetDefaults()
        {
            Projectile.aiStyle = -1;
            Projectile.penetrate = -1;
            Projectile.timeLeft = 60;
            Projectile.DamageType = DamageClass.Melee;
            Projectile.tileCollide = false;
            Projectile.width = 128;
            Projectile.height = 128;
            Projectile.friendly = false;
            Projectile.hostile = false;
            Main.projFrames[Projectile.type] = 4;
        }
        private float Num = 1f;//乘数
        public EntitySource_ByProjectileSourceId projectileSource;
        private Vector2 OldVec = Vector2.Zero;
        public override void AI()
        {
            float R = 0f;
            if (Projectile.spriteDirection == -1)
            {
                R = 4f;
            }
            else
            {
                R = -4f;
            }
            if (Num == 1f)
            {
                Projectile.ai[1] += 15f * Num;
            }
            else
            {
                Projectile.ai[1] += 15f * Num;
            }
            if (Projectile.ai[1] >= 200f && Num == 1f)
            {
                Num = -0.5f;
            }
            if (Num < 0f && Projectile.ai[1] <= 0f)
            {
                Projectile.Kill();
            }

            Player player = Main.player[Projectile.owner];
            Projectile.frameCounter++;
            if (Projectile.frameCounter > 3)
            {
                Projectile.frame++;
                Projectile.frameCounter = 0;
            }
            if (Projectile.frame > 5)
            {
                Projectile.alpha = 255;
            }
            if (Projectile.frame == 2)
            {
                SoundEngine.PlaySound(SoundID.Item71, base.Projectile.Center);
                Projectile.friendly = true;
            }
            else
            {
                Projectile.friendly = false;
            }
            Projectile.position = player.MountedCenter - Projectile.Size / 2f + new Vector2((player.direction * -20), 0f);
            Projectile.spriteDirection = Projectile.direction;
            Projectile.rotation = Utils.ToRotation(Projectile.velocity) + ((Projectile.direction == -1) ? 3.14f : 0f) + MathHelper.Pi / 4;
            player.ChangeDir(Projectile.direction);
            player.heldProj = Projectile.whoAmI;
            player.itemTime = 2;
            player.itemAnimation = 2;
            player.itemRotation = (float)Math.Atan2((double)(Projectile.velocity.Y * (float)Projectile.direction), (double)(Projectile.velocity.X * (float)Projectile.direction));
            Projectile.Center = player.Center;
            if (Projectile.timeLeft > 48)
            {
                for (int i = 0; i < Main.rand.Next(2, 3); i++)
                {
                    Vector2 pos = new Vector2(200).RotateRandom(2 * Math.PI);
                    Vector2 spd = -pos;
                    spd.Normalize();
                    Projectile.NewProjectile(projectileSource, player.Center + pos, spd * Projectile.velocity.Length(), ModContent.ProjectileType<SpearOfCanglanGodItemProj2>(), 5000, 0, Projectile.owner);
                }
            }
            if (Projectile.timeLeft > 58)
            {
                OldPlayer = player.Center;
                OldProj = Projectile.Center;
            }
            if (Projectile.timeLeft >= 50)
            {
                Proj = Projectile.Center;
            }
            OldPlayer.Normalize();
        }
        public override bool PreDraw(ref Color lightColor)
        {
            Player player = Main.player[Projectile.owner];
            Texture2D texture = BloodSoulUtils.GetTexture("Images/TailStar").Value;
            Vector2 vector = new Vector2(texture.Width * 0.5f, texture.Height * 0.5f);
            Main.spriteBatch.Draw(texture, (Proj + OldProj) / 2 - Main.screenPosition, new Rectangle(0, 0, texture.Width, texture.Height), new Color(30, 144, 255, 100), Projectile.rotation, vector, new Vector2(Math.Abs(Projectile.Center.X - OldPlayer.X) / texture.Width / texture.Width, Math.Min(base.Projectile.ai[1], 100f) / 100f / 4), SpriteEffects.None, 0f);
            return true;
        }
        public override void OnHitNPC(NPC target, int damage, float knockback, bool crit)
        {
            target.AddBuff(ModContent.BuffType<EnergyPiercing>(), 120);
            var Player = Main.player[Projectile.owner];
            Player.HealEffect(4);
            Player.statLife += 4;
        }
        public override Color? GetAlpha(Color lightColor)
        {
            lightColor.A = 0;
            return new Color?(lightColor);
        }
    }
}
