namespace AO
{
    using AO;
    using EGamePlay.Combat;
    using ET;
    using ET.Server;
    using UnityEngine;
    using TComp = AO.MapApp;

    public static class MapAppSystem
    {
        [ObjectSystem]
        public class MapAppAwakeSystem : AwakeSystem<TComp>
        {
            protected override void Awake(TComp self)
            {
                //Log.Console(self.GetType().Name);
                self.AddComponent<MapSceneComponent>();

                EGamePlay.Entity.EnableLog = false;
                EGamePlay.MasterEntity.Create();
                EGamePlay.MasterEntity.Instance.AddChild<CombatContext>();

                var sceneComp = self.GetComponent<MapSceneComponent>();
                
                // 增加map1 地图
                var map1Scene = sceneComp.AddChild<Scene, string>("Map1");
                sceneComp.Add(map1Scene);
                // map1 地图增加一个测试怪物
                var map1TestMonster = ActorFactory.Create(ActorType.NonPlayer, map1Scene);
                map1Scene.GetComponent<SceneUnitComponent>().Add(map1TestMonster);
                // 向中心世界服注册map1场景id
                var registerMap1Msg = new RegisterMapSceneRequest() { MapType = map1Scene.Type, SceneId = map1Scene.InstanceId };
                AOZone.GetAppCall<WorldServiceAppCall>().RegisterMapSceneRequest(registerMap1Msg).Coroutine();
                
                // 增加技能编辑地图
                var linkScene = sceneComp.AddChild<Scene, string>("ExecutionLinkScene");
                sceneComp.Add(linkScene);
                // 技能编辑场景增加一个测试怪物
                var eLTestMonster = ActorFactory.Create(ActorType.NonPlayer, linkScene);
                linkScene.GetComponent<SceneUnitComponent>().Add(eLTestMonster);
                // 向中心世界服注册技能编辑场景id
                var registerElMapMsg = new RegisterMapSceneRequest() { MapType = linkScene.Type, SceneId = linkScene.InstanceId };
                AOZone.GetAppCall<WorldServiceAppCall>().RegisterMapSceneRequest(registerElMapMsg).Coroutine();
                
                
                TestRunEvent().Coroutine();
            }
        }

        private static async ETTask TestRunEvent()
        {
            await TimerComponent.Instance.WaitAsync(1000);
            Process_UnitDeadProcess.Execute(new Actor(), new Actor());
        }

        [ObjectSystem]
        public class MapAppUpdateSystem : UpdateSystem<TComp>
        {
            protected override void Update(TComp self)
            {
                EGamePlay.MasterEntity.Instance.Update();
            }
        }
    }
}