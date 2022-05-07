using Terraria.ModLoader;
using Terraria;
using Terraria.Localization;
using Terraria.IO;
using Terraria.Audio;
using Microsoft.Xna.Framework;
using Terraria.ID;
using System;
using Microsoft.Xna.Framework.Graphics;
using BloodSoul.NPCs.Bosses.HolyLightSwords;
using BloodSoul.Items.Weapons.SwordSoul;
using BloodErosion.NPCs.Bosses.FlameGhostKing;

namespace BloodErosion.Items.Boss.FlameGhostKings
{
    class FlameGhostKingBossBag : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Flame Ghost King BossBag");
            DisplayName.AddTranslation((int)GameCulture.CultureName.Chinese, "宝藏袋");
            Tooltip.SetDefault("{$CommonItemTooltip.RightClickToOpen}");
        }
        public override void SetDefaults()
        {
            Item.maxStack = 999;
            Item.consumable = true;
            Item.width = 32;
            Item.height = 32;
            Item.rare = ItemRarityID.Expert;
            Item.expert = true;
        }
        public override bool CanRightClick()
        {
            return true;
        }
        public override void OpenBossBag(Player player)
        {
            player.TryGettingDevArmor(player.GetItemSource_OpenItem(Type));
            player.QuickSpawnItem(player.GetItemSource_OpenItem(Type), ModContent.ItemType<BurningGhostKingFragment>(), 25);
        }
        public override int BossBagNPC => ModContent.NPCType<FlameGhostKing>();
    }
}
