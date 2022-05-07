using Terraria;
using Terraria.Localization;
using Terraria.ID;
using Microsoft.Xna.Framework;
using Terraria.DataStructures;
using BloodSoul.NPCs;
using Terraria.ModLoader;

namespace BloodErosion.NPCs.Bosses.ApostleOfDeath
{
    class DeathAppearance : FSMnpc
    {
        public EntitySource_ByProjectileSourceId Source_NPC;

        private int interval = 0;
        private static float gravity = 0.3f;
        public new Vector2 PlayerOldPos = Vector2.Zero;

        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Clock of death");
            DisplayName.AddTranslation((int)GameCulture.CultureName.Chinese, "死神之钟");
            Main.npcFrameCount[NPC.type] = 12;
        }
        public override void SetDefaults()
        {
            NPC.friendly = false;
            NPC.width = 154;
            NPC.height = 174;
            NPC.aiStyle = -1;
            NPC.damage = 250;
            NPC.defense = 25;
            NPC.lifeMax = 10000;
            NPC.HitSound = SoundID.NPCHit20;
            NPC.DeathSound = SoundID.NPCDeath39;
            NPC.knockBackResist = 0f;
            NPC.lavaImmune = true;
            NPC.noGravity = true;
            NPC.noTileCollide = true;
            NPC.npcSlots = 0.8f;
            NPC.dontTakeDamage = true;
            NPC.boss = true;
            if (!Main.dedServ)
            {
                Music = MusicLoader.GetMusicSlot(Mod, "Sounds/Music/Death2");
            }
        }
        public override void FindFrame(int frameHeight)
        {
            if (NPC.velocity.X > 0f)
            {
                NPC.spriteDirection = 1;
            }
            if (NPC.velocity.X < 0f)
            {
                NPC.spriteDirection = -1;
            }
            NPC.rotation = NPC.velocity.X * 0.1f;
            NPC.frameCounter += 1.0;
            int num155 = 15;
            int num156 = Main.npcFrameCount[NPC.type];
            if (NPC.frameCounter >= (double)num155)
            {
                NPC.frame.Y = NPC.frame.Y + frameHeight;
                NPC.frameCounter = 0.0;
            }
            if (NPC.frame.Y >= frameHeight * num156)
            {
                NPC.active = false;
                NPC.NewNPC(NPC.GetSpawnSourceForNPCFromNPCAI(), (int)(NPC.Center.X), (int)(NPC.Center.Y), ModContent.NPCType<AwakeningDeathApostles>(), NPC.whoAmI);
            }
        }
        protected int State
        {
            get { return (int)NPC.ai[0]; }
            set { NPC.ai[0] = value; }
        }
        protected int Timer
        {
            get { return (int)NPC.ai[1]; }
            set { NPC.ai[1] = value; }
        }
        protected virtual void SwitchState(int state)
        {
            State = state;
        }
        enum NPCState
        {
            Normal,
            Attack,
        }
        public override void AI()
        {
            NPC.velocity *= 0;
        }
    }
}
