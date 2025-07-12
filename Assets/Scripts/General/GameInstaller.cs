using Zenject;
public class GameInstaller : MonoInstaller
{
    public override void InstallBindings()
    {
        Container.Bind<SkillRuntimeFactory>().AsSingle();
        Container.Bind<PlayerState>().FromComponentInHierarchy().AsSingle();
        Container.Bind<PlayerInput>().FromComponentInHierarchy().AsSingle();
        Container.Bind<PlayerAnimator>().FromComponentInHierarchy().AsSingle();
        Container.Bind<CameraShake>().FromComponentInHierarchy().AsSingle();
        Container.Bind<PlayerMove>().FromComponentInHierarchy().AsSingle();
        Container.Bind<PlayerHP>().FromComponentInHierarchy().AsSingle();
        Container.Bind<SkillUIHandler>().FromComponentInHierarchy().AsSingle();
        Container.Bind<FadeInOutEffect>().FromComponentInHierarchy().AsSingle();
        Container.Bind<DamageTextPool>().FromComponentInHierarchy().AsSingle();
        Container.Bind<SkillSceneBootstrap>().FromComponentInHierarchy().AsSingle();
        Container.Bind<PlayerContext>().FromComponentInHierarchy().AsSingle();
        Container.Bind<PlayerSkillManager>().FromComponentInHierarchy().AsSingle();
        Container.Bind<PlayerEnemyHandler>().FromComponentInHierarchy().AsSingle();
        Container.Bind<SkillModifierHub>().FromComponentInHierarchy().AsSingle();
        Container.Bind<UltimateSelector>().FromComponentInHierarchy().AsSingle();
    }
}
