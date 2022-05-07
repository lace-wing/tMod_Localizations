using Microsoft.Xna.Framework;
using System;
using Terraria;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;
using BloodSoul;
using BloodSoul.Projectiles;
using BloodSoul.Items;
using Terraria.DataStructures;
using BloodSoul.Projectiles.Melee;
using BloodErosion.NPCs.Bosses.GoldAndSilverDoubleSwords;

namespace BloodErosion.Items.Boss.GoldAndSilverDoubleSwords
{
    public class GoldAndSilverSpiritSwordI : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Gold And Silver Spirit Sword");
            DisplayName.AddTranslation((int)GameCulture.CultureName.Chinese, "金银灵剑");
            Tooltip.SetDefault("They have successfully fused!?");
            Tooltip.AddTranslation((int)GameCulture.CultureName.Chinese, "[c/FFD306:它们居然成功][c/FFF8D7:融合在一起了][c/FFF8D7:！][c/FFD306:？]");
            Main.RegisterItemAnimation(Item.type, new DrawAnimationVertical(7, 8));
        }
        public override void SetDefaults()
        {
            //useThisBaseItem = true;
            //Item.CloneDefaults(ItemID.Arkhalis);
            Item.width = 36;
            Item.height = 36;
            Item.damage = 30;
            Item.DamageType = DamageClass.NoScaling;
            Item.knockBack = 2f;
            Item.useTime = 20;
            Item.useAnimation = 20;
            Item.rare = -12;
            Item.value = 25000;
            Item.useTurn = false;
            Item.autoReuse = false;
            Item.useStyle = 1;
            Item.crit = 11;
            Item.UseSound = SoundID.Item1;
            Item.noMelee = true;
            Item.shoot = ModContent.ProjectileType<GoldAndSilverSpiritSwordProj>();
            Item.shootSpeed = 15f;
            Item.noUseGraphic = true;
            Item.channel = true;

        }
        public override int BossBagNPC => ModContent.NPCType <GoldenSpiritSword>();
    }
}
