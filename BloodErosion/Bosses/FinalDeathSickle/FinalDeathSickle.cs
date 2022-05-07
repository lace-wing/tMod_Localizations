using BloodSoul.NPCs;
using Microsoft.Xna.Framework;
using System;
using System.IO;
using Terraria;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.GameContent.ItemDropRules;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;

namespace BloodErosion.NPCs.Bosses.FinalDeathSickle
{
    [AutoloadBossHead]
    class FinalDeathSickle : FSMnpc
    {
        public EntitySource_ByProjectileSourceId Source_NPC;
        private Vector2 endPiont;
        private int frameTime = 0;
        private int interval = 0;
        private int TimeV = 0;
        private static float gravity = 0.3f;
        public new Vector2 PlayerOldPos = Vector2.Zero;
        private int leavl = 0;
        private int Time1 = 0;
        private int Time3 = 0;
        private int Time2 = 0;
        private int Time4 = 0;
        private enum FinalDeathSickleAI
        {
            S1,//开幕
            S2,//开局摸鱼
            S3,//死亡旋风
            S4,//摸鱼
            S5,//冲刺
            S6,//路径弹幕
            S7,//闪现
            S8,//散射
        }
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Final Death Sickle");
            DisplayName.AddTranslation((int)GameCulture.CultureName.Chinese, "最终死神镰");
        }
        public override void SetDefaults()
        {
            base.SetDefaults();
            NPC.lifeMax = 120000 / 3;
            NPC.defense = 55;
            NPC.damage = 315 / 3;
            NPC.boss = true;
            NPC.npcSlots = 1f;
            NPC.knockBackResist = 0f;
            NPC.width = 72;
            NPC.height = 84;
            NPC.value = 100000;
            NPC.lavaImmune = true;
            NPC.noGravity = true;
            NPC.noTileCollide = true;
            NPC.netAlways = true;
            NPC.HitSound = SoundID.NPCHit4;
            NPC.DeathSound = SoundID.NPCDeath10;
            NPC.buffImmune[BuffID.OnFire] = true;
            NPC.buffImmune[BuffID.OnFire3] = true;
            NPC.aiStyle = -1;
            NPC.scale = 1f;
            //BossBag = ModContent.ItemType<Item.Boss.SpiritOfSparks.PermanentCombustionSpark>();
            if (!Main.dedServ)
            {
                Music = MusicLoader.GetMusicSlot(Mod, "Sounds/Music/SupremeSickle");
            }
        }
        public override void ModifyNPCLoot(NPCLoot npcLoot)
        {
            npcLoot.Add(ItemDropRule.BossBag(BossBag));
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
                base.SendExtraAI(writer);
                writer.Write(leavl);
                writer.WriteVector2(endPiont);
                writer.Write(interval);
                writer.Write(State3);
                writer.Write(State4);
            }
        }
        public override void ReceiveExtraAI(BinaryReader reader)
        {
            if (Main.netMode != 1)
            {
                base.ReceiveExtraAI(reader);
                leavl = reader.ReadInt32();
                endPiont = reader.ReadVector2();
                interval = reader.ReadInt32();
                State3 = reader.ReadSingle();
                State4 = reader.ReadSingle();
            }
        }
        public override void AI()
        {
            NPC.TargetClosest();
            if (Main.player[NPC.target].dead || Math.Abs(NPC.position.X - Main.player[NPC.target].position.X) > 6000f || Math.Abs(NPC.position.Y - Main.player[NPC.target].position.Y) > 6000f)
            {
                NPC.TargetClosest(false);
                if (Main.player[NPC.target].dead || Math.Abs(NPC.position.X - Main.player[NPC.target].position.X) > 6000f || Math.Abs(NPC.position.Y - Main.player[NPC.target].position.Y) > 6000f)
                {
                    DespawnHandler();
                }
                return;
            }
            if (NPC.target < 0 || NPC.target == 255 || Main.player[NPC.target].dead || !Main.player[NPC.target].active) NPC.TargetClosest();
            TimeV++;
            Vector2 ToTarget = (Target.position - NPC.position).SafeNormalize(Vector2.UnitX) * 15;
            NPC.rotation = TimeV * 7;
            Vector2 TargetVel = Vector2.Normalize(Target.position - NPC.Center);
            float accX = 0.3f;
            float accY = 0.3f;
            //NPC.velocity.X += (NPC.velocity.X < TargetVel.X ? 1 : -1) * accX;
            //NPC.velocity.Y += (NPC.velocity.Y < TargetVel.Y ? 1 : -1) * accY;
            //NPC.velocity = TargetVel * 8f;
            if (State2 == 2)
            {
                State1 = 1;
                NPC.dontTakeDamage = false;
            }
            int DeathSwordWind = ModContent.ProjectileType<FinalDeathSwordWind>();
            int DeathSwordWind2 = ModContent.ProjectileType<FinalDeathSwordWind2>();
            switch ((FinalDeathSickleAI)State1)
            {
                case FinalDeathSickleAI.S1:
                    {
                        Time3++;
                        Vector2 Head = new Vector2(Target.Center.X, Target.Center.Y - 250);
                        float ToHead = Vector2.Distance(NPC.Center, Head);
                        NPC.velocity = (NPC.velocity * 10 + (Head - NPC.Center)
                            .SafeNormalize(Vector2.UnitX) * 15) / 11;
                        NPC.dontTakeDamage = true;
                        Color textColor = Color.Purple;
                        if (ToHead < 10)
                        {
                            Time1++;
                            if (Time1 > 2)
                            {
                                Time1 = 0;
                            }
                        }
                        switch (Time3)
                        {
                            case 150:
                                {
                                    Main.NewText(".....", Color.Purple);
                                    CombatText.NewText(NPC.Hitbox, textColor, "......", true, false);
                                    break;
                                }
                            case 350:
                                {
                                    Main.NewText("首先...恭喜你终于集齐了材料将吾召唤出来", Color.Purple);
                                    CombatText.NewText(NPC.Hitbox, textColor, "首先...恭喜你终于集齐了材料将吾召唤出来", true, false);
                                    break;
                                }
                            case 650:
                                {
                                    Main.NewText("言归正传,一直以来都是我们任你宰割", Color.Purple);
                                    CombatText.NewText(NPC.Hitbox, textColor, "言归正传,一直以来都是我们任你宰割", true, false);
                                    break;
                                }
                            case 950:
                                {
                                    Main.NewText("我乃终焉之镰", Color.Purple);
                                    CombatText.NewText(NPC.Hitbox, textColor, "我乃终焉之镰", true, false);
                                    break;
                                }
                            case 1250:
                                {
                                    Main.NewText("岂会任由汝宰割使用？", Color.Purple);
                                    CombatText.NewText(NPC.Hitbox, textColor, "岂会任由汝宰割使用？", true, false);
                                    break;
                                }
                            case 1550:
                                {
                                    Main.NewText("接下来，轮到我来宰割你了！", Color.Red);
                                    CombatText.NewText(NPC.Hitbox, Color.Red, "接下来，轮到我来宰割你了！", true, false);
                                    Time1 = 0;
                                    NPC.dontTakeDamage = false;
                                    SwitchState2(0);
                                    SwitchState1((int)FinalDeathSickleAI.S2, (int)FinalDeathSickleAI.S2 + 1);
                                    break;
                                }
                        }
                        break;
                    }
                case FinalDeathSickleAI.S2:
                    {
                        NPC.velocity.X += (NPC.velocity.X < TargetVel.X ? 1 : -1) * accX;
                        NPC.velocity.Y += (NPC.velocity.Y < TargetVel.Y ? 1 : -1) * accY;
                        NPC.velocity = TargetVel * 6f;
                        Time1--;
                        if (Time1 < 0)
                        {
                            Time1 = 60;
                            Vector2 ToPlayer = (ToTarget * 1.1f) + Target.velocity;
                            Projectile.NewProjectile(NPC.GetSpawnSourceForNPCFromNPCAI(), NPC.Center, ToPlayer * 2, DeathSwordWind, 45, 2f, Main.myPlayer);
                            Time2++;
                            if (Time2 > 10)
                            {
                                Time1 = 0;
                                Time2 = 0;
                                SwitchState2(0);
                                SwitchState1((int)FinalDeathSickleAI.S3, (int)FinalDeathSickleAI.S3 + 1);
                            }
                        }
                        break;
                    }
                case FinalDeathSickleAI.S3:
                    {
                        Time1++;
                        if (Time1 < 5)
                        {
                            endPiont = Target.Center;
                            NPC.velocity *= 0.6f;
                        }
                        else
                        {
                            NPC.rotation += Main.GlobalTimeWrappedHourly / 5;
                            Vector2 ves = (endPiont + (Main.GlobalTimeWrappedHourly * 1.1f).ToRotationVector2() * 600) - NPC.position;
                            float speed = ves.Length() > 500 ? 500 : ves.Length();
                            NPC.velocity = ves.SafeNormalize(Vector2.Zero) * speed / 5;
                            if (Vector2.Distance(Target.Center, endPiont) > 600)
                            {
                                Target.velocity = (endPiont - Target.Center) / 100;
                            }
                            for (float i = 0; i <= MathHelper.TwoPi; i += MathHelper.Pi / 50)
                            {
                                Vector2 center = endPiont + (i.ToRotationVector2() * 600);
                                if (Main.netMode != 1)
                                {
                                    int dust = Dust.NewDust(center, 1, 1, DustID.PurpleTorch);
                                    Main.dust[dust].noGravity = true;
                                }
                            }
                            if (Time1 % 5 == 0 && Main.netMode != 1)
                            {
                                var player = Main.player[NPC.target];
                                Vector2 ToPlayer = player.Center - NPC.Center;
                                ToPlayer.Normalize();
                                for (int h = -2; h <= 2; h++)
                                {
                                    Vector2 r = (ToPlayer.ToRotation() + (h * MathHelper.Pi / 2)).ToRotationVector2() * 4;
                                    int proj = Projectile.NewProjectile(Source_NPC, NPC.Center, (endPiont - NPC.Center).SafeNormalize(Vector2.Zero) * 8 + r, DeathSwordWind, 45, 1.2f, Main.myPlayer);
                                    Main.projectile[proj].extraUpdates = 2;
                                    Main.projectile[proj].alpha = 200;
                                }
                            }
                            if (Time1 > 800)
                            {
                                Time1 = 0;
                                SwitchState2(0);
                                SwitchState1((int)FinalDeathSickleAI.S4, (int)FinalDeathSickleAI.S4 + 1);
                            }
                        }
                        break;
                    }
                case FinalDeathSickleAI.S4:
                    {
                        NPC.velocity.X += (NPC.velocity.X < TargetVel.X ? 1 : -1) * accX;
                        NPC.velocity.Y += (NPC.velocity.Y < TargetVel.Y ? 1 : -1) * accY;
                        NPC.velocity = TargetVel * 6f;
                        Time1--;
                        if (Time1 < 0)
                        {
                            Time1 = 60;
                            Vector2 ToPlayer = (ToTarget * 1.1f) + Target.velocity;
                            Projectile.NewProjectile(NPC.GetSpawnSourceForNPCFromNPCAI(), NPC.Center, ToPlayer * 2, DeathSwordWind, 45, 2f, Main.myPlayer);
                            Time2++;
                            if (Time2 > 10)
                            {
                                Time1 = 0;
                                Time2 = 0;
                                SwitchState2(0);
                                SwitchState1((int)FinalDeathSickleAI.S5, (int)FinalDeathSickleAI.S8 + 1);
                            }
                        }
                        break;
                    }
                case FinalDeathSickleAI.S5:
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
                                        Vector2 ToPlayer = (ToTarget * 1.1f) + Target.velocity;
                                        NPC.velocity = ToPlayer * 1.1f;
                                        Time1 = 0;
                                        SwitchState2(1);
                                    }
                                    break;
                                }
                            case 1:
                                {
                                    Time1++;

                                    if (Time1 > 45)
                                    {
                                        Time1 = 0;
                                        Time2++;
                                        if (Time2 > 8)
                                        {
                                            NPC.velocity *= 1f;
                                            var player = Main.player[NPC.target];
                                            Vector2 ToPlayer = player.Center - NPC.Center;
                                            for (int i = 0; i < 8; i++)
                                            {
                                                Vector2 r = (ToPlayer.ToRotation() + (float)Math.Sin(Main.time) * 0.5f + (i * MathHelper.Pi / 4)).ToRotationVector2() * 9;
                                                Projectile.NewProjectile(Source_NPC, NPC.Center, r * 1.2f, DeathSwordWind, 45, 0f, Main.myPlayer);
                                                interval++;
                                            }
                                            //SwitchState2(1);
                                            Time2 = 0;
                                            SwitchState2(0);
                                            SwitchState1((int)FinalDeathSickleAI.S4, (int)FinalDeathSickleAI.S8 + 1);
                                        }
                                        else
                                        {
                                            NPC.velocity *= 1f;
                                            var player = Main.player[NPC.target];
                                            Vector2 ToPlayer = player.Center - NPC.Center;
                                            for (int i = 0; i < 8; i++)
                                            {
                                                Vector2 r = (ToPlayer.ToRotation() + (float)Math.Sin(Main.time) * 0.5f + (i * MathHelper.Pi / 4)).ToRotationVector2() * 9;
                                                Projectile.NewProjectile(Source_NPC, NPC.Center, r * 1.2f,
                                                DeathSwordWind, 45, 0f, Main.myPlayer);
                                                interval++;
                                            }
                                            SwitchState2(0);
                                        }
                                    }
                                    break;
                                }
                        }
                        break;
                    }
                case FinalDeathSickleAI.S6:
                    {
                        switch (State2)
                        {
                            case 0://冲刺
                                {
                                    Time1++;
                                    Time4++;
                                    if (Time1 > 45)
                                    {
                                        Time2++;
                                        SoundEngine.PlaySound(SoundID.Item71, NPC.position);
                                        Vector2 ToPlayer = (ToTarget * 1.1f) + Target.velocity;
                                        NPC.velocity = ToPlayer * 1.1f;
                                        Time1 = 0;
                                        SwitchState2(1);
                                    }
                                    if (Time4 > 2)
                                    {
                                        var player = Main.player[NPC.target];
                                        Vector2 ToPlayer = player.Center - NPC.Center;
                                        ToPlayer.Normalize();
                                        for (int h = 1; h <= 1; h++)
                                        {
                                            Vector2 r = (ToPlayer.ToRotation() + (h * MathHelper.Pi / 2)).ToRotationVector2() * 4;
                                            int proj = Projectile.NewProjectile(Source_NPC, NPC.Center, (endPiont - NPC.Center).SafeNormalize(Vector2.Zero) * 8 + r, DeathSwordWind2, 45, 1.2f, Main.myPlayer);
                                            Main.projectile[proj].extraUpdates = 2;
                                            Main.projectile[proj].alpha = 200;
                                        }
                                        Time4 = 0;
                                    }
                                    break;
                                }
                            case 1:
                                {
                                    Time1++;

                                    if (Time1 > 45)
                                    {
                                        Time1 = 0;
                                        Time2++;
                                        if (Time2 > 8)
                                        {
                                            NPC.velocity *= 1f;
                                            Time2 = 0;
                                            SwitchState2(0);
                                            SwitchState1((int)FinalDeathSickleAI.S4, (int)FinalDeathSickleAI.S8 + 1);
                                        }
                                        else
                                        {
                                            NPC.velocity *= 1f;
                                            SwitchState2(0);
                                        }
                                    }
                                    Time3++;
                                    if (Time3 > 2)
                                    {
                                        var player = Main.player[NPC.target];
                                        Vector2 ToPlayer = player.Center - NPC.Center;
                                        ToPlayer.Normalize();
                                        for (int h = 1; h <= 1; h++)
                                        {
                                            Vector2 r = (ToPlayer.ToRotation() + (h * MathHelper.Pi / 2)).ToRotationVector2() * 4;
                                            int proj = Projectile.NewProjectile(Source_NPC, NPC.Center, (endPiont - NPC.Center).SafeNormalize(Vector2.Zero) * 8 + r, DeathSwordWind2, 45, 1.2f, Main.myPlayer);
                                            Main.projectile[proj].extraUpdates = 2;
                                            Main.projectile[proj].alpha = 200;
                                        }
                                        Time3 = 0;
                                    }
                                    break;
                                }
                        }
                        break;
                    }
                case FinalDeathSickleAI.S7:
                    {
                        Time1++;
                        if (Time1 == 25)
                        {
                            Vector2 center = Target.Center + Main.rand.NextFloat(MathHelper.TwoPi)
                                .ToRotationVector2() * 650;
                            Dust.NewDustDirect(center, 10, 10, DustID.DemonTorch);
                            NPC.position = center;
                            NPC.rotation = ToTarget.ToRotation();
                            NPC.netUpdate = true;
                        }
                        if (Time1 > 25)
                        {
                            Time1++;
                            Vector2 ToPlayer = (ToTarget * 0.8f);
                            for (int i = 0; i < 2; i++)
                            {
                                Vector2 r = (ToPlayer.ToRotation() + (i * MathHelper.Pi / 3)).ToRotationVector2() * 9;
                                Projectile.NewProjectile(Source_NPC, NPC.Center, r / 2, DeathSwordWind, 45, 0f, Main.myPlayer);
                                interval++;
                            }
                            SoundEngine.PlaySound(SoundID.Item71, NPC.position);
                            if (Time1 > 25)
                            {
                                Vector2 center = Target.Center + Main.rand.NextFloat(MathHelper.TwoPi)
                                .ToRotationVector2() * 650;
                                Dust.NewDustDirect(center, 10, 10, DustID.DemonTorch);
                                NPC.position = center;
                                NPC.rotation = ToTarget.ToRotation();
                                NPC.netUpdate = true;
                                for (int i = 0; i < 2; i++)
                                {
                                    Vector2 r = (ToPlayer.ToRotation() + (i * MathHelper.Pi / 3)).ToRotationVector2() * 9;
                                    Projectile.NewProjectile(Source_NPC, NPC.Center, r / 2, DeathSwordWind, 45, 0f, Main.myPlayer);
                                    interval++;
                                }
                            }
                            if (Time1 > 90)
                            {
                                Time1 = 0;
                                Time2 = 0;
                                SwitchState2(0);
                                SwitchState1((int)FinalDeathSickleAI.S4, (int)FinalDeathSickleAI.S8 + 1);
                            }
                        }
                        break;
                    }
                case FinalDeathSickleAI.S8:
                    {
                        Time1++;
                        if (Time1 == 25)
                        {
                            Vector2 center = Target.Center + Main.rand.NextFloat(MathHelper.TwoPi)
                                .ToRotationVector2() * 350;
                            Dust.NewDustDirect(center, 10, 10, DustID.DemonTorch);
                            NPC.position = center;
                            NPC.rotation = ToTarget.ToRotation();
                            NPC.velocity = NPC.rotation.ToRotationVector2();
                            NPC.netUpdate = true;
                        }
                        if (Time1 > 25)
                        {
                            Time1++;
                            Vector2 ToPlayer = (ToTarget * 0.8f);
                            for (int i = 0; i < 3; i++)
                            {
                                Vector2 r = (ToPlayer.ToRotation() + (i * MathHelper.Pi / 36)).ToRotationVector2() * 13;
                                Projectile.NewProjectile(Source_NPC, NPC.Center, r,
                                DeathSwordWind, 45, 0f, Main.myPlayer);
                                interval++;
                            }
                            SoundEngine.PlaySound(SoundID.Item71, NPC.position);
                            if (Time1 > 45)
                            {
                                NPC.velocity = ToPlayer;
                                Time1 = 0;
                                Time2 = 0;
                                SwitchState2(0);
                                SwitchState1((int)FinalDeathSickleAI.S4, (int)FinalDeathSickleAI.S7 + 1);
                            }
                        }
                        break;
                    }
            }
        }
        public override bool CheckDead()
        {
            NPC.active = false;
            for (int i = 0; i < 3; i++)
            {
                SoundEngine.PlaySound(SoundID.Item62, NPC.position);
            }
            Projectile.NewProjectile(projectileSource, NPC.Center.X, NPC.Center.Y, 0, 0, ModContent.ProjectileType<DeathBoom>(), 0, 0, 0);
            Main.NewText("终焉三镰,助我！", Color.Red);
            CombatText.NewText(NPC.Hitbox, Color.Red, "终焉三镰,助我！", true, false);
            NPC.NewNPC(NPC.GetSpawnSourceForNPCFromNPCAI(), (int)(NPC.Center.X), (int)(NPC.Center.Y), ModContent.NPCType<FinalFlameSickle>(), NPC.whoAmI);
            NPC.NewNPC(NPC.GetSpawnSourceForNPCFromNPCAI(), (int)(NPC.Center.X), (int)(NPC.Center.Y), ModContent.NPCType<FinalAwakeningSickle>(), NPC.whoAmI);
            NPC.NewNPC(NPC.GetSpawnSourceForNPCFromNPCAI(), (int)(NPC.Center.X), (int)(NPC.Center.Y), ModContent.NPCType<FinalFrostSickle>(), NPC.whoAmI);
            return base.CheckDead();
        }
        public EntitySource_ByProjectileSourceId projectileSource;
        public override void OnHitPlayer(Player Target, int damage, bool crit)
        {
            Target.AddBuff(BuffID.ShadowFlame, 180);
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
