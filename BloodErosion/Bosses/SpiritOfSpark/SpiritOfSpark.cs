using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.ModLoader;
using Terraria.Localization;
using Terraria.ID;
using Microsoft.Xna.Framework;
using System.IO;
using Terraria.ModLoader.Utilities;
using Terraria.DataStructures;
using BloodSoul.NPCs;
using Terraria.Audio;
using BloodSoul.MyUtils;
using Terraria.GameContent.ItemDropRules;
using BloodErosion.Items.Boss.SpiritOfSparks;
using BloodErosion.Items.MasterTrophy;

namespace BloodErosion.NPCs.Bosses.SpiritOfSpark
{
    [AutoloadBossHead]
    class SpiritOfSpark : FSMnpc
    {
        public EntitySource_ByProjectileSourceId Source_NPC;

        private int frameTime = 0;
        private int interval = 0;
        private int Time1 = 0;
        private int Time2 = 0;
        private static float gravity = 0.3f;
        public new Vector2 PlayerOldPos = Vector2.Zero;
        private enum SpiritOfSparkAI
        {
            chase,//追逐
            VoidSpike,//虚空突刺
            SparkBallF,//火花球
            BigFireBallF,//大火球
            Spike2//连续突刺
        }
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Spirit Of Spark");
            DisplayName.AddTranslation((int)GameCulture.CultureName.Chinese, "火花之灵");
            Main.npcFrameCount[NPC.type] = 6;
        }
        public override void SetDefaults()
        {
            base.SetDefaults();
            NPC.lifeMax = 2100;
            NPC.defense = 10;
            NPC.damage = 65 / 3;
            NPC.boss = true;
            NPC.npcSlots = 1f;
            NPC.knockBackResist = 0f;
            NPC.width = 16;
            NPC.height = 30;
            NPC.value = 30000;
            NPC.lavaImmune = true;
            NPC.noGravity = true;
            NPC.noTileCollide = true;
            NPC.netAlways = true;
            NPC.HitSound = SoundID.NPCHit3;
            NPC.DeathSound = SoundID.NPCDeath14;
            NPC.buffImmune[BuffID.OnFire] = true;
            NPC.buffImmune[BuffID.OnFire3] = true;
            NPC.aiStyle = -1;
            NPC.scale = 1.5f;
            BossBag = ModContent.ItemType<PermanentCombustionSpark>();
            if (!Main.dedServ)
            {
                Music = MusicLoader.GetMusicSlot(Mod, "Sounds/Music/SpiritSpark");
            }
        }
        public override void FindFrame(int frameHeight)
        {
            NPC.frameCounter++;
            if (NPC.frameCounter > 6)
            {
                NPC.frameCounter = 0;
                NPC.frame.Y += frameHeight;
                if (NPC.ai[0] == 2 || NPC.ai[0] == 3 || NPC.ai[0] == 4)
                {
                    if (NPC.frame.Y < 4 * frameHeight || NPC.frame.Y < 7 * frameHeight)
                    {
                        NPC.frame.Y = 4 * frameHeight;
                    }
                }
                else
                {
                    if (NPC.frame.Y > 4 * frameHeight)
                    {
                        NPC.frame.Y = 0;
                    }
                }
            }
        }
        public override void ModifyNPCLoot(NPCLoot npcLoot)
        {
            npcLoot.Add(ItemDropRule.BossBag(BossBag));
            npcLoot.Add(ItemDropRule.MasterModeCommonDrop(ModContent.ItemType<SpiritOfSparkRelicItem>()));
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
            NPC.TargetClosest();
            Player TargetPlayer = Main.player[NPC.target];
            if (Main.player[NPC.target].dead || Math.Abs(NPC.position.X - Main.player[NPC.target].position.X) > 6000f || Math.Abs(NPC.position.Y - Main.player[NPC.target].position.Y) > 6000f)
            {
                NPC.TargetClosest(false);
                if (Main.player[NPC.target].dead || Math.Abs(NPC.position.X - Main.player[NPC.target].position.X) > 6000f || Math.Abs(NPC.position.Y - Main.player[NPC.target].position.Y) > 6000f)
                {
                    DespawnHandler();
                }
                return;
            }
            bool forceChange = false;

            Vector2 ToTarget = (Target.position - NPC.position).SafeNormalize(Vector2.UnitX) * 15;
            NPC.rotation = ToTarget.ToRotation() + MathHelper.Pi / 2;

            Vector2 TargetVel = Vector2.Normalize(Target.position - NPC.Center);
            float X = NPC.Center.X - Target.Center.X;
            int Cen = (X > 0) ? 1 : -1;

            int FireBomb = ModContent.ProjectileType<FireBomb>();
            int SPB = ModContent.ProjectileType<SparkBall>();
            int BFB = ModContent.ProjectileType<BigFireBall>();


            TargetVel *= 7f;
            float accX = 0.21f;
            float accY = 0.21f;
            NPC.velocity.X += (NPC.velocity.X < TargetVel.X ? 1 : -1) * accX;
            NPC.velocity.Y += (NPC.velocity.Y < TargetVel.Y ? 1 : -1) * accY;

            switch ((SpiritOfSparkAI)State1)
            {

                case SpiritOfSparkAI.chase:
                    {
                        Time1++;
                        if (Time1 < 220)
                        {
                            Vector2 Head = new Vector2(Target.Center.X, Target.Center.Y - 250);
                            float ToHead = Vector2.Distance(NPC.Center, Head);
                            NPC.velocity = (NPC.velocity * 10 + (Head - NPC.Center)
                                .SafeNormalize(Vector2.UnitX) * 15) / 11;
                            if (ToHead < 2)
                            {
                                Time1++;
                                if (Time1 > 5)
                                {
                                    Time1 = 0;
                                }
                            }
                            Time2++;
                            Timer++;
                            if (Timer == 1000)
                            {
                                // 重置计时器
                                Timer = 0;
                            }
                            if (Timer % 80 == 0)
                            {

                                // NPC的攻击目标
                                Player p = Main.player[NPC.target];

                                {
                                    for (int i = 1; i <= 2; i++)
                                    {
                                        Projectile.NewProjectile(NPC.GetSpawnSourceForNPCFromNPCAI(), NPC.Center, (NPC.rotation - MathHelper.Pi / 2).ToRotationVector2() * 16
                                                        , FireBomb, 30, 2f, Main.myPlayer);
                                        SoundEngine.PlaySound(SoundID.Item20, NPC.position);
                                        interval++;
                                    }
                                    return;
                                }
                            }
                        }
                        if (Time2 > 200)
                        {
                            Time1 = 0;
                            Time2 = 0;
                            Timer = 0;
                            SwitchState2(0);
                            SwitchState1((int)SpiritOfSparkAI.chase, (int)SpiritOfSparkAI.Spike2 + 1);

                        }
                        break;
                    }
                case SpiritOfSparkAI.VoidSpike:
                    {
                        switch (State2)
                        {
                            case 0:
                                {
                                    Time1++;
                                    if (Time1 == 25)
                                    {
                                        Vector2 center = Target.Center + Main.rand.NextFloat(MathHelper.TwoPi)
                                            .ToRotationVector2() * 350;
                                        Dust.NewDustDirect(center, 10, 10, MyDustId.Fire);
                                        NPC.position = center;
                                        NPC.velocity = NPC.rotation.ToRotationVector2();
                                        NPC.netUpdate = true;
                                        SoundEngine.PlaySound(SoundID.Item8, NPC.position);
                                    }
                                    if (Time1 > 25)
                                    {
                                        Time1++;
                                        Vector2 ToPlayer = (ToTarget * 1f) + Target.velocity;
                                        SoundEngine.PlaySound(SoundID.Item20, NPC.position);
                                        if (Time1 > 200)
                                        {
                                            NPC.velocity = ToPlayer;
                                            Time1 = 0;
                                            Time2 = 0;
                                            SwitchState2(0);
                                            SwitchState1((int)SpiritOfSparkAI.chase, (int)SpiritOfSparkAI.Spike2 + 1);
                                        }
                                    }
                                    break;
                                }
                        }
                        break;
                    }
                case SpiritOfSparkAI.SparkBallF:
                    {
                        if (Time1 < 220)
                        {
                            Time2++;
                            Timer++;
                            if (Timer == 1000)
                            {
                                // 重置计时器
                                Timer = 0;
                            }
                            if (Timer % 90 == 0)
                            {

                                // NPC的攻击目标
                                Player p = Main.player[NPC.target];

                                {
                                    for (int i = 1; i <= 1; i++)
                                    {
                                        Projectile.NewProjectile(NPC.GetSpawnSourceForNPCFromNPCAI(), NPC.Center, (NPC.rotation - MathHelper.Pi / 2).ToRotationVector2() * 14
                                                        , SPB, 30, 2f, Main.myPlayer);
                                        SoundEngine.PlaySound(SoundID.Item20, NPC.position);
                                        interval++;
                                    }
                                    return;
                                }
                            }
                        }
                        if (Time2 > 200)
                        {
                            Time1 = 0;
                            Time2 = 0;
                            Timer = 0;
                            SwitchState2(0);
                            SwitchState1((int)SpiritOfSparkAI.chase, (int)SpiritOfSparkAI.Spike2 + 1);

                        }
                        break;
                    }
                case SpiritOfSparkAI.BigFireBallF:
                    {
                        Time1++;
                        if (Time1 < 220)
                        {
                            Time2++;
                            Timer++;
                            if (Timer == 1000)
                            {
                                Timer = 0;
                            }
                            if (Timer % 70 == 0)
                            {

                                // NPC的攻击目标
                                Player p = Main.player[NPC.target];
                                Vector2 ToPlayer = p.Center - NPC.Center;
                                //ToPlayer.Normalize();
                                {
                                    for (int i = 1; i <= 1; i++)
                                    {
                                        Projectile.NewProjectile(NPC.GetSpawnSourceForNPCFromNPCAI(), NPC.Center, (NPC.rotation - MathHelper.Pi / 2).ToRotationVector2() * 10
                                                        , BFB, 45, 2f, Main.myPlayer);
                                        SoundEngine.PlaySound(SoundID.Item73, NPC.position);
                                        interval++;
                                    }
                                    return;
                                }
                            }
                        }
                        if (Time2 > 200)
                        {
                            Time1 = 0;
                            Time2 = 0;
                            Timer = 0;
                            SwitchState2(0);
                            SwitchState1((int)SpiritOfSparkAI.chase, (int)SpiritOfSparkAI.Spike2 + 1);

                        }
                        break;
                    }
                case SpiritOfSparkAI.Spike2:
                    {
                        switch (State2)
                        {
                            case 0://到达头顶
                                {
                                    Vector2 Head = new Vector2(Target.Center.X, Target.Center.Y - 500);
                                    float ToHead = Vector2.Distance(NPC.Center, Head);
                                    NPC.velocity = (NPC.velocity * 10 + (Head - NPC.Center)
                                        .SafeNormalize(Vector2.UnitX) * 15) / 11;
                                    if (ToHead < 2)
                                    {
                                        Time1++;
                                        if (Time1 > 10)
                                        {
                                            Time1 = 0;
                                            SwitchState2(1);
                                        }
                                    }
                                    break;
                                }
                            case 1://冲刺
                                {
                                    Time1++;
                                    SoundEngine.PlaySound(SoundID.Item20, NPC.position);
                                    if (Time1 > 45)
                                    {
                                        Time2++;
                                        NPC.velocity = ToTarget * 1.1f;
                                        Time1 = 0;
                                        SwitchState2(2);
                                    }
                                    break;
                                }
                            case 2://保持速度
                                {
                                    Time1++;
                                    NPC.velocity *= 1f;
                                    if (Time1 > 30)
                                    {
                                        Time1 = 0;
                                        if (Time2 > 4)
                                        {
                                            SwitchState2(0);
                                            SwitchState1((int)SpiritOfSparkAI.chase, (int)SpiritOfSparkAI.chase + 1);
                                        }
                                        else
                                        {
                                            SwitchState2(1);
                                        }
                                    }
                                    break;
                                }
                        }
                        break;
                    }
            }
        }
        public override void OnKill()
        {
                var player = Main.player[NPC.target];
                Vector2 ToPlayer = player.Center - NPC.Center;
                for (int i = 0; i < 16; i++)
                {
                    Vector2 r = (ToPlayer.ToRotation() + (i * MathHelper.Pi / 8)).ToRotationVector2() * 13;
                    Projectile.NewProjectile(Source_NPC, NPC.position, r,
                    328, 30, 0f, Main.myPlayer);
                    interval++;
                }
            base.OnKill();
        }
        public override void OnHitPlayer(Player Target, int damage, bool crit)
        {
            Target.AddBuff(BuffID.OnFire3, 180);
        }

        private void DespawnHandler()
        {
            Player player = Main.player[NPC.target];
            NPC.TargetClosest(false);
            player = Main.player[NPC.target];
            if (!player.active || player.dead || Main.dayTime)
            {
                NPC.velocity.X = 0;
                NPC.velocity.Y -= 1;
            }
        }
    }
}
