using BloodSoul.MyUtils;
using BloodSoul.Projectiles.SoulSword;
using BloodSoul.StarRiverLoong;
using Microsoft.Xna.Framework;
using System;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;

namespace BloodErosion.Items.Boss.FlameGhostKings
{
    class BurningKingSword : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Burning King Sword");
            DisplayName.AddTranslation((int)GameCulture.CultureName.Chinese, "王炎之剑");
        }
        public override void SetDefaults()
        {
            Item.width = 54;
            Item.height = 54;
            Item.UseSound = SoundID.Item71;
            Item.DamageType = DamageClass.Melee;
            Item.damage = 59;
            Item.crit = 1;
            Item.knockBack = 3;
            Item.value = 100000;
            Item.useTurn = true;
            Item.useTime = 60;
            Item.useAnimation = 60;
            Item.rare = ItemRarityID.Red;
            Item.useStyle = 1;
            Item.shootSpeed = 1;
            Item.noUseGraphic = true;
            Item.noMelee = true;
            Item.autoReuse = true;
            Item.channel = true;
            Item.shoot = ModContent.ProjectileType<BurningKingSwordProj>();
        }
        public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {
            Vector2 vector = Main.MouseWorld - player.Center;
            vector.Normalize();
            vector *= 15;
            Vector2 plrToMouse = Main.MouseWorld - player.Center;
            float r = (float)Math.Atan2(plrToMouse.Y, plrToMouse.X);
            //Projectile.NewProjectile(player.GetProjectileSource_Item(Item), player.Center, vector, ModContent.ProjectileType<AirS>(), damage, knockback, player.whoAmI);
            for (int i = 0; i <= 0; i++)
            {
                float r2 = r + i * MathHelper.Pi / 36f;
                Vector2 shootVel = r2.ToRotationVector2() * 12;
                Terraria.Projectile.NewProjectile(source, position, shootVel, 295, 70, 10, player.whoAmI);
            }
            type = ModContent.ProjectileType<BurningKingSwordProj>();
            BASEPlayer modplayer = player.GetModPlayer<BASEPlayer>();
            Projectile.NewProjectileDirect(source, position, velocity, type, damage, knockback, player.whoAmI, modplayer.WaveDirection);
            modplayer.WaveDirection *= -1;
            return false;
        }
        class BASEPlayer : ModPlayer
        {
            public int WaveDirection { get; set; } = 1;
        }
        public Projectile Sword1;
        public override Vector2? HoldoutOrigin()
        {
            //泰拉坐标轴向下为正
            return new Vector2(0, 6);
        }
        public override void AddRecipes()
        {
            CreateRecipe()
                .AddIngredient(ModContent.ItemType<BurningGhostKingFragment>(), 15)
                .AddTile(TileID.Anvils)
                .Register();
        }
    }
    public class BurningKingSwordProj : Base
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("王炎剑");
        }
        public override void MeleeDefaults()
        {
            //大小 填你的贴图大小就OK了（物品的size也一样）
            Projectile.Size = new Vector2(54, 54);
            //偏差系数 刀光有偏差时使用
            OldPosOffsetNum = 0.5f;
            //绘制模式 DrawMode为0时使用ColorBar DrawMode为1时使用Color
            DrawMode = 1;
            //颜色 DrawMode为1时使用
            Color = new Color(185, 0, 0);
            Projectile.localNPCHitCooldown = 30;
            Projectile.netUpdate = true;
            Projectile.netImportant = true;
            Projectile.usesLocalNPCImmunity = true;
            //颜色条带 DrawMode为0时使用
            //ColorBar = BloodSoulUtils.GetTexture("Images/Extra_191").Value;
        }
        public EntitySource_ByProjectileSourceId projectileSource;
        public override void OnHitNPC(NPC target, int damage, float knockback, bool crit)
        {
            int numProj = 1;
            for (int i = 0; i < numProj; i++)
            {
                Projectile.NewProjectile(projectileSource, Player.Center, Vector2.Zero, ModContent.ProjectileType<BurningStar>(), (int)(Projectile.damage * 0.5f), Projectile.knockBack, 0);
            }
        }
    }
}
