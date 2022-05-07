using Microsoft.Xna.Framework;
using Terraria;
using Terraria.DataStructures;
using Terraria.GameContent.Creative;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;

namespace BloodErosion.Items.Boss.FlameGhostKings
{
    public class BurningKingBow : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Burning King Bow");
            DisplayName.AddTranslation((int)GameCulture.CultureName.Chinese, "王炎弓");

            CreativeItemSacrificesCatalog.Instance.SacrificeCountNeededByItemId[Type] = 1;
        }

        private float glowRot = 0;

        public override void SetDefaults()
        {
            Item.damage = 39;
            Item.height = 36;
            Item.width = 62;
            Item.useTime = 40;
            Item.useAnimation = 40;
            Item.DamageType = DamageClass.Ranged;
            Item.useStyle = ItemUseStyleID.Shoot;
            Item.knockBack = 7;
            Item.channel = true;
            Item.rare = ItemRarityID.Red;
            Item.noMelee = true;
            Item.autoReuse = true;
            Item.value = Item.sellPrice(0, 12, 50, 0);
            Item.UseSound = SoundID.Item117;
            Item.shootSpeed = 16f;
            Item.shoot = ModContent.ProjectileType<BurningRay>();

        }
        public override Vector2? HoldoutOffset()
        {
            return new Vector2(-10, -1);
        }
        private int CastCount;
        public override void HoldItem(Player player)
        {
            if (!player.channel)
                CastCount = 0;
        }

        public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {
            Vector2 plrToMouse = Main.MouseWorld - player.Center;
            float numberProjectiles = 3;
            float rotation = MathHelper.ToRadians(1);
            for (int i = 0; i < numberProjectiles; i++)
            {
                Vector2 perturbedSpeed = velocity.RotatedBy(MathHelper.Lerp(-rotation, rotation, i / (numberProjectiles - 1)));
                Projectile.NewProjectile(source, position, perturbedSpeed, type, damage, knockback, player.whoAmI);
            }
            return false;
        }

        public override void ModifyShootStats(Player player, ref Vector2 position, ref Vector2 velocity, ref int type, ref int damage, ref float knockback)
        {
            Vector2 Offset = Vector2.Normalize(velocity) * 70f;

            if (Collision.CanHit(position, 0, 0, position + Offset, 0, 0))
            {
                position += Offset;
            }
        }

        public override void AddRecipes()
        {
            CreateRecipe()
                .AddIngredient(ModContent.ItemType<BurningGhostKingFragment>(), 15)
                .AddTile(TileID.Anvils)
                .Register();
        }

        public override void PostUpdate()
        {
            glowRot += 0.03f;
        }

    }
}