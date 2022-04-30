using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Terraria;
using System;
using Terraria.GameContent.Creative;
using Terraria.GameContent.ItemDropRules;
using Terraria.Audio;
using Terraria.GameContent.Bestiary;
using Terraria.ModLoader.Utilities;
using System.IO;
using Terraria.DataStructures;
using BloodSoul;
using Terraria.GameContent;
using BloodSoul.MyUtils;
using BloodSoul.NPCs.Bosses;
using Microsoft.Xna.Framework.Graphics;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.Localization;

namespace BloodSoul.NPCs.Bosses.RockSnake
{
	[AutoloadBossHead]
	internal class RockSnakeHead : ModNPC
	{
		public int Time;
		public int Time2;
		public float speed;
		public float turnspeed;
		public override void BossHeadRotation(ref float rotation)
		{
			rotation = NPC.rotation;
		}
		public override void SetStaticDefaults()
		{
			DisplayName.SetDefault("Ground shaking snake");
			DisplayName.AddTranslation((int)GameCulture.CultureName.Chinese, "撼地蛇");
		}
		public override void SetDefaults()
		{
			NPC.width = 138;
			NPC.height = 100;
			NPC.netAlways = true;
			NPC.damage = 85;
			NPC.aiStyle = -1;
			NPC.defense = 10;
			NPC.boss = true;
			NPC.lifeMax = 5500;
			NPC.HitSound = SoundID.NPCHit1;
			NPC.DeathSound = SoundID.NPCDeath1;
			NPC.noGravity = true;
			NPC.noTileCollide = true;
			NPC.knockBackResist = 0f;
			NPC.behindTiles = true;
			NPC.dontCountMe = true;
			if (!Main.dedServ)
			{
				Music = MusicLoader.GetMusicSlot(Mod, "Sounds/Music/Snake");
			}
		}
		public override void ModifyNPCLoot(NPCLoot npcLoot)
		{
			npcLoot.Add(ItemDropRule.BossBag(ModContent.ItemType<Items.BossBag.RockSnakeBossBag>()));
		}

