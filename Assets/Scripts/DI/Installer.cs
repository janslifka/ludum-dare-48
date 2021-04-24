using Anglerfish;
using Camera;
using Fish;
using UI;
using UnityEngine;
using Zenject;

namespace DI
{
    public class Installer : MonoInstaller<Installer>
    {
        [SerializeField] GameObject anglerfishPrefab;
        [SerializeField] GameObject fishPrefab;
        [SerializeField] GameObject fishSpawner;
        [SerializeField] GameObject cameraPrefab;
        [SerializeField] GameObject uiPrefab;

        public override void InstallBindings()
        {
            Container.Bind<AnglerfishController>().FromComponentInNewPrefab(anglerfishPrefab).AsSingle().NonLazy();
            Container.Bind<CameraController>().FromComponentInNewPrefab(cameraPrefab).AsSingle().NonLazy();
            Container.Bind<UIController>().FromComponentInNewPrefab(uiPrefab).AsSingle().NonLazy();
            Container.BindMemoryPool<FishController, FishController.Factory>().WithInitialSize(10).FromComponentInNewPrefab(fishPrefab);
            Container.Bind<FishSpawner>().FromComponentInNewPrefab(fishSpawner).AsSingle().NonLazy();
            Container.BindInterfacesAndSelfTo<GameManager>().AsSingle();
        }
    }
}