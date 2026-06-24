namespace GameResources.Features.UISystem
{
    using UnityEngine;
    using Zenject;

    public abstract class BaseWindow : MonoBehaviour
    {
        protected SignalBus _signalBus;

        [Inject]
        private void Construct(SignalBus signalBus) => _signalBus = signalBus;

        public virtual void Show() => gameObject.SetActive(true);

        public virtual void Hide() => gameObject.SetActive(false);
    }
}