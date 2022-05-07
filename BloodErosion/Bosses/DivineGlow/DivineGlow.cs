using BloodErosion.Items.Boss.DivineGlows;
using BloodErosion.Items.MasterTrophy;
using BloodErosion.NPCs.Bosses.SpiritOfSpark;
using BloodSoul.MyUtils;
using BloodSoul.NPCs;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.IO;
using Terraria;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.GameContent.Bestiary;
using Terraria.GameContent.ItemDropRules;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;

namespace BloodErosion.NPCs.Bosses.DivineGlow
{
    [AutoloadBossHead]
    class DivineGlow : FSMnpc
    {
        float r = 0;
        public override void UpdateLifeRegen(ref int damage)
        {
            r += 0.01f;
        }
        private int leavl = 0;
        public EntitySource_ByProjectileSourceId Source_NPC;
        private Vector2 endPiont;
        private int frameTime = 0;
        private int interval = 0;
        private static float gravity = 0.3f;
        public new Vector2 PlayerOldPos = Vector2.Zero;
        private int Time1 = 0;
        private int Time2 = 0;
        private int Time3 = 0;
        private int Time4 = 0;
        private enum DivineGlowAI
        {
            start,//开场
            chase,//追逐
            VoidSpike,//虚空突刺
            LightSprint,//圣光连冲
            PathLight,//路径圣光???
            PathLight2,//路光
            RL//旋光
        }
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("DivineGlow");
            DisplayName.AddTranslation((int)GameCulture.CultureName.Chinese, "神圣之辉");
        }
        public override void SetDefaults()
        {
            base.SetDefaults();
            NPC.lifeMax = 23000;
            NPC.defense = 15;
            NPC.damage = 125 / 3;
            NPC.boss = true;
            NPC.npcSlots = 1f;
            NPC.knockBackResist = 0f;
            NPC.width = 72;
            NPC.height = 72;
            NPC.value = 30000;
            NPC.lavaImmune = true;
            NPC.noGravity = true;
            NPC.noTileCollide = true;
            NPC.netAlways = true;
            NPC.HitSound = SoundID.NPCHit5;
            NPC.DeathSound = SoundID.NPCDeath7;
            NPC.buffImmune[BuffID.OnFire] = true;
            NPC.buffImmune[BuffID.OnFire3] = true;
            NPC.aiStyle = -1;
            NPC.scale = 1f;
            NPC.alpha = 255;

            BossBag = ModContent.ItemType<HolyLightFragment>();
            if (!Main.dedServ)
            {
                Music = MusicLoader.GetMusicSlot(Mod, "Sounds/Music/Light");
            }
        }
        public override void SetBestiary(BestiaryDatabase database, BestiaryEntry bestiaryEntry)
        {
            bestiaryEntry.Info.AddRange(new IBestiaryInfoElement[] {
                BestiaryDatabaseNPCsPopulator.CommonTags.SpawnConditions.Biomes.Surface,
                BestiaryDatabaseNPCsPopulator.CommonTags.SpawnConditions.Times.DayTime,
                new FlavorTextBestiaryInfoElement("某位古神的微弱力量碎片，小到那位古神甚至没有注意到它，但它对你仍有巨大的威胁")
            });
        }
        public override void ModifyNPCLoot(NPCLoot npcLoot)
        {
            npcLoot.Add(ItemDropRule.BossBag(BossBag));
            npcLoot.Add(ItemDropRule.MasterModeCommonDrop(ModContent.ItemType<DivineGlowRelicItem>()));
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
            Vector2 toTarget = Target.position - NPC.position;
            Vector2 ToTarget = (Target.position - NPC.position).SafeNormalize(Vector2.UnitX) * 15;
            NPC.rotation = ToTarget.ToRotation() + MathHelper.Pi / 2;
            for (float i = 0; i <= MathHelper.TwoPi; i += MathHelper.Pi / 50)
            {
                Vector2 center = NPC.Center + (i.ToRotationVector2() * 1200);
                int dust = Dust.NewDust(center, 1, 1, DustID.Gold);
                Main.dust[dust].velocity *= 0;
                Main.dust[dust].noGravity = true;
            }
            if (NPC.position.Distance(Target.position) > 1200)
            {
                Target.velocity = -toTarget.SafeNormalize(toTarget) * 3;
            }
            Vector2 TargetVel = Vector2.Normalize(Target.position - NPC.Center);
            float X = NPC.Center.X - Target.Center.X;
            int Cen = (X > 0) ? 1 : -1;
            if (NPC.life < NPC.lifeMax * 0.7f && leavl == 0)
            {
                leavl = 1;
                Time1 = 0;
                Time2 = 0;
                Main.NewText("圣斧...圣锤...处决他！", Color.Gold);
                CombatText.NewText(NPC.Hitbox, Color.Gold, "圣斧...圣锤...处决他！", true, false);
                NPC.NewNPC(NPC.GetSpawnSourceForNPCFromNPCAI(),(int)(NPC.Center.X), (int)(NPC.Center.Y), ModContent.NPCType<AwakeningHolyHammer>(), NPC.whoAmI);
                NPC.NewNPC(NPC.GetSpawnSourceForNPCFromNPCAI(),(int)(NPC.Center.X), (int)(NPC.Center.Y), ModContent.NPCType<AwakeningHolyPickaxe>(), NPC.whoAmI);
            }
            if (NPC.life < NPC.lifeMax * 0.5f && leavl == 1)
            {
                leavl = 2;
                Time1 = 0;
                Time2 = 0;
                Main.NewText("圣枪！圣钻！拦下他！", Color.Gold);
                CombatText.NewText(NPC.Hitbox, Color.Gold, "圣枪！圣钻！拦下他！", true, false);
                NPC.NewNPC(NPC.GetSpawnSourceForNPCFromNPCAI(),(int)(NPC.Center.X), (int)(NPC.Center.Y), ModContent.NPCType<AwakeningHolyDrill>(), NPC.whoAmI);
                NPC.NewNPC(NPC.GetSpawnSourceForNPCFromNPCAI(),(int)(NPC.Center.X), (int)(NPC.Center.Y), ModContent.NPCType<AwakeningEternalHolyLance>(), NPC.whoAmI);
            }
            if (NPC.life < NPC.lifeMax * 0.3f && leavl == 2)
            {
                leavl = 3;
                Time1 = 0;
                Time2 = 0;
                Main.NewText("圣剑！圣弩！助吾！", Color.Gold);
                CombatText.NewText(NPC.Hitbox, Color.Gold, "圣剑！圣弩！助吾！", true, false);
                NPC.NewNPC(NPC.GetSpawnSourceForNPCFromNPCAI(),(int)(NPC.Center.X), (int)(NPC.Center.Y), ModContent.NPCType<AwakeningHolySword>(), NPC.whoAmI);
                NPC.NewNPC(NPC.GetSpawnSourceForNPCFromNPCAI(),(int)(NPC.Center.X), (int)(NPC.Center.Y), ModContent.NPCType<AwakeningHolyCrossbow>(), NPC.whoAmI);
            }

            int L = ModContent.ProjectileType<Light>();


            TargetVel *= 7f;
            float accX = 0.21f;
            float accY = 0.21f;
            NPC.velocity.X += (NPC.velocity.X < TargetVel.X ? 1 : -1) * accX;
            NPC.velocity.Y += (NPC.velocity.Y < TargetVel.Y ? 1 : -1) * accY;

            switch ((DivineGlowAI)State1)
            {
                case DivineGlowAI.start:
                    {
                        Time3++;
                        Vector2 Head = new Vector2(Target.Center.X, Target.Center.Y - 250);
                        float ToHead = Vector2.Distance(NPC.Center, Head);
                        NPC.velocity = (NPC.velocity * 10 + (Head - NPC.Center)
                            .SafeNormalize(Vector2.UnitX) * 15) / 11;
                        NPC.dontTakeDamage = true;
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
                                    Main.NewText("你！卑劣的邪恶之子", Color.Gold);
                                    CombatText.NewText(NPC.Hitbox, Color.Gold, "你！卑劣的邪恶之子", true, false);
                                    break;
                                }
                            case 350:
                                {
                                    Main.NewText("接受审判吧！", Color.Gold);
                                    CombatText.NewText(NPC.Hitbox, Color.Gold, "接受审判吧！", true, false);
                                    Time1 = 0;
                                    NPC.dontTakeDamage = false;
                                    SwitchState2(0);
                                    SwitchState1((int)DivineGlowAI.chase, (int)DivineGlowAI.chase + 1);
                                    break;
                                }
                        }
                        break;
                    }
                case DivineGlowAI.chase:
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
                                                        , L, 130 / 6, 2f, Main.myPlayer);
                                        SoundEngine.PlaySound(SoundID.Item15, NPC.position);
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
                            SwitchState1((int)DivineGlowAI.chase, (int)DivineGlowAI.RL + 1);

                        }
                        break;
                    }
                case DivineGlowAI.VoidSpike:
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
                                        SoundEngine.PlaySound(SoundID.Item15, NPC.position);
                                        if (Time1 > 200)
                                        {
                                            NPC.velocity = ToPlayer;
                                            Time1 = 0;
                                            Time2 = 0;
                                            SwitchState2(0);
                                            SwitchState1((int)DivineGlowAI.chase, (int)DivineGlowAI.RL + 1);
                                        }
                                    }
                                    break;
                                }
                        }
                        break;
                    }
                case DivineGlowAI.LightSprint:
                    {
                        switch (State2)
                        {
                            case 0://冲刺
                                {
                                    Time1++;
                                    if (Time1 > 45)
                                    {
                                        Time2++;
                                        SoundEngine.PlaySound(SoundID.Item15, NPC.position);
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
                                                Vector2 r = (ToPlayer.ToRotation() + (float)Math.Sin(Main.time) * 0.5f + (i * MathHelper.Pi / 4)).ToRotationVector2() * 16;
                                                Projectile.NewProjectile(Source_NPC, NPC.Center, r * 1.2f, L, 35, 0f, Main.myPlayer);
                                                interval++;
                                            }
                                            SwitchState2(0);
                                            SwitchState1((int)DivineGlowAI.chase, (int)DivineGlowAI.RL + 1);
                                            Time2 = 0;
                                        }
                                        else
                                        {
                                            NPC.velocity *= 1f;
                                            var player = Main.player[NPC.target];
                                            Vector2 ToPlayer = player.Center - NPC.Center;
                                            for (int i = 0; i < 8; i++)
                                            {
                                                Vector2 r = (ToPlayer.ToRotation() + (float)Math.Sin(Main.time) * 0.5f + (i * MathHelper.Pi / 4)).ToRotationVector2() * 16;
                                                Projectile.NewProjectile(Source_NPC, NPC.Center, r * 1.2f,
                                                L, 135 / 6, 0f, Main.myPlayer);
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
                case DivineGlowAI.PathLight:
                    {
                        switch (State2)
                        {
                            case 0:
                                {
                                    Time1++;
                                    if (Time1 == 25)
                                    {
                                        Vector2 center = Target.Center + Main.rand.NextFloat(MathHelper.TwoPi)
                                            .ToRotationVector2() * 900;
                                        Dust.NewDustDirect(center, 10, 10, MyDustId.DemonTorch);
                                        NPC.position = center;
                                        NPC.rotation = ToTarget.ToRotation();
                                        NPC.netUpdate = true;
                                    }
                                    if (Time1 > 25)
                                    {
                                        Time1++;
                                        Vector2 ToPlayer = (ToTarget * 0.8f);
                                        SoundEngine.PlaySound(SoundID.Item15, NPC.position);
                                        if (Time1 > 25)
                                        {
                                            Vector2 center = Target.Center + Main.rand.NextFloat(MathHelper.TwoPi)
                                            .ToRotationVector2() * 650;
                                            Dust.NewDustDirect(center, 10, 10, MyDustId.DemonTorch);
                                            NPC.position = center;
                                            NPC.rotation = ToTarget.ToRotation();
                                            NPC.netUpdate = true;
                                            for (int i = 0; i < 1; i++)
                                            {
                                                Vector2 r = (ToPlayer.ToRotation() + (i * MathHelper.Pi / 3)).ToRotationVector2() * 9;
                                                Projectile.NewProjectile(Source_NPC, NPC.Center, r / 2, L, 125 / 6, 0f, Main.myPlayer);
                                                interval++;
                                            }
                                        }
                                        if (Time1 > 90)
                                        {
                                            Time1 = 0;
                                            Time2 = 0;
                                            SwitchState2(0);
                                            SwitchState1((int)DivineGlowAI.chase, (int)DivineGlowAI.RL + 1);
                                        }
                                    }
                                    break;
                                }
                        }
                        break;
                    }
                case DivineGlowAI.PathLight2:
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
                                        SoundEngine.PlaySound(SoundID.Item15, NPC.position);
                                        Vector2 ToPlayer = (ToTarget * 1.1f) + Target.velocity;
                                        NPC.velocity = ToPlayer * 1.1f;
                                        Time1 = 0;
                                        SwitchState2(1);
                                    }
                                    if (Time4 > 3)
                                    {
                                        var player = Main.player[NPC.target];
                                        Vector2 ToPlayer = player.Center - NPC.Center;
                                        ToPlayer.Normalize();
                                        for (int h = 1; h <= 1; h++)
                                        {
                                            Vector2 r = (ToPlayer.ToRotation() + (h * MathHelper.Pi / 2)).ToRotationVector2() * 4;
                                            int proj = Projectile.NewProjectile(Source_NPC, NPC.Center, (endPiont - NPC.Center).SafeNormalize(Vector2.Zero) * 8 + r, L, 125 / 6, 1.2f, Main.myPlayer);
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
                                            SwitchState1((int)DivineGlowAI.chase, (int)DivineGlowAI.RL + 1);
                                        }
                                        else
                                        {
                                            NPC.velocity *= 1f;
                                            SwitchState2(0);
                                        }
                                    }
                                    Time3++;
                                    if (Time3 > 3)
                                    {
                                        var player = Main.player[NPC.target];
                                        Vector2 ToPlayer = player.Center - NPC.Center;
                                        ToPlayer.Normalize();
                                        for (int h = 1; h <= 1; h++)
                                        {
                                            Vector2 r = (ToPlayer.ToRotation() + (h * MathHelper.Pi / 2)).ToRotationVector2() * 4;
                                            int proj = Projectile.NewProjectile(Source_NPC, NPC.Center, (endPiont - NPC.Center).SafeNormalize(Vector2.Zero) * 8 + r, L, 125 / 6, 1.2f, Main.myPlayer);
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
                case DivineGlowAI.RL:
                    {
                        switch (State2)
                        {
                            case 0:
                                {
                                    Time1++;
                                    Vector2 Head = new Vector2(Target.Center.X, Target.Center.Y - 250);
                                    float ToHead = Vector2.Distance(NPC.Center, Head);
                                    NPC.velocity = (NPC.velocity * 10 + (Head - NPC.Center)
                                        .SafeNormalize(Vector2.UnitX) * 15) / 11;
                                    if (ToHead < 10)
                                    {
                                        Time1++;
                                        if (Time1 > 2)
                                        {
                                            Time1 = 0;
                                        }
                                    }
                                    var player = Main.player[NPC.target];
                                    Vector2 ToPlayer = player.Center - NPC.Center;
                                    if (Time1 > 45)
                                    {
                                        Time2++;
                                        for (int i = 0; i < 32; i++)
                                        {
                                            Vector2 r = (ToPlayer.ToRotation() + (i * MathHelper.Pi / 18)).ToRotationVector2() * 20;
                                            Projectile.NewProjectile(Source_NPC, NPC.position, r,
                                            L, 130 / 6, 0f, Main.myPlayer);
                                            interval++;
                                            SoundEngine.PlaySound(SoundID.Item15, NPC.position);
                                        }
                                        Time1 = 0;
                                        SwitchState2(1);
                                    }
                                    break;
                                }
                            case 1:
                                {
                                    Time1++;
                                    if (Time1 > 30)
                                    {
                                        Time1 = 0;
                                        if (Time2 > 4)
                                        {
                                            SwitchState2(0);
                                            SwitchState1((int)DivineGlowAI.chase, (int)DivineGlowAI.RL + 1);
                                        }
                                        else
                                        {
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
        public override bool CheckDead()
        {
            foreach (NPC n in Main.npc)
            {
                if (n.type == ModContent.NPCType<AwakeningHolySword>() || n.type == ModContent.NPCType<AwakeningHolyHammer>() || n.type == ModContent.NPCType<AwakeningHolyCrossbow>() || n.type == ModContent.NPCType<AwakeningHolyDrill>() || n.type == ModContent.NPCType<AwakeningEternalHolyLance>() || n.type == ModContent.NPCType<AwakeningHolyPickaxe>() && n.active)
                {
                    n.active = false;
                    n.netUpdate = true;
                }
            }
            return base.CheckDead();
        }
        public override void OnKill()
        {
            {
                var player = Main.player[NPC.target];
                Vector2 ToPlayer = player.Center - NPC.Center;
                for (int i = 0; i < 16; i++)
                {
                    Vector2 r = (ToPlayer.ToRotation() + (i * MathHelper.Pi / 8)).ToRotationVector2() * 13;
                    Projectile.NewProjectile(Source_NPC, NPC.position, r,
                    ModContent.ProjectileType<Light>(), 100 / 6, 0f, Main.myPlayer);
                    interval++;
                }
            }
            base.OnKill();
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
        public override bool PreDraw(SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor)
        {
            Texture2D texture2 = BloodSoulUtils.GetTexture("Images/Ray").Value;
            Vector2 drawOrigin2;
            drawOrigin2 = new Vector2(texture2.Width * 0.5f, texture2.Height * 0.5f);
            Main.spriteBatch.Draw(texture2, NPC.Center - Main.screenPosition, null, new Color(255, 255, 170, 0), r, drawOrigin2, new Vector2(1.7f, 1.7f), SpriteEffects.None, 0);

            Texture2D texture3 = BloodSoulUtils.GetTexture("Images/Start").Value;
            Vector2 drawOrigin3;
            drawOrigin3 = new Vector2(texture3.Width * 0.5f, texture3.Height * 0.5f);
            Main.spriteBatch.Draw(texture3, NPC.Center - Main.screenPosition, null, new Color(255, 255, 170, 0), -r, drawOrigin3, new Vector2(0.6f, 0.6f), SpriteEffects.None, 0);

            return true;
        }
    }
}
