using RuntimeSceneInjector;

public class InjectionSample3 : InjectionSample2
{
    [Inject] private IHoge4 _hoge4;
    private IHoge5 _hoge5;

    [Inject]
    private void Construct(IHoge5 hoge5)
    {
        _hoge5 = hoge5;
    }

    protected override void Awake()
    {
        base.Awake();

        _hoge4.Hoge();

        _hoge5.Hoge();
    }
}