		public override void SetBestiary(BestiaryDatabase database, BestiaryEntry bestiaryEntry)
		{
			bestiaryEntry.Info.AddRange(new IBestiaryInfoElement[] {
				BestiaryDatabaseNPCsPopulator.CommonTags.SpawnConditions.Biomes.Underground,
				BestiaryDatabaseNPCsPopulator.CommonTags.SpawnConditions.Biomes.Caverns,
				new FlavorTextBestiaryInfoElement("长居地底的变异巨蛇")
			});
		}
		public override void AI()
		{
			NPC.spriteDirection = NPC.direction = NPC.velocity.X <0 ? 1 : -1;
			NPC.rotation = NPC.velocity.ToRotation() + (NPC.spriteDirection == -1 ? 0f : MathHelper.Pi);
			Player player = Main.player[NPC.target];
			NPC.TargetClosest(true);
			NPC.rotation = (float)Math.Atan2(NPC.velocity.Y, NPC.velocity.X);
			float distance = Vector2.Distance(player.Center, NPC.Center);
			if (player.dead)
			{
				NPC.TargetClosest(false);
				NPC.active = false;
				NPC.noTileCollide = true;
				NPC.velocity.Y = -100;
				NPC.timeLeft = 0;
			}
			if (NPC.ai[0] == 0f)
			{
				NPC.ai[3] = NPC.whoAmI;
				NPC.realLife = NPC.whoAmI;
				int num1 = NPC.whoAmI;
				for (int l = 0; l < 16; l++)
				{
					int SpawnBody1 = NPC.NewNPC(NPC.GetSpawnSourceForNPCFromNPCAI(), (int)(NPC.Center.X + NPC.width / 2), (int)(NPC.Center.Y + NPC.height / 2), ModContent.NPCType<RockSnakeBody>(), NPC.whoAmI);
					Main.npc[SpawnBody1].ai[3] = num1;
					Main.npc[SpawnBody1].ai[1] = num1;
					Main.npc[num1].ai[0] = SpawnBody1;
					num1 = SpawnBody1;
					Main.npc[SpawnBody1].realLife = NPC.whoAmI;
					NPC.netUpdate = true;
				}
				if (Main.netMode != NetmodeID.MultiplayerClient)
				{
					int num2 = NPC.NewNPC(NPC.GetSpawnSourceForNPCFromNPCAI(), (int)(NPC.Center.X / 2), (int)(NPC.Center.Y / 2), ModContent.NPCType<RockSnakeTail>(), NPC.whoAmI);
					if (Main.netMode == NetmodeID.Server && num2 < 200) NetMessage.SendData(MessageID.SyncNPC, -1, -1, null, num2);
					Main.npc[num2].ai[3] = NPC.whoAmI;
					Main.npc[num2].realLife = NPC.whoAmI;
					Main.npc[num2].ai[1] = num1;
					Main.npc[num2].ai[0] = num1;
				}
			}

			bool flag1 = true;
			float num188 = speed;
			float num189 = turnspeed;

			float speed1 = 17.8f;
			float turnspeed1 = 0.1f;
			if (distance >= 600)
			{
				speed1 = 15f;
				turnspeed1 = 0.5f;
			}
			if (distance < 300)
			{
				turnspeed1 = 0.1f;
				speed1 = (600 - distance) / 100 + 20f;
				if (distance < 400)
					turnspeed = 0f;
			}

			Vector2 vector18 = new Vector2(NPC.position.X + (float)NPC.width * 0.5f, NPC.position.Y + (float)NPC.height * 0.5f);
			float num191 = player.position.X + (float)(player.width / 2);
			float num192 = player.position.Y + (float)(player.height / 2);

			num188 = flag1 ? speed1 : 1f;
			num189 = flag1 ? turnspeed1 : 0.15f;
			float num48 = num188 * 1.3f;
			float num49 = num188 * 0.7f;
			float num42 = -1;
			float num50 = NPC.velocity.Length();
			if (num50 > 0f)
			{
				if (num50 > num48)
				{
					NPC.velocity.Normalize();
					NPC.velocity *= num48;
				}
				else if (num50 < num49)
				{
					NPC.velocity.Normalize();
					NPC.velocity *= num49;
				}
			}

			if (num42 > 0)
			{
				for (int num51 = 0; num51 < 200; num51++)
				{
					if (Main.npc[num51].active && Main.npc[num51].type == NPC.type && num51 != NPC.whoAmI)
					{
						Vector2 vector3 = Main.npc[num51].Center - NPC.Center;
						if (vector3.Length() < 400f)
						{
							vector3.Normalize();
							vector3 *= 1000f;
							num191 -= vector3.X;
							num192 -= vector3.Y;
						}
					}
				}
			}
			else
			{
				for (int num52 = 0; num52 < 200; num52++)
				{
					if (Main.npc[num52].active && Main.npc[num52].type == NPC.type && num52 != NPC.whoAmI)
					{
						Vector2 vector4 = Main.npc[num52].Center - NPC.Center;
						if (vector4.Length() < 60f)
						{
							vector4.Normalize();
							vector4 *= 200f;
							num191 -= vector4.X;
							num192 -= vector4.Y;
						}
					}
				}
			}

			num191 -= vector18.X;
			num192 -= vector18.Y;
			float num193 = (float)System.Math.Sqrt((double)(num191 * num191 + num192 * num192));
			if (NPC.ai[1] > 0f && NPC.ai[1] < (float)Main.npc.Length)
			{
				try
				{
					vector18 = new Vector2(NPC.position.X + (float)NPC.width * 0.5f, NPC.position.Y + (float)NPC.height * 0.5f);
					num191 = Main.npc[(int)NPC.ai[1]].position.X + (float)(Main.npc[(int)NPC.ai[1]].width / 2) - vector18.X;
					num192 = Main.npc[(int)NPC.ai[1]].position.Y + (float)(Main.npc[(int)NPC.ai[1]].height / 2) - vector18.Y;
				}
				catch
				{
				}
				NPC.rotation = (float)System.Math.Atan2((double)num192, (double)num191) + 1.57f;
				num193 = (float)System.Math.Sqrt((double)(num191 * num191 + num192 * num192));
				int num194 = NPC.width;
				num193 = (num193 - (float)num194) / num193;
				num191 *= num193 * 0.5f;
				num192 *= num193 * 0.5f;
				NPC.velocity = Vector2.Zero;
				NPC.position.X = NPC.position.X + num191;
				NPC.position.Y = NPC.position.Y + num192;
				if (num191 < 0f)
					NPC.spriteDirection = 1;
				if (num191 > 0f)
					NPC.spriteDirection = -1;
			}
			else
			{
				num193 = (float)System.Math.Sqrt((double)(num191 * num191 + num192 * num192));
				float num196 = System.Math.Abs(num191);
				float num197 = System.Math.Abs(num192);
				float num198 = num188 / num193;
				num191 *= num198;
				num192 *= num198;

				if ((NPC.velocity.X > 0f && num191 > 0f) || (NPC.velocity.X < 0f && num191 < 0f) || (NPC.velocity.Y > 0f && num192 > 0f) || (NPC.velocity.Y < 0f && num192 < 0f))
				{
					if (NPC.velocity.X < num191)
						NPC.velocity.X = NPC.velocity.X + num189;
					else if (NPC.velocity.X > num191)
						NPC.velocity.X = NPC.velocity.X - num189;

					if (NPC.velocity.Y < num192)
						NPC.velocity.Y = NPC.velocity.Y + num189;
					else if (NPC.velocity.Y > num192)
						NPC.velocity.Y = NPC.velocity.Y - num189;

					if ((double)System.Math.Abs(num192) < (double)num188 * 0.2 && ((NPC.velocity.X > 0f && num191 < 0f) || (NPC.velocity.X < 0f && num191 > 0f)))
					{
						if (NPC.velocity.Y > 0f)
							NPC.velocity.Y = NPC.velocity.Y + num189;
						else
							NPC.velocity.Y = NPC.velocity.Y - num189;
					}

					if ((double)System.Math.Abs(num191) < (double)num188 * 0.2 && ((NPC.velocity.Y > 0f && num192 < 0f) || (NPC.velocity.Y < 0f && num192 > 0f)))
					{
						if (NPC.velocity.X > 0f)
							NPC.velocity.X = NPC.velocity.X + num189;
						else
							NPC.velocity.X = NPC.velocity.X - num189;
					}
				}
				else
				{
					if (num196 > num197)
					{
						if (NPC.velocity.X < num191)
							NPC.velocity.X = NPC.velocity.X + num189;
						else if (NPC.velocity.X > num191)
							NPC.velocity.X = NPC.velocity.X - num189;

						if ((double)(System.Math.Abs(NPC.velocity.X) + System.Math.Abs(NPC.velocity.Y)) < (double)num188 * 0.5)
						{
							if (NPC.velocity.Y > 0f)
								NPC.velocity.Y = NPC.velocity.Y + num189;
							else
								NPC.velocity.Y = NPC.velocity.Y - num189;
						}
					}
					else
					{
						if (NPC.velocity.Y < num192)
							NPC.velocity.Y = NPC.velocity.Y + num189;
						else if (NPC.velocity.Y > num192)
							NPC.velocity.Y = NPC.velocity.Y - num189;

						if ((double)(System.Math.Abs(NPC.velocity.X) + System.Math.Abs(NPC.velocity.Y)) < (double)num188 * 0.5)
						{
							if (NPC.velocity.X > 0f)
								NPC.velocity.X = NPC.velocity.X + num189;
							else
								NPC.velocity.X = NPC.velocity.X - num189;
						}
					}
				}
			}
			Time++;
			Vector2 Pvector = player.Center - NPC.Center;
			Pvector.Normalize();
			Pvector *= 10;
			if(Time == 120)
            {
				Projectile.NewProjectile(NPC.GetSpawnSourceForNPCFromNPCAI(), NPC.Center,Pvector, ModContent.ProjectileType<RockFragments>(), 10, 0, Main.myPlayer, 0, 0);
			}
			if(Time == 240)
            {
				Projectile.NewProjectile(NPC.GetSpawnSourceForNPCFromNPCAI(), NPC.Center, Pvector, ModContent.ProjectileType<RockFragments2>(), 5, 0, Main.myPlayer, 0, 0);
			}
			if(Time == 480)
            {
				Projectile.NewProjectile(NPC.GetSpawnSourceForNPCFromNPCAI(), NPC.Center, Pvector, ModContent.ProjectileType<RockFragments3>(), 8, 0, Main.myPlayer, 0, 0);
				Time = 0;
			}
			if(NPC.life <= NPC.lifeMax / 2)
            {
				Time2++;
				if (Time2 == 600)
				{
					Vector2 Pvector1 = NPC.Center - new Vector2(NPC.Center.X, NPC.Center.Y - 1);
					Pvector1.Normalize();
					Pvector1 *= 10.5f;
					Projectile.NewProjectile(NPC.GetSpawnSourceForNPCFromNPCAI(), player.Center.X, player.Center.Y - 900, Pvector1.X, Pvector1.Y, ModContent.ProjectileType<Stalactite>(), 15, 0, Main.myPlayer, 0, 0);
					Projectile.NewProjectile(NPC.GetSpawnSourceForNPCFromNPCAI(), player.Center.X + 80, player.Center.Y - 900, Pvector1.X, Pvector1.Y, ModContent.ProjectileType<Stalactite>(), 15, 0, Main.myPlayer, 0, 0);
					Projectile.NewProjectile(NPC.GetSpawnSourceForNPCFromNPCAI(), player.Center.X - 80, player.Center.Y - 900, Pvector1.X, Pvector1.Y, ModContent.ProjectileType<Stalactite>(), 15, 0, Main.myPlayer, 0, 0);
					Time2 = 0;
				}
			}
		}
	}

