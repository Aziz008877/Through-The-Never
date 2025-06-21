using Zenject;
public class GameInstaller : MonoInstaller
{
    public override void InstallBindings()
    {
        Container.Bind<PlayerState>().FromComponentInHierarchy().AsSingle();
        Container.Bind<PlayerInput>().FromComponentInHierarchy().AsSingle();
        Container.Bind<PlayerAnimator>().FromComponentInHierarchy().AsSingle();
        Container.Bind<CameraShake>().FromComponentInHierarchy().AsSingle();
    }
}
