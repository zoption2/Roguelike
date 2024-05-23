using Obstacles;
using Pool;
using UnityEngine;
using Zenject;

public interface IBuffFactory
{
    public void Init();
    public IBuff CreateBuff(Transform position, BuffType type);
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

    public IBuff CreateBuff(Transform point, BuffType type)
    {
        IMyPoolable newb = _pool.Pull<IMyPoolable>(type, point.position, point.rotation, point.parent);
        IBuff buff = newb.gameObject.GetComponent<IBuff>();
        buff.Init(_pool);
        return buff;
    }

}
