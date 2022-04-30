using BloodSoul.Items.BossBag;
using BloodSoul.MyUtils;
using BloodSoul.Projectiles.Bosses;
using BloodSoul.Projectiles.Bosses.Bubble;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using Terraria;
using Terraria.Audio;
using Terraria.GameContent.ItemDropRules;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;

namespace BloodSoul.NPCs.Bosses.Tidespirit
{
    [AutoloadBossHead]
    public class Tideboss : FSMnpc
    {
        public float velx;
        public Vector2 rush;
        public int rain = 0;
        public Vector2 ra;
        public Vector2 posi1;
        public Vector2 posi;
        public int random = 0;
        public Vector2 pos;
        public override void SetStaticDefaults()
        {
            Main.npcFrameCount[NPC.type] = 3;
            DisplayName.SetDefault("Spirit of tides");
            DisplayName.AddTranslation((int)GameCulture.CultureName.Chinese, "潮涌之灵");
        }
        public override void SetDefaults()
        {
            NPC.lifeMax = 50000 / 3;
            NPC.damage = 165 / 3;
            NPC.defense = 32;
            NPC.friendly = false;
            NPC.noGravity = true;
            NPC.noTileCollide = true;
            NPC.width = 100;
            NPC.height = 200;
            NPC.aiStyle = -1;
            NPC.boss = true;
            NPC.knockBackResist = 0f;
            NPC.value = Item.buyPrice(0, 1, 12, 0);
            NPC.lavaImmune = true;
            NPC.buffImmune[BuffID.OnFire] = true;
            NPC.buffImmune[BuffID.OnFire3] = true;
            NPC.HitSound = SoundID.NPCDeath9;
            NPC.DeathSound = SoundID.NPCDeath19;
            BossBag = ModContent.ItemType<TideSpiritBossBag>();
            if (!Main.dedServ)
            {
                Music = MusicLoader.GetMusicSlot(Mod, "Sounds/Music/OverpowerTide");
            }
        }
        public override void ModifyNPCLoot(NPCLoot npcLoot)
        {
            npcLoot.Add(ItemDropRule.BossBag(BossBag));
        }
        public override void FindFrame(int frameHeight)
        {
            NPC.frameCounter++;
            switch (NPC.frameCounter)
            {
                case 5:
                    {
                        NPC.frame.Y = 0;
                        break;
                    }
                case 10:
                    {
                        NPC.frame.Y = frameHeight;
                        break;
                    }
                case 15:
                    {
                        NPC.frame.Y = frameHeight * 2;
                        break;
                    }

            }
            if (NPC.frameCounter == 16) { NPC.frameCounter = 0; }
        }
        private enum Tidebossai
        {
            Start,//开场AI（自机据
            phs1,//潮涌长枪（自机据不说了
            phs2,//三向球（这是我下手最毒的一个AI，但也是有解的
            phs3,//晶核(解法是活在裆下
            phs4,//小预判冲刺，克盾外解法目前只有钩爪史莱姆
            phs5,//切换2阶段
            phs6,//长枪和巨剑之雨
            phs7,//预判天降（宇宙英灵诉讼
            phs8,//万剑狂潮（如巨浪涌来般的巨剑
            End1,//尾杀
            End2,//死亡动画
        }
        public int Air = 0;
        public int Air2 = 0;
        public override void AI()
        {
            if (NPC.target <= 0 || NPC.target == 255 || !Target.active || Target.dead)//寻找npc的目标
            {
                NPC.TargetClosest();
            }
            if (Target.dead) { NPC.active = false; }

            Main.raining = true;
            Vector2 TP = Target.Center - NPC.Center;
            Target.breathCD = 900;
            Target.gills = false;
            //Target.breathMax = 90;
            Air++;
            Air2++;

            if (Air2 >= 120)
            {
                Projectile.NewProjectile(NPC.GetSpawnSourceForNPCFromNPCAI(), Target.Center + new Vector2(Main.rand.Next(-180, 180), Main.rand.Next(-180, 180)), TP * 0.05f, ModContent.ProjectileType<Bubble>(), 0, 1.2f, Main.myPlayer);
                Projectile.NewProjectile(NPC.GetSpawnSourceForNPCFromNPCAI(), NPC.Center + new Vector2(Main.rand.Next(-180, 180), Main.rand.Next(-180, 180)), TP * 0.05f, ModContent.ProjectileType<Bubble>(), 0, 1.2f, Main.myPlayer);
                if (Main.rand.NextBool(3))
                {
                    Projectile.NewProjectile(NPC.GetSpawnSourceForNPCFromNPCAI(), NPC.Center + new Vector2(Main.rand.Next(-180, 180), Main.rand.Next(-180, 180)), TP * 0.05f, ModContent.ProjectileType<ForceBubble>(), 0, 1.2f, Main.myPlayer);
                }
                if (Main.rand.NextBool(2))
                {
                    Projectile.NewProjectile(NPC.GetSpawnSourceForNPCFromNPCAI(), NPC.Center + new Vector2(Main.rand.Next(-180, 120), Main.rand.Next(-180, 180)), TP * 0.05f, ModContent.ProjectileType<HealthBubble>(), 0, 1.2f, Main.myPlayer);
                }
                Air2 = 0;
            }

            if (Air >= 5)
            {
                Target.breath -= 17;

                Air = 0;
            }
            if (Target.breath <= 0)
            {
                Target.lifeRegen = 0;
                Target.statLife--;
            }
            if (Target.statLife < 0)
            {
                Main.NewText("观星者:它认为你死了,所以它走了", Color.Red);
                CombatText.NewText(Target.Hitbox, Color.Red, "观星者:它认为你死了,所以它走了", true, false);
                Target.statLife = 1;
                Target.KillMe((Terraria.DataStructures.PlayerDeathReason)NPC.GetSpawnSourceForNPCFromNPCAI(), 25, 25);
            }

            switch ((Tidebossai)State1)
            {
                case Tidebossai.Start://开场
                    {
                        NPC.rotation = 0;
                        Timer2++;
                        if (Timer2 < 180) { NPC.Center = Vector2.Lerp(NPC.Center, new Vector2(Target.Center.X, Target.Center.Y - 500 + Timer2), 0.4f); }
                        if (Timer2 >= 180)
                        {
                            float dire = NPC.Center.X - Target.Center.X;
                            Vector2 pos1 = new Vector2(Target.Center.X + 100, Target.Center.Y - 90); Vector2 pos2 = new Vector2(Target.Center.X - 100, Target.Center.Y - 90);
                            if (dire >= 0) { NPC.Center = Vector2.Lerp(NPC.Center, pos1, 0.03f); }
                            if (dire < 0) { NPC.Center = Vector2.Lerp(NPC.Center, pos2, 0.03f); }
                            Vector2 projv = Target.Center - NPC.Center;
                            projv.Normalize();


                            Timer1++;

                            for (int i = 0; i < 6; i++)
                            {
                                if (Timer1 == 5 * i)
                                {

                                    Projectile.NewProjectile(NPC.GetSpawnSourceForNPCFromNPCAI(), NPC.Center, 18 * projv,
                                       ModContent.ProjectileType<Tidebeam1>(), 70, 1.2f, Main.myPlayer);
                                }
                            }

                            if (Timer1 > 64) { Timer1 = 0; }
                        }
                        if (Timer2 > 343) { Timer2 = 0; Timer1 = 0; SwitchState1((int)Tidebossai.phs1); }
                        if (NPC.life <= NPC.lifeMax * 0.55f) { NPC.velocity *= 0; Timer1 = 0; SwitchState1((int)Tidebossai.phs5); }
                        break;
                    }

                case Tidebossai.phs1:
                    {
                        NPC.rotation = 0;
                        float dire = NPC.Center.X - Target.Center.X;
                        Vector2 pos1 = new Vector2(Target.Center.X + 100, Target.Center.Y - 90); Vector2 pos2 = new Vector2(Target.Center.X - 100, Target.Center.Y - 90);
                        if (dire >= 0) { NPC.Center = Vector2.Lerp(NPC.Center, pos1, 0.03f); }
                        if (dire < 0) { NPC.Center = Vector2.Lerp(NPC.Center, pos2, 0.03f); }
                        Vector2 projv = Target.Center - NPC.Center;
                        projv.Normalize();
                        Timer3++;

                        Timer4++;

                        for (int i = 0; i < 10; i++)
                        {
                            if (Timer3 == 20 * i)
                            {

                                Projectile.NewProjectile(NPC.GetSpawnSourceForNPCFromNPCAI(), NPC.Center, 18 * projv,
                                   ModContent.ProjectileType<Tidelance>(), 70, 1.2f, Main.myPlayer);
                            }
                        }

                        if (Timer3 > 60) { Timer3 = 0; }
                        if (Timer4 > 300)
                        {
                            SwitchState1((int)Tidebossai.phs2); if (NPC.life <= NPC.lifeMax * 0.55f) { NPC.velocity *= 0; Timer1 = 0; SwitchState1((int)Tidebossai.phs5); }
                            Timer4 = 0; Timer3 = 0; Timer1 = 0;
                        }
                        break;
                    }
                case Tidebossai.phs2:
                    {
                        NPC.rotation = 0;
                        for (int i = 0; i < 100; i++)
                        {
                            Vector2 rot = (i * MathHelper.Pi / 50).ToRotationVector2() * 1000f;
                            Dust dust = Dust.NewDustDirect(NPC.Center + rot, 1, 1, DustID.IceTorch, 0, 0, 0, default, 1);
                            dust.fadeIn = 0.2f;

                        }
                        if ((Target.Center - NPC.Center).Length() > 1000) { Vector2 back = (NPC.Center - Target.Center); back.Normalize(); back *= 6f; Target.velocity = back; }
                        Vector2 projv = Target.Center - NPC.Center;
                        projv.Normalize();


                        Timer1++;

                        for (int i = 0; i < 13; i++)
                        {
                            if (Timer1 == 60 * i)
                            {
                                for (int j = 0; j < 3; j++)
                                {
                                    float rota = (Target.Center - NPC.Center).ToRotation() + (-1 + j) * (MathHelper.Pi / 20); Vector2 re = rota.ToRotationVector2() * 14f;
                                    Projectile.NewProjectile(NPC.GetSpawnSourceForNPCFromNPCAI(), NPC.Center, re,
                                    ModContent.ProjectileType<Tideball>(), 70, 1.2f, Main.myPlayer);


                                }

                            }
                        }

                        if (Timer1 > 390) { Timer1 = 0; SwitchState1((int)Tidebossai.phs3); }
                        if (NPC.life <= NPC.lifeMax * 0.55f) { NPC.velocity *= 0; Timer1 = 0; SwitchState1((int)Tidebossai.phs5); }
                        break;
                    }
                case Tidebossai.phs3:
                    {
                        NPC.rotation = 0;
                        for (int i = 0; i < 100; i++)
                        {
                            Vector2 rot = (i * MathHelper.Pi / 50).ToRotationVector2() * 1000f;
                            Dust dust = Dust.NewDustDirect(NPC.Center + rot, 1, 1, DustID.IceTorch, 0, 0, 0, default, 1);
                            dust.fadeIn = 0.2f;

                        }//结界粒子
                        if ((Target.Center - NPC.Center).Length() > 1000) { Vector2 back = (NPC.Center - Target.Center); back.Normalize(); back *= 6f; Target.velocity = back; }//碰壁弹回
                        Vector2 projv = Target.Center - NPC.Center;
                        projv.Normalize();


                        Timer1++;

                        for (int i = 0; i < 16; i++)
                        {
                            if (Timer1 == 45 * i)
                            {
                                for (int j = 0; j < Main.rand.Next(6, 9); j++)
                                {
                                    Vector2 up1 = Main.rand.NextFloat(-MathHelper.Pi / 3, 0).ToRotationVector2() * 16f;
                                    Vector2 up = Main.rand.NextFloat(-MathHelper.Pi, -MathHelper.Pi + MathHelper.Pi / 3).ToRotationVector2() * 16f;
                                    Projectile.NewProjectile(NPC.GetSpawnSourceForNPCFromNPCAI(), NPC.Center, up,
                                       ModContent.ProjectileType<Tidecell>(), 70, 1.2f, Main.myPlayer);
                                    Projectile.NewProjectile(NPC.GetSpawnSourceForNPCFromNPCAI(), NPC.Center, up1,
                                     ModContent.ProjectileType<Tidecell>(), 70, 1.2f, Main.myPlayer);
                                }

                            }

                        }
                        if (Timer1 > 270)
                        {
                            Timer1 = 0; SwitchState1((int)Tidebossai.phs4); NPC.Center = new Vector2(Target.Center.X - 620, Target.Center.Y);

                            for (int i = 0; i < 100; i++)
                            {
                                Vector2 rot = (i * MathHelper.Pi / 50).ToRotationVector2() * 12f;
                                Dust dust = Dust.NewDustDirect(NPC.Center, 1, 1, DustID.FishronWings, rot.X, rot.Y, 0, default, 1);


                            }
                            for (int i = 0; i < 24; i++)
                            {
                                Vector2 rot = (i * MathHelper.Pi / 12).ToRotationVector2() * 8f;
                                Projectile.NewProjectile(NPC.GetSpawnSourceForNPCFromNPCAI(), NPC.Center, rot,
                                ModContent.ProjectileType<Tidecell>(), 70, 1.2f, Main.myPlayer);

                            }

                        }
                        if (NPC.life <= NPC.lifeMax * 0.55f) { NPC.velocity *= 0; Timer1 = 0; SwitchState1((int)Tidebossai.phs5); }

                        break;
                    }
                case Tidebossai.phs4:
                    {
                        Vector2 vel = Target.Center - NPC.Center + Target.velocity * 15f;
                        vel.Normalize();

                        Timer1++;


                        for (int i = 0; i < 10; i++)
                        {

                            if (Timer1 > 100 * (4 * i - 3) && Timer1 < 100 * (4 * i - 3) + 100)
                            {
                                for (int j = 0; j < 6; j++)
                                {
                                    if (Timer1 == 100 * (4 * i - 3) + 10 * j)
                                    {
                                        Vector2 sp = new Vector2(12, 0);
                                        Projectile.NewProjectile(NPC.GetSpawnSourceForNPCFromNPCAI(), NPC.Center, sp, ModContent.ProjectileType<Tidebeam1>(), 70, 1.2f, Main.myPlayer, 1);
                                    }
                                }
                                NPC.Center = Vector2.Lerp(NPC.Center, new Vector2(Target.Center.X - 620, Target.Center.Y), 0.4f);
                                NPC.velocity *= 0; NPC.rotation = 0;
                            }

                        }

                        for (int i = 0; i < 3; i++)
                        {
                            if (Timer1 == 100 * (4 * i - 2))
                            {
                                rush = vel * 29f;
                                NPC.velocity *= 0;
                            }
                            if (Timer1 > 100 * (4 * i - 2) && Timer1 < 100 * (4 * i - 2) + 100)
                            {
                                for (int n = 0; n < 20; n++)
                                {
                                    Vector2 rot = (n * MathHelper.Pi / 5).ToRotationVector2() * 20f;
                                    Dust dust = Dust.NewDustDirect(NPC.Center + rot, 1, 1, DustID.IceTorch, 0, 0, 0, default, 1);
                                    dust.fadeIn = 0.2f;
                                    dust.velocity = -NPC.velocity * 0.2f;
                                }
                                NPC.velocity = rush;
                                NPC.rotation = NPC.velocity.ToRotation() + MathHelper.Pi / 2;
                            }

                        }
                        for (int i = 0; i < 3; i++)
                        {

                            if (Timer1 > 100 * (4 * i - 1) && Timer1 < 100 * (4 * i - 1) + 100)
                            {
                                for (int j = 0; j < 10; j++)
                                {
                                    if (Timer1 == 100 * (4 * i - 1) + 10 * j)
                                    {
                                        Vector2 sp = new Vector2(-12, 0);
                                        Projectile.NewProjectile(NPC.GetSpawnSourceForNPCFromNPCAI(), NPC.Center, sp, ModContent.ProjectileType<Tidelance>(), 70, 1.2f, Main.myPlayer, 1);
                                    }
                                }
                                NPC.velocity *= 0;
                                NPC.Center = Vector2.Lerp(NPC.Center, new Vector2(Target.Center.X + 620, Target.Center.Y), 0.4f);
                                NPC.rotation = 0;
                            }

                        }
                        for (int i = 0; i < 3; i++)
                        {
                            if (Timer1 == 100 * (4 * i))
                            {
                                rush = vel * 29f;
                                NPC.velocity *= 0;
                            }
                            if (Timer1 > 100 * (4 * i) && Timer1 < 100 * (4 * i) + 100)
                            {
                                NPC.velocity = rush; NPC.rotation = NPC.velocity.ToRotation() + MathHelper.Pi / 2;
                                for (int n = 0; n < 10; n++)
                                {
                                    Vector2 rot = (n * MathHelper.Pi / 5).ToRotationVector2() * 20f;
                                    Dust dust = Dust.NewDustDirect(NPC.Center + rot, 1, 1, DustID.IceTorch, 0, 0, 0, default, 1);
                                    dust.fadeIn = 0.2f;
                                    dust.velocity = -NPC.velocity * 0.2f;
                                }
                            }

                        }
                        if (Timer1 > 1080) { NPC.velocity *= 0; NPC.rotation = 0; Timer1 = 0; SwitchState1((int)Tidebossai.phs1); }
                        if (NPC.life <= NPC.lifeMax * 0.55f) { NPC.velocity *= 0; Timer1 = 0; SwitchState1((int)Tidebossai.phs5); }

                        break;
                    }
                case Tidebossai.phs5:
                    {
                        NPC.rotation = 0;
                        rain = 1;
                        for (int i = 0; i < 24; i++)
                        {
                            Vector2 rot = (i * MathHelper.Pi / 12).ToRotationVector2() * 610f;
                            Dust dust = Dust.NewDustDirect(NPC.Center + rot, 1, 1, DustID.IceTorch, 0, 0, 0, default, 1);
                            dust.fadeIn = 0.2f;

                        }
                        Timer1++;
                        if (Timer1 <= 60 && (NPC.Center.X - Target.Center.X) < 0)
                        {
                            NPC.velocity *= 0;
                            for (int n = 0; n < 10; n++)
                            {
                                { NPC.Center = Vector2.Lerp(NPC.Center, new Vector2(Target.Center.X - 310, Target.Center.Y), 0.03f); }
                                Vector2 rot = (n * MathHelper.Pi / 5).ToRotationVector2() * 500f;
                                Dust dust = Dust.NewDustDirect(rot, 1, 1, DustID.IceTorch, 0.05f * rot.X, 0.05f * rot.Y, 0, default, 1);
                                dust.fadeIn = 0.2f;

                            }

                        }
                        if (Timer1 <= 60 && (NPC.Center.X - Target.Center.X) >= 0)
                        {
                            { NPC.Center = Vector2.Lerp(NPC.Center, new Vector2(Target.Center.X + 310, Target.Center.Y), 0.03f); }
                            NPC.velocity *= 0;
                            for (int n = 0; n < 10; n++)
                            {
                                Vector2 rot = (n * MathHelper.Pi / 5).ToRotationVector2() * 500f;
                                Dust dust = Dust.NewDustDirect(rot, 1, 1, DustID.IceTorch, 0.05f * rot.X, 0.05f * rot.Y, 0, default, 1);
                                dust.fadeIn = 0.2f;

                            }

                        }
                        if ((Target.Center - NPC.Center).Length() > 610) { Vector2 back = (NPC.Center - Target.Center); back.Normalize(); back *= 6f; Target.velocity = back; }
                        if (Timer1 > 60) { Timer1 = 0; SwitchState1((int)Tidebossai.phs6); }
                        break;
                    }
                case Tidebossai.phs6:
                    {
                        if (Timer1 > 120)
                        {
                            for (int a = 0; a < 50; a++)
                            {
                                if (Timer1 == 120 + 100 * a)
                                {
                                    for (int j = 0; j < 8; j++)
                                    {
                                        Vector2 Asoul = new Vector2(Target.Center.X + 1000, Target.Center.Y - 600 + 300 * j);
                                        Projectile.NewProjectile(NPC.GetSpawnSourceForNPCFromNPCAI(), Asoul, new Vector2(-12, 0), ModContent.ProjectileType<Tidelance>(), 45, 2, Target.whoAmI);
                                    }
                                }
                            }
                        }
                        rain = 2;
                        Timer1++;
                        for (int n = 0; n < 5; n++)
                        {
                            if (Timer1 == 60 + 120 * n)
                            {
                                random = Main.rand.Next(0, 2);


                            }
                            if (Timer1 == 60)
                            {

                                ra = new Vector2(Target.Center.X, Target.Center.Y);


                            }

                        }
                        if (Timer1 > 60 && (NPC.Center.X - Target.Center.X) >= 0) { NPC.Center = Vector2.Lerp(NPC.Center, new Vector2(ra.X + 310, ra.Y), 0.03f); }
                        if (Timer1 > 60 && (NPC.Center.X - Target.Center.X) < 0) { NPC.Center = Vector2.Lerp(NPC.Center, new Vector2(ra.X - 310, ra.Y), 0.03f); }
                        for (int n = 0; n < 15; n++)
                        {
                            if (Timer1 > 60 + 120 * n && Timer1 < 120 * n + 120 + 60)
                            {
                                if (random != 0) { ra.X -= 3.6f; }
                                if (random == 0) { ra.X += 3.6f; }
                                for (int n2 = 0; n2 < 60; n2++)
                                {
                                    if (Timer1 == 60 + 2 * n2 + 120 * n)
                                    {
                                        posi = new Vector2(ra.X - 160f, ra.Y - 750f);
                                        posi1 = new Vector2(ra.X + 160f, ra.Y - 750f);
                                        Vector2 posi2 = new Vector2(ra.X - 160f - Main.rand.NextFloat(0, 500), ra.Y - 750f);
                                        Vector2 posi3 = new Vector2(ra.X + 160f + Main.rand.NextFloat(0, 500), ra.Y - 750f);
                                        Vector2 rai = new Vector2(Main.rand.NextFloat(-0.05f, 0.05f), 50f);
                                        Projectile po = Projectile.NewProjectileDirect(NPC.GetSpawnSourceForNPCFromNPCAI(), posi3, rai, ModContent.ProjectileType<Tidelance>(), 70, 1.2f, Main.myPlayer);
                                        Projectile po1 = Projectile.NewProjectileDirect(NPC.GetSpawnSourceForNPCFromNPCAI(), posi2, rai, ModContent.ProjectileType<Tidebeam1>(), 70, 1.2f, Main.myPlayer);

                                    }



                                }
                            }

                        }
                        for (int i = 0; i < 24; i++)
                        {
                            Vector2 rot = (i * MathHelper.Pi / 12).ToRotationVector2() * 610f;
                            Dust dust = Dust.NewDustDirect(NPC.Center + rot, 1, 1, DustID.IceTorch, 0, 0, 0, default, 1);
                            dust.fadeIn = 0.2f;

                        }
                        if ((Target.Center - NPC.Center).Length() > 610) { Vector2 back = (NPC.Center - Target.Center); back.Normalize(); back *= 6f; Target.velocity = back; }
                        if (Timer1 > 900) { rain = 0; Timer1 = 0; SwitchState1((int)Tidebossai.phs7); }
                        break;
                    }
                case Tidebossai.phs7:
                    {
                        if (Target.velocity.X * 70 < 200 && Target.velocity.X * 70 > -200) { velx = Target.velocity.X * 70; }
                        if (Target.velocity.X * 70 >= 200) { velx = 200; }
                        if (Target.velocity.X * 70 <= -200) { velx = -200; }
                        rain = 4; Timer1++;
                        for (int l = 0; l < 6; l++)
                            if (Timer1 >= 1 + l * 200 && Timer1 < l * 200 + 101)
                            { NPC.Center = Vector2.Lerp(NPC.Center, new Vector2(Target.Center.X + velx, Target.Center.Y - 960), 1); }
                        for (int l = 0; l < 6; l++)
                            if (Timer1 >= 101 + l * 200 && Timer1 < 200 + l * 200)
                            {
                                NPC.velocity = new Vector2(0, 30f);
                                for (int n = 0; n < 20; n++)
                                {
                                    Vector2 rot = (n * MathHelper.Pi / 5).ToRotationVector2() * 20f;
                                    Dust dust = Dust.NewDustDirect(NPC.Center + rot, 1, 1, DustID.IceTorch, 0, 0, 0, default, 1);
                                    dust.fadeIn = 0.2f;
                                    dust.velocity = -NPC.velocity * 0.2f;
                                }

                                Vector2 posi3 = new Vector2(NPC.Center.X - Main.rand.NextFloat(0, 200) - 220, Target.Center.Y - 1000);
                                Vector2 posi2 = new Vector2(NPC.Center.X + Main.rand.NextFloat(0, 200) + 220, Target.Center.Y - 1000);
                                Vector2 rai = new Vector2(Main.rand.NextFloat(-0.05f, 0.05f), 50f);
                                Projectile po = Projectile.NewProjectileDirect(NPC.GetSpawnSourceForNPCFromNPCAI(), posi3, rai, ModContent.ProjectileType<Tidelance>(), 60, 1.2f, Main.myPlayer);
                                Projectile po1 = Projectile.NewProjectileDirect(NPC.GetSpawnSourceForNPCFromNPCAI(), posi2, rai, ModContent.ProjectileType<Tidebeam1>(), 60, 1.2f, Main.myPlayer);
                            }
                        if (Timer1 > 1000) { rain = 0; Timer1 = 0; SwitchState1((int)Tidebossai.phs8); }
                        break;
                    }
                case Tidebossai.phs8:
                    {
                        NPC.velocity *= 0;
                        NPC.Center = Vector2.Lerp(NPC.Center, new Vector2(Target.Center.X - 400, Target.Center.Y + 100), 0.5f);
                        rain = 5; Timer1++;
                        for (int u = 0; u < 40; u++)
                            if (Timer1 == 45 * u)
                            {
                                float posr = MathHelper.Pi / 4 + u * MathHelper.Pi / 2;
                                pos = Target.Center + posr.ToRotationVector2() * 640f;




                                for (int j = -2; j <= 2; j++)
                                {
                                    float r = (Target.Center - pos).ToRotation() + MathHelper.Pi / 60 * j;
                                    Vector2 k = r.ToRotationVector2() * 10f;
                                    Projectile po1 = Projectile.NewProjectileDirect(NPC.GetSpawnSourceForNPCFromNPCAI(), pos, k, ModContent.ProjectileType<Tidebeam1>(), 60, 1.2f, Main.myPlayer, 1);
                                }

                            }
                        if (Timer1 > 1000) { rain = 0; Timer1 = 0; SwitchState1((int)Tidebossai.phs5); }
                        break;
                    }
                case Tidebossai.End1:
                    {
                        Vector2 Head = new Vector2(Target.Center.X, Target.Center.Y - 400);
                        NPC.velocity = (NPC.velocity * 10 + (Head - NPC.Center)
                            .SafeNormalize(Vector2.UnitX) * 15) / 11;
                        Timer1++;
                        Vector2 ToPlayer = Target.Center - NPC.Center;
                        if (Timer1 <= 300)
                        {
                            Timer2++;
                            if(Timer2 >= 5)
                            {
                                Projectile.NewProjectile(NPC.GetSpawnSourceForNPCFromNPCAI(), Target.Center + new Vector2(Main.rand.Next(-180, 180), Main.rand.Next(-180, 180)), TP * 0.05f, ModContent.ProjectileType<ForceBubble>(), 0, 1.2f, Main.myPlayer);
                                Timer2 = 0;
                            }
                        }
                        if(Timer1 >= 420)
                        {
                            Timer2 = 0;
                            Timer1 = 0;
                            SwitchState1((int)Tidebossai.End2);
                        }
                        break;
                    }
                    case Tidebossai.End2:
                    {
                        NPC.velocity *= 0;
                        Timer1++;
                        Timer2++;
                        Timer3++;
                        if(Timer3 <= 5)
                        {
                            NPC.scale -= 0.02f;
                        }
                        if(Timer3 >= 5)
                        {
                            NPC.scale += 0.015f;
                        }
                        if(Timer3 >= 10)
                        {
                            Timer3 = 0;
                        }
                        if(Timer1 >= 15)
                        {
                            SoundEngine.PlaySound(SoundID.Item4, NPC.Center);
                            Projectile.NewProjectile(NPC.GetSpawnSourceForNPCFromNPCAI(), NPC.Center + new Vector2(Main.rand.Next(-90, 90), Main.rand.Next(-90, 90)), TP * 0.1f, ModContent.ProjectileType<Bubble>(), 0, 1.2f, Main.myPlayer);
                            Timer1 = 0;
                        }
                        if(Timer2 >= 300)
                        {
                            Vector2 ToPlayer = Target.Center - NPC.Center;
                            for (int i = 0; i < 30; i++)
                            {
                                Vector2 r = (ToPlayer.ToRotation() + (float)Math.Sin(Main.time) * 0.5f + (i * MathHelper.Pi / 15)).ToRotationVector2() * 7.5f;
                                Projectile.NewProjectile(NPC.GetSpawnSourceForNPCFromNPCAI(), NPC.Center, r * 1.5f, ModContent.ProjectileType<Bubble>(), 0, 0f, Main.myPlayer);
                            }
                            Timer2 = 0;
                            Timer1 = 0;
                            SoundEngine.PlaySound(SoundID.Item4, NPC.Center);
                            NPC.life = 0;
                            NPC.checkDead();
                        }
                        break;
                    }
            }
            base.AI();
        }
        public int T = 0;
        public override bool CheckDead()
        {
            if (State1 != (int)Tidebossai.End1 && State1 != (int)Tidebossai.End2)
            {
                SoundEngine.PlaySound(SoundID.Item4, NPC.Center);
                Timer1 = 0;
                Timer2 = 0;
                SwitchState2(0);
                NPC.life = 5000;
                NPC.dontTakeDamage = true;
                SwitchState1((int)Tidebossai.End1);
                return false;
            }
            return true;
        }
        public override bool PreDraw(SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor)
        {
            Color color = Color.White;
            if (rain == 1 && Timer1 < 60)
            {
                color = Color.SkyBlue;
                Vector2 vector = new Vector2(Target.Center.X, Target.Center.Y - 300) - Main.screenPosition;
                Texture2D value2 = (Texture2D)BloodSoulUtils.GetTexture("Images/Warning1");
                Vector2 origin = value2.Frame(1, 1, 0, 0).Size() * new Vector2(0f, 0.5f);
                Main.spriteBatch.Draw(value2, vector, null, new Color(255, 255, 255, 100), MathHelper.Pi / 2, origin, new Vector2(1, 1f), SpriteEffects.None, 0f);
            }
            if (rain == 2 && Timer1 < 900)
            {
                color = Color.SkyBlue;
                Vector2 vector = posi - Main.screenPosition;
                Texture2D value2 = (Texture2D)BloodSoulUtils.GetTexture("Images/Extra_178");
                Vector2 origin = value2.Frame(1, 1, 0, 0).Size() * new Vector2(0f, 0.5f);
                Main.spriteBatch.Draw(value2, vector, null, new Color(0, 0, 255, 100), MathHelper.Pi / 2, origin, new Vector2(100, 0.5f), SpriteEffects.None, 0f);
            }
            if (rain == 2 && Timer1 < 900)
            {
                color = Color.SkyBlue;
                Vector2 vector = posi1 - Main.screenPosition;
                Texture2D value2 = (Texture2D)BloodSoulUtils.GetTexture("Images/Extra_178");
                Vector2 origin = value2.Frame(1, 1, 0, 0).Size() * new Vector2(0f, 0.5f);
                Main.spriteBatch.Draw(value2, vector, null, new Color(0, 0, 255, 100), MathHelper.Pi / 2, origin, new Vector2(100, 0.5f), SpriteEffects.None, 0f);
            }
            for (int l = 0; l < 6; l++)
                if (Timer1 >= 1 + l * 200 && Timer1 < l * 200 + 101 && rain == 4)
                {
                    spriteBatch.End();
                    spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.Additive, SamplerState.AnisotropicClamp, DepthStencilState.None, RasterizerState.CullNone, null, Main.GameViewMatrix.TransformationMatrix);
                    color = Color.SkyBlue;
                    Vector2 vector = new Vector2(Target.Center.X + velx, Target.Center.Y - 960) - Main.screenPosition;
                    Texture2D value2 = (Texture2D)BloodSoulUtils.GetTexture("Images/Extra_197");
                    Vector2 origin = value2.Frame(1, 1, 0, 0).Size() * new Vector2(0f, 0.5f);
                    Main.spriteBatch.Draw(value2, vector, null, new Color(255, 0, 0, 255), MathHelper.Pi / 2, origin, new Vector2(100, 0.6f), SpriteEffects.None, 0f);
                    spriteBatch.End();
                    spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, SamplerState.AnisotropicClamp, DepthStencilState.None, RasterizerState.CullNone, null, Main.GameViewMatrix.TransformationMatrix);
                }
            return true;
        }
    }

}
