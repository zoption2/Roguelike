using Obstacles;
using Pool;
using UnityEngine;
using Zenject;

public interface IBuffFactory
{
    public void Init();
    public IBuff CreateBuff(Vector3 position, Transform parent, BuffType type);
}
public class BuffFactory : IBuffFactory
{
    private BuffPooler _pool;

    [Inject]
    public void Construct(
        BuffPooler pool
        )
    {
        _pool = pool;

        _pool.Init();
    }

    public void Init()
    {
    }

    public IBuff CreateBuff(Vector3 position, Transform parent, BuffType type)
    {
        IMyPoolable newb = _pool.Pull<IMyPoolable>(type, position, parent.rotation, parent.parent);
        IBuff buff = newb.gameObject.GetComponent<IBuff>();
        buff.Init(_pool);
        return buff;
    }

}
