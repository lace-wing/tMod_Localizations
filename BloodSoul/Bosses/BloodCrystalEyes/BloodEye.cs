using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using Terraria.ModLoader;
using Terraria.Localization;
using Microsoft.Xna.Framework;
using Terraria.ID;
using System.IO;
using Microsoft.Xna.Framework.Graphics;
using Terraria.ModLoader.Utilities;

namespace BloodSoul.NPCs.Bosses.BloodCrystalEyes
{
    class BloodEye : FSMnpc
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Blood eye larva");
            DisplayName.AddTranslation((int)GameCulture.CultureName.Chinese, "血眼幼体");
            Main.npcFrameCount[NPC.type] = 4;
            NPCID.Sets.TrailCacheLength[NPC.type] = 12;
            NPCID.Sets.TrailingMode[NPC.type] = 1;
        }
        public override void SetDefaults()
        {
            NPC.lifeMax = 40;
            NPC.friendly = false;
            NPC.defense = 15;
            NPC.damage = 30;
            NPC.knockBackResist = 0.5f;
            NPC.width = 32;
            NPC.height = 32;
            NPC.value = Item.buyPrice(0, 0, 19, 19);
            NPC.lavaImmune = true;
            NPC.noGravity = true;
            NPC.noTileCollide = true;
            NPC.HitSound = SoundID.NPCHit1;
            NPC.DeathSound = SoundID.NPCDeath1;
            NPC.aiStyle = -1;
        }
        public override void AI()
        {
            Timer1++;
            var player = Main.player[NPC.target];
            float max = 400;
            float ToPlayer = Vector2.Distance(NPC.Center, player.Center);
            Vector2 vector = player.Center - NPC.Center;
            vector.Normalize();
            NPC.rotation = NPC.velocity.ToRotation();
            if (max > ToPlayer)
            {
                NPC.aiStyle = -1;
                if (Timer1 % 30 == 0)
                {
                    NPC.velocity = vector * 8;
                    Timer1 = 0;
                }
            }
            else
            {
                NPC.aiStyle = 2;
            }
        }
        public override void FindFrame(int frameHeight)
        {
            NPC.frameCounter++;
            NPC.frameCounter += NPC.velocity.Length() * 3;
            if(NPC.frameCounter > 4)
            {
                NPC.frame.Y += frameHeight;
                if(NPC.frame.Y >= frameHeight * 4)
                {
                    NPC.frame.Y = 0;
                }
            }
        }
        public override float SpawnChance(NPCSpawnInfo spawnInfo)
        {
            if (!Main.dayTime)
            {
                return SpawnCondition.Overworld.Chance * 0.005f;
            }
            return 0;
        }
        public override bool PreDraw(SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor)
        {
            BloodSoulWay.NpcDrawTail(NPC, drawColor, Color.White);
            return true;
        }
        public override void PostDraw(SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor)
        {
            BloodSoulWay.NpcDrawTail(NPC, drawColor, Color.White);
        }
    }
}
