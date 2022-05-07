using BloodSoul.Items.Weapons.SwordSoul;
using BloodSoul.MyUtils;
using BloodSoul.Projectiles.Bosses;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.IO;
using Terraria;
using Terraria.DataStructures;
using Terraria.GameContent;
using Terraria.GameContent.ItemDropRules;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;
using BloodErosion; 
using BloodSoul.NPCs;
using Terraria.Audio;
using BloodErosion.Items.Boss.GoldAndSilverDoubleSwords;

namespace BloodErosion.NPCs.Bosses.GoldAndSilverDoubleSwords
{
    public abstract class GoldAndSilverDoubleSword : FSMnpc
    {
        public EntitySource_ByProjectileSourceId Source_NPC;

        private int frameTime = 0;
        private int interval = 0;
        private int Time1 = 0;
        private int Time2 = 0;
        private int Time3 = 0;
        private static float gravity = 0.3f;
        public Vector2 PlayerOldPos = Vector2.Zero;
        private enum GoldAndSilverDoubleSwordsAI
        {
            chase,//追逐
            SwordWind,//剑风
            ShortSword,//头顶短剑发射
            Spike,//预判突刺
            SwordArray,//剑阵
            Spike2,//连续突刺
            ShortSwordVolley,//短剑散射
            VoidSpike//虚空突刺
        }
        public override void SetStaticDefaults()
        {
            Main.npcFrameCount[NPC.type] = 8;
        }
        public override void SetDefaults()
        {
            NPC.boss = true;
            NPC.npcSlots = 1f;
            NPC.knockBackResist = 0f;
            NPC.width = 32;
            NPC.height = 32;
            NPC.value = 50000;
            NPC.lavaImmune = true;
            NPC.noGravity = true;
            NPC.noTileCollide = true;
            NPC.netAlways = true;
            NPC.HitSound = SoundID.NPCHit4;
            NPC.DeathSound = SoundID.NPCDeath7;
            NPC.aiStyle = -1;
            NPC.scale = 1.2f;
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
        public override void BossHeadRotation(ref float rotation) => rotation = NPC.rotation;
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
        private Vector2 _TargetPos;
        // 冲刺起始位置
        private Vector2 _startPos;
        public override void AI()
        {
            NPC.TargetClosest();
            Player TargetPlayer = Main.player[NPC.target];
            if (Main.player[NPC.target].dead)
            {
                NPC.TargetClosest(false);
                if (Main.player[NPC.target].dead)
                {
                    DespawnHandler();
                }
                return;
            }
            Vector2 ToTarget = (Target.position - NPC.position).SafeNormalize(Vector2.UnitX) * 15;
            NPC.rotation = ToTarget.ToRotation() + MathHelper.Pi / 4;

            Vector2 TargetVel = Vector2.Normalize(Target.position - NPC.Center);
            float X = NPC.Center.X - Target.Center.X;
            int Cen = (X > 0) ? 1 : -1;

            int SwordWind = ModContent.ProjectileType<SilverGoldenSpiritSwordWind>();
            int ShortSword = ModContent.ProjectileType<GoldenSilverSpiritShortSword>();
            /*if (NPC.life < NPC.lifeMax * 0.9f)
            {
                SwitchState2(1);
            }*/
            switch ((GoldAndSilverDoubleSwordsAI)State1)
            {

                case GoldAndSilverDoubleSwordsAI.chase:
                    {
                        Time1++;
                        if (Time1 < 450)
                        {
                            TargetVel *= 7f;
                            float accX = 0.3f;
                            float accY = 0.3f;
                            NPC.velocity.X += (NPC.velocity.X < TargetVel.X ? 1 : -1) * accX;
                            NPC.velocity.Y += (NPC.velocity.Y < TargetVel.Y ? 1 : -1) * accY;
                            Time2++;
                        }
                        if (Time2 > 400)
                        {
                            Time1 = 0;
                            Time2 = 0;
                            SwitchState2(0);
                            SwitchState1((int)GoldAndSilverDoubleSwordsAI.chase, (int)GoldAndSilverDoubleSwordsAI.VoidSpike + 1);
                            
                        }
                            break;
                    }
                case GoldAndSilverDoubleSwordsAI.SwordWind:
                    {
                        float V = NPC.Center.X - Target.Center.X;
                        int VAN = (V > 0) ? 1 : -1;
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
                                        if (Time1 > 5)
                                        {
                                            Time1 = 0;
                                            SwitchState2(1);
                                        }
                                    }
                                    break;
                                }
                            case 1://发射弹幕
                                {
                                    Time1++;
                                    if (Time1 == 25)
                                    {
                                        Vector2 center = Target.Center + Main.rand.NextFloat(MathHelper.TwoPi)
                                            .ToRotationVector2() * 300;
                                        Dust.NewDustDirect(center, 10, 10, MyDustId.YellowGoldenFire);
                                        NPC.position = center;
                                        NPC.rotation = ToTarget.ToRotation();
                                        NPC.velocity = NPC.rotation.ToRotationVector2();
                                        NPC.netUpdate = true;
                                    }
                                    if (Time1 > 25)
                                    {
                                        NPC.velocity = NPC.velocity.RotatedBy(0.055f) * -1;
                                        NPC.rotation = NPC.velocity.ToRotation() + MathHelper.Pi / 2;
                                        if (Time1 % 10 == 0 && Main.netMode != 1)
                                        {
                                            Projectile.NewProjectile(NPC.GetSpawnSourceForNPCFromNPCAI(), NPC.Center, (NPC.rotation - MathHelper.Pi / 2).ToRotationVector2() * 5
                                                , SwordWind, 55/3, 2f, Main.myPlayer);
                                            SoundEngine.PlaySound(SoundID.Item71, NPC.position);
                                            interval++;
                                            Time2++;
                                        }
                                    }
                                    if (Time2 > 20)
                                    {
                                        Time1 = 0;
                                        Time2 = 0;
                                        SwitchState2(0);
                                        SwitchState1((int)GoldAndSilverDoubleSwordsAI.chase, (int)GoldAndSilverDoubleSwordsAI.VoidSpike + 1);
                                    }
                                    break;
                                }
                                
                        }
                        break;
                    }
                case GoldAndSilverDoubleSwordsAI.ShortSword:
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
                                        if (Time1 > 5)
                                        {
                                            Time1 = 0;
                                            SwitchState2(1);
                                        }
                                    }
                                    break;
                                }
                            case 1://发射弹幕
                                {
                                    Time1++;
                                    var player = Main.player[NPC.target];
                                    Vector2 ToPlayer = player.Center - NPC.Center;
                                    ToPlayer.Normalize();
                                    if (Time1 < 300)
                                    {
                                        TargetVel *= 7f;
                                        float accX = 0.25f;
                                        float accY = 0.25f;
                                        NPC.velocity.X += (NPC.velocity.X < TargetVel.X ? 1 : -1) * accX;
                                        NPC.velocity.Y += (NPC.velocity.Y < TargetVel.Y ? 1 : -1) * accY;
                                        Player p = Main.player[NPC.target];
                                            Time3++;
                                            if(Time3 > 15)
                                            {
                                                for (int h = -2; h <= 2; h++)
                                                {
                                                    Vector2 r = (ToPlayer.ToRotation() + (h * MathHelper.Pi / 6)).ToRotationVector2() * 22;
                                                    Projectile.NewProjectile(Source_NPC, NPC.position, r,
                                                    ShortSword, 55 / 3, 0f, Main.myPlayer);
                                                    interval++;
                                                    SoundEngine.PlaySound(SoundID.Item71, NPC.position);
                                                    Time3 = 0;
                                                }
                                                for (int h = -3; h <= 3; h++)
                                                {
                                                    Vector2 r = (ToPlayer.ToRotation() + (h * MathHelper.Pi / 6)).ToRotationVector2() * 22;
                                                    Projectile.NewProjectile(Source_NPC, NPC.position, r,
                                                    ShortSword, 55 / 3, 0f, Main.myPlayer);
                                                    interval++;
                                                    SoundEngine.PlaySound(SoundID.Item71, NPC.position);
                                                    Time3 = 0;
                                                }
                                            return;
                                            }
                                        Time2++;
                                    }
                                    if (Time2 > 15)
                                    {
                                        Time1 = 0;
                                        Time2 = 0;
                                        SwitchState2(0);
                                        SwitchState1((int)GoldAndSilverDoubleSwordsAI.chase, (int)GoldAndSilverDoubleSwordsAI.VoidSpike + 1);
                                    }
                                    break;
                                }
                        }
                        break;
                    }
                case GoldAndSilverDoubleSwordsAI.Spike:
                    {
                            switch (State2)
                            {
                            case 0://到达头顶
                                {
                                    Vector2 Head = new Vector2(Target.Center.X, Target.Center.Y - 500);
                                    float ToHead = Vector2.Distance(NPC.Center, Head);
                                    NPC.velocity = (NPC.velocity * 10 + (Head - NPC.Center)
                                        .SafeNormalize(Vector2.UnitX) * 15) / 11;
                                    if (ToHead < 10)
                                    {
                                        Time1++;
                                        if (Time1 > 2)
                                        {
                                            Time1 = 0;
                                            SwitchState2(1);
                                        }
                                    }
                                    break;
                                }
                            case 1://预判冲刺
                                {
                                    Time1++;
                                    Vector2 ToPlayer = (ToTarget * 1.5f) + Target.velocity;
                                    SoundEngine.PlaySound(SoundID.Item71, NPC.position);
                                    if (Time1 > 45)
                                    {
                                        NPC.velocity = ToPlayer;
                                        Time1 = 0;
                                        SwitchState2(2);
                                    }
                                    break;
                                }
                                case 2://保持速度
                                {
                                    Time1++;
                                    NPC.velocity *= 1f;
                                    if (Time1 > 60)
                                    {
                                        Time1 = 0;
                                        SwitchState2(0);
                                        SwitchState1((int)GoldAndSilverDoubleSwordsAI.chase, (int)GoldAndSilverDoubleSwordsAI.VoidSpike + 1);
                                    }
                                    break;
                                }
                        }
                        break;
                    }
                case GoldAndSilverDoubleSwordsAI.SwordArray:
                    {
                        Time1++;
                        NPC.velocity *= 0;
                        switch (Time1)
                        {
                            case 60:
                            case 120:
                                {
                                    if (Main.netMode != 1)
                                    {
                                        for (float r = 0; r < MathHelper.TwoPi; r += MathHelper.TwoPi / 18)
                                        {
                                            Vector2 center = Target.Center + (r.ToRotationVector2() * 500);
                                            var proj = Projectile.NewProjectileDirect(NPC.GetSpawnSourceForNPCFromNPCAI(),
                                                center, Vector2.Normalize(Target.Center - center) * 5, ShortSword,
                                                50 / 3, 2.3f, Main.myPlayer);
                                            SoundEngine.PlaySound(SoundID.Item71, NPC.position);
                                            proj.timeLeft = 1000;
                                            proj.tileCollide = false;
                                        }
                                    }
                                    break;
                                }
                            default:
                                {
                                    if (Time1 > 210)
                                    {
                                        Time1 = 0;
                                        SwitchState2(0);
                                        SwitchState1((int)GoldAndSilverDoubleSwordsAI.chase, (int)GoldAndSilverDoubleSwordsAI.VoidSpike + 1);
                                    }
                                    break;
                                }
                        }
                        break;
                    }
                case GoldAndSilverDoubleSwordsAI.Spike2:
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
                                    SoundEngine.PlaySound(SoundID.Item71, NPC.position);
                                    if (Time1 > 45)
                                    {
                                        Time2++;
                                        NPC.velocity = ToTarget * 1;
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
                                            SwitchState1((int)GoldAndSilverDoubleSwordsAI.chase, (int)GoldAndSilverDoubleSwordsAI.VoidSpike + 1);
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
                case GoldAndSilverDoubleSwordsAI.ShortSwordVolley:
                    {
                        switch(State2)
                        {
                            case 0:
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
                            case 1:
                                {
                                    Time1++;
                                    NPC.velocity *= 0;
                                    var player = Main.player[NPC.target];
                                    Vector2 ToPlayer = player.Center - NPC.Center;
                                    if (Time1 > 45)
                                    {
                                        Time2++;
                                        for (int i = 0; i < 100; i++)
                                        {
                                            Vector2 r = (ToPlayer.ToRotation() + (i * MathHelper.Pi / 18)).ToRotationVector2() * 20;
                                            Projectile.NewProjectile(Source_NPC, NPC.position, r,
                                            ShortSword, 45 / 3, 0f, Main.myPlayer);
                                            interval++;
                                            SoundEngine.PlaySound(SoundID.Item71, NPC.position);
                                        }
                                        Time1 = 0;
                                        SwitchState2(2);
                                    }
                                    break;
                                }
                            case 2:
                                {
                                    Time1++;
                                    if (Time1 > 30)
                                    {
                                        Time1 = 0;
                                        if (Time2 > 4)
                                        {
                                            SwitchState2(0);
                                            SwitchState1((int)GoldAndSilverDoubleSwordsAI.chase, (int)GoldAndSilverDoubleSwordsAI.VoidSpike + 1);
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
                case GoldAndSilverDoubleSwordsAI.VoidSpike:
                    {
                        switch(State2)
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
                                        if (Time1 > 5)
                                        {
                                            Time1 = 0;
                                            SwitchState2(1);
                                        }
                                    }
                                    break;
                                }
                            case 1:
                                {
                                    Time1++;
                                    if (Time1 == 25)
                                    {
                                        Vector2 center = Target.Center + Main.rand.NextFloat(MathHelper.TwoPi)
                                            .ToRotationVector2() * 350;
                                        Dust.NewDustDirect(center, 10, 10, MyDustId.YellowGoldenFire);
                                        NPC.position = center;
                                        NPC.rotation = ToTarget.ToRotation();
                                        NPC.velocity = NPC.rotation.ToRotationVector2();
                                        NPC.netUpdate = true;
                                    }
                                    if (Time1 > 25)
                                    {
                                        Time1++;
                                        Vector2 ToPlayer = (ToTarget * 1.5f) + Target.velocity;
                                        SoundEngine.PlaySound(SoundID.Item71, NPC.position);
                                        if (Time1 > 45)
                                        {
                                            NPC.velocity = ToPlayer;
                                            Time1 = 0;
                                            Time2 = 0;
                                            SwitchState2(0);
                                            SwitchState1((int)GoldAndSilverDoubleSwordsAI.chase, (int)GoldAndSilverDoubleSwordsAI.VoidSpike + 1);
                                        }
                                    }
                                    break;
                                }
                        }
                        break;
                    }
            }
            
        }
        private void DespawnHandler()
        {
            Player player = Main.player[NPC.target];
            NPC.TargetClosest(false);
            player = Main.player[NPC.target];
            if (!player.active || player.dead)
            {
                NPC.velocity.X = 0;
                NPC.velocity.Y -= 1;
            }
        }
    }
}
