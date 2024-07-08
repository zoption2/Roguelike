using Obstacles;
using Pool;
using UnityEngine;
using Zenject;
using Cysharp.Threading.Tasks;

public interface IBuffFactory
{
    public void Init();
    public IBuff CreateBuffAsync(Vector3 position, Transform parent, BuffType type);
}
public class BuffFactory : IBuffFactory
{
    private BuffPooler _buffPool;

    [Inject]
    public void Construct(IPoolManager poolManager)
    {
        _buffPool = poolManager.GetBuffPooler();
    }

    public void Init()
    {
    }

    public IBuff CreateBuffAsync(Vector3 position, Transform parent, BuffType type)
    {
        IMyPoolable newb =  _buffPool.Pull<IMyPoolable>(type, position, parent.rotation, parent.parent);
        IBuff buff = newb.gameObject.GetComponent<IBuff>();
        buff.Init(_buffPool);
        return buff;
    }

}
