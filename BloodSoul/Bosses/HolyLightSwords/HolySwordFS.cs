using Terraria.ModLoader;
using Terraria;
using Terraria.Localization;
using Terraria.IO;
using Terraria.Audio;
using Microsoft.Xna.Framework;
using Terraria.ID;
using System;
using Microsoft.Xna.Framework.Graphics;
using System.IO;
using BloodSoul.MyUtils;
using Terraria.DataStructures;
using BloodSoul.Projectiles.Bosses;

namespace BloodSoul.NPCs.Bosses.HolyLightSwords
{
    class HolySwordFS : FSMnpc
    {
        public override string Texture => "BloodSoul/NPCs/Bosses/HolyLightSwords/HolySwordFS";
        public override void SetStaticDefaults()
        {
            //设置英文名字
            DisplayName.SetDefault("远古英灵剑");
            //设置NPC图片的帧数
            Main.npcFrameCount[NPC.type] = 1;
        }
        public override void SetDefaults()
        {
            NPC.lifeMax = 5500;
            NPC.defense = 50;
            NPC.damage = 85 / 3;
            NPC.knockBackResist = 0f;
            NPC.width = 60;
            NPC.height = 60;
            NPC.value = Item.buyPrice(0, 11, 45, 14);
            NPC.lavaImmune = true;
            NPC.noGravity = true;
            NPC.noTileCollide = true;
            NPC.aiStyle = -1;
            NPC.dontTakeDamage = true;
            NPC.boss = false;
            NPCID.Sets.TrailCacheLength[NPC.type] = 12;
            NPCID.Sets.TrailingMode[NPC.type] = 1;
        }
        public override void AI()
        {
            if (NPC.target < 0 || NPC.target == 255 || Main.player[NPC.target].dead || !Main.player[NPC.target].active)
            {
                NPC.TargetClosest();
            }
            Vector2 ToTarget = (Target.position - NPC.position).SafeNormalize(Vector2.UnitX) * 15;
            NPC.rotation = ToTarget.ToRotation() + MathHelper.Pi / 4;
            int Feng = ModContent.ProjectileType<HolySwordFSWind>();
            if(Target.dead)
            {
                NPC.life = 0;
                return;
            }
            #region 冲刺部分
            if (Timer1 < 30) Timer1 = 150;
            else Timer1--;
            NPC.rotation = NPC.velocity.ToRotation() + MathHelper.Pi / 4;
            if (Timer1 == 30)
            {
                int r = Main.rand.Next(2);//确定随机出现的位置
                Vector2 center = Target.position + (MathHelper.Pi * r).ToRotationVector2() * 600;//确定点的位置
                NPC.Center = center;
                Dust.NewDustDirect(center, 10, 10, MyDustId.YellowGoldenFire);
                NPC.netUpdate = true;
            }
            else if (Timer1 == 150 && Main.netMode != NetmodeID.MultiplayerClient)
            {
                NPC.velocity = ToTarget;
                Projectile.NewProjectile(null, NPC.position + new Vector2(0, (Main.rand.NextBool() ? 300 : -300)), NPC.velocity, ModContent.ProjectileType<HolySwordFSWind>(), 30, 1.3f, Main.myPlayer);
                NPC.netUpdate = true;
            }
            #endregion

        }
        public override bool CheckActive()
        {
            Player player = Main.player[NPC.target];
            if (player.dead) return true;
            return false;
        }
        public override bool PreDraw(SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor)
        {
            Color Tail = Color.White;
            BloodSoulWay.NpcDrawTail(NPC, drawColor, Tail);
            return true;
        }
    }
}
