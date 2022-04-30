using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;

namespace BloodSoul.NPCs.Bosses.SaintGoldDetector
{
    public class SaintGoldGuards : FSMnpc
    {
        private int _frameHeight = 0;
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Saint Gold Guards");
            DisplayName.AddTranslation((int)GameCulture.CultureName.Chinese, "圣金近卫机");
            Main.npcFrameCount[NPC.type] = 5;
            NPCID.Sets.TrailCacheLength[NPC.type] = 10;
            NPCID.Sets.TrailingMode[NPC.type] = 1;
        }
        public override void SetDefaults()
        {
            NPC.lifeMax = 1250;
            NPC.defense = 20;
            NPC.damage = 85;
            NPC.knockBackResist = 0f;
            NPC.width = 60;
            NPC.height = 60;
            NPC.value = Item.buyPrice(0, 11, 45, 14);
            NPC.lavaImmune = true;
            NPC.noGravity = true;
            NPC.noTileCollide = true;
            NPC.HitSound = SoundID.NPCHit4;
            NPC.DeathSound = SoundID.NPCDeath10;
            NPC.aiStyle = -1;
            NPC.alpha = 255;
        }
        public override void AI()
        {
            GetNPCTarget();
            NPC.velocity = SpeedGradient(NPC.velocity, Vector2.Normalize(Target.Center - NPC.Center) * 10, 50);
            NPC.rotation = NPC.velocity.ToRotation() + MathHelper.Pi;
        }
        public override void FindFrame(int frameHeight)
        {
            NPC.frameCounter++;
            _frameHeight = frameHeight;
            if (NPC.frameCounter > 5)
            {
                NPC.frameCounter = 0;
                NPC.frame.Y += frameHeight;
                if (NPC.frame.Y > 4 * frameHeight)
                {
                    NPC.frame.Y = 0;
                }
            }
        }
        public override bool PreDraw(SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor)
        {
            Texture2D texture;
            if (State1 == 0)
            {
                if (Vector2.Distance(NPC.position, Target.position) > 200)
                {
                    texture = ModContent.Request<Texture2D>("BloodSoul/NPCs/Bosses/SaintGoldDetector/SaintGoldGuards").Value;
                }
                else
                {
                    texture = ModContent.Request<Texture2D>("BloodSoul/NPCs/Bosses/SaintGoldDetector/SaintGoldGuards2").Value;
                }
            }
            else
            {
                texture = ModContent.Request<Texture2D>("BloodSoul/NPCs/Bosses/SaintGoldDetector/SaintGoldGuards1").Value;
            }
            Main.spriteBatch.Draw(texture, NPC.Center - Main.screenPosition,
                new Rectangle(0, (int)(texture.Height / 5 + NPC.frame.Y / _frameHeight), texture.Width, texture.Height / 5), Color.White,
                NPC.rotation, new Vector2(texture.Width / 2, texture.Height / 10), State1 == 1 ? 2f : 1f, SpriteEffects.None, 0f);
            return false;
        }
        public override void OnKill()
        {
            Main.npc[(int)NPC.ai[3]].ai[1]++;
        }
    }
}
