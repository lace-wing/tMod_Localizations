using BloodSoul.MyUtils;
using BloodSoul.NPCs;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.IO;
using Terraria;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;

namespace BloodErosion.NPCs.Bosses.DivineGlow
{
    class AwakeningHolyDrill : FSMnpc
    {
        float r = 0;
        public EntitySource_ByProjectileSourceId Source_NPC;
        private Vector2 endPiont;
        private int frameTime = 0;
        private int interval = 0;
        private int TimeV = 0;
        private static float gravity = 0.3f;
        public new Vector2 PlayerOldPos = Vector2.Zero;
        private int Time1 = 0;
        private int Time2 = 0;
        private enum AwakeningHolyDrillAI
        {
            Spike2//连续突刺
        }
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Awakening·Holy Drill");
            DisplayName.AddTranslation((int)GameCulture.CultureName.Chinese, "真·圣钻");
        }
        public override void SetDefaults()
        {
            NPC.lifeMax = 10000;
            NPC.defense = 25;
            NPC.damage = 125 / 3;
            NPC.knockBackResist = 0f;
            NPC.width = 56;
            NPC.height = 18;
            NPC.value = 0;
            NPC.lavaImmune = true;
            NPC.noGravity = true;
            NPC.noTileCollide = true;
            NPC.dontTakeDamage = true;
            NPC.aiStyle = -1;
            NPC.boss = true;
            NPCID.Sets.TrailCacheLength[NPC.type] = 12;
            NPCID.Sets.TrailingMode[NPC.type] = 1;
            if (!Main.dedServ)
            {
                Music = MusicLoader.GetMusicSlot(Mod, "Sounds/Music/Light");
            }
        }
        public override void BossHeadRotation(ref float rotation) => rotation = -NPC.rotation;
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
        public override void SendExtraAI(BinaryWriter writer)
        {
            if (Main.netMode == 2)
            {
                writer.Write(interval);
                writer.Write(State3);
                writer.Write(State4);
            }
        }
        public override void ReceiveExtraAI(BinaryReader reader)
        {
            if (Main.netMode != 1)
            {
                interval = reader.ReadInt32();
                State3 = reader.ReadSingle();
                State4 = reader.ReadSingle();
            }
        }
        public override void AI()
        {
            if (NPC.target < 0 || NPC.target == 255 || Main.player[NPC.target].dead || !Main.player[NPC.target].active) NPC.TargetClosest();
            TimeV++;
            Vector2 ToTarget = (Target.position - NPC.position).SafeNormalize(Vector2.UnitX) * 15;
            NPC.rotation = ToTarget.ToRotation() + MathHelper.Pi;
            Vector2 TargetVel = Vector2.Normalize(Target.position - NPC.Center);
            if (Target.dead)
            {
                NPC.life = 0;
                return;
            }
            var player = Main.player[NPC.target];
            Vector2 ToPlayer = player.Center - NPC.Center;
            switch ((AwakeningHolyDrillAI)State1)
            {
                case AwakeningHolyDrillAI.Spike2:
                    {
                        switch (State2)
                        {
                            case 0://冲刺
                                {
                                    Time1++;
                                    if (Time1 > 45)
                                    {
                                        Time2++;
                                        SoundEngine.PlaySound(SoundID.Item71, NPC.position);
                                        NPC.velocity = ToTarget * 0.9f;
                                        Time1 = 0;
                                        SwitchState2(1);
                                    }
                                    break;
                                }
                            case 1://保持速度
                                {
                                    Time1++;

                                    if (Time1 > 30)
                                    {
                                        Time1 = 0;
                                        Time2++;
                                        if (Time2 > 4)
                                        {
                                            NPC.velocity *= 1f;
                                            Time2 = 0;
                                            SwitchState1((int)AwakeningHolyDrillAI.Spike2, (int)AwakeningHolyDrillAI.Spike2 + 1);
                                        }
                                        else
                                        {
                                            NPC.velocity *= 1f;
                                            SwitchState2(0);
                                        }
                                    }
                                    break;
                                }
                        }
                        break;
                    }
            }
        }
        public override bool CheckActive()
        {
            Player player = Main.player[NPC.target];
            if (player.dead) return true;
            return false;
        }
        public override void OnKill()
        {
            var player = Main.player[NPC.target];
            Vector2 ToPlayer = player.Center - NPC.Center;
            for (int i = 0; i < 8; i++)
            {
                Vector2 r = (ToPlayer.ToRotation() + (i * MathHelper.Pi / 4)).ToRotationVector2() * 16;
                Projectile.NewProjectile(Source_NPC, NPC.position, r,
                ModContent.ProjectileType<Light>(), 100 / 6, 0f, Main.myPlayer);
                interval++;
            }
            base.OnKill();
        }
        public override bool PreDraw(SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor)
        {
            Texture2D texture3 = BloodSoulUtils.GetTexture("Images/Ray").Value;
            Vector2 drawOrigin3;
            drawOrigin3 = new Vector2(texture3.Width * 0.5f, texture3.Height * 0.5f);
            Main.spriteBatch.Draw(texture3, NPC.Center - Main.screenPosition, null, new Color(255, 149, 202, 0), -r, drawOrigin3, new Vector2(0.6f, 0.6f), SpriteEffects.None, 0);
            return true;
        }
    }
}
