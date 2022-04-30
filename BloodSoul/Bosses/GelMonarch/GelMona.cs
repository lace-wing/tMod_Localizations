using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using Terraria.ModLoader;
using Terraria.Localization;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria.ID;
using BloodSoul.MyUtils;
using BloodSoul.Projectiles.Bosses;
using System.IO;
using BloodSoul.NPCs.Bosses.HolyLightSwords;
using Terraria.Graphics.Shaders;
using Terraria.Graphics.Effects;
using Terraria.DataStructures;
using Terraria.GameContent;
using Terraria.Audio;
using BloodSoul.Items.BossBag;
using Terraria.GameContent.ItemDropRules;
using Terraria.GameContent.Bestiary;

namespace BloodSoul.NPCs.Bosses.GelMonarch
{
    [AutoloadBossHead]
    public class GelMona : FSMnpc
    {
        public override string Texture => "BloodSoul/NPCs/Bosses/GelMonarch/GelMona1";
        public override string BossHeadTexture => "BloodSoul/NPCs/Bosses/GelMonarch/GelMona1";
        /// <summary>
        /// 切换帧图用
        /// </summary>
        private int _FrameSp = 0;
        /// <summary>
        /// 射出的凝胶即计数器
        /// </summary>
        private int _shootGel = 0;
        /// <summary>
        /// 君主的攻击
        /// </summary>
        enum GelF
        {
            /// <summary>
            /// 五连砍
            /// </summary>
            FiveK,
            /// <summary>
            /// 很正常的散弹
            /// </summary>
            SanD,
            /// <summary>
            /// 超级散弹
            /// </summary>
            SuperSanTanD,
            /// <summary>
            /// 在哪里砍
            /// </summary>
            WhereK,
            /// <summary>
            /// 玩家必死
            /// </summary>
            PlayerOfDead
        }
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Gel Monarch");
            DisplayName.AddTranslation((int)GameCulture.CultureName.Chinese, "凝胶君主");
            Main.npcFrameCount[NPC.type] = 9;
            //NPCID.Sets.TrailCacheLength[NPC.type] = 12;
            //NPCID.Sets.TrailingMode[NPC.type] = 1;
        }
        public override void SetDefaults()
        {
            NPC.lifeMax = 10000;
            NPC.damage = 90;
            NPC.defense = 27;
            NPC.friendly = false;
            NPC.noGravity = true;
            NPC.noTileCollide = true;
            NPC.width = 260;
            NPC.height = 260;
            NPC.aiStyle = -1;
            NPC.boss = true;
            NPC.knockBackResist = 0f;
            NPC.value = Item.buyPrice(0, 16, 66, 66);
            NPC.lavaImmune = true;
            NPC.HitSound = SoundID.NPCHit1;
            NPC.DeathSound = SoundID.NPCDeath11;
            //DrawOffsetY = 20;
            //music = mod.GetSoundSlot(SoundType.Music, "Sounds/Music/BloodCrystalEye");
            //musicPriority = MusicPriority.BossHigh;
            //BossBag = ModContent.ItemType<BloodEyeBossBag>();
        }
        public override void SendExtraAI(BinaryWriter writer)
        {
            writer.Write(_FrameSp);
            writer.Write(_shootGel);
        }
        public override void ReceiveExtraAI(BinaryReader reader)
        {
            _FrameSp = reader.ReadInt32();
            _shootGel = reader.ReadInt32();
        }
        public override void AI()
        {
            var GelSw = ModContent.ProjectileType<GelSw>();

            NPC.TargetClosest();
            Vector2 ToPlayer = Target.Center - NPC.Center;//获取npc到玩家的向量
            ToPlayer.Normalize();
            NPC.spriteDirection = NPC.direction = (Target.Center.X- NPC.Center.X > 0).ToDirectionInt();//改变npc的朝向
            switch((GelF)State1)
            {
                case GelF.FiveK:
                    {
                        Timer1++;
                        _FrameSp++;
                        if (Timer1 % 48 == 0 && Main.netMode != 1)
                        {
                            Projectile.NewProjectile(NPC.GetSpawnSource_ForProjectile(),NPC.Center,ToPlayer * 3f,
                                GelSw,90,2.3f,Main.myPlayer,0);
                            _shootGel++;
                        }
                        if(_shootGel >=6)
                        {
                            _shootGel = 0;
                            _FrameSp = 0;
                            Timer1 = 0;
                            SwitchState1(1);
                        }
                        break;
                    }
                case GelF.SanD:
                    {
                        Timer1++;
                        float floatTo = ToPlayer.ToRotation();
                        if(Timer1 > 20) _FrameSp++;
                        if (Main.netMode != 1 && Timer1 > 30)
                        {
                            for (int i = -1; i <= 1; i++)
                            {
                                float r2 = floatTo + i * MathHelper.Pi / 36f;
                                Vector2 shootVel = r2.ToRotationVector2() * 10;
                                Projectile.NewProjectile(NPC.GetSpawnSource_ForProjectile(), NPC.Center, shootVel, GelSw, 100, 10, Main.myPlayer,i);
                                _shootGel++;
                            }
                        }
                        if(_shootGel >= 3)
                        {
                            _shootGel = 0;
                            _FrameSp = 0;
                            Timer1 = 0;
                            SwitchState1(2);
                        }
                        break;
                    }
                case GelF.SuperSanTanD:
                    {
                        Timer1++;
                        if (Timer1 < 30 && Main.netMode != 1)
                        {
                            if (NPC.direction == 1)
                            {
                                NPC.position.X -= 1;
                            }
                            else
                            {
                                NPC.position.X += 1;
                            }
                        }
                        if(Timer1 > 30 && Main.netMode != 1)
                        {
                            _FrameSp++;
                            if (Timer1 == 40)
                            {
                                float floatTo = ToPlayer.ToRotation();
                                for (int i = -1; i <= 1; i++)
                                {
                                    float r2 = floatTo + i * MathHelper.Pi / 36f;
                                    Vector2 shootVel = r2.ToRotationVector2() * 10;
                                    Projectile.NewProjectile(NPC.GetSpawnSource_ForProjectile(), NPC.Center, shootVel, GelSw, 100, 10, Main.myPlayer);
                                    _shootGel++;
                                }
                            }
                            if(Timer1 == 50)
                            {
                                float floatTo = ToPlayer.ToRotation();
                                for (int i = -1; i < 1; i++)
                                {
                                    float r2 = floatTo + i * MathHelper.Pi / 18f;
                                    Vector2 shootVel = r2.ToRotationVector2() * 10;
                                    Projectile.NewProjectile(NPC.GetSpawnSource_ForProjectile(), NPC.Center, shootVel, GelSw, 100, 10, Main.myPlayer);
                                    _shootGel++;
                                }
                            }

                        }
                        if(_shootGel >= 5)
                        {
                            _shootGel = 0;
                            _FrameSp = 0;
                            Timer1 = 0;
                            SwitchState1(0);
                        }
                        break;
                    }
                case GelF.WhereK:
                    {
                        Timer1++;
                        if (Timer1 == 42)
                        {
                            if (NPC.spriteDirection == 1)
                            {
                                NPC.position = new Vector2(Target.position.X + 300, Target.position.Y);
                            }
                            else
                            {
                                NPC.position = new Vector2(Target.position.X - 300, Target.position.Y);
                            }
                        }
                        else if(Timer1 > 42)
                        {
                            _FrameSp++;
                            if(Timer1 > 50)
                            {
                                if(State2 == 0)
                                {
                                    float floatTo = ToPlayer.ToRotation();
                                    for (int i = -1; i <= 1; i++)
                                    {
                                        float r2 = floatTo + i * MathHelper.Pi / 36f;
                                        Vector2 shootVel = r2.ToRotationVector2() * 10;
                                        Projectile.NewProjectile(NPC.GetSpawnSource_ForProjectile(), NPC.Center, shootVel, GelSw, 100, 10, Main.myPlayer);
                                        _shootGel++;
                                    }
                                }
                                else if(State2 == 1)
                                {
                                    float floatTo = ToPlayer.ToRotation();
                                    for (int i = -1; i < 1; i++)
                                    {
                                        float r2 = floatTo + i * MathHelper.Pi / 18f;
                                        Vector2 shootVel = r2.ToRotationVector2() * 10;
                                        Projectile.NewProjectile(NPC.GetSpawnSource_ForProjectile(), NPC.Center, shootVel, GelSw, 100, 10, Main.myPlayer);
                                        _shootGel++;
                                    }
                                }
                                Timer1 = 0;
                                State2++;
                            }
                        }
                        if(State2 >= 2)
                        {
                            Timer1 = 0;
                            State2 = 0;
                            SwitchState1((int)GelF.PlayerOfDead);
                        }
                        break;
                    }
                case GelF.PlayerOfDead:
                    {
                        break;
                    }
                default://传送
                    {
                        break;
                    }
            }
        }
        public override bool CheckActive()
        {
            if (Target.dead) return true;
            return false;
        }
        public override bool CheckDead()
        {
            return true;
        }
        public override void FindFrame(int frameHeight)
        {
            switch((GelF)State1)
            {
                case GelF.FiveK:
                    {
                        //if (_FrameSp == 12) NPC.frame.Y = frameHeight * 4;
                        //else if (_FrameSp == 24) NPC.frame.Y = frameHeight * 4;
                        //else if (_FrameSp == 36)NPC.frame.Y = frameHeight * 4;
                        //else if (_FrameSp == 48) { NPC.frame.Y = frameHeight * 4; _FrameSp = 0; }
                        switch(_FrameSp)
                        {
                            case 9:
                                NPC.frame.Y = frameHeight * 3;break;
                            case 18:
                                NPC.frame.Y = frameHeight * 4; break;
                            case 27:
                                NPC.frame.Y = frameHeight * 5; break;
                            case 36:
                                NPC.frame.Y = frameHeight * 6; break;
                            case 45:
                                NPC.frame.Y = frameHeight * 7; _FrameSp=0; break;
                        }
                        break;
                    }
                case GelF.SanD:
                    {
                        if(Timer1 > 20 && Timer1 <= 30)
                        {
                            switch (_FrameSp)
                            {
                                case 0:
                                    NPC.frame.Y = frameHeight * 3; break;
                                case 2:
                                    NPC.frame.Y = frameHeight * 4; break;
                                case 4:
                                    NPC.frame.Y = frameHeight * 5; break;
                                case 6:
                                    NPC.frame.Y = frameHeight * 6; break;
                                case 8:
                                    NPC.frame.Y = frameHeight * 7; _FrameSp = 0; break;
                            }
                        }
                        break;
                    }
                case GelF.SuperSanTanD:
                    {
                        if(Timer1>30)
                        {
                            switch (_FrameSp)
                            {
                                case 0:
                                    NPC.frame.Y = frameHeight * 3; break;
                                case 2:
                                    NPC.frame.Y = frameHeight * 4; break;
                                case 4:
                                    NPC.frame.Y = frameHeight * 5; break;
                                case 6:
                                    NPC.frame.Y = frameHeight * 6; break;
                                case 8:
                                    NPC.frame.Y = frameHeight * 7; _FrameSp = 0; break;
                            }
                        }
                        break;
                    }
                case GelF.WhereK:
                    {
                        if (Timer1 > 42)
                        {
                            switch (_FrameSp)
                            {
                                case 0:
                                    NPC.frame.Y = frameHeight * 3; break;
                                case 2:
                                    NPC.frame.Y = frameHeight * 4; break;
                                case 4:
                                    NPC.frame.Y = frameHeight * 5; break;
                                case 6:
                                    NPC.frame.Y = frameHeight * 6; break;
                                case 8:
                                    NPC.frame.Y = frameHeight * 7; _FrameSp = 0; break;
                            }
                        }
                        break;
                    }
                case GelF.PlayerOfDead:
                    {
                        break;
                    }
                default:
                    {
                        break;
                    }
            }
        }
        public override void SetBestiary(BestiaryDatabase database, BestiaryEntry bestiaryEntry)
        {
        }
    }
}
