if (ModLoader.HasMod("BossChecklist"))//如果有那个mod
            {
                Mod bossCheckList = ModLoader.GetMod("BossChecklist");//获取boss列表模组

                bossCheckList.Call(
                    "AddBoss",//添加boss
                    1.5f,//时期 史莱姆王后
                    ModContent.NPCType<GelSpider>(),//npc
                    this,//本Mod
                    "$Mods.BloodSoul.NPCName.GelSpider",//获取命名
                    () => BloodSoulSystem.downedGelSpider,//检测击败
                    ModContent.ItemType<Items.EvidenceOfKingSlime>(),//召唤物
                    new List<int>(0),//没有收藏品
                    new List<int> { ModContent.ItemType<Items.GelGold>(), ModContent.ItemType<Items.BossBag.GelSpiderBossBag>() },//添加普通掉落物
                    "$Mods.BloodSoul.BossSpawnInfo.GelSpider",//召唤条件
                    "$Mods.BloodSoul.Introduce.GelSpider",//介绍
                    "BloodSoul/Images/donwedGelSpiderImages"//图片
                    );

                bossCheckList.Call(
                    "AddBoss",//添加boss
                    2.5f,//时期 克眼后
                    ModContent.NPCType<NPCs.Bosses.PhantomUang.Uang>(),//npc
                    this,//本Mod
                    "$Mods.BloodSoul.NPCName.Uang",//获取命名
                    () => BloodSoulSystem.downedUang,//检测击败
                    ModContent.ItemType<Items.Phantom.PhantomBait>(),//召唤物
                    new List<int> { ModContent.ItemType<Mount.Uang.UangEgg>() },
                    new List<int> { ModContent.ItemType<Items.BossBag.UangBossBag>() },//添加普通掉落物
                    "$Mods.BloodSoul.BossSpawnInfo.Uang",//召唤条件
                    "$Mods.BloodSoul.Introduce.Uang",//介绍
                    "BloodSoul/Images/donwedUangImages"//图片
                    );

                bossCheckList.Call(
                    "AddBoss",//添加boss
                    3.3f,//时期 邪恶Boss后,1
                    ModContent.NPCType<NPCs.Bosses.RockSnake.RockSnakeHead>(),//npc
                    this,//本Mod
                    "$Mods.BloodSoul.NPCName.RockSnake",//获取命名
                    () => BloodSoulSystem.downedRockSnake,//检测击败
                    ModContent.ItemType<Items.花岗岩彩石>(),//召唤物
                    new List<int> (0),
                    new List<int> { ModContent.ItemType<Items.BossBag.RockSnakeBossBag>() },//添加普通掉落物
                    "$Mods.BloodSoul.BossSpawnInfo.RockSnake",//召唤条件
                    "$Mods.BloodSoul.Introduce.RockSnake",//介绍
                    "BloodSoul/Images/donwedRockSnakeImages"//图片
                    );

                bossCheckList.Call(
                    "AddBoss",//添加boss
                    5.5f,//时期 骷髅王后
                    ModContent.NPCType<NPCs.Bosses.HolyLightSwords.HolyLightSword>(),//npc
                    this,//本Mod
                    "$Mods.BloodSoul.NPCName.HolyLightSword",//获取阴凌剑的命名
                    () => BloodSoulSystem.downedHolyLightSword,//检测击败
                    ModContent.ItemType<Items.HolySword>(),//召唤物
                    new List<int>(0),//没有收藏品
                    new List<int> { ModContent.ItemType<Items.HolyFragment>(), ModContent.ItemType<Items.BossBag.HolySwordBossBag>() },//添加普通掉落物
                    "$Mods.BloodSoul.BossSpawnInfo.HolyLightSword",//召唤条件
                    "$Mods.BloodSoul.Introduce.HolyLightSword",//介绍
                    "BloodSoul/Images/donwedHolyLightSwordImages"//图片
                    );

                bossCheckList.Call(
                    "AddBoss",//添加boss
                    5.6f,//时期 阴凌剑后
                    ModContent.NPCType<NPCs.Bosses.BloodCrystalEyes.BloodCrystalEye>(),//npc
                    this,//本Mod
                    "$Mods.BloodSoul.NPCName.BloodCrystalEye",//获取血眼的命名
                    () => BloodSoulSystem.downedBloodCrystalEye,//检测击败
                    ModContent.ItemType<Items.SumBloodEyeBoss>(),//召唤物
                    new List<int>(0),//没有收藏品
                    new List<int> { ModContent.ItemType<Items.BloodToothChaosFlesh>(), ModContent.ItemType<Items.BossBag.BloodEyeBossBag>() },//添加普通掉落物
                    "$Mods.BloodSoul.BossSpawnInfo.BloodCrystalEye",//召唤条件
                    "$Mods.BloodSoul.Introduce.BloodCrystalEye",//介绍
                    "BloodSoul/Images/donwedBloodCrystalEyeImages"//图片
                    );

                bossCheckList.Call(
                    "AddBoss",//添加boss
                    5.7f,//时期 血眼后
                    ModContent.NPCType<NPCs.Bosses.SharaIshvalda.SharaIshvaldaBody>(),//npc
                    this,//本Mod
                    "$Mods.BloodSoul.NPCName.SharaIshvaldaBody",//获取弟弟龙的命名
                    () => BloodSoulSystem.SharaIshvalda,//检测击败
                    ModContent.ItemType<Items.DifferentWorld2>(),//召唤物
                    new List<int>(0),//没有收藏品
                                     //new List<int> { ModContent.ItemType<Items.BloodToothChaosFlesh>(), ModContent.ItemType<Items.BossBag.BloodEyeBossBag>() },//添加普通掉落物
                    "$Mods.BloodSoul.BossSpawnInfo.SharaIshvaldaBody",//召唤条件
                    "$Mods.BloodSoul.Introduce.SharaIshvaldaBody",//介绍
                    "BloodSoul/Images/SharaIshvaldaImages"//图片
                    );

                bossCheckList.Call(
                    "AddBoss",//添加boss
                    8.7f,//时期 史皇后7
                    ModContent.NPCType<NPCs.Bosses.Tidespirit.Tideboss>(),//npc
                    this,//本Mod
                    "$Mods.BloodSoul.NPCName.TideSpirit",//获取命名
                    () => BloodSoulSystem.downedTideSpirit,//检测击败
                    ModContent.ItemType<Items.TurbulentWater>(),//召唤物
                    new List<int> (0),
                    new List<int> { ModContent.ItemType<Items.BossBag.TideSpiritBossBag>() },//添加普通掉落物
                    "$Mods.BloodSoul.BossSpawnInfo.TideSpirit",//召唤条件
                    "$Mods.BloodSoul.Introduce.TideSpirit",//介绍
                    "BloodSoul/Images/donwedTideSpiritImages"//图片
                    );

                bossCheckList.Call(
                    "AddBoss",//添加boss
                    16.5f,//时期 教徒后
                    ModContent.NPCType<NPCs.Bosses.TheStarGazer.StarGazerBoss>(),//npc
                    this,//本Mod
                    "$Mods.BloodSoul.NPCName.StarGazerBoss",//获取命名
                    () => BloodSoulSystem.downedStarGazer,//检测击败
                    ModContent.ItemType<Items.StarProof>(),//召唤物
                    new List<int>(0),//没有收藏品
                    new List<int>(0),//添加普通掉落物
                    "$Mods.BloodSoul.BossSpawnInfo.StarGazerBoss",//召唤条件
                    "$Mods.BloodSoul.Introduce.StarGazerBoss",//介绍
                    "BloodSoul/Images/donwedStarGazerBossImages"//图片
                    );
            }