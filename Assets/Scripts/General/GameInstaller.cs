using Zenject;
public class GameInstaller : MonoInstaller
{
    public override void InstallBindings()
    {
        Container.Bind<PlayerState>().FromComponentInHierarchy().AsSingle();
        Container.Bind<PlayerInput>().FromComponentInHierarchy().AsSingle();
        Container.Bind<PlayerAnimator>().FromComponentInHierarchy().AsSingle();
        Container.Bind<CameraShake>().FromComponentInHierarchy().AsSingle();
        Container.Bind<PlayerDash>().FromComponentInHierarchy().AsSingle();
        Container.Bind<PlayerMove>().FromComponentInHierarchy().AsSingle();
        Container.Bind<PlayerHP>().FromComponentInHierarchy().AsSingle();
        Container.Bind<PlayerSkillHandler>().FromComponentInHierarchy().AsSingle();
        Container.Bind<FireballSkill>().FromComponentInHierarchy().AsSingle();
        Container.Bind<FireAOESkill>().FromComponentInHierarchy().AsSingle();
        Container.Bind<SkillUIHandler>().FromComponentInHierarchy().AsSingle();
        Container.Bind<FadeInOutEffect>().FromComponentInHierarchy().AsSingle();
        Container.Bind<DamageTextPool>().FromComponentInHierarchy().AsSingle();
        Container.Bind<SkillSelectionSaver>().FromComponentInHierarchy().AsSingle();
        Container.Bind<SkillSceneBootstrap>().FromComponentInHierarchy().AsSingle();
        Container.Bind<PlayerContext>().FromComponentInHierarchy().AsSingle();
        Container.Bind<PlayerSkillManager>().FromComponentInHierarchy().AsSingle();
    }
}