	internal class RockSnakeBody : ModNPC
	{
		public int Time;
		public static Random random = new Random();
		public override void SetStaticDefaults()
		{
			NPCID.Sets.NPCBestiaryDrawModifiers value = new NPCID.Sets.NPCBestiaryDrawModifiers(0)
			{
				Hide = true
			};
			NPCID.Sets.NPCBestiaryDrawOffset.Add(NPC.type, value);
		}
		public override void SetDefaults()
		{
			NPC.width = 80;
			NPC.height = 66;
			NPC.netAlways = true;
			NPC.damage = 65;
			NPC.lifeMax = 30;
			NPC.boss = true;
			NPC.defense = 25;
			NPC.aiStyle = -1;
			NPC.HitSound = SoundID.NPCHit1;
			NPC.DeathSound = SoundID.NPCDeath1;
			NPC.noGravity = true;
			NPC.noTileCollide = true;
			NPC.knockBackResist = 0f;
			NPC.behindTiles = true;
		}
		public override void AI()
		{
			NPC.spriteDirection = NPC.direction = NPC.velocity.X < 0 ? 1 : -1;
			NPC.rotation = NPC.velocity.ToRotation() + (NPC.spriteDirection == -1 ? 0f : MathHelper.Pi);
			Player player = Main.player[NPC.target];
			if (player.dead)
			{
				NPC.TargetClosest(false);
				NPC.active = false;
				NPC.noTileCollide = true;
				NPC.velocity.Y = -100;
				NPC.timeLeft = 0;
			}
			if (Main.netMode != NetmodeID.MultiplayerClient)
			{
				if (!Main.npc[(int)NPC.ai[1]].active)
				{
					NPC.life = 0;
					NPC.HitEffect(0, 10.0);
					NPC.active = false;
					NPC.netUpdate = true;
				}
			}
			int dustIndex = Dust.NewDust(NPC.position, NPC.width, NPC.height, 54, 0f, 0f, 0, default(Color), 1f);
			Main.dust[dustIndex].noGravity = true;
			Main.dust[dustIndex].scale = 1f;
			if (Main.player[NPC.target].dead && NPC.timeLeft > 300)
				NPC.timeLeft = 0;
			if (NPC.ai[1] < (double)Main.npc.Length)
			{
				Vector2 npcCenter = new Vector2(NPC.position.X + NPC.width * 0.5f, NPC.position.Y + NPC.height * 0.5f);
				float dirX = Main.npc[(int)NPC.ai[1]].position.X + Main.npc[(int)NPC.ai[1]].width / 2 - npcCenter.X;
				float dirY = Main.npc[(int)NPC.ai[1]].position.Y + Main.npc[(int)NPC.ai[1]].height / 2 - npcCenter.Y;
				NPC.rotation = (float)Math.Atan2(dirY, dirX);
				float length = (float)Math.Sqrt(dirX * dirX + dirY * dirY);
				float dist = (length - NPC.width) / length;
				float posX = dirX * dist;
				float posY = dirY * dist;

				if (dirX < 0f)
				{
					NPC.spriteDirection = 1;

				}
				else
				{
					NPC.spriteDirection = -1;
				}
				NPC.position.X = NPC.position.X + posX;
				NPC.position.Y = NPC.position.Y + posY;
			}
			Time++;
			if(Time == 200)
            {
				if (random.Next(30) == 0)
				{
					Vector2 Pvector = player.Center - NPC.Center;
					Pvector.Normalize();
					Pvector *= 14;
					Projectile.NewProjectile(NPC.GetSpawnSourceForNPCFromNPCAI(), NPC.Center, Pvector, ModContent.ProjectileType<BigStone>(), 10, 0, Main.myPlayer, 0, 0);
				}
			}
			if(Time == 300)
            {
				if(random.Next(22) == 0)
                {
					Vector2 Pvector = player.Center - NPC.Center;
					Pvector.Normalize();
					Pvector *= 12;
					Projectile.NewProjectile(NPC.GetSpawnSourceForNPCFromNPCAI(), NPC.Center, Pvector, ModContent.ProjectileType<RockFragments>(), 8, 0, Main.myPlayer, 0, 0);
				}
				Time = 0;
            }
		}
		public override bool? DrawHealthBar(byte hbPosition, ref float scale, ref Vector2 position)
		{
			return false;
		}
		public override bool PreDraw(SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor)
		{
			Color Tail = Color.White;
			//BloodSoulNPC.NpcDrawTail(NPC, drawColor, Tail);
			return true;
		}
	}

	internal class RockSnakeTail : ModNPC
	{
		public override void SetStaticDefaults()
		{
			NPCID.Sets.NPCBestiaryDrawModifiers value = new NPCID.Sets.NPCBestiaryDrawModifiers(0)
			{
				Hide = true
			};
			NPCID.Sets.NPCBestiaryDrawOffset.Add(NPC.type, value);
		}
		public override void SetDefaults()
		{
			NPC.width = 110 - 20;
			NPC.height = 56;
			NPC.netAlways = true;
			NPC.damage = 65;
			NPC.defense = 25;
			NPC.lifeMax = 30;
			NPC.boss = true;
			NPC.HitSound = SoundID.NPCHit1;
			NPC.DeathSound = SoundID.NPCDeath1;
			NPC.noGravity = true;
			NPC.noTileCollide = true;
			NPC.knockBackResist = 0f;
			NPC.behindTiles = true;
			NPC.dontCountMe = true;
			NPC.aiStyle = -1;
		}
		public override void AI()
		{
			NPC.spriteDirection = NPC.direction = NPC.velocity.X < 0 ? 1 : -1;
			NPC.rotation = NPC.velocity.ToRotation() + (NPC.spriteDirection == -1 ? 0f : MathHelper.Pi);
			Player player = Main.player[NPC.target];
			if (player.dead)
			{
				NPC.TargetClosest(false);
				NPC.active = false;
				NPC.noTileCollide = true;
				NPC.TargetClosest(false);
				NPC.velocity.Y = -100;
				NPC.timeLeft = 0;
			}
			if (Main.netMode != NetmodeID.MultiplayerClient)
			{
				if (!Main.npc[(int)NPC.ai[1]].active)
				{
					NPC.life = 0;
					NPC.HitEffect(0, 10.0);
					NPC.active = false;
					NPC.netUpdate = true;
				}
			}
			int dustIndex = Dust.NewDust(NPC.position, NPC.width, NPC.height, 54, 0f, 0f, 0, default(Color), 1f);
			Main.dust[dustIndex].noGravity = true;
			Main.dust[dustIndex].scale = 1f;
			if (Main.player[NPC.target].dead && NPC.timeLeft > 300)
				NPC.timeLeft = 0;
			if (NPC.ai[1] < (double)Main.npc.Length)
			{
				Vector2 npcCenter = new Vector2(NPC.position.X + NPC.width / 2, NPC.position.Y + NPC.height / 2);
				float dirX = Main.npc[(int)NPC.ai[1]].position.X + Main.npc[(int)NPC.ai[1]].width / 2 - npcCenter.X;
				float dirY = Main.npc[(int)NPC.ai[1]].position.Y + Main.npc[(int)NPC.ai[1]].height / 2 - npcCenter.Y;
				float length = (float)Math.Sqrt(dirX * dirX + dirY * dirY);
				NPC.rotation = (float)Math.Atan2(dirY, dirX);
				float dist = (length - NPC.width) / length;
				float posX = dirX * dist;
				float posY = dirY * dist;
				NPC.position.X = NPC.position.X + posX;
				NPC.position.Y = NPC.position.Y + posY;
				NPC.direction = 0;
			}
		}
		public override bool? DrawHealthBar(byte hbPosition, ref float scale, ref Vector2 position)
		{
			return false;
		}
	}
	public class RockFragments : ModProjectile
	{
		public override void SetStaticDefaults()
		{
			DisplayName.SetDefault("岩石碎片");
		}
		public override void SetDefaults()
		{
			Projectile.tileCollide = false;
			Projectile.DamageType = DamageClass.Magic;
			Projectile.ignoreWater = true;
			Projectile.friendly = false;
			Projectile.penetrate = 2;
			Projectile.hostile = true;
			Projectile.height = 8;
			Projectile.width = 18;
			Projectile.aiStyle = 1;
			Projectile.damage = 15;
			Projectile.scale = 1f;
		}
		public override void AI()
		{
			base.Projectile.rotation = (float)Math.Atan2((double)base.Projectile.velocity.Y, (double)base.Projectile.velocity.X);
			int dustIndex = Dust.NewDust(Projectile.position, Projectile.width, Projectile.height, 109, 0f, 0f, 0, default(Color), 1f);
			Main.dust[dustIndex].noGravity = true;
			Main.dust[dustIndex].scale = 1f;
		}
		public override void PostDraw(Color lightColor)
		{
			Texture2D projectileTexture = TextureAssets.Projectile[Projectile.type].Value;
			int frameHeight = projectileTexture.Height / Main.projFrames[Projectile.type];
			Vector2 drawOrigin = new Vector2(TextureAssets.Projectile[Projectile.type].Value.Width * 0.5f, Projectile.height * 0.5f);
			for (int k = 0; k < Projectile.oldPos.Length; k++)
			{
				Vector2 drawPos = Projectile.oldPos[k] - Main.screenPosition + drawOrigin + new Vector2(0f, Projectile.gfxOffY);
				Color color = Projectile.GetAlpha(lightColor * 0.5f) * ((float)(Projectile.oldPos.Length - k) / (float)Projectile.oldPos.Length);
				Main.spriteBatch.Draw(projectileTexture, drawPos, new Rectangle(0, frameHeight * Projectile.frame, projectileTexture.Width, frameHeight), color, Projectile.rotation, drawOrigin, MathHelper.Lerp(Projectile.scale, 0.75f, (Projectile.oldPos.Length + k) / 15f), SpriteEffects.None, 0f);
			}
		}
	}
	public class RockFragments2 : ModProjectile
	{
		public override void SetStaticDefaults()
		{
			DisplayName.SetDefault("岩石碎片2");
		}
		public override void SetDefaults()
		{
			Projectile.tileCollide = false;
			Projectile.DamageType = DamageClass.Magic;
			Projectile.ignoreWater = true;
			Projectile.friendly = false;
			Projectile.hostile = true;
			Projectile.height = 14;
			Projectile.width = 16;
			Projectile.penetrate = 2;
			Projectile.aiStyle = 1;
			Projectile.damage = 15;
			Projectile.penetrate = 5;
			Projectile.scale = 1f;
		}
		public override void AI()
		{
			base.Projectile.rotation = (float)Math.Atan2((double)base.Projectile.velocity.Y, (double)base.Projectile.velocity.X);
			int dustIndex = Dust.NewDust(Projectile.position, Projectile.width, Projectile.height, 109, 0f, 0f, 0, default(Color), 1f);
			Main.dust[dustIndex].noGravity = true;
			Main.dust[dustIndex].scale = 1f;
		}
		public override void PostDraw(Color lightColor)
		{
			Texture2D projectileTexture = TextureAssets.Projectile[Projectile.type].Value;
			int frameHeight = projectileTexture.Height / Main.projFrames[Projectile.type];
			Vector2 drawOrigin = new Vector2(TextureAssets.Projectile[Projectile.type].Value.Width * 0.5f, Projectile.height * 0.5f);
			for (int k = 0; k < Projectile.oldPos.Length; k++)
			{
				Vector2 drawPos = Projectile.oldPos[k] - Main.screenPosition + drawOrigin + new Vector2(0f, Projectile.gfxOffY);
				Color color = Projectile.GetAlpha(lightColor * 0.5f) * ((float)(Projectile.oldPos.Length - k) / (float)Projectile.oldPos.Length);
				Main.spriteBatch.Draw(projectileTexture, drawPos, new Rectangle(0, frameHeight * Projectile.frame, projectileTexture.Width, frameHeight), color, Projectile.rotation, drawOrigin, MathHelper.Lerp(Projectile.scale, 0.75f, (Projectile.oldPos.Length + k) / 15f), SpriteEffects.None, 0f);
			}
		}
	}
	public class RockFragments3 : ModProjectile
	{
		public override void SetStaticDefaults()
		{
			DisplayName.SetDefault("岩石碎片3");
		}
		public override void SetDefaults()
		{
			Projectile.tileCollide = false;
			Projectile.DamageType = DamageClass.Magic;
			Projectile.ignoreWater = true;
			Projectile.friendly = false;
			Projectile.hostile = true;
			Projectile.height = 18;
			Projectile.width = 14;
			Projectile.aiStyle = 1;
			Projectile.damage = 15;
			Projectile.penetrate = 2;
			Projectile.scale = 1f;
		}
		public override void AI()
		{
			base.Projectile.rotation = (float)Math.Atan2((double)base.Projectile.velocity.Y, (double)base.Projectile.velocity.X);
			int dustIndex = Dust.NewDust(Projectile.position, Projectile.width, Projectile.height, 109, 0f, 0f, 0, default(Color), 1f);
			Main.dust[dustIndex].noGravity = true;
			Main.dust[dustIndex].scale = 1f;
		}
		public override void PostDraw(Color lightColor)
		{
			Texture2D projectileTexture = TextureAssets.Projectile[Projectile.type].Value;
			int frameHeight = projectileTexture.Height / Main.projFrames[Projectile.type];
			Vector2 drawOrigin = new Vector2(TextureAssets.Projectile[Projectile.type].Value.Width * 0.5f, Projectile.height * 0.5f);
			for (int k = 0; k < Projectile.oldPos.Length; k++)
			{
				Vector2 drawPos = Projectile.oldPos[k] - Main.screenPosition + drawOrigin + new Vector2(0f, Projectile.gfxOffY);
				Color color = Projectile.GetAlpha(lightColor * 0.5f) * ((float)(Projectile.oldPos.Length - k) / (float)Projectile.oldPos.Length);
				Main.spriteBatch.Draw(projectileTexture, drawPos, new Rectangle(0, frameHeight * Projectile.frame, projectileTexture.Width, frameHeight), color, Projectile.rotation, drawOrigin, MathHelper.Lerp(Projectile.scale, 0.75f, (Projectile.oldPos.Length + k) / 15f), SpriteEffects.None, 0f);
			}
		}
	}
	public class BigStone : ModProjectile
	{
		public int Counter;
		public override void SetStaticDefaults()
		{
			DisplayName.SetDefault("大石鳞");
		}
		public override void SetDefaults()
		{
			Projectile.tileCollide = false;
			Projectile.DamageType = DamageClass.Magic;
			Projectile.ignoreWater = true;
			Projectile.friendly = false;
			Projectile.penetrate = 1;
			Projectile.hostile = true;
			Projectile.height = 14;
			Projectile.width = 28;
			Projectile.timeLeft = 80;
			Projectile.aiStyle = 1;
			Projectile.damage = 30;
			Projectile.scale = 1f;
		}
		public override void AI()
		{
			int dustIndex = Dust.NewDust(Projectile.position, Projectile.width, Projectile.height, 109, 0f, 0f, 0, default(Color), 1f);
			Main.dust[dustIndex].noGravity = true;
			Main.dust[dustIndex].scale = 1f;
		}
		public override void PostAI()
		{
			Projectile.rotation = (float)Math.Atan2((double)base.Projectile.velocity.Y, (double)base.Projectile.velocity.X);
			Counter++;
			if (Counter == 80)
			{
				for (float r = 0f; r < MathHelper.TwoPi; r += MathHelper.TwoPi / 3f)
				{
					Vector2 velocity = new Vector2((float)Math.Cos(r), (float)Math.Sin(r)) * 10f;
					Projectile.NewProjectile(Projectile.InheritSource(Projectile), Projectile.Center, velocity, ModContent.ProjectileType<RockFragments2>(), 15, 0, Main.myPlayer, 0, 0);
				}
			}
		}
		public override void PostDraw(Color lightColor)
		{
			Texture2D projectileTexture = TextureAssets.Projectile[Projectile.type].Value;
			int frameHeight = projectileTexture.Height / Main.projFrames[Projectile.type];
			Vector2 drawOrigin = new Vector2(TextureAssets.Projectile[Projectile.type].Value.Width * 0.5f, Projectile.height * 0.5f);
			for (int k = 0; k < Projectile.oldPos.Length; k++)
			{
				Vector2 drawPos = Projectile.oldPos[k] - Main.screenPosition + drawOrigin + new Vector2(0f, Projectile.gfxOffY);
				Color color = Projectile.GetAlpha(lightColor * 0.5f) * ((float)(Projectile.oldPos.Length - k) / (float)Projectile.oldPos.Length);
				Main.spriteBatch.Draw(projectileTexture, drawPos, new Rectangle(0, frameHeight * Projectile.frame, projectileTexture.Width, frameHeight), color, Projectile.rotation, drawOrigin, MathHelper.Lerp(Projectile.scale, 0.75f, (Projectile.oldPos.Length + k) / 15f), SpriteEffects.None, 0f);
			}
		}
	}
	public class Stalactite : ModProjectile
	{
		public override void SetStaticDefaults()
		{
			DisplayName.SetDefault("钟乳石");
		}
		public int Counter;
		public override void SetDefaults()
		{
			Projectile.tileCollide = false;
			Projectile.DamageType = DamageClass.Magic;
			Projectile.ignoreWater = true;
			Projectile.friendly = false;
			Projectile.penetrate = 3;
			Projectile.hostile = true;
			Projectile.height = 20;
			Projectile.timeLeft = 90;
			Projectile.width = 66;
			Projectile.aiStyle = 0;
			Projectile.damage = 30;
			Projectile.scale = 1f;
		}
		public override void AI()
		{
		    int dustIndex = Dust.NewDust(Projectile.position, Projectile.width, Projectile.height, 109, 0f, 0f, 0, default(Color), 1f);
			Main.dust[dustIndex].noGravity = true;
			Main.dust[dustIndex].scale = 1f;
			Counter++;
			if (Counter == 90)
			{
				for (float r = 0f; r < MathHelper.TwoPi; r += MathHelper.TwoPi / 3f)
				{
					Vector2 velocity = new Vector2((float)Math.Cos(r), (float)Math.Sin(r)) * 10f;
					Projectile.NewProjectile(Projectile.InheritSource(Projectile), Projectile.Center, velocity, ModContent.ProjectileType<BigStone>(), 15, 0, Main.myPlayer, 0, 0);
				}
			}
		}
		public override void PostDraw(Color lightColor)
		{
			Texture2D projectileTexture = TextureAssets.Projectile[Projectile.type].Value; 
			int frameHeight = projectileTexture.Height / Main.projFrames[Projectile.type]; 
			Vector2 drawOrigin = new Vector2(TextureAssets.Projectile[Projectile.type].Value.Width * 0.5f, Projectile.height * 0.5f);
			for (int k = 0; k < Projectile.oldPos.Length; k++)
			{
				Vector2 drawPos = Projectile.oldPos[k] - Main.screenPosition + drawOrigin + new Vector2(0f, Projectile.gfxOffY); 
				Color color = Projectile.GetAlpha(lightColor * 0.5f) * ((float)(Projectile.oldPos.Length - k) / (float)Projectile.oldPos.Length); 
				Main.spriteBatch.Draw(projectileTexture, drawPos, new Rectangle(0, frameHeight * Projectile.frame, projectileTexture.Width, frameHeight), color, Projectile.rotation, drawOrigin, MathHelper.Lerp(Projectile.scale, 0.75f, (Projectile.oldPos.Length + k) / 15f), SpriteEffects.None, 0f);
			}
		}
	}
}